using System.Net.Http.Json;

namespace SupervisorMobility.Client.Services.ProductDistributionService
{
    public class ProductDistributionService : IProductDistributionService
    {
        private readonly HttpClient _http;
        private readonly JsonSerializerOptions _options;

        // Constructor
        public ProductDistributionService(HttpClient http)
        {
            _http = http;
            _options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        }

        // Create product distribution
        public async Task<ProductDistribution> CreateDistribution(int productId, ProductDistribution productDistribution)
        {
            var response = await _http.PostAsJsonAsync($"product/{productId}/distributions", productDistribution);
            var newProductDistribution = await response.Content.ReadFromJsonAsync<ProductDistribution>();

            return newProductDistribution;
        }

        // Delete distribution
        public async Task DeleteDistribution(int productId, int productDistributionId)
        {
            var response = await _http.DeleteAsync($"product/{productId}/distributions/{productDistributionId}");
        }

        // Get distribution by Id
        public async Task<ProductDistribution> GetDistributionById(int productId, int productDistributionId)
        {
            var response = await _http.GetAsync($"product/{productId}/distributions/{productDistributionId}");
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new ApplicationException(content);
            }

            var productDistribution = JsonSerializer.Deserialize<ProductDistribution>(content, _options);

            return productDistribution;
        }

        // Get all distributions by plant and area id
        public async Task<List<ProductDistribution>> GetDistributions(int productId)
        {
            var response = await _http.GetAsync($"product/{productId}/distributions");
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new ApplicationException(content);
            }

            var productDistributions = JsonSerializer.Deserialize<List<ProductDistribution>>(content, _options);

            return productDistributions;
        }

        // Update distribution
        public async Task<bool> UpdateDistribution(int productId, ProductDistribution productDistribution)
        {
            var response = await _http.PutAsJsonAsync($"product/{productId}/distributions/{productDistribution.ProductDistributionId}", productDistribution);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return true;
            }

            return false;
        }
    }
}
