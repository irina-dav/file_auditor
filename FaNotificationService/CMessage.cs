using System.Runtime.Serialization;

namespace FaNotificationService
{
    [DataContract]
    public class CMessage
    {
        [DataMember]
        public string Receiver { get; set; }

        [DataMember]
        public string Subject { get; set; }

        [DataMember]
        public string Body { get; set; }
    }
}
