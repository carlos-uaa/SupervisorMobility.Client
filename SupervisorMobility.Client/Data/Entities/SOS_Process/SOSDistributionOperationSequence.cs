namespace SupervisorMobility.Client.Data.Entities.SOS_Process
{
    public class SOSDistributionOperationSequence
    {
        public int SOSDistributionOperationSequenceId { get; set; }
        public int? SequenceId { get; set; }

        public int? SectionId { get; set; }
        public Section? Section { get; set; }

        public string? Times { get; set; } = "";
        public bool? IsActive { get; set; }
    }
}