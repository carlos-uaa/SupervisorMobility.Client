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

        public bool IsActive { get; set; }

        [Required]
        public DateTime? DateStart { get; set; }
        [Required]
        public DateTime? DateEnd { get; set; }

        public DateTime? DateFinalized { get; set; }
        public string? Justification { get; set; }
        public int? Status { get; set; }


        [Required(ErrorMessage = "Observer is required")]
        public string Observer { get; set; }
        [Required(ErrorMessage = "Operator is required")]
        public string Operator { get; set; }

        public int Option { get; set; }
        public string? Anomaly { get; set; }


        public string Time1HOE { get; set; }
        public string Time2HOE { get; set; }
        public string Models { get; set; }
        public string Cicles { get; set; }

        public string SArea { get; set; }
        public string QArea { get; set; }
        public string DArea { get; set; }
        public string CArea { get; set; }
        public string OthersArea { get; set; }

        public string IdentifiedActivity { get; set; }
        public string SsvCommentary { get; set; }
        public string OperatorCommentary { get; set; }
        public string SsvSignature { get; set; }
        public string OperatorSignature { get; set; }

    }
}
