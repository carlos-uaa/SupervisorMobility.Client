namespace SupervisorMobility.Client.Data.Entities
{
    public class Product
    {
        public int ProductId { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool? IsActive { get; set; } = false;

        public List<Distribution> Distributions { get; set; } = new List<Distribution>();
    }
}
