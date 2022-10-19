namespace SupervisorMobility.Client.Services.OperationService
{
    public interface IOperationService
    {
        // Get all operations
        Task<List<Operation>> GetOperations(int plantId, int areaId);

        // Get operation by Id
        Task<Operation> GetOperationById(int plantId, int areaId, int operationId);

        // Create operation
        Task<Operation> CreateOperation(int plantId, int areaId, Operation operation);

        // Update operation
        Task UpdateOperation(int plantId, int areaId, Operation operation);

        // Delete operation
        Task DeleteOperation(int plantId, int areaId, int operationId);
    }
}
