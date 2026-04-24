namespace SupervisorMobility.Client.Data.Entites.Dtos.HRICyclesDtos
{
    public class UpdateHRICycleDto
    {
        public int CycleId { get; set; }
        public int HriId { get; set; }
        public int Cycle { get; set; } 
        public int SupervisorUserId { get; set; }
        public int OperatorUserId { get; set; }
        public bool IsActive { get; set; } = true;
        public bool delete { get; set; } = false;
    }
}
