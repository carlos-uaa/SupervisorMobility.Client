namespace SupervisorMobility.Client.Data.Entities.SOS_Process
{
    public class SOSSynopticPointsLogbook
    {
        public int SOSSynopticPointsLogbookId { get; set; }
        public string? Changes { get; set; }
        public DateTime? Date { get; set; }
        public int? NoRevision { get; set; }
        public int? Status { get; set; }
        public bool? IsActive { get; set; }

        public int SOSSynopticTableofControlPointsiD { get; set; }
        public SOSSynopticTableofControlPoints? SOSSynopticTableofControlPoints { get; set; }


        public int? ApproverId { get; set; }
        public User? Approver { get; set; }
        public FileUpload? ApproverSignatureImage { get; set; } = new();

    }
}