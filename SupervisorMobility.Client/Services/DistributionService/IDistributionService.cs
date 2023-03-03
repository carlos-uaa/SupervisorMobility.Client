namespace SupervisorMobility.Client.Services.DistributionService
{
    public interface IDistributionService
    {
        // Get all distributions by area id
        Task<List<Distribution>> GetDistributions(int plantId, int areaId);

        // Get distribution by Id
        Task<Distribution> GetDistributionById(int plantId, int areaId, int distributionId);
        Task<Distribution> GetDistributionWhitCollections(int plantId, int areaId, int distributionId);

        // Create distribution
        Task<Distribution> CreateDistribution(int plantId, int areaId, Distribution distribution);

        // Update distribution
        Task<bool> UpdateDistribution(int plantId, int areaId, Distribution distribution);

        // Delete distribution
        Task DeleteDistribution(int plantId, int areaId, int distributionId);

        //delete product from distribution
        Task DeleteProductFromDistribution(int plantId, int areaId, int distributionId, int productId);
    }
}
