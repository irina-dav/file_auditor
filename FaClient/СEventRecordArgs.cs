using Infrastructure.Models;
using System;

namespace FaClient
{
    public class CEventRecordArgs : EventArgs
    {
        public CEventInfo EventInfo { get; set; }
        public CRule Rule { get; set; }
    }
}
