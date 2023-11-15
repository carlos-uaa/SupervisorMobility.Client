using System.ComponentModel.DataAnnotations;

namespace SupervisorMobility.Client.Data.Entities
{
    public class JobObservation
    {

        public int JobObservationId { get; set; }

        public Plant Plant { get; set; }
        public Area Area { get; set; }
        public Distribution Distribution { get; set; }
        public Operation Operation { get; set; }
        public ICollection<Lup> Lup { get; set; } = new List<Lup>();

        public User Supervisor { get; set; }

        public User Operator { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Please indicate a Plant")]
        public int PlantId { get; set; }
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Please indicate an Area")]
        public int AreaId { get; set; }
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Please indicate a Distribution")]
        public int DistributionId { get; set; }
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Please indicate a Operation")]
        public int OperationId { get; set; }    
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Please indicate a Supervisor")]
        public int SupervisorId { get; set; }
        public int OperatorId { get; set; }

        public bool IsActive { get; set; }
        public int? Type { get; set; }

        [Required]
        public DateTime? StartDate { get; set; }
        [Required]
        public DateTime? EndDate { get; set; }

        public DateTime? PlannedStartDate { get; set; }
        public DateTime? PlannedEndDate { get; set; }
        public DateTime? FinishedDate { get; set; }
        public string? Justification { get; set; }
        public int? Status { get; set; }

        public int Option { get; set; }
        public string? Anomaly { get; set; }

        public string HOEStandardTimes { get; set; }
        public string Models { get; set; }
        public string Cycles { get; set; }

        public string SsvCommentary { get; set; }
        public string OperatorCommentary { get; set; }
        public string SsvSignature { get; set; }
        public string OperatorSignature { get; set; }
        public string? ReleasedFeedback { get; set; }

        public int? KpiId { get; set; }
        public string? TaktTime { get; set; }
        public string? Questions { get; set; }

        public ICollection<ChecklistAnswer>? ChecklistAnswers { get; set; } = new List<ChecklistAnswer>();
    }
}
