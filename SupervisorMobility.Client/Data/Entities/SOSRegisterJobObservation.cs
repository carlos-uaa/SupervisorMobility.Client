using System.ComponentModel.DataAnnotations.Schema;

namespace SupervisorMobility.Client.Data.Entities
{
    public class SOSRegisterJobObservation
    {
        public int SOSRegisterJobid { get; set; }

        public int? JobObservationId { get; set; }
        public JobObservationNulls? JobObservation { get; set; }

        public int? DistributionId { get; set; }
        public Distribution? Distribution { get; set; }

        public int? SOSReviewProgramid { get; set; }
        public SOSReviewProgram? SOSReviewProgram { get; set; } 

        public DateTime? PreviewDate { get; set; }

        public DateTime? ConfirmationDate { get; set; }


        public DateTime? CreationDate { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
    }
}
