namespace SupervisorMobility.Client.Data.Entities
{
    public class PatSubordinate
    {
        public int PatSubordinateId { get; set; }
        public int PatId { get; set; }
        public int UserId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}