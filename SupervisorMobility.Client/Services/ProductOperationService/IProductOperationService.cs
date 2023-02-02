namespace SupervisorMobility.Client.Services.ProductOperationService
{
    public interface IProductOperationService
    {
        // Get all operations by distribution id
        Task<List<ProductOperation>> GetOperations(int productId, int productDistributionId);

        // Get operation by Id
        Task<ProductOperation> GetOperationById(int productId, int productDistributionId, int productOperationId);

        // Create operation
        Task<ProductOperation> CreateOperation(int productId, int productDistributionId, ProductOperation productOperation);

        // Update operation
        Task UpdateOperation(int productId, int productDistributionId, int productOperationId, ProductOperation productOperation);

        // Delete operation 
        Task DeleteOperation(int productId, int productDistributionId, int productOperationId);
    }
}
