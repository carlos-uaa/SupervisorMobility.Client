using SupervisorMobility.Client.Data.Entities.Hri;

namespace SupervisorMobility.Client.Data.Entites.Dtos.HRIRevisionCycles
{
    public class GetRevisionCyclesDto
    {
        public int RevisionCycleId { get; set; }
        public int Cycle { get; set; }
        public int? HRIRevisionItemsId { get; set; }
        public HRIRevisionItems? HRIRevisionItems { get; set; }
        public List<DailyRevisions>? DailyRevisions { get; set; }
    }
}
