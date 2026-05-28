using System.ComponentModel.DataAnnotations;

namespace SupervisorMobility.Client.Data.Entities.Hri
{
    public class RevisionCycles
    {
        [Key]
        public int RevisionCycleId { get; set; }
        public int Cycle { get; set; }
        public int? HRIRevisionItemsId { get; set; }
        public HRIRevisionItems? HRIRevisionItems { get; set; }
        public bool? IsActive { get; set; }
        public List<DailyRevisions>? DailyRevisions { get; set; }

    }
}
