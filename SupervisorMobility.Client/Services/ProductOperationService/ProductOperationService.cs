using System.Net.Http.Json;

namespace SupervisorMobility.Client.Services.ProductOperationService
{
    public class ProductOperationService : IProductOperationService
    {
        private readonly HttpClient _http;
        private readonly JsonSerializerOptions _options;

        // Constructor
        public ProductOperationService(HttpClient http)
        {
            _http = http;
            _options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        }

        // Create operation
        public async Task<ProductOperation> CreateOperation(int productId, int productDistributionId, ProductOperation productOperation)
        {
            var response = await _http.PostAsJsonAsync($"products/{productId}/distributions/{productDistributionId}/operations", productOperation);
            var newProductOperation = await response.Content.ReadFromJsonAsync<ProductOperation>();

            return newProductOperation;
        }

        // Delete operation
        public async Task DeleteOperation(int productId, int productDistributionId, int productOperationId)
        {
            var response = await _http.DeleteAsync($"products/{productId}/distributions/{productDistributionId}/operations/{productOperationId}");
        }

        // Get operation by Id
        public async Task<ProductOperation> GetOperationById(int productId, int productDistributionId, int productOperationId)
        {
            var response = await _http.GetAsync($"products/{productId}/distributions/{productDistributionId}/operations/{productOperationId}");
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new ApplicationException(content);
            }

            var productOperation = JsonSerializer.Deserialize<ProductOperation>(content, _options);

            return productOperation;
        }

        // Get all operations by distribution id
        public async Task<List<ProductOperation>> GetOperations(int productId, int productDistributionId)
        {
            var response = await _http.GetAsync($"products/{productId}/distributions/{productDistributionId}/operations");
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new ApplicationException(content);
            }

            var productOperations = JsonSerializer.Deserialize<List<ProductOperation>>(content, _options);

            return productOperations;
        }

        // Update operation
        public async Task UpdateOperation(int productId, int productDistributionId, int productOperationId, ProductOperation productOperation)
        {
            var response = await _http.PutAsJsonAsync($"products/{productId}/distributions/{productDistributionId}/operations/{productOperation.ProductOperationId}", productOperation);
        }
    }
}
