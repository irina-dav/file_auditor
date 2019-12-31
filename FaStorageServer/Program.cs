using System;
using System.ServiceModel;
using NLog;

namespace FaStorageServer
{
    class Program
    {
        private static readonly Logger s_logger = LogManager.GetCurrentClassLogger();

        static void Main(string[] args)
        {
            try
            {
                CContextFactory factory = new CContextFactory();
                factory.Register<CDbContext>();
                CFaStorageService faStorageService = new CFaStorageService(factory);
                using (ServiceHost host = new ServiceHost(faStorageService))
                {
                    host.Open();
                    Console.WriteLine($"{nameof(CFaStorageService)} service is started...");
                    Console.ReadLine();
                    host.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An exception occurred: {ex.Message}");
                s_logger.Error(ex);
                Console.ReadLine();
            }
        }
    }
}
