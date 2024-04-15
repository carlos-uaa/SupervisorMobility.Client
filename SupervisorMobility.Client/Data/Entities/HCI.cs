

namespace SupervisorMobility.Client.Data.Entities
{
    public class HCI
    {
        public int HCIId { get; set; }

        public string? HCIName { get; set; }
        public string? HCISectionName { get; set; }
        public int? HCINo { get; set; }

        public int? UserId { get; set; }
        public User? User { get; set; }

        public ICollection<HCITransaction>? Transactions { get; set; }
        public ICollection<HCICategory>? Categories { get; set; }


        public ICollection<Commentary>? Comments { get; set; }
        public bool? IsActive { get; set; }
    }
}
