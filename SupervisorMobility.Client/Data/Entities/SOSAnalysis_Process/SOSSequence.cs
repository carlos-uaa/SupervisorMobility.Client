namespace SupervisorMobility.Client.Data.Entities.SOSAnalysis_Process
{
    public class SOSSequence
    {
        public int SOSSequenceId { get; set; }


        public string? InternalControlNumber { get; set; }
        public string? OperationName { get; set; }
        public string? ProcessName { get; set; }

        public ICollection<SOSSequenceLogbook>? SequenceLogbooks { get; set; } = new List<SOSSequenceLogbook>();

        public ICollection<FileUpload>? Illustrations { get; set; } = new List<FileUpload>();
        public ICollection<Commentary>? Notes { get; set; } = new List<Commentary>();
        public ICollection<SpecialCaseAbnormalSituation>? SpecialCasesAbnormalSituations { get; set; } = new List<SpecialCaseAbnormalSituation>();

        public DateTime? CreatedDate { get; set; }

        public bool? IsActive { get; set; }

        public int SOSHubId { get; set; }
        public SOSHub? SOSHub { get; set; }
    }
}
