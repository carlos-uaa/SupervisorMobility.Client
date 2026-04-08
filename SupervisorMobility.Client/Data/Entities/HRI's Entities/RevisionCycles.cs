using System.ComponentModel.DataAnnotations;

namespace SupervisorMobility.Client.Data.Entities.Hri
{
    public class RevisionCycles
    {
        [Key]
        public int RevisionCycleId { get; set; }
        public int HriId { get; set; }
        public HRI HRI { get; set; }
        public  int Cycle { get; set; }
        public  List<DailyRevisions> HourmeterRevision { get; set; }
        public User OperatorRevisor { get; set; }
        public List<DailyRevisions> OperatorRevisions { get; set; }
        public User SupervisorRevisor  { get; set; }
        public List<DailyRevisions> SupervisorRevisions { get; set; }

    }
}
