using System.Net.Http.Json;

namespace SupervisorMobility.Client.Services.DistributionService
{
    public class DistributionService : IDistributionService
    {
        private readonly HttpClient _http;
        private readonly JsonSerializerOptions _options;

        // Constructor
        public DistributionService(HttpClient http)
        {
            _http = http;
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
        public async Task<Distribution> GetDistributionWhitCollections(int plantId, int areaId, int distributionId)
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

        public async Task<List<Distribution>> GetDistributionsWhitCollections(int plantId, int areaId)
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
    }
}
