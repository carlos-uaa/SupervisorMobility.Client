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
        //Get All AssyChartsByPlant
        Task<List<AssyChart>> GetAssyChartsByPlant(int plantId);
        //Get All AssyChartsByArea
        Task<List<AssyChart>> GetAssyChartsByArea(int plantId, int areaId);
        //get all assycharts by distribution
        Task<List<AssyChart>> GetAssyChartsByDistribution(int plantId, int areaId, int distributionId);

        //UPDATE
        Task UpdateAssyChart(int assychartId, AssyChart _newAssyChart);

        //DELETE
        Task DeleteAssyChart(int assychartId);
       
    }
}
