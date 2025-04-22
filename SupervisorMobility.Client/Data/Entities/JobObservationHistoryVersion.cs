using System.ComponentModel.DataAnnotations;

namespace SupervisorMobility.Client.Data.Entities
{
    public class JobObservationHistoryVersion
    {
        public int JobObservationVersionId { get; set; }
        public DateTime? DateModification { get; set; }
        public string? resumeVersion { get; set; }
        public string? MadeBy { get; set; }


        public int JobObservationId { get; set; }
        public Plant? Plant { get; set; }
        public Area? Area { get; set; }
        public Distribution? Distribution { get; set; }
        public IEnumerable<Operation>? Operations { get; set; } = new List<Operation>();

        //people
        public User Supervisor { get; set; }
        public User Operator { get; set; }
        //lup
        public ICollection<Lup> Lup { get; set; } = new List<Lup>();
        //history
        public ICollection<JobObservationHistoryVersion> History { get; set; } = new List<JobObservationHistoryVersion>();
        //answers question
        public ICollection<ChecklistAnswer>? ChecklistAnswers { get; set; } = new List<ChecklistAnswer>();

        public int? PlantId { get; set; }
        public int? AreaId { get; set; }
        public int? DistributionId { get; set; }
        public int? SupervisorId { get; set; }
        public int? OperatorId { get; set; }

        public bool? IsActive { get; set; }
        public int? Type { get; set; }

        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public DateTime? PlannedStartDate { get; set; }
        public DateTime? PlannedEndDate { get; set; }
        public DateTime? FinishedDate { get; set; }
        public string? Justification { get; set; }
        public int? Status { get; set; }

        public int Option { get; set; }
        public string? Anomaly { get; set; }

        public string? HOEStandardTimes { get; set; }
        public string? ModelsSpecification { get; set; }
        public string Cycles { get; set; }

        public string SsvCommentary { get; set; }
        public string OperatorCommentary { get; set; }
        public string SsvSignature { get; set; }
        public string OperatorSignature { get; set; }
        public string? ReleasedFeedback { get; set; }

        public int? KpiId { get; set; }
        public string? TaktTime { get; set; }
        public string? Questions { get; set; }
        public int? ProductId { get; set; }

        public string? ProductIds { get; set; }
        public string? ProductSpecifications { get; set; }

        public string? OperationTimesJson { get; set; }
        public string? StepsNumber { get; set; }
        public string? DoubleManagment { get; set; }
        public string? Waiting { get; set; }
        public string? SectionIds { get; set; }

        public FileUpload? SignatureImage { get; set; } = new();
        public bool WillNotRequireSSVAproval { get; set; } = false;
    }
}
