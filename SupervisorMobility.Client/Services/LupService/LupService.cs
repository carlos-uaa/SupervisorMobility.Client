using Microsoft.JSInterop;
using System.Net.Http.Json;

namespace SupervisorMobility.Client.Services.LupService
{
    public class LupService : ILupService
    {
        private readonly HttpClient _http;
        private readonly HttpClient _httpBridge;
        private readonly JsonSerializerOptions _options;

        // Constructor
        public LupService(CustomHttpClientService customHttpClientService, IJSRuntime jSRuntime)
        {
            _http = customHttpClientService.GetApiHttpClient();
            _httpBridge = customHttpClientService.GetBridgeHttpClient();
            _options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        }

        public async Task<Lup> CreateLup(Lup lup)
        {
            var response = await _http.PostAsJsonAsync($"lup", lup);
            var newLup = await response.Content.ReadFromJsonAsync<Lup>();

            return newLup;
        }

        public async Task DeleteLup(int lupId)
        {
            var response = await _http.DeleteAsync($"lup/{lupId}");
        }


        public async Task<List<Lup>> GetAllLup()
        {
            var response = await _http.GetAsync($"lup");
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new ApplicationException(content);
            }

            var lup = JsonSerializer.Deserialize<List<Lup>>(content, _options);

            return lup;
        }

        public async Task<Lup> GetLupById(int lupId)
        {
            var response = await _http.GetAsync($"lup/{lupId}");
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new ApplicationException(content);
            }

            var lup = JsonSerializer.Deserialize<Lup>(content, _options);

            return lup;
        }

        public async Task<bool> UpdateLup(Lup lup)
        {
            var response = await _http.PutAsJsonAsync($"lup/{lup.LupId}", lup);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return true;
            }

            return false;
        }

    }
}
