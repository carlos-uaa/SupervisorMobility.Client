namespace SupervisorMobility.Client.Data.Entities
{
    public class DocumentType
    {
        public int DocTypeId { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool? IsActive { get; set; } = false;
    }
}
