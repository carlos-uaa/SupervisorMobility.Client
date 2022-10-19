namespace SupervisorMobility.Client.Data.Entities
{
    public class Operation
    {
        public int OperationId { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool? IsActive { get; set; } = false;
        public int AreaId { get; set; }
    }
}
