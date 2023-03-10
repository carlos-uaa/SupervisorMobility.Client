using Microsoft.JSInterop;
using System.Net.Http.Json;

namespace SupervisorMobility.Client.Services.DistributionService
{
    public class DistributionService : IDistributionService
    {
        private readonly HttpClient _http;
        private readonly IJSRuntime _js;
        private readonly JsonSerializerOptions _options;

        // Constructor
        public DistributionService(HttpClient http, IJSRuntime jSRuntime)
        {
            _http = http;
            _js = jSRuntime;
            _options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        }

        // Create distribution
        public async Task<Distribution> CreateDistribution(int plantId, int areaId, Distribution distribution)
        {
            var response = await _http.PostAsJsonAsync($"plants/{plantId}/areas/{areaId}/distributions", distribution);
            var newDistribution = await response.Content.ReadFromJsonAsync<Distribution>();

            return newDistribution;
        }

        // Delete distribution
        public async Task DeleteDistribution(int plantId, int areaId, int distributionId)
        {
            var response = await _http.DeleteAsync($"plants/{plantId}/areas/{areaId}/distributions/{distributionId}");
        }

        //delete product from distribution
        public async Task DeleteProductFromDistribution(int plantId, int areaId, int distributionId, int productId)
        {
            var response = await _http.DeleteAsync($"plants/{plantId}/areas/{areaId}/distributions/{distributionId}/products/{productId}");
        }

        // Get distribution by Id
        public async Task<Distribution> GetDistributionById(int plantId, int areaId, int distributionId)
        {
            var response = await _http.GetAsync($"plants/{plantId}/areas/{areaId}/distributions/{distributionId}");
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new ApplicationException(content);
            }

            var distribution = JsonSerializer.Deserialize<Distribution>(content, _options);

            return distribution;
        } 
        public async Task<Distribution> GetDistributionWithCollections(int plantId, int areaId, int distributionId)
        {
            var response = await _http.GetAsync($"plants/{plantId}/areas/{areaId}/distributions/{distributionId}?includeCollections=true");
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new ApplicationException(content);
            }

            var distribution = JsonSerializer.Deserialize<Distribution>(content, _options);

            return distribution;
        }

        // Get all distributions by plant and area id
        public async Task<List<Distribution>> GetDistributions(int plantId, int areaId)
        {
            var response = await _http.GetAsync($"plants/{plantId}/areas/{areaId}/distributions");
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new ApplicationException(content);
            }

            var distributions = JsonSerializer.Deserialize<List<Distribution>>(content, _options);

            return distributions;
        }

        public async Task<List<Distribution>> GetDistributionsWithCollections(int plantId, int areaId)
        {
            var response = await _http.GetAsync($"plants/{plantId}/areas/{areaId}/distributions?includecollections=true");
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new ApplicationException(content);
            }

            var distributions = JsonSerializer.Deserialize<List<Distribution>>(content, _options);

            return distributions;
        }

        // Update distribution
        public async Task<bool> UpdateDistribution(int plantId, int areaId, Distribution distribution)
        {
            var response = await _http.PutAsJsonAsync($"plants/{plantId}/areas/{areaId}/distributions/{distribution.DistributionId}", distribution);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return true;
            }

            return false;
        }
        
        //Product
        public async Task<Product> CreateProduct(int plantId, int areaId, int distributionId, Product product)
        {

            var response = await _http.PostAsJsonAsync<Product>($"plants/{plantId}/areas/{areaId}/distributions{distributionId}/products", product);
            var content = await response.Content.ReadAsStringAsync();

           
            if (response.IsSuccessStatusCode)
            {
                var newproduct = await response.Content.ReadFromJsonAsync<Product>();
                return newproduct;
            }
            await _js.InvokeVoidAsync("alert", $"Error : {response.Content.ReadAsStringAsync().Result}");


            return null;
        }
        
        public async Task<Product> AddProduct(int plantId, int areaId, int distributionId, Product product)
        {
            var response = await _http.PostAsJsonAsync<Product>($"plants/{plantId}/areas/{areaId}/distributions{distributionId}/products/add", product);
            var content = await response.Content.ReadAsStringAsync();


            if (response.IsSuccessStatusCode)
            {
                var newproduct = await response.Content.ReadFromJsonAsync<Product>();
                return newproduct;
            }
            await _js.InvokeVoidAsync("alert", $"Error : {response.Content.ReadAsStringAsync().Result}");


            return null;

        }

    }
}
