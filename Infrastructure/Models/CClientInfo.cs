using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Infrastructure.Models
{
    [Table("Client")]
    public class CClientInfo
    {
        // parameter-less constructor for Entity Framework
        private CClientInfo()
        {
        }

        public CClientInfo(Guid clientInfoId, string computerName, string ipAddress, DateTime lastEventDateTime, EClientState state)
        {
            ClientInfoId = clientInfoId;
            ComputerName = computerName;
            IpAddress = ipAddress;
            LastEventDateTimeUtc = lastEventDateTime;
            State = state;
        }

        public CClientInfo(string computerName, string ipAddress)
        {
            ClientInfoId = Guid.Empty;
            ComputerName = computerName;
            IpAddress = ipAddress;
            LastEventDateTimeUtc = null;
            State = EClientState.Unknown;
        }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public Guid ClientInfoId { get; set; }

        public string ComputerName { get; set; }

        public string IpAddress { get; set; }

        public DateTime? LastEventDateTimeUtc { get; set; }

        public EClientState State { get; set; }

        public override bool Equals(object obj)
        {
            if (obj == null || !GetType().Equals(obj.GetType()))
            {
                return false;
            }
            CClientInfo clientInfo = (CClientInfo)obj;

            return string.Equals(ComputerName, clientInfo.ComputerName, StringComparison.OrdinalIgnoreCase) 
                   && IpAddress.Equals(clientInfo.IpAddress);
        }

        public override int GetHashCode() => new { ComputerName, IpAddress }.GetHashCode();
    }
}
