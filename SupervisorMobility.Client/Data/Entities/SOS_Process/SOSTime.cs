namespace SupervisorMobility.Client.Data.Entities.SOS_Process
{
    public class SOSTime
    {
        public int SOSTimeId { get; set; }
        public int SectionId { get; set; }
        public int? AnalysisId { get; set; }
        public string? Time { get; set; } = "";

        public bool? IsActive { get; set; }
    }
}