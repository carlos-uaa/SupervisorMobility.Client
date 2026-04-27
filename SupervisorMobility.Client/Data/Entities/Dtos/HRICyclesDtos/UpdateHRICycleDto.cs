namespace SupervisorMobility.Client.Data.Entites.Dtos.HRICyclesDtos
{
    public class UpdateHRICycleDto
    {
        public int CycleId { get; set; }
        public int Cycle { get; set; } 
        public bool Deleted { get; set; } = false;
        public int HriId { get; set; }
        public int OperatorUserId { get; set; }
        public int SupervisorUserId { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
