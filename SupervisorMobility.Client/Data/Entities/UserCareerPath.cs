

namespace SupervisorMobility.Client.Data.Entities
{
    public class UserCareerPath
    {
        public int UserCareerPathId { get; set; }
        public int CareerPathNo { get; set; }

        public DateTime? ChangeDate { get; set; }
        public DateTime? EndDate { get; set; }

        public string Department { get; set; }
        public string Process { get; set; }

        public string? OperationDescription { get; set; }
        public bool? IsActive { get; set; } = true;
    }
}
