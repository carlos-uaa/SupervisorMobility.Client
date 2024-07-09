namespace SupervisorMobility.Client.Data.Entities
{
    public class Station
    {
        public int StationId { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool? IsActive { get; set; }
    }
}
