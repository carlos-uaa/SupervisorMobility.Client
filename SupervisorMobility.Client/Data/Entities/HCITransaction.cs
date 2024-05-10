namespace SupervisorMobility.Client.Data.Entities
{
    public class HCITransaction
    {

        public int HCITransactionId { get; set; }

        public string? Name { get; set; }
        public string? Description { get; set; }
        public DateTime? DateStart { get; set; }
        public DateTime? DateEnd { get; set; }
        public int Type { get; set; }
        public bool? IsActive { get; set; } = true;
    }
}