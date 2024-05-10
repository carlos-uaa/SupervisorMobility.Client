using Microsoft.JSInterop;
using System.Net.Http.Json;

namespace SupervisorMobility.Client.Services.PillarService
{
    public class PillarService : IPillarService
    {
        private readonly HttpClient _http;
        private readonly JsonSerializerOptions _options;

        // Constructor
        public PillarService(HttpClient customHttpClientService, IJSRuntime jSRuntime)
        {
            _http = customHttpClientService;
            _options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        }

        // Create pillar
        public async Task<Pillar> CreatePillar(Pillar pillar)
        {
            var response = await _http.PostAsJsonAsync("pillars", pillar);
            var newPillar = await response.Content.ReadFromJsonAsync<Pillar>();

            return newPillar;
        }

        // Delete pillar
        public async Task DeletePillar(int id)
        {
            var response = await _http.DeleteAsync($"pillars/{id}");
        }

        // Get pillar by Id
        public async Task<Pillar> GetPillarById(int id)
        {
            var response = await _http.GetAsync($"pillars/{id}");
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new ApplicationException(content);
            }

            var pillar = JsonSerializer.Deserialize<Pillar>(content, _options);

            return pillar;
        }

        // Get all pillars
        public async Task<List<Pillar>> GetPillars()
        {
            var response = await _http.GetAsync("pillars");
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new ApplicationException(content);
            }

            var pillars = JsonSerializer.Deserialize<List<Pillar>>(content, _options);

            return pillars;
        }

        // Update pillar
        public async Task<bool> UpdatePillar(Pillar pillar)
        {
            var response = await _http.PutAsJsonAsync($"pillars/{pillar.PillarId}", pillar);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return true;
            }

            return false;
        }
    }
}
