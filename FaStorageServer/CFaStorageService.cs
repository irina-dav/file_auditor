using FaStorageServer.FaExportService;
using FaStorageServer.FaNotificationService;
using Infrastructure.Models;
using NLog;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;
using System.ServiceModel;

namespace FaStorageServer
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class CFaStorageService : IFaStorageService
    {
        private static readonly Logger s_logger = LogManager.GetCurrentClassLogger();

        private readonly CContextFactory _contextFactory;
        private readonly INotificationService _notificationService;
        private readonly IExportService _exportService;

        public CFaStorageService(CContextFactory contextFactory)
        {
            _contextFactory = contextFactory;
            _notificationService = new NotificationServiceClient();
            _exportService = new ExportServiceClient();
        }

        public List<CRule> FindAllRules()
        {
            List<CRule> rules;
            using (var context = _contextFactory.Get<CDbContext>())
            {
                context.TryCall(con => con.Rules.ToList(), out rules);
            }
            return rules;
        }

        public List<CRule> FindRulesByClientId(Guid clientId)
        {
            List<CRule> rules;
            using (var context = _contextFactory.Get<CDbContext>())
            {
                context.TryCall(con => con.Rules.Where(r => r.ClientInfoId == clientId).ToList(), out rules);
            }
            return rules;
        }

        public CRule FindRuleById(Guid ruleId)
        {
            CRule rule;
            using (var context = _contextFactory.Get<CDbContext>())
            {
                context.TryCall(con => con.Rules.FirstOrDefault(r => r.RuleId.Equals(ruleId)), out rule);
            }
            return rule;
        }

        public void InsertRule(CRule rule)
        {
            using (var context = _contextFactory.Get<CDbContext>())
            {
                context.TryAdd(rule);
            }
        }

        public void UpdateRule(CRule rule)
        {
            using (var context = _contextFactory.Get<CDbContext>())
            {
                context.TryUpdate(rule);
            }
        }

        public void DeleteRule(Guid ruleId)
        {
            using (var context = _contextFactory.Get<CDbContext>())
            {
                var entity = context.Rules.Find(ruleId);
                if (entity == null)
                {
                    s_logger.Error("Attempt to delete not existed rule");
                }
                else
                {
                    context.Rules.Remove(entity);
                    context.SaveChanges();
                }
            }
        }

        public CEventInfo FindEventInfoById(Guid eventInfoId)
        {
            CEventInfo eventInfo;
            using (var context = _contextFactory.Get<CDbContext>())
            {
                context.TryCall(con => con.Events.FirstOrDefault(e => e.EventInfoId.Equals(eventInfoId)), out eventInfo);
            }
            return eventInfo;
        }

        public List<CClientInfo> FindClientsWithLastDate()
        {
            List<CClientInfo> clients;
            using (var context = _contextFactory.Get<CDbContext>())
            {
                try
                {
                    context.TryCall(con => con.Clients.ToList(), out clients);
                    foreach (var client in clients)
                    {
                        context.TryCall(
                            con => con.Events.Where(e => e.ClientInfoId.Equals(client.ClientInfoId))
                                .OrderByDescending(e => e.TimeCreatedUtc).FirstOrDefault(), out CEventInfo eventInfo);
                        client.LastEventDateTimeUtc = eventInfo?.TimeCreatedUtc;
                    }
                }
                catch (Exception ex)
                {
                    s_logger.Error(ex);
                    throw;
                }
            }
            return clients;
        }

        public CClientInfo FindClientInfoById(Guid clientInfoId)
        {
            CClientInfo clientInfo;
            using (var context = _contextFactory.Get<CDbContext>())
            {
                context.TryCall(con => con.Clients.FirstOrDefault(c => c.ClientInfoId.Equals(clientInfoId)), out clientInfo);
            }
            return clientInfo;
        }

        public List<CClientInfo> FindClientInfo(string computerName, string ipAddress)
        {
            List<CClientInfo> clients;
            using (var context = _contextFactory.Get<CDbContext>())
            {
                context.TryCall(con => con.Clients.AsEnumerable().Where(c => c.Equals(new CClientInfo(computerName, ipAddress))).ToList(), out clients);
            }
            return clients;
        }

        public CClientInfo RegisterClient(string computerName, string ipAddress)
        {
            CClientInfo client;
            try
            {
                client = FindClientInfo(computerName, ipAddress).FirstOrDefault();
                if (client == null)
                {
                    client = new CClientInfo(computerName, ipAddress);
                    using (var context = _contextFactory.Get<CDbContext>())
                    {
                        context.TryAdd(client);
                    }
                }
            }
            catch (Exception ex)
            {
                s_logger.Error(ex);
                throw;
            }
            return client;
        }

        public void UpdateClient(CClientInfo client)
        {
            using (var context = _contextFactory.Get<CDbContext>())
            {
                context.TryUpdate(client);
            }
        }

        public ReadOnlyCollection<CEventInfo> SearchEvents(CSearchFilter filter)
        {
            using (var context = _contextFactory.Get<CDbContext>())
            {
                context.TryCall(con =>
                {
                    IQueryable<CEventInfo> events = con.Events.AsQueryable();
                    if (!string.IsNullOrEmpty(filter.ComputerName))
                        events = events.Where(e => e.Computer.Contains(filter.ComputerName));
                    if (!string.IsNullOrEmpty(filter.IpAddress))
                        events = events.Where(e => e.Computer.Contains(filter.IpAddress));
                    if (!string.IsNullOrEmpty(filter.UserName))
                        events = events.Where(e => e.UserName.Contains(filter.UserName));
                    if (!string.IsNullOrEmpty(filter.FileName))
                        events = events.Where(e => e.ObjectName.Contains(filter.FileName));
                    if (filter.PeriodStart.HasValue)
                        events = events.Where(e => e.TimeCreatedUtc >= filter.PeriodStart);
                    if (filter.PeriodEnd.HasValue)
                        events = events.Where(e => e.TimeCreatedUtc <= filter.PeriodEnd);
                    if (filter.FileEvents != EFileEvents.None)
                        events = events.Where(e => filter.FileEvents.HasFlag(e.FileEvent));
                    return new ReadOnlyCollection<CEventInfo>(events
                        .OrderByDescending(e => e.TimeCreatedUtc)
                        .Take(filter.Limit)
                        .Include(e => e.ClientInfo)
                        .ToList());
                }, out ReadOnlyCollection<CEventInfo> searchedEvents);
                return searchedEvents;
            }
        }

        public bool WriteEventInfo(CEventInfo eventInfo)
        {
            bool success = false;
            using (var context = _contextFactory.Get<CDbContext>())
            {
                context.CallWithTransaction(con =>
                {
                    success = con.TryAdd(eventInfo) && con.TryExecute("sp_IncrementEventTriggerCount @p0", new object[] { eventInfo.FileEvent.ToString()});
                    return success;
                });
            }
            return success;
        }

        public bool SendNotification(CEventInfo eventInfo, CRule rule)
        {
            bool success = false;
            try
            {
                _notificationService.Send(ESenders.Smtp, new CMessage
                {
                    Receiver = rule.Email,
                    Body = $"New event with file. Event: {eventInfo}, Rule: {rule}",
                    Subject = $"New event with file {eventInfo.FileEvent}"
                });
                success = true;
            }
            catch (Exception ex)
            {
                s_logger.Error(ex);
            }
            return success;
        }

        public bool ExportEventsToCsv(List<CEventInfo> events, string path)
        {
            bool success = false;
            try
            {
                _exportService.ExportEventsToFile(EExportFormat.Csv, events, path);
                success = true;
            }
            catch (Exception ex)
            {
                s_logger.Error(ex);
            }
            return success;
        }
    }
}
