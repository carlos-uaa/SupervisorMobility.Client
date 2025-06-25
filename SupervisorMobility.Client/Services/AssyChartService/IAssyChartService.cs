namespace SupervisorMobility.Client.Services.AssyChartService
{
    public interface IAssyChartService
    {
        //CREATE
        //Create AssyChart
        Task<AssyChart> CreateAssyChart(AssyChart _newAssyChart);

        //READ
        //get all AssyCharts
        Task<List<AssyChart>> GetAssyCharts();
        //get Assy Chart by Id
        Task<AssyChart> GetAssyChart(int assyChartId);
        Task<AssyChart> GetAssyChartAdvance(int plantId, int areaId, int distributionId, int operationId);
        //Get All AssyChartsByPlant
        Task<List<AssyChart>> GetAssyChartsByPlant(int plantId);
        //Get All AssyChartsByArea
        Task<List<AssyChart>> GetAssyChartsByArea(int plantId, int areaId);
        //get all assycharts by distribution
        Task<List<AssyChart>> GetAssyChartsByDistribution(int plantId, int areaId, int distributionId);
        //get one assyu
        Task<AssyChart> GetAssyChartJobObservation(int plantId, int areaId, int distributionId);

        //UPDATE
        Task<bool> UpdateAssyChart(int assychartId, AssyChart _newAssyChart);

        //DELETE
        Task DeleteAssyChart(int assychartId);

        Task DownloadAssyChartFormat();


        //CODE PATHS

        Task<SOSCodePath> CreateCodePath(SOSCodePath _newCodePath);
        Task<List<SOSCodePath>> GetAllCodePaths();
        Task<SOSCodePath?> GetCodePath(int CodePathId);
        Task<bool> UpdateCodePath(int CodePathId, SOSCodePath codePath);


    }
}
