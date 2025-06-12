namespace SupervisorMobility.Client.Services.PATService
{
    public interface IPATService
    {
        Task<PAT> CreatePat(PAT pat);
        Task<PAT?> getPat(int patid);
        Task<int?> getPatByJob(int Jobid, int areaid, int plantid);
        Task<List<PAT>> GetAllPATs();
        Task<List<PAT>> GetAllPATSforSSV(int SSVid);
        Task<List<PAT>> GetAllPATSforSV(int SVid);
        Task<bool> DeletePat(int patId);
        Task<bool> UpdatePat(PAT pat);

    }
}
