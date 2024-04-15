
namespace SupervisorMobility.Client.Data.Entities
{
    public class HCICategory
    {

        public int HCICategoryId { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public DateTime? Date { get; set; }
        public bool? IsActive { get; set; }
    }
}