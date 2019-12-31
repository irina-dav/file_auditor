using System.Net;
using System.Net.Sockets;

namespace Infrastructure.Helpers
{
    public static class SNetworkHelper
    {
        public static string GetLocalIp()
        {
            string localIp = "";
            try
            {
                using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, 0))
                {
                    socket.Connect("8.8.8.8", 65530);
                    IPEndPoint endPoint = (IPEndPoint) socket.LocalEndPoint;
                    if (endPoint != null) localIp = endPoint.Address.ToString();
                }
            }
            catch
            {
                localIp = "undefined";
            }

            return localIp;
        }

        public static string GetComputerName()
        {
            return Dns.GetHostName();
        }
    }
}
