using System;

namespace Infrastructure.Models
{
    public class CSearchFilter
    {
        public string ComputerName { get; set; }

        public string FileName { get; set; }

        public string UserName { get; set; }

        public string IpAddress { get; set; }

        public int Limit { get; set; }

        public DateTime? PeriodStart { get; set; }

        public DateTime? PeriodEnd { get; set; }

        public EFileEvents FileEvents { get; set; }

    }
}
