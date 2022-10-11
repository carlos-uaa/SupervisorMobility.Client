using System.Net.Http.Json;

namespace SupervisorMobility.Client.Services.AreaService
{
    public class AreaService : IAreaService
    {
        private readonly HttpClient _http;
        private readonly JsonSerializerOptions _options;

        // Constructor
        public AreaService(HttpClient http)
        {
            _http = http;
            _options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        }

        // Create area
        public async Task<Area> CreateArea(int plantId, Area area)
        {
            var response = await _http.PostAsJsonAsync($"plants/{plantId}/areas", area);
            var newArea = await response.Content.ReadFromJsonAsync<Area>();

            return newArea;
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
    }
}
