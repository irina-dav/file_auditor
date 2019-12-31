using Infrastructure.Extensions;
using Infrastructure.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using NLog;

namespace FaClient
{
    public class CEventLogWatcher : IDisposable
    {
        private static readonly Logger s_logger = LogManager.GetCurrentClassLogger();

        private readonly EventLogWatcher _watcher;        
        private readonly List<CRule> _rules;
        private readonly Guid _clientId;

        public event EventHandler<CEventRecordArgs> OnNewEventRecord;
        private readonly ConcurrentQueue<EventRecord> _recordsQueue;
        private readonly ManualResetEvent _mreNewRecord;
        private readonly ManualResetEvent _mreStop;
        private readonly Thread _analyzer;

        public CEventLogWatcher(List<CRule> rules, Guid clientId)
        {
            _rules = rules;
            _clientId = clientId;

            _recordsQueue = new ConcurrentQueue<EventRecord>();

            _mreNewRecord = new ManualResetEvent(false);
            _mreStop = new ManualResetEvent(false);

            _analyzer = new Thread(AnalyzeQueueRecords) {IsBackground = true};

            EventLogSession eventLogSession = new EventLogSession();
            EventLogQuery eventLogQuery = new EventLogQuery("Security", PathType.LogName, $"*[System/EventID={(int)ESecurityEventType.AttemtToAccess}]")
            {
                Session = eventLogSession
            };
            _watcher = new EventLogWatcher(eventLogQuery);
            _watcher.EventRecordWritten += EventRecordWritten;
        }

        public void Dispose()
        {
            _watcher.Dispose();
            _mreNewRecord.Dispose();
            _mreStop.Dispose();
        }

        public void StartWatcher()
        {
            try
            {
                _analyzer.Start();
                _mreNewRecord.Reset();
                _watcher.Enabled = true;
                s_logger.Trace("Started watcher");
            }
            catch (Exception ex)
            {
                s_logger.Error(ex);
                throw;
            }
        }

        public void StopWatcher()
        {
            try
            {
                _watcher.Enabled = false;
                _mreStop.Set();
                _analyzer.Join();
                s_logger.Trace("Stopped watcher");
            }
            catch (Exception ex)
            {
                s_logger.Error(ex);
                throw;
            }
        }

        private void EventRecordWritten(object sender, EventRecordWrittenEventArgs e)
        {
            _recordsQueue.Enqueue(e.EventRecord);
            _mreNewRecord.Set();
        }


        private void AnalyzeQueueRecords()
        {
            while (true)
            {
                int waitResult = WaitHandle.WaitAny(new WaitHandle[] { _mreStop, _mreNewRecord });
                if (waitResult == 0)
                {
                    break;
                }
                if (_recordsQueue.IsEmpty)
                    _mreNewRecord.Reset();

                bool deq = _recordsQueue.TryDequeue(out var record);
                if (!deq) continue;

                var task = new Task(() => { AnalyzeRecord(record); });
                task.Start();
            }
        }

        private void AnalyzeRecord(EventRecord record)
        {
            CEventInfo eventInfo = new CEventInfo(record, _clientId);
            if (eventInfo.AccessMask.HasFlag(EAccessMask.Delete))
            {
                eventInfo = AnalyzeEventDelete(eventInfo);
            }
            else
            {
                eventInfo = AnalyzeEventAccess(eventInfo);
            }

            if (IsRelevantEvent(eventInfo, out CRule connectedRule))
            {
               s_logger.Trace($"Detected new relevant event: {eventInfo.EventRecordId}, {eventInfo.FileEvent.ToString()}, {eventInfo.ObjectName}");
               OnNewEventRecord?.Invoke(this, new CEventRecordArgs() {EventInfo = eventInfo, Rule = connectedRule});
            }
        }

        private bool IsRelevantEvent(CEventInfo eventInfo, out CRule connectedRule)
        {
            connectedRule = null;

            if (eventInfo.FileEvent == EFileEvents.None)
                return false;

            var eventSystemPath = new CFileSystemPath(Path.GetDirectoryName(eventInfo.ObjectName));
            var rulesSameDirectory = _rules.Where(r => r.FolderPath.Equals(eventSystemPath));
            foreach (var rule in rulesSameDirectory)
            {
               
                if (!rule.FileEvents.HasFlag(eventInfo.FileEvent))
                    continue;

                string fileName = eventInfo.ObjectName;
                if (!string.IsNullOrEmpty(rule.MasksExclude))
                {
                    var excludeMasks = rule.MasksExclude.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
                    if (FileNameMatchesAnyMask(fileName, excludeMasks))
                        continue;
                }

                if (!string.IsNullOrEmpty(rule.MasksInclude))
                {
                    var includeMasks = rule.MasksInclude.Split(Convert.ToChar(Environment.NewLine));
                    if (FileNameMatchesAnyMask(fileName, includeMasks))
                    {
                        connectedRule = rule;
                        return true;
                    }
                }
                else
                {
                    connectedRule = rule;
                    return true;
                }
            }
            return false;
        }

        private bool FileNameMatchesAnyMask(string fileName, string[] masks)
        {
            return masks.Any(m => FileNameMatchesMask(fileName, m));
        }

        private bool FileNameMatchesMask(string fileName, string fileMask)
        {
            Regex mask = new Regex(fileMask.Replace(".", "[.]").Replace("*", ".*").Replace("?", "."));
            return mask.IsMatch(fileName);
        }

        // 4660 - event about deleted file but without file name,
        // 4663 - event only about attempt, but contains all info, and it raises always before 4660 (immediately, EventRecordId differ by 1)
        // so we need to use both events
        private CEventInfo AnalyzeEventDelete(CEventInfo eventInfo)
        {
            var nextEventRecordId = eventInfo.EventRecordId + 1;
            var handleId = eventInfo.HandleId;
            var fileEventRecord = EFileEvents.None;
            EventLogQuery query = new EventLogQuery("Security", PathType.LogName, $"*[System/EventRecordID={nextEventRecordId}]");
            EventLogReader reader = new EventLogReader(query);
            EventRecord recordNext = reader.ReadEvent();
            if (recordNext != null)
            {
                CEventInfo eventNext = new CEventInfo(recordNext, _clientId);
                if (handleId.Equals(eventNext.HandleId))
                {
                    if (eventNext.EventId == (int) ESecurityEventType.OdjectWasDeleted)
                    {
                        fileEventRecord = fileEventRecord.AddValue(EFileEvents.Deleted);
                    }
                }
                else
                {
                    fileEventRecord = fileEventRecord.AddValue(EFileEvents.Renamed);
                }
            }
            else
            {
                fileEventRecord = EFileEvents.None;
            }

            eventInfo.FileEvent = fileEventRecord;
            return eventInfo;
        }

        private CEventInfo AnalyzeEventAccess(CEventInfo eventInfo)
        {
            var fileEventRecord = EFileEvents.None;

            if (eventInfo.AccessMask.HasFlag(EAccessMask.WriteData))
                fileEventRecord = fileEventRecord.AddValue(EFileEvents.Created);

            if (eventInfo.AccessMask.HasFlag(EAccessMask.AppendData))
                fileEventRecord = fileEventRecord.AddValue(EFileEvents.Changed);

            eventInfo.FileEvent = fileEventRecord;
            return eventInfo;
        }
    }
}
