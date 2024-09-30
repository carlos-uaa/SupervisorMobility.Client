namespace SupervisorMobility.Client.Data.Entities.SOS_Process
{
    public class SOSDistributionAdditionalTime
    {
        public int SOSDistributionAdditionalTimeId { get; set; }
        public string? TakeQuantity { get; set; }
        public string? TakeTime { get; set; }
        public string? LeaveQuantity { get; set; }
        public string? LeaveTime { get; set; }
        public string? StepsQuantity { get; set; }
        public string? StepsTime { get; set; }
        public bool? IsActive { get; set; }
    }
}
