namespace SupervisorMobility.Client.Data.Entities
{
    public class UserCourse
    {
        public string PayRol { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Reticulate { get; set; } = string.Empty;
        public string Course { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public decimal Calification { get; set; } = 0;
        public DateTime Date { get; set; }
    }
}
