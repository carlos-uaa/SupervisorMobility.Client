namespace SupervisorMobility.Client.Data.Entities.SOS_Process
{
    public class SOSDistribution
    {
        public int SOSDistributionId { get; set; }
        public string? InternalControlNumber { get; set; }
        public string? OperationName { get; set; }
        public string? ProcessName { get; set; }

        public int? ReviewerId { get; set; }
        public User? Reviewer { get; set; }
        public FileUpload? ReviewerSignatureImage { get; set; } = new();

        public int? ApproverId { get; set; }
        public User? Approver { get; set; }
        public FileUpload? ApproverSignatureImage { get; set; } = new();

        public string? TackTime { get; set; }

        public ICollection<Turn>? Turns { get; set; }
        public string? AplicationModels { get; set; } = "§§§§";
        public ICollection<ModelTimeStep>? AplicationModelsTimes { get; set; }


        public string? AdditionalTime { get; set; } = "§§§§";
        public string? CycleTime { get; set; } = "§§§§";
        public string? ControlNumber { get; set; }
        public ICollection<SOSDistributionLogbook>? DistributionLogbooks { get; set; } = new List<SOSDistributionLogbook>();
        public ICollection<FileUpload>? Illustrations { get; set; } = new List<FileUpload>();
        public ICollection<Commentary>? Notes { get; set; } = new List<Commentary>();
        public DateTime? CreatedAt { get; set; }

        public bool? IsActive { get; set; }
        public int SOSHubId { get; set; }
        public SOSHub? SOSHub { get; set; }
    }
}
