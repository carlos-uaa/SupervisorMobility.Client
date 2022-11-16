namespace SupervisorMobility.Client.Services.SupportDocumentTypeService
{
    public interface ISupportDocumentTypeService
    {
        // Support document types changed
        event Action OnChange;

        // Get all support document types
        Task<List<SupportDocumentType>> GetSupportDocumentTypes();

        // Get support document type by Id
        Task<SupportDocumentType> GetSupportDocumentTypeById(int id);

        // Create support document type
        Task<SupportDocumentType> CreateSupportDocumentType(SupportDocumentType supportDocumentType);

        // Update support document type
        Task UpdateSupportDocumentType(SupportDocumentType supportDocumentType);

        // Delete support document type
        Task DeleteSupportDocumentType(int id);
    }
}
