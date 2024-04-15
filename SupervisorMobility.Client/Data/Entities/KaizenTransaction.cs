namespace SupervisorMobility.Client.Data.Entities
{
    public class KaizenTransaction
    {
        public int? KaizenTransactionId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int Type { get; set; }
        public bool? IsActive { get; set; }
    }
}
