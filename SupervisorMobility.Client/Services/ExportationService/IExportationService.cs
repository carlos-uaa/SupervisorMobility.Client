namespace SupervisorMobility.Client.Services.ExportationService
{
    public interface IExportationService
    {
        Task ExportAnalysisToExcel(int idAnalysis);
    }
}
