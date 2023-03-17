namespace SupervisorMobility.Client.Services.LupService
{
    public interface ILupService
    {
        Task<List<Lup>> GetAllLup();
        Task<Lup> GetLupById(int lupId);
        Task<Lup> GetLupByIdWhitFile(int lupId);
        Task<Lup> CreateLup(Lup lup);
        Task<bool> UpdateLup(Lup lup);
        Task DeleteLup(int lup);
    }
}
