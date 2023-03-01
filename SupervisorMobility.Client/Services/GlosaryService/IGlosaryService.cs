namespace SupervisorMobility.Client.Services.GlosaryService
{
    public interface IGlosaryService
    {
        // Get all groups
        Task<List<Glosary>> GetGlosary();

        // Get group by Id
        Task<Glosary> GetGlosaryWordbyId(int id);

        // Create group
        Task<Glosary> CreateGlosaryWord(Glosary glosaryWord);

        // Update group
        Task<bool> UpdateGlosaryWord(Glosary glosaryWord);

        // Delete group
        Task DeleteGlosaryWord(int id);
    }
}
