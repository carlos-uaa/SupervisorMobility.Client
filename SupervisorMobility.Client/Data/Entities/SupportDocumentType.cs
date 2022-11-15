namespace SupervisorMobility.Client.Data.Entities
{
    public class SupportDocumentType
    {
        public int SupportDocumentTypeId { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool? IsActive { get; set; } = false;
    }
}
