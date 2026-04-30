namespace SupervisorMobility.Clinet.Data.Entities.Dtos.HRIHistory
{
    public class GetUserForHRIDailyRevsionDto
    {
        public int UserId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Email { get; set; } = string.Empty;
        public int UserType { get; set; }
    }
}
