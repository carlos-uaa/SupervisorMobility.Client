namespace SupervisorMobility.Client.Services.PlantService
{
    public class PlantService : IPlantService
    {
        private readonly HttpClient _http;
        private readonly JsonSerializerOptions _options;

        // Constructor
        public PlantService(HttpClient http)
        {
            _http = http;
            _options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        }

        // Get plant by Id
        public async Task<Plant> GetPlantById(int id)
        {
            var response = await _http.GetAsync($"plants/{id}");
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new ApplicationException(content);
            }

            var plant = JsonSerializer.Deserialize<Plant>(content, _options);

            return plant;
        }

        // Get plant including areas
        public async Task<Plant> GetPlantIncludingAreas(int id)
        {
            var response = await _http.GetAsync($"plants/{id}?IncludeAreas=true");
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new ApplicationException(content);
            }

            var plant = JsonSerializer.Deserialize<Plant>(content, _options);

            return plant;
        }

        // Get all plants
        public async Task<List<Plant>> GetPlants()
        {
            var response = await _http.GetAsync("plants");
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new ApplicationException(content);
            }

            var plants = JsonSerializer.Deserialize<List<Plant>>(content, _options);

            return plants;
        }
    }
}
