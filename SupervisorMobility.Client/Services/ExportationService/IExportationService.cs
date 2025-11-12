namespace SupervisorMobility.Client.Services.ExportationService
{
    public interface IExportationService
    {
        Task ExportAnalysisToExcel(int idAnalysis);
        Task ExportSequenceToExcel(int idSequence);
        Task ExportDistributionToExcel(int idDistribution);
        Task ExportYearlyPATToExcel(int idPAT);
        Task ExportMonthlyPATToExcel(int idPAT, int month);
        Task ExportHCIToExcel(int idHCI);
        Task ExportFlowToExcel (int idFlow, MultipartFormDataContent content);
        Task ExportCombinatationToExcel(int combinationId);
    }
}
