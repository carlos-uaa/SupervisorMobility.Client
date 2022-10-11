namespace SupervisorMobility.Client.Services.AreaService
{
    public interface IAreaService
    {
        // Get all areas by plant id
        Task<List<Area>> GetAreas(int plantId);

        // Create area
        Task<Area> CreateArea(int plantId, Area area);
    }
}
