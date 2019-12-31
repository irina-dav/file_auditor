namespace FaExportService
{
    public interface IExporter
    {
       string ExportToString();
       void ExportToFile(string path);
    }
}
