namespace SupervisorMobility.Client.Data.Entities
{
    public class ProductOperation
    {
        public int ProductOperationId { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool? IsActive { get; set; } = false;
        public int ProductId { get; set; }
    }
}
