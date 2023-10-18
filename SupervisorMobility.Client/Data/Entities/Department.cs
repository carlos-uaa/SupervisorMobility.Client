namespace SupervisorMobility.Client.Data.Entities
{
    public class Department
    {
        public int DepartmentId { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public bool? IsActive { get; set; } = false;
    }
}
