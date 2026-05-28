

namespace SupervisorMobility.Client.Data.Entities
{
    public class HCI
    {
        public int HCIId { get; set; }

        public int? UserId { get; set; }
        public User? User { get; set; }

        public int? SOSHubId { get; set; }
        public SOSHub? SOSHub { get; set; }

        public List<HCITransaction>? Transactions { get; set; }
        public List<HCICategory>? Categories { get; set; }
        public List<ILURegister>? ILUs { get; set; }
        public List<UserCareerPath>? CareerPaths { get; set; }

        public List<Commentary>? Commentaries { get; set; }
        public List<LocalUserCourses>? Courses { get; set; }
        public bool? IsActive { get; set; }
    }
}
