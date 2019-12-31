using System;
using System.Windows;
using FaReport.Controllers;
using NLog;

namespace FaReport
{
    public partial class App : Application
    {
        private static readonly Logger s_logger = LogManager.GetCurrentClassLogger();

        private void ApplicationStartup(object sender, StartupEventArgs e)
        {
            AppDomain.CurrentDomain.UnhandledException += HandleException;

            var storageService = new FaStorageService.FaStorageServiceClient();

            CMainController controller = new CMainController(storageService);

            controller.Start();
        }


        static void HandleException(object sender, UnhandledExceptionEventArgs args)
        {
            Exception ex = (Exception)args.ExceptionObject;
            MessageBox.Show("An unexpected error occurred, see the log for details.", "Error",
                MessageBoxButton.OK, MessageBoxImage.Error);
            s_logger.Error(ex);
        }
    }
}
