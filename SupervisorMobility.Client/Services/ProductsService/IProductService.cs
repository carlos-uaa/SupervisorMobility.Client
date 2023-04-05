namespace SupervisorMobility.Client.Services.ProductsService
{
    public interface IProductService
    {
        // Get all products
        Task<List<Product>> GetProducts();

        // Get product by Id
        Task<Product> GetProductById(int id);
        Task<Product> GetProductAndCollection(int id);

        // Create product
        Task<Product> CreateProduct(Product product);

        // Update product
        Task<bool> UpdateProduct(Product product);

        // Delete product
        Task DeleteProduct(int id);


        //Distributions
        Task<Distribution> GetDistributionOnlyById(int productId, int distributionId);
        Task<Distribution> CreateDistribution(int productId, int plantId, int areaId, Distribution distribution);
        Task<Distribution> AddDistribution(int productId, int plantId, int areaId, Distribution distribution);
        Task<bool> UpdateDistributionForProduct(int productId, Distribution distribution);
        Task DeleteDistribution(int productId, int distributionId);


    }
}
