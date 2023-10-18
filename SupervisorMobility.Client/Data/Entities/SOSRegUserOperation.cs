namespace SupervisorMobility.Client.Data.Entities
{
    public class SOSRegUserOperation
    {
        public int SOSRegUserOperationId { get; set; }

        public int? SOSReviewProgramid { get; set; }
        public SOSReviewProgram? SOSReviewProgram { get; set; }

        public int? OperationId { get; set; }
        public Operation? Operation { get; set; }

        public int? SupervisorId { get; set; }
        public User? Supervisor { get; set; }
    }
}
