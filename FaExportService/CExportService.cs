using Infrastructure;
using System;
using System.Collections.Generic;
using System.ServiceModel;
using Infrastructure.Models;
using NLog;

namespace FaExportService
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class CExportService : IExportService
    {
        private static readonly Logger s_logger = LogManager.GetCurrentClassLogger();

        public void ExportEventsToFile(EExportFormat exportType, List<CEventInfo> items, string path)
        {
            try
            {
                IExporter exporter = null;
                switch (exportType)
                {
                    case EExportFormat.Csv:
                        exporter = new CCsvExport<CEventInfo>(items);
                        break;
                    default:
                        s_logger.Warn("Unknown export format");
                        break;
                }

                if (exporter == null)
                    throw new NullReferenceException();

                exporter.ExportToFile(path);
            }
            catch (Exception ex)
            {
                s_logger.Error(ex);
                throw;
            }
        }
    }
}
