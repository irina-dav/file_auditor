using Infrastructure;
using Infrastructure.Models;
using System.Globalization;

namespace FaReport.ViewModels
{
    class CEventViewModel : CBaseObservableObject
    {
        public readonly CEventInfo _eventInfo;

        public CEventViewModel(CEventInfo eventInfo)
        {
            _eventInfo = eventInfo;
        }

        public string Computer => $"{_eventInfo.ClientInfo.ComputerName}, {_eventInfo.ClientInfo.IpAddress}";

        public string File => _eventInfo.ObjectName;

        public string FileEvent => _eventInfo.FileEvent.ToString();

        public string EventDateTime => _eventInfo.TimeCreatedUtc.ToString(CultureInfo.InvariantCulture);

        public string UserName => _eventInfo.UserName;

        public CEventInfo EventInfo => _eventInfo;

    }
}
