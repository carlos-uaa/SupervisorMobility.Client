namespace SupervisorMobility.Client.Data.Entities
{
    public class Distribution
    {
        public int DistributionId { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public bool? IsActive { get; set; } = false;
        public int AreaId { get; set; }
        public List<Operation> Operations { get; set; } = new List<Operation>();
        public List<Product> Products { get; set; } = new List<Product>();
        public bool ShowDetails { get; set; }
        public int CriticalType { get; set; }
    }
}
