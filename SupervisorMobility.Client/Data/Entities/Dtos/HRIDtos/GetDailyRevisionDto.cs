namespace SupervisorMobility.Client.Data.Entities.Dtos.HRIDtos
{
    public class GetDailyRevisionDto
    {
        public int RevisionId { get; set; }
        public int? RevisionCycleId { get; set; }
        public int? CycleId { get; set; }
        public int? HourmeterRevisionId { get; set; }
        public int Day { get; set; }
        public int Month { get; set; }
        public int? UserId { get; set; }
        public GetUserForHRIDailyRevsionDto? Responsible { get; set; }
        public string? UserType { get; set; }
        public string? Status { get; set; }
        public bool? IsActive { get; set; }
    }
}
