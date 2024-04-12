namespace SupervisorMobility.Client.Data.Entities
{
    public class Kaizen
    {
        public int KaizenId { get; set; }
        public string KaizenName { get; set; }
        public bool? IsActive { get; set; }


        //Structure
        public Plant? Plant { get; set; }
        public int? PlantId { get; set; }
        public Area? Area { get; set; }
        public int? AreaId { get; set; }
        public Pillar? Pillar { get; set; }
        public int PillarId { get; set; }


        //people
        public User? Supervisor { get; set; }
        public User? SeniorSupervisor { get; set; }
        public User? Proposed { get; set; }
        public int? SupervisorId { get; set; }
        public int? SeniorSupervisorId { get; set; }
        public int? ProposedId { get; set; }


        public ICollection<FileUpload>? PreviousEvidences { get; set; }
           = new List<FileUpload>();
        public ICollection<FileUpload>? ThenEvidences { get; set; }
          = new List<FileUpload>();

        public ICollection<KaizenTransaction>? Transactions { get; set; }
          = new List<KaizenTransaction>();

        public string? PreviousJustification { get; set; }
        public string? ThenJustification { get; set; }
        public string? StandardModification { get; set; }

        public string? Calculationformula { get; set; }
        public double? Total { get; set; }

        public DateTime? CreateDate { get; set; }
        public DateTime? FinishedDate { get; set; }

        public int Status { get; set; }
        public string kpiName { get; set; }
    }
}
