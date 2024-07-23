namespace SupervisorMobility.Client.Data.Entities.SOSAnalysis_Process
{
    public class CommonDirection
    {
        public int CommonDirectionId { get; set; }
        public int DOC_ID { get; set; }
        public string route { get; set; } = string.Empty;
        public string name { get; set; } = string.Empty;
        public int type { get; set; } //To check origin of file (CCP/GOS)
        public bool IsActive { get; set; } = true;
    }
}
