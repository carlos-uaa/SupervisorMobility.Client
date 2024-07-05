namespace SupervisorMobility.Client.Data.Entities.SOSAnalysis_Process
{
    public class SOSCombination
    {
        public int SOSCombinationId { get; set; }
        public bool? IsActive { get; set; }

        public int SOSHubId { get; set; }
        public SOSHub? SOSHub { get; set; }
    }
}
