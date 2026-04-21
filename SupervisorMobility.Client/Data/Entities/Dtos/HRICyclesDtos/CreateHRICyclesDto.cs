using SupervisorMobility.Client.Data.Entities.Hri;

namespace SupervisorMobility.Client.Data.Entites.Dtos.HRICyclesDtos
{
    public class CreateHRICyclesDto
    {
        public int HriId { get; set; }
        public HRI HRI { get; set; }
        public int Cycle { get; set; }
        public int OperatorId { get; set; } = 0;
        public int SupervisorId { get; set; } = 0;
        public List<DailyRevisions> DailyRevisions { get; set; }
    }
}
