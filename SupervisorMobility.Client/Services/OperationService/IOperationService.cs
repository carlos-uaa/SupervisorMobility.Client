namespace SupervisorMobility.Client.Services.OperationService
{
    public interface IOperationService
    {
        // Get all operations by distribution id
        Task<List<Operation>> GetOperations(int plantId, int areaId, int distributionId);

        // Get operation by Id
        Task<Operation> GetOperationById(int plantId, int areaId, int distributionId, int operationId);

        // Create operation
        Task<Operation> CreateOperation(int plantId, int areaId, int distributionId, Operation operation);

        // Update operation
        Task UpdateOperation(int plantId, int areaId, int distributionId, int operationId, Operation operation);

        // Delete operation 
        Task DeleteOperation(int plantId, int areaId, int distributionId, int operationId);
    }
}
