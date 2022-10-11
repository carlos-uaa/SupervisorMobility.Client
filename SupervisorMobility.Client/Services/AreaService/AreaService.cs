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
