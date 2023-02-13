using System.ComponentModel.DataAnnotations;

namespace SupervisorMobility.Client.Data.Entities
{
    public class JobObservation
    {

        public int JobObservationId { get; set; }
        
        [Required]
        public Plant Plant { get; set; }
        [Required]
        public Area Area { get; set; }
        [Required]
        public Distribution Distribution { get; set; }
        [Required]
        public Operation Operation { get; set; }

        [Required]
        public int PlantId;
        [Required]
        public int AreaId;
        [Required]
        public int DistributionId;
        [Required]
        public int OperationId;

        public bool IsActive { get; set; }

        public DateTime? dateStart { get; set; }
        public DateTime? dateEnd { get; set; }

        public string Observer { get; set; }
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
