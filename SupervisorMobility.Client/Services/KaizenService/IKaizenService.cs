namespace SupervisorMobility.Client.Services.KaizenService
{
    public interface IKaizenService
    {
        Task<List<Kaizen>> GetAllKaizen();
        Task<Kaizen> GetKaizenById(int kaizenId);
        Task<Kaizen> GetKaizenByIdWhitFile(int kaizenId);
        Task<Kaizen> CreateKaizen(Kaizen kaizen);
        Task<bool> UpdateKaizen(Kaizen kaizen);
        Task DeleteKaizen(int kaizen);
        Task<bool> RemoveEvidence(int kaizenId, int fileUploadId);
    }
}
