

namespace SupervisorMobility.Client.Data.Entities.SOS_Process.STRO
{
    public class SOSSynopticTableRequirementOperationDifficulty
    {
        public int Id { get; set; }
        public int SOSSynopticTableofOperatingRequirementsId { get; set; }
        public SOSSynopticTableofOperatingRequirements? SOSSynopticTableofOperatingRequirements { get; set; }
        public int SOSHubId { get; set; }
        public SOSHub? SOSHub { get; set; }
        public DifficultyLevel DifficultyLevel { get; set; }
    }
}
