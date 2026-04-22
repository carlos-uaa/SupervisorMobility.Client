namespace SupervisorMobility.Client.Data.Entities.Dtos.HRIRevisionItemsDtos
{
    public class CreateDailyRevisionDto
    {
        public int? EntityRelationId { get; set; }
        public int Day { get; set; }
        public int Month { get; set; }
        public int? UserId { get; set; }
        public string? UserType { get; set; }
        public string? Status { get; set; }
    }
}
