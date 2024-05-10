namespace SupervisorMobility.Client.Services.PillarService
{
    public interface IPillarService
    {
        // Get all pillars
        Task<List<Pillar>> GetPillars();

        // Get pillar by Id
        Task<Pillar> GetPillarById(int id);

        // Create pillar
        Task<Pillar> CreatePillar(Pillar pillar);

        // Update pillar
        Task<bool> UpdatePillar(Pillar pillar);

        // Delete pillar
        Task DeletePillar(int id);
    }
}
