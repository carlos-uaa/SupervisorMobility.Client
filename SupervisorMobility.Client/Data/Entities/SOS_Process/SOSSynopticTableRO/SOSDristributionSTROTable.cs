namespace SupervisorMobility.Client.Data.Entities.SOS_Process.SOSSynopticTableRO
{
    public class SOSDristributionSTROTable
    {
        public int SOSDistributionId { get; set; }
        public string? InternalControlNumber { get; set; }
        public string? OperationName { get; set; }
        public string? ProcessName { get; set; }
        public string? ControlNumber { get; set; }
        public DateTime? CreatedAt { get; set; }
        public bool? IsActive { get; set; }
        public int? SOSHubId { get; set; }
        public IEnumerable<SOSHub>? SOSHubs { get; set; } = new List<SOSHub>();
        public IEnumerable<SOSAnalysis>? Analyses { get; set; } = new List<SOSAnalysis>();
        public IEnumerable<SOSSequence>? Sequences { get; set; } = new List<SOSSequence>();
        public ICollection<SOSDistributionOperationSequence>? SOSDistributionOperationSequence { get; set; }
    }
}
