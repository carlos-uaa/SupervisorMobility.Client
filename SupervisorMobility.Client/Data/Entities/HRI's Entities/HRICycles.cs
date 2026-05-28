using System.ComponentModel.DataAnnotations;

namespace SupervisorMobility.Client.Data.Entities.Hri
{
    public class HRICycles
    {
        [Key]
        public int CycleId { get; set; }
        public int HriId { get; set; }
        public HRI HRI { get; set; }
        public int Cycle { get; set; }
        public List<DailyRevisions> DailyRevisions { get; set; }
    }
}
