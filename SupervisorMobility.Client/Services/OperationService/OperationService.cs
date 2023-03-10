using Microsoft.JSInterop;
using System.Net.Http.Json;

namespace SupervisorMobility.Client.Services.OperationService
{
    public class OperationService : IOperationService
    {
        private readonly HttpClient _http;
        private readonly HttpClient _httpBridge;
        private readonly JsonSerializerOptions _options;

        // Constructor
        public OperationService(CustomHttpClientService customHttpClientService, IJSRuntime jSRuntime)
        {
            _http = customHttpClientService.GetApiHttpClient();
            _httpBridge = customHttpClientService.GetBridgeHttpClient();
            _options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        }

        // Create operation
        public async Task<Operation> CreateOperation(int plantId, int areaId, int distributionId, Operation operation)
        {
            var response = await _http.PostAsJsonAsync($"plants/{plantId}/areas/{areaId}/distributions/{distributionId}/operations", operation);
            var newOperation = await response.Content.ReadFromJsonAsync<Operation>();

            return newOperation;
        }

        // Delete operation
        public async Task DeleteOperation(int plantId, int areaId, int distributionId, int operationId)
        {
            var response = await _http.DeleteAsync($"plants/{plantId}/areas/{areaId}/distributions/{distributionId}/operations/{operationId}");
        }

        // Get operation by Id
        public async Task<Operation> GetOperationById(int plantId, int areaId, int distributionId, int operationId)
        {
            var response = await _http.GetAsync($"plants/{plantId}/areas/{areaId}/distributions/{distributionId}/operations/{operationId}");
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new ApplicationException(content);
            }

            var operation = JsonSerializer.Deserialize<Operation>(content, _options);

            return operation;
        }

        // Get all operations by distribution id
        public async Task<List<Operation>> GetOperations(int plantId, int areaId, int distributionId)
        {
            var response = await _http.GetAsync($"plants/{plantId}/areas/{areaId}/distributions/{distributionId}/operations");
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new ApplicationException(content);
            }

            var operations = JsonSerializer.Deserialize<List<Operation>>(content, _options);

            return operations;
        }

        // Update operation
        public async Task<bool> UpdateOperation(int plantId, int areaId, int distributionId, int operationId, Operation operation)
        {
            var response = await _http.PutAsJsonAsync($"plants/{plantId}/areas/{areaId}/distributions/{distributionId}/operations/{operation.OperationId}", operation);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return true;
            }

            return false;
        }
    }
}
