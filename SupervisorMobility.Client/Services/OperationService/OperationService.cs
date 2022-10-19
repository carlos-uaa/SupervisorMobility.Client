using System.Net.Http.Json;

namespace SupervisorMobility.Client.Services.OperationService
{
    public class OperationService : IOperationService
    {
        private readonly HttpClient _http;
        private readonly JsonSerializerOptions _options;

        // Constructor
        public OperationService(HttpClient http)
        {
            _http = http;
            _options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        }

        // Create operation
        public async Task<Operation> CreateOperation(int plantId, int areaId, Operation operation)
        {
            var response = await _http.PostAsJsonAsync($"plants/{plantId}/areas/{areaId}/Operations", operation);
            var newOperation = await response.Content.ReadFromJsonAsync<Operation>();

            return newOperation;
        }

        // Delete operation
        public Task DeleteOperation(int plantId, int areaId, int operationId)
        {
            throw new NotImplementedException();
        }

        // Get operation by Id
        public Task<Operation> GetOperationById(int operationId)
        {
            throw new NotImplementedException();
        }

        // Get all operations
        public async Task<List<Operation>> GetOperations(int plantId, int areaId)
        {
            var response = await _http.GetAsync($"plants/{plantId}/areas/{areaId}/Operations");
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new ApplicationException(content);
            }

            var operations = JsonSerializer.Deserialize<List<Operation>>(content, _options);

            return operations;
        }

        // Update operation
        public Task UpdateOperation(int plantId, int areaId, Operation operation)
        {
            throw new NotImplementedException();
        }
    }
}
