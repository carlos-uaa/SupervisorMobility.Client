namespace SupervisorMobility.Client.Data.Entities.SOSAnalysis_Process
{
    public class ToolUsed
    {
        public int ToolUsedId { get; set; }

        public int ToolId { get; set; }
        public Tool Tool { get; set; }

        public double Quantity { get; set; }

        public bool? IsActive { get; set; }
    }
}
