using System;
using System.ServiceModel;
using NLog;

namespace FaNotificationService
{
    class Program
    {
        private static readonly Logger s_logger = LogManager.GetCurrentClassLogger();

        static void Main(string[] args)
        {
            try
            {
                CNotificationService notificationService = new CNotificationService();
                using (ServiceHost host = new ServiceHost(notificationService))
                {
                    host.Open();
                    Console.WriteLine($"{nameof(CNotificationService)} service is started...");
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
