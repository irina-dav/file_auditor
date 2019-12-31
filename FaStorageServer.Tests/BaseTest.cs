using Infrastructure.Models;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;

namespace FaStorageServer.Tests
{
    public class BaseTest
    {
        protected CFaStorageService _faStorageService;

        [SetUp]
        public void Setup()
        {
            var clients = CreateClients().AsQueryable();
            var rules = CreateRules().AsQueryable();
            var events = CreateEvents(clients).AsQueryable();

            var clientInfoMockSet = new Mock<DbSet<CClientInfo>>();
            clientInfoMockSet.As<IQueryable<CClientInfo>>().Setup(m => m.Expression).Returns(clients.Expression);
            clientInfoMockSet.As<IQueryable<CClientInfo>>().Setup(m => m.ElementType).Returns(clients.ElementType);
            clientInfoMockSet.As<IQueryable<CClientInfo>>().Setup(m => m.GetEnumerator()).Returns(clients.GetEnumerator);
            clientInfoMockSet.As<IQueryable<CClientInfo>>().Setup(m => m.Provider).Returns(clients.Provider);

            var rulesMockSet = new Mock<DbSet<CRule>>();
            rulesMockSet.As<IQueryable<CRule>>().Setup(m => m.Expression).Returns(rules.Expression);
            rulesMockSet.As<IQueryable<CRule>>().Setup(m => m.ElementType).Returns(rules.ElementType);
            rulesMockSet.As<IQueryable<CRule>>().Setup(m => m.GetEnumerator()).Returns(rules.GetEnumerator);
            rulesMockSet.As<IQueryable<CRule>>().Setup(m => m.Provider).Returns(rules.Provider);

            var eventsMockSet = new Mock<DbSet<CEventInfo>>();
            eventsMockSet.As<IQueryable<CEventInfo>>().Setup(m => m.Expression).Returns(events.Expression);
            eventsMockSet.As<IQueryable<CEventInfo>>().Setup(m => m.ElementType).Returns(events.ElementType);
            eventsMockSet.As<IQueryable<CEventInfo>>().Setup(m => m.GetEnumerator()).Returns(events.GetEnumerator);
            eventsMockSet.As<IQueryable<CEventInfo>>().Setup(m => m.Provider).Returns(events.Provider);

            var mockContext = new Mock<CDbContext>();
            CDbContext.ConnectionString = ("empty");
            mockContext.Setup(m => m.Clients).Returns(clientInfoMockSet.Object);
            mockContext.Setup(m => m.Rules).Returns(rulesMockSet.Object);
            mockContext.Setup(m => m.Events).Returns(eventsMockSet.Object);
            mockContext.Setup(m => m.Set<CRule>()).Returns(rulesMockSet.Object);

            var contextFactory = new CContextFactory();
            contextFactory.Register(mockContext.Object);
            _faStorageService = new CFaStorageService(contextFactory);
        }

        [OneTimeTearDown]
        public void TearDown()
        {
        }

        private List<CRule> CreateRules()
        {
            return new List<CRule>
            {
                CRule.CreateRule(Guid.NewGuid(),
                    "C:/Folder1",
                    true,
                    "test@domain.com",
                    true,
                    "*.txt",
                    "*.tmp",
                    EFileEvents.Changed | EFileEvents.Renamed,
                    ERuleState.Active,
                    new Guid()
                ),
                CRule.CreateRule(Guid.NewGuid(),
                    "C:/Folder2",
                    false,
                    "",
                    false,
                    "",
                    "",
                    EFileEvents.Deleted,
                    ERuleState.Paused,
                    new Guid()
                ),
                CRule.CreateRule(Guid.NewGuid(),
                    "C:/Folder3",
                    true,
                    "test@domain.com",
                    true,
                    "*.xls\n*.pdf",
                    "*.tmp",
                    EFileEvents.Changed | EFileEvents.Renamed | EFileEvents.Created | EFileEvents.Deleted,
                    ERuleState.Active,
                    new Guid()
                )
            };
        }

        private List<CClientInfo> CreateClients()
        {
            Random rnd = new Random();
            return new List<CClientInfo>
            {
                new CClientInfo(Guid.NewGuid(),
                    "ComputerFirst",
                    "10.50.1.75",
                    DateTime.UtcNow.AddMinutes(-rnd.Next(0, 200)),
                    EClientState.Paused
                ),
                new CClientInfo(Guid.NewGuid(),
                    "ComputerSecond",
                    "10.50.1.76",
                    DateTime.UtcNow.AddMinutes(-rnd.Next(0, 200)),
                    EClientState.Active
                ),
                new CClientInfo(Guid.NewGuid(),
                    "ComputerThird",
                    "10.50.1.77",
                    DateTime.UtcNow.AddMinutes(-rnd.Next(0, 200)),
                    EClientState.Active
                )
            };
        }

        Random rnd = new Random();
        Array masks = Enum.GetValues(typeof(EAccessMask));
        Array fileEvents = Enum.GetValues(typeof(EFileEvents));

        private List<CEventInfo> CreateEvents(IQueryable<CClientInfo> clients)
        {
            int eventsSetMax = 10;
            int eventId = (int)ESecurityEventType.OdjectWasDeleted;
            var events = new List<CEventInfo>();
            // creating events for all clients except first
            foreach (var client in clients.Skip(1))
            {
                for (int j = 0; j < eventsSetMax; j++)
                {
                    events.Add(CreateEvent(client, eventId));
                }
            }

            ;
            return events;
        }

        protected CEventInfo CreateEvent(CClientInfo client, int eventId)
        {
            return new CEventInfo(Guid.NewGuid(), eventId, rnd.Next(10000),
                DateTime.UtcNow.AddMinutes(rnd.Next(-1000, 1000)), client.ComputerName,
                "user", "domain", Path.GetRandomFileName(),
                (EAccessMask) masks.GetValue(rnd.Next(masks.Length)),
                (EFileEvents) fileEvents.GetValue(rnd.Next(fileEvents.Length)),
                rnd.Next(10000).ToString(), client.ClientInfoId);
        }
    }
}

