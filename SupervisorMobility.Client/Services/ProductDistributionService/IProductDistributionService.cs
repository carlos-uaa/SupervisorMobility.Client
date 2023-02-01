namespace SupervisorMobility.Client.Services.ProductDistributionService
{
    public interface IProductDistributionService
    {
        // Get all distributions by area id
        Task<List<ProductDistribution>> GetDistributions(int productId);

        // Get distribution by Id
        Task<ProductDistribution> GetDistributionById(int productId, int productDistributionId);

        // Create distribution
        Task<ProductDistribution> CreateDistribution(int productId, ProductDistribution productDistribution);

        // Update distribution
        Task UpdateDistribution(int productId, ProductDistribution productDistribution);

        // Delete distribution
        Task DeleteDistribution(int product, int productDistributionId);
    }
}
