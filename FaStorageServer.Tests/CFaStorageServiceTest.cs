using FluentAssertions;
using Infrastructure.Models;
using NUnit.Framework;
using System.Linq;

namespace FaStorageServer.Tests
{
    [TestFixture]
    public class CFaStorageServiceTest : BaseTest
    {
        [Test]
        public void FindClientsWithLastDate_FoundAtLeastOneClient()
        {
            var clients = _faStorageService.FindClientsWithLastDate();

            clients.Should().NotBeNullOrEmpty();
            clients.Count.Should().BeGreaterOrEqualTo(1);
        }

        [Test]
        public void FindClientsWithLastDate_FoundOneClientsWithNullDateTime()
        {
            var clients = _faStorageService.FindClientsWithLastDate();

            clients.All(c => c.LastEventDateTimeUtc.HasValue).Should().BeFalse();
        }

        [Test]
        public void FindAllRules_FoundAtLeastOneRule()
        {
            var rules = _faStorageService.FindAllRules();

            rules.Should().NotBeNullOrEmpty();
            rules.Count.Should().BeGreaterOrEqualTo(1);
        }

        [Test]
        public void WriteEventInfo_TransactionWithFail()
        {
            var clients = _faStorageService.FindClientsWithLastDate();
            CEventInfo newEventInfo = CreateEvent(clients.First(), (int)ESecurityEventType.OdjectWasDeleted);

            bool success = _faStorageService.WriteEventInfo(newEventInfo);

            success.Should().BeFalse();
        }
    }
}
