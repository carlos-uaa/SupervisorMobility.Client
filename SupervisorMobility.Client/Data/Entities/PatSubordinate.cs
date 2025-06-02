namespace SupervisorMobility.Client.Data.Entities
{
    public class PatSubordinate
    {
        public int PatSubordinateId { get; set; }
        public int PatId { get; set; }
        public int UserId { get; set; }

        public List<PatSubordinateDates> PatSubordinateDates { get; set; } = new List<PatSubordinateDates>();
    }

    public class PatSubordinateDates
    {
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}