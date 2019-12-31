using Infrastructure.Models;

namespace Infrastructure.Extensions
{
    public static class CFileEventsExtension
    {
        public static EFileEvents RemoveValue(this EFileEvents flags, EFileEvents value) 
        {
            return flags & ~value;
        }

        public static EFileEvents AddValue(this EFileEvents flags, EFileEvents value)
        {
            return flags | value;
        }
    }
}
