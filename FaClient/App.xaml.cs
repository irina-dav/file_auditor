using FaClient.Controllers;
using Infrastructure.Helpers;
using Infrastructure.Models;
using System;
using System.Windows;
using NLog;

namespace FaClient
{
    public partial class App : Application
    {
        private static readonly Logger s_logger = LogManager.GetCurrentClassLogger();

        private void ApplicationStartup(object sender, StartupEventArgs e)
        {
            AppDomain.CurrentDomain.UnhandledException += HandleException;
            
            var storageService = new FaStorageService.FaStorageServiceClient();

            string computerName = SNetworkHelper.GetComputerName();
            string ipAddress = SNetworkHelper.GetLocalIp();
            CClientInfo clientInfo = storageService.RegisterClient(computerName, ipAddress);

            CRuleController controller = new CRuleController(storageService, clientInfo);

            controller.Start();
        }

        static void HandleException(object sender, UnhandledExceptionEventArgs args)
        {
            Exception ex = (Exception)args.ExceptionObject;
            MessageBox.Show("An unexpected error occurred, see the log for details.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            s_logger.Error(ex);
        }
    }
}
