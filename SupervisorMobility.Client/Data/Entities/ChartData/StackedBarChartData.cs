namespace SupervisorMobility.Client.Data.Entities.ChartData
{
    public class StackedBarChartData
    {
        public string label {  get; set; }
        public int[] data { get; set; }
        public string backgroundColor {  get; set; }
        public string stack {  get; set; }
    }
}
