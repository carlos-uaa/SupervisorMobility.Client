namespace SupervisorMobility.Client.Services.PATService
{
    public interface IPATService
    {
        Task<PAT> CreatePat(PAT pat);
        Task<PAT?> getPat(int patid);
        Task<List<PAT>> GetAllPATs();
        Task<List<PAT>> GetAllPATSforSSV(int SSVid);
        Task<List<PAT>> GetAllPATSforSV(int SVid);
        Task<bool> UpdatePat(PAT pat);

    }
}
