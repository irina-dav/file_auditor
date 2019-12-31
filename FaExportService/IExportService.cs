using System.Collections.Generic;
using System.ServiceModel;
using Infrastructure.Models;

namespace FaExportService
{
    [ServiceContract]
    public interface IExportService
    { 
        [OperationContract]
        void ExportEventsToFile(EExportFormat exportType, List<CEventInfo> events, string path);
    }
}
