using Microsoft.JSInterop;
using System.Net.Http.Json;

namespace SupervisorMobility.Client.Services.AreaService
{
    public class AreaService : IAreaService
    {
        private readonly HttpClient _http;
        private readonly HttpClient _httpBridge;
        private readonly JsonSerializerOptions _options;

        // Constructor
        public AreaService(CustomHttpClientService customHttpClientService, IJSRuntime jSRuntime)
        {
            _http = customHttpClientService.GetApiHttpClient();
            _httpBridge = customHttpClientService.GetBridgeHttpClient();
            _options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        }

        // Create area
        public async Task<Area> CreateArea(int plantId, Area area)
        {
            var response = await _http.PostAsJsonAsync($"plants/{plantId}/areas", area);
            var newArea = await response.Content.ReadFromJsonAsync<Area>();

            return newArea;
        }

        // Delete area
        public async Task DeleteArea(int plantId, int areaId)
        {
            var response = await _http.DeleteAsync($"plants/{plantId}/areas/{areaId}");
        }

        // Get area by Id
        public async Task<Area> GetAreaById(int plantId, int areaId)
        {
            var response = await _http.GetAsync($"plants/{plantId}/areas/{areaId}");
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new ApplicationException(content);
            }

            var area = JsonSerializer.Deserialize<Area>(content, _options);

            return area;
        }

        // Get area including operations
        public async Task<Area> GetAreaIncludingOperations(int plantId, int areaId)
        {
            var response = await _http.GetAsync($"plants/{plantId}/areas/{areaId}?includeOperations=true");
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new ApplicationException(content);
            }

            var area = JsonSerializer.Deserialize<Area>(content, _options);

            return area;
        }

        // Get all areas by plant id
        public async Task<List<Area>> GetAreas(int plantId)
        {
            var response = await _http.GetAsync($"plants/{plantId}/areas");
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new ApplicationException(content);
            }

            var areas = JsonSerializer.Deserialize<List<Area>>(content, _options);

            return areas;
        }

        // Update area
        public async Task<bool> UpdateArea(int plantId, Area area)
        {
            var response = await _http.PutAsJsonAsync($"plants/{plantId}/areas/{area.AreaId}", area);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return true;
            }

            return false;
        }
    }
}
