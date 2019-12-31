using System.Configuration;

namespace FaStorageServer
{
    public static class SSettings
    {
        public static ConnectionStringSettings ConnectionStringDb => ConfigurationManager.ConnectionStrings["ConnectionStringDb"];
    }
}
