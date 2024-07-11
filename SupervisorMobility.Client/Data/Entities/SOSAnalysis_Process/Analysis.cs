namespace SupervisorMobility.Client.Data.Entities.SOSAnalysis_Process
{
    public class Analysis
    {
        public int AnalysisId { get; set; }
        public string Text { get; set; }
        public List<string> CriticalPoints { get; set; } = new List<string>();
        public List<string> Reasons { get; set; } = new List<string>();
        public bool? IsActive { get; set; }
    }
}