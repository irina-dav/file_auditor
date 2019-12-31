using System;
using System.Runtime.Serialization;

namespace Infrastructure.Models
{
    [Flags]
    public enum EAccessMask
    {
        Unknown = 0x0,
        ReadData = 0x1,
        WriteData = 0x2,
        AppendData = 0x4,
        ReadExtAttr = 0x8,
        WriteExtAttr = 0x10,
        Execute = 0x20,
        DeleteChild = 0x40,
        ReadAttributes = 0x80,
        WriteAttributes	= 0x100,
        Delete = 0x10000,
        ReadControl = 0x20000,
        WriteDac =	0x40000,
        WriteOwner = 0x80000,
        Synchronize = 0x100000
    }
}