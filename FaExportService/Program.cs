using System;
using System.ServiceModel;
using NLog;

namespace FaExportService
{
    class Program
    {
        private static readonly Logger s_logger = LogManager.GetCurrentClassLogger();

        static void Main(string[] args)
        {
            try
            {
                CExportService exportService = new CExportService();
                using (ServiceHost host = new ServiceHost(exportService))
                {
                    host.Open();
                    Console.WriteLine($"{nameof(CExportService)} service is started...");
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
