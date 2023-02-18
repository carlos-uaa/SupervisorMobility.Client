namespace SupervisorMobility.Client.Services.PlantService
{
    public interface IPlantService
    {
        // Get all plants
        Task<List<Plant>> GetPlants();

        // Get plant by Id
        Task<Plant> GetPlantById(int id);

        // Get plant including areas
        Task<Plant> GetPlantIncludingAreas(int id);

        // Create plant
        Task<Plant> CreatePlant(Plant plant);

        // Update plant
        Task<bool> UpdatePlant(Plant plant);

        // Delete plant
        Task DeletePlant(int id);

    }
}
