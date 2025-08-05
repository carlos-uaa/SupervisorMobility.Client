namespace SupervisorMobility.Client.Data.Entities.SOS_Process
{
    public class SOSSynopticRequirementsOperationSequence
    {
        public int SOSSynopticRequirementsOperationSequenceId { get; set; }
        public int? Sequence { get; set; }

        public int? SectionId { get; set; }
        public Section? Section { get; set; }

        public string? Times { get; set; } = "";
        public bool? IsActive { get; set; }
    }
}