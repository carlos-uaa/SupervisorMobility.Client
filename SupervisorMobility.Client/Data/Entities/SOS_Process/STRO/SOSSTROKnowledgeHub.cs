
namespace SupervisorMobility.Client.Data.Entities.SOS_Process.STRO
{
    public class SOSSTROKnowledgeHub
    {
        public int Id { get; set; }
        public int KnowledgeId { get; set; }
        public Knowledge? Knowledge { get; set; }
        public int SOSHubId { get; set; }
        public SOSHub? SOSHub { get; set; }
        public int SOSSynopticTableofOperatingRequirementsId { get; set; }
        public SOSSynopticTableofOperatingRequirements? SOSSynopticTableofOperatingRequirements{ get; set; }
    }
}