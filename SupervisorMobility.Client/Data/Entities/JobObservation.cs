namespace SupervisorMobility.Client.Data.Entities
{
    public class JobObservation
    {
        public int JobObservationId { get; set; }

        public Plant Plant { get; set; }
        public string PlantId { get; set; }

        public Area Area { get; set; }
        public string AreaId { get; set; }

        public int Target { get; set; }

        public string? Anomaly {get; set; }

        public DateTime Time1 { get; set; }
        public DateTime Time2 { get; set; }

        public string[] Models { get; set; }
        public string[] Cicles { get; set; }

        public string Code { get; set; }
        public string Description { get; set; }
        public bool? IsActive { get; set; } = false;
    }
}
