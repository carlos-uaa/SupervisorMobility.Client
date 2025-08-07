namespace SupervisorMobility.Client.Services.MetricsService
{
    public interface IMetricsService
    {
        Task<int> GetTotalJobObs(MetricsFiltersDto filters);
        Task<Dictionary<string, int>> GetJobsStatusChartData(MetricsFiltersDto filters);
        Task<Dictionary<string, int>> GetJobsTypeChartData(MetricsFiltersDto filters);
        Task<Dictionary<string, int>> GetLUPData(MetricsFiltersDto filters);
        Task<Dictionary<string, int>> GetLUPProgressData(MetricsFiltersDto filters);
        Task<int> GetTotalLUPs(MetricsFiltersDto filters);
    }
}
