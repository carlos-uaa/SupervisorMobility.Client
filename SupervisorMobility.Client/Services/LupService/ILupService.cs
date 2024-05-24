namespace SupervisorMobility.Client.Services.LupService
{
    public interface ILupService
    {
        Task<List<Lup>> GetAllLup();
        Task<List<Lup>> GetAllLupInsidences(int checklistQuestionId, int supervisor_id, int distributionId);
        Task<List<Lup>> GetLupsByFilters(DateTime? startDate = null, DateTime? endDate = null, int plantId = 0, int areaId = 0, int distributionId = 0, int operationId = 0, int supervisorId = 0, int status = 0);
        Task<Lup> GetLupById(int lupId);
        Task<Lup> GetLupByIdWhitFile(int lupId);
        Task<Lup> CreateLup(Lup lup);
        Task<bool> UpdateLup(Lup lup);
        Task DeleteLup(int lup);
        Task<bool> RemoveEvidence(int lupId, int fileUploadId);
    }
}
