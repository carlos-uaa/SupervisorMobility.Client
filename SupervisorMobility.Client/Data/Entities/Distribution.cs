namespace SupervisorMobility.Client.Data.Entities
{
    public class Distribution
    {
        public int DistributionId { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public bool? IsActive { get; set; } = false;
        public int AreaId { get; set; }
        public ICollection<Operation> Operations { get; set; } = new List<Operation>();
    }
}
