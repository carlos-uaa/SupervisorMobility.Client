namespace SupervisorMobility.Client.Data.Entities
{
    public class JobObservationType
    {
        public int JobObservationTypeId { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public bool? IsActive { get; set; } = false;
    }
}
