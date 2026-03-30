namespace SupervisorMobility.Client.Data.Entities
{
    public class LocalUserCourses
    {
        public int CourseId { get; set; }
        public string Reticulate { get; set; } = string.Empty;
        public DateTime? Date { get; set; } = DateTime.Now;
        public decimal Calification { get; set; } = 0;
        public string Type { get; set; } = string.Empty;
        public int HCIId { get; set; }
        public HCI? HCI { get; set; }
    }
}
