namespace SupervisorMobility.Client.Data.Entities.SOS_Process
{
    public class SOSFlowLogbook
    {
        public int SOSFlowLogbookId { get; set; }
        public string? Changes { get; set; }
        public DateTime? Date { get; set; }
        public int? NoRevision { get; set; }
        public int? Status { get; set; }
        public bool? IsActive { get; set; }

        public int SOSFlowId { get; set; }
        public SOSFlow? SOSFlow { get; set; }


        public int? ApproverId { get; set; }
        public User? Approver { get; set; }
        public FileUpload? ApproverSignatureImage { get; set; } = new();

        public int? ReviewerId { get; set; }
        public User? Reviewer { get; set; }
        public FileUpload? ReviewerSignatureImage { get; set; } = new();
    }
}