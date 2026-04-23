namespace SupervisorMobility.Client.Data.Entities.Dtos.HRIDtos
{
    public class CreateWeeklyRevisionDto
    {
        public int HriId { get; set; }
        public int? UserId { get; set; }
        public int Month { get; set; }
        public int Week { get; set; }
        public bool? IsActive { get; set; } = true;
        public string? Status { get; set; }
    }
}
