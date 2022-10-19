namespace SupervisorMobility.Client.Data.Entities
{
    public class Area
    {
        public int AreaId { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool? IsActive { get; set; } = false;
        public int PlantId { get; set; }
        public ICollection<Operation> Operations { get; set; } = new List<Operation>();
    }
}
