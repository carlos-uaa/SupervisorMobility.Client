namespace SupervisorMobility.Client.Services.AreaService
{
    public interface IAreaService
    {
        // Get all areas by plant id
        Task<List<Area>> GetAreas(int plantId);
        Task<List<Area>> GetAreasIncludeCollections(int plantId);

        // Create area
        Task<Area> CreateArea(int plantId, Area area);

        // Get area by Id
        Task<Area> GetAreaById(int plantId, int areaId);

        // Get area including operations
        Task<Area> GetOneAreaIncludingCollections(int plantId, int areaId);

        // Update area
        Task<bool> UpdateArea(int plantId, Area area);

        // Delete area
        Task DeleteArea(int plantId, int areaId);
    }
}
