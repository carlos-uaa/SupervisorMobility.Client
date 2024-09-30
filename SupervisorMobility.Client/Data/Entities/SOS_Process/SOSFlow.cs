namespace SupervisorMobility.Client.Data.Entities.SOS_Process
{
    public class SOSFlow
    {
        public int SOSFlowId { get; set; }
        public string? InternalControlNumber { get; set; }
        public string? OperationName { get; set; }
        public string? ProcessName { get; set; }

       
        public int? ReviewerHSId { get; set; }
        public User? ReviewerHS { get; set; }
        public FileUpload? ReviewerHSSignatureImage { get; set; } = new();


        public DateTime? CreatedAt { get; set; }

        public string? TargetTime { get; set; }

        public ICollection<SOSFlowLogbook>? FlowLogbooks { get; set; } = new List<SOSFlowLogbook>();


        public bool? IsActive { get; set; }
        public int SOSHubId { get; set; }
        public SOSHub? SOSHub { get; set; }
    }
}
