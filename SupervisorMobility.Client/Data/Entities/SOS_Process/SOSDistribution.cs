namespace SupervisorMobility.Client.Data.Entities.SOS_Process
{
    public class SOSDistribution
    {
        public int SOSDistributionId { get; set; }
        public string? InternalControlNumber { get; set; }
        public string? OperationName { get; set; }
        public string? ProcessName { get; set; }

    
        public string? TackTime { get; set; }

        public ICollection<Turn>? Turns { get; set; }
        public string? AplicationModels { get; set; } = "§§§§";
        public ICollection<SOSDistributionOperationSequence>? SOSDistributionOperationSequence { get; set; }

        public string? AdditionalTime { get; set; } = "§§§§";
        public string? CycleTime { get; set; } = "§§§§";
        public string? ControlNumber { get; set; }
        public ICollection<SOSDistributionLogbook>? DistributionLogbooks { get; set; } = new List<SOSDistributionLogbook>();
        public ICollection<FileUpload>? Illustrations { get; set; } = new List<FileUpload>();
        public ICollection<Commentary>? Notes { get; set; } = new List<Commentary>();
        public DateTime? ApplicationMonth { get; set; }
        public DateTime? CreatedAt { get; set; }

        public int? SOSDistributionAdditionalTimeId { get; set; }
        public SOSDistributionAdditionalTime? SOSDistributionAdditionalTime { get; set; }

        public bool? IsActive { get; set; }
        public int? SOSHubId { get; set; }

        public ICollection<SOSHub>? SOSHubs { get; set; } = new List<SOSHub>();
        public ICollection<SOSAnalysis>? Analyses { get; set; } = new List<SOSAnalysis>();
        public ICollection<SOSSequence>? Sequences { get; set; } = new List<SOSSequence>();



    }
}
