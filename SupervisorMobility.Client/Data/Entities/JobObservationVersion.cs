namespace SupervisorMobility.Client.Data.Entities
{
    public class JobObservationVersion
    {
        public int JobObservationVersionId { get; set; }
        public DateTime? DateModification { get; set; }
        public string? resumeVersion { get; set; }

        public int JobObservationId { get; set; }
        public bool? IsActive { get; set; }

        public Plant? Plant { get; set; }
        public int? PlantId { get; set; }
        public Area? Area { get; set; }
        public int? AreaId { get; set; }
        public Distribution? Distribution { get; set; }
        public int? DistributionId { get; set; }
        public Operation? Operation { get; set; }
        public int? OperationId { get; set; }
        public User? Supervisor { get; set; }
        public User? Operator { get; set; }
        public int? SupervisorId { get; set; }
        public int? OperatorId { get; set; }

        public int? Type { get; set; }

        public DateTime? DateStart { get; set; }
        public DateTime? DateEnd { get; set; }
        public DateTime? DateFinalized { get; set; }

        public string? Justification { get; set; }
        public int? Status { get; set; }
        public int? Option { get; set; }
        public string? Anomaly { get; set; }


        public string? Time1HOE { get; set; }
        public string? Time2HOE { get; set; }
        public string? Models { get; set; }
        public string? Cicles { get; set; }

        public string? SsvCommentary { get; set; }
        public string? OperatorCommentary { get; set; }
        public string? SsvSignature { get; set; }
        public string? OperatorSignature { get; set; }

        public ICollection<Lup> Lup { get; set; } = new List<Lup>();
    }
}
