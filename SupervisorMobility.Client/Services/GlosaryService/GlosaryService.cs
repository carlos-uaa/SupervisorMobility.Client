using Microsoft.JSInterop;
using System.Net.Http.Json;

namespace SupervisorMobility.Client.Services.GlosaryService
{
    public class GlosaryService : IGlosaryService
    {
        private readonly HttpClient _http;
        private readonly JsonSerializerOptions _options;

        // Constructor
        public GlosaryService(HttpClient customHttpClientService, IJSRuntime jSRuntime)
        {
            _http = customHttpClientService;
            _options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        }

        public async Task<Glosary> CreateGlosaryWord(Glosary glosaryWord)
        {
            var response = await _http.PostAsJsonAsync("glosary", glosaryWord);
            var newGlosaryWord = await response.Content.ReadFromJsonAsync<Glosary>();

            return newGlosaryWord;
        }

        public async Task DeleteGlosaryWord(int id)
        {
            var response = await _http.DeleteAsync($"glosary/{id}");
        }

        public async Task<Glosary> GetGlosaryWordbyId(int id)
        {
            var response = await _http.GetAsync($"glosary/{id}");
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new ApplicationException(content);
            }

            var glosaryWord = JsonSerializer.Deserialize<Glosary>(content, _options);

            return glosaryWord;
        }

        public async Task<List<Glosary>> GetGlosary()
        {
            var response = await _http.GetAsync("glosary");
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new ApplicationException(content);
            }

            var glosary = JsonSerializer.Deserialize<List<Glosary>>(content, _options);

            return glosary;
        }

        public async Task<bool> UpdateGlosaryWord(Glosary glosaryWord)
        {
            var response = await _http.PutAsJsonAsync($"glosary/{glosaryWord.GlosaryWordId}", glosaryWord);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return true;
            }

            return false;
        }
    }
}
