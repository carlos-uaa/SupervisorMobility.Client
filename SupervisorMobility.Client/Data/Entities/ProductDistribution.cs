namespace SupervisorMobility.Client.Data.Entities
{
    public class ProductDistribution
    {
        public int ProductDistributionId { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public bool? IsActive { get; set; } = false;
        public int ProductId { get; set; }
        public ICollection<ProductOperation> ProductOperations { get; set; } = new List<ProductOperation>();
    }
}
