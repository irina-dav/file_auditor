using Infrastructure.Extensions;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml.Linq;

namespace Infrastructure.Models
{
    [Table("EventInfo")]
    public class CEventInfo
    {
        private static readonly XNamespace s_eventXNamespace = "http://schemas.microsoft.com/win/2004/08/events/event";

        private PropertyInfo[] _propertyInfos = null;

        // parameter-less constructor for Entity Framework
        private CEventInfo()
        {
        }

        public CEventInfo(EventRecord record, Guid clientId)
        {
            XDocument xml = XDocument.Parse(record.ToXml());
            var nodes = xml.Descendants(s_eventXNamespace + "Data")
                .Select(n => new { Name = n.Attribute("Name")?.Value, Value = n.Value.ToString() });
            var attributes = nodes.ToDictionary(n => n.Name, n => n.Value);
            ObjectName = attributes.GetValueOrDefault("ObjectName");
            UserName = attributes.GetValueOrDefault("SubjectUserName");
            DomainName = attributes.GetValueOrDefault("SubjectDomainName");
            AccessMask = (EAccessMask)Convert.ToInt32(attributes.GetValueOrDefault("AccessMask"), 16);
            FileEvent = EFileEvents.None;
            HandleId = attributes.GetValueOrDefault("HandleId");
            Computer = record.MachineName;
            EventId = record.Id;
            EventRecordId = record.RecordId;
            TimeCreatedUtc = record.TimeCreated ?? DateTime.UtcNow;
            ClientInfoId = clientId;
        }

        public CEventInfo(Guid eventInfoId, int eventId, long? eventRecordId, DateTime dtCreatedUtc, string computer,
            string userName, string domainName, string objectName,
            EAccessMask accessMask, EFileEvents fileEvent, string handleId, Guid clientId)
        {
            EventInfoId = eventInfoId;
            ObjectName = objectName;
            UserName = userName;
            DomainName = domainName;
            AccessMask = accessMask;
            FileEvent = fileEvent;
            HandleId = handleId;
            Computer = computer;
            EventId = EventId;
            EventRecordId = eventRecordId;
            TimeCreatedUtc = dtCreatedUtc;
            ClientInfoId = clientId;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid EventInfoId { get; set; }

        public int EventId { get; set; }            // type of event, e.g 4663, 4664 (included in event log entry)

        public long? EventRecordId { get; set; }    // index number of the event in Windows Event log (included in event log entry)

        public DateTime TimeCreatedUtc { get; set; }

        public string Computer { get; set; }

        public string UserName { get; set; }

        public string DomainName { get; set; }

        public string ObjectName { get; set; }

        public EAccessMask AccessMask { get; set; }

        public EFileEvents FileEvent { get; set; }

        public string HandleId { get; set; }

        public Guid ClientInfoId { get; set; }

        public virtual CClientInfo ClientInfo { get; set; }

        public override string ToString()
        {
            if (_propertyInfos == null)
                _propertyInfos = GetType().GetProperties();

            var sb = new StringBuilder();

            foreach (var info in _propertyInfos)
            {
                var value = info.GetValue(this, null) ?? "(null)";
                sb.Append($"{info.Name} : {value} ");
            }

            return sb.ToString();
        }
    }
}
