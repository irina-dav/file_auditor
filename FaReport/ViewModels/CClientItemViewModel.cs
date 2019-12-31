using System.Globalization;
using Infrastructure;
using Infrastructure.Models;

namespace FaReport.ViewModels
{
    class CClientItemViewModel : CBaseObservableObject
    {
        public CClientItemViewModel(CClientInfo client)
        {
            Client = client;
        }

        public CClientInfo Client { get; }

        public string Name => Client.ComputerName;

        public string IpAddress => Client.IpAddress;

        public string LastEventDateTime => Client.LastEventDateTimeUtc?.ToString(CultureInfo.InvariantCulture);

        public string State => Client.State.ToString();

    }
}
