using Infrastructure.Models;
using System;
using System.Collections.ObjectModel;

namespace Infrastructure.Store
{
    public interface IEventsStore
    {
        void WriteEventInfo(CEventInfo eventInfo);

        ReadOnlyCollection<CEventInfo> FindLastEvents(int limit);

        CEventInfo FindEventInfoById(Guid eventInfoId);
    }
}
