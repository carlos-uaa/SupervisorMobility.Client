namespace SupervisorMobility.Client.Services.LupService
{
    public interface ILupService
    {
        Task<List<Lup>> GetAllLup();
        Task<List<Lup>> GetAllLupInsidences(int QuestionId, int supervisor_id);
        Task<List<Lup>> GetLupsByFilters(int year, int operationId);
        Task<Lup> GetLupById(int lupId);
        Task<Lup> GetLupByIdWhitFile(int lupId);
        Task<Lup> CreateLup(Lup lup);
        Task<bool> UpdateLup(Lup lup);
        Task DeleteLup(int lup);
        Task<bool> RemoveEvidence(int lupId, int fileUploadId);
    }
}
