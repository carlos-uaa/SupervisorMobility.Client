namespace SupervisorMobility.Client.Services.PATService
{
    public interface IPATService
    {

        Task<PAT?> getPat(int patid);
        Task<List<PAT>> GetAllPATSforSSV(int SSVid);
        Task<List<PAT>> GetAllPATSforSV(int SVid);

    }
}
