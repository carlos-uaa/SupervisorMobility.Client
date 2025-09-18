
namespace SupervisorMobility.Client.Data.Entities.SOS_Process.STRO
{
    public class SOSSTROSkillHub
    {
        public int Id { get; set; }
        public int SkillId { get; set; }
        public Skill? Skill { get; set; }
        public int SOSHubId { get; set; }
        public SOSHub? SOSHub { get; set; }
        public int SOSSynopticTableofOperatingRequirementsId { get; set; }
        public SOSSynopticTableofOperatingRequirements? SOSSynopticTableofOperatingRequirements{ get; set; }
    }
}