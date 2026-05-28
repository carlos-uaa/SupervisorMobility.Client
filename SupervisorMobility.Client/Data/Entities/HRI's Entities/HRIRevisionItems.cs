using System.ComponentModel.DataAnnotations;

namespace SupervisorMobility.Client.Data.Entities.Hri
{
    public class HRIRevisionItems
    {
        [Key]
        public int ItemId { get; set; }
        public int HriId { get; set; }
        public HRI HRI { get; set; }
        public int ItemNumber { get; set; }
        public string  RevisionPoint { get; set; }
        public int? RevisionMethodId { get; set; }
        public RevisionMethod? RevisionMethod { get; set; }
        public int? VeredictId { get; set; }
        public Veredict? Veredict { get; set; }
        public int? FrequencyId { get; set; }
        public Frequency? Frequency { get; set; }
    }
}
