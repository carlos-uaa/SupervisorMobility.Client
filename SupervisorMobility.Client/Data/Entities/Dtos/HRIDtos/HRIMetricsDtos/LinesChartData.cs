using MudBlazor;

namespace SupervisorMobility.Client.Data.Entities.Dtos.HRIDtos.HRIMetricsDtos
{
    public class LinesChartData
    {
        public string[] Labels { get; set; }
        public List<ChartSeries> Series { get; set; }
    }
}
