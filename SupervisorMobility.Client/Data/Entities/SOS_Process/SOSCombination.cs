namespace SupervisorMobility.Client.Data.Entities.SOS_Process
{
    public class SOSCombination
    {
        public int SOSCombinationId { get; set; }
        public bool? IsActive { get; set; }
        public string? InternalControlNumber { get; set; }
        public string? OperationName { get; set; }
        public string? ProcessName { get; set; }
        public int? ReviewerId { get; set; }
        public User? Reviewer { get; set; }
        public FileUpload? ReviewerSignatureImage { get; set; } = new();

        public int? ReviewerHSId { get; set; }
        public User? ReviewerHS { get; set; }
        public FileUpload? ReviewerHSSignatureImage { get; set; } = new();

        public int? ApproverId { get; set; }
        public User? Approver { get; set; }
        public FileUpload? ApproverSignatureImage { get; set; } = new();

        public ICollection<Turn>? Turns { get; set; }

        public DateTime? ApplicationMonth { get; set; }

        public string? ProductionVolumePerShift { get; set; }
        public string? ControlNumber { get; set; }
        public ICollection<SOSCombinationLogbook>? CombinationLogbooks { get; set; } = new List<SOSCombinationLogbook>();


        public int SOSHubId { get; set; }
        public SOSHub? SOSHub { get; set; }
    }
}
