using SupervisorMobility.Client.Data.Entities.Hri;

namespace SupervisorMobility.Client.Data.Entites.Dtos.HRICyclesDtos
{
    public class CreateHRICyclesDto
    {
        public int HriId { get; set; }
        public int Cycle { get; set; } 
        public int SupervisorUserId { get; set; }
        public int OperatorUserId { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
