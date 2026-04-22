using SupervisorMobility.Client.Data.Entities.Hri;

namespace SupervisorMobility.Client.Data.Entities.Dtos.HRIDtos.HRIHourmeterRevisionDto
{
    public class GetHourmeterRevisionDto
    {
        public int Id { get; set; }
        public int? HriId { get; set; }
        public List<GetDailyRevisionDto>? DailyRevisions { get; set; }
        public bool? IsActive { get; set; }
    }
}
