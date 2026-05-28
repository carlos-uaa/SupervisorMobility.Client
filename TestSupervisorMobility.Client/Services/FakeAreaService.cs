using SupervisorMobility.Client.Data.Entities;
using SupervisorMobility.Client.Services.AreaService;

namespace TestSupervisorMobility.Client.Services
{
    public sealed class FakeAreaService : IAreaService
    {
        public Task<List<Area>> GetAreas(int plantId) => Task.FromResult(new List<Area>
        {
            new() { AreaId = 1, Code = "A01", Description = "Área de prueba", IsActive = true, PlantId = plantId }
        });

        public Task<List<Area>> GetAreasByIds(List<int> areasids) => throw new NotImplementedException();
        public Task<List<Area>> GetAreasIncludeCollections(int plantId) => throw new NotImplementedException();
        public Task<Area> CreateArea(int plantId, Area area) => throw new NotImplementedException();
        public Task<Area> GetAreaById(int plantId, int areaId) => throw new NotImplementedException();
        public Task<Area> GetOneAreaIncludingCollections(int plantId, int areaId) => throw new NotImplementedException();
        public Task<bool> UpdateArea(int plantId, Area area) => throw new NotImplementedException();
        public Task DeleteArea(int plantId, int areaId) => throw new NotImplementedException();
    }
}