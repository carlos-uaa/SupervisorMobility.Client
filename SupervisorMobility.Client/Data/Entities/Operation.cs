namespace SupervisorMobility.Client.Data.Entities
{
    public class Operation
    {
        public int OperationId { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string? RestrictionOrComment { get; set; }
        public string? ProductName { get; set; }
        public string? NameTime { get; set; }
        public string? Time { get; set; }
        public string? AdditionalTime { get; set; }
        public string? StandardTime { get; set; }
        public int CriticalType { get; set; }

        public bool? IsActive { get; set; } = false;
        public int AreaId { get; set; }

        public override bool Equals(object obj)
        {
            if (obj is Operation other)
            {
                return this.OperationId == other.OperationId;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return OperationId.GetHashCode();
        }
    }
}
