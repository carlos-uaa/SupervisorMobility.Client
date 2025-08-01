namespace SupervisorMobility.Client.Data.Entities.SOS_Process
{
    public class SOSSynopticTableofControlPoints
    {
        public int SOSSynopticTableofControlPointsId { get; set; }

        public string? InternalControlNumber { get; set; }
        public string? ProcessName { get; set; }

      
        public int? CreatorId { get; set; }
        public User? Creator { get; set; }
        public int? ReviewerId { get; set; }
        public User? Reviewer { get; set; }
        public int? ApproverId { get; set; }
        public User? Approver { get; set; }
  

        public DateTime? CreatedAt { get; set; }

    

        public ICollection<SOSSynopticPointsLogbook>? SynopticPointsLogbooks { get; set; } = new List<SOSSynopticPointsLogbook>();


        public bool? IsActive { get; set; }
        public int SOSHubId { get; set; }
        public SOSHub? SOSHub { get; set; }
    }
}
