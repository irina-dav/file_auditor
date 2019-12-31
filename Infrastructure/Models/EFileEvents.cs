using System;
using System.Runtime.Serialization;

namespace Infrastructure.Models
{
    [Flags]
    public enum EFileEvents
    {
        None = 0,
        Created = 1,
        Deleted = 2,
        Renamed = 4,
        Changed = 8
    }
}
