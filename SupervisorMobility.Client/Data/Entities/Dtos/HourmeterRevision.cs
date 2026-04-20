using SupervisorMobility.Client.Data.Entities.Hri;

namespace SupervisorMobility.Client.Data.Entities.Dtos
{
    public class HourmeterRevision
    {
        public int Id { get; set; }
        public int? HriId { get; set; }
        public HRI? HRI { get; set; }
        public List<DailyRevisions>? DailyRevisions { get; set; }
    }
}
