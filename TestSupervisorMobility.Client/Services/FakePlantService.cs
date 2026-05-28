using SupervisorMobility.Client.Data.Entities;
using SupervisorMobility.Client.Services.PlantService;

namespace TestSupervisorMobility.Client.Services
{
    public sealed class FakePlantService : IPlantService
    {
        public Task<List<Plant>> GetPlants() => Task.FromResult(new List<Plant>
        {
            new() { PlantId = 1, Code = "P01", Description = "Planta de prueba", IsActive = true }
        });

        public Task<Plant> GetPlantById(int id) => throw new NotImplementedException();
        public Task<Plant> GetPlantIncludingAreas(int id) => throw new NotImplementedException();
        public Task<Plant> CreatePlant(Plant plant) => throw new NotImplementedException();
        public Task<bool> UpdatePlant(Plant plant) => throw new NotImplementedException();
        public Task DeletePlant(int id) => throw new NotImplementedException();
    }
}