namespace SupervisorMobility.Client.Services.DistributionService
{
    public interface IDistributionService
    {
        // Get all distributions by area id
        Task<List<Distribution>> GetDistributions(int plantId, int areaId);
        Task<List<Distribution>> GetDistributionsWithCollections(int plantId, int areaId);

        // Get distribution by Id
        Task<Distribution> GetDistributionById(int plantId, int areaId, int distributionId);
        Task<Distribution> GetDistributionWithCollections(int plantId, int areaId, int distributionId);

        // Create distribution
        Task<Distribution> CreateDistribution(int plantId, int areaId, Distribution distribution);

        // Update distribution
        Task<bool> UpdateDistribution(int plantId, int areaId, Distribution distribution);

        // Delete distribution
        Task DeleteDistribution(int plantId, int areaId, int distributionId);

        //delete product from distribution
        Task DeleteProductFromDistribution(int plantId, int areaId, int distributionId, int productId);

        //product
        Task<Product> CreateProduct(int plantId, int areaId, int distributionId, Product product);
        Task<Product> AddProduct(int plantId, int areaId, int distributionId, Product product);

    }
}
