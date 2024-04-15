using Microsoft.JSInterop;
using System.Net.Http.Json;

namespace SupervisorMobility.Client.Services.KaizenService
{
    public class KaizenService : IKaizenService
    {
        private readonly HttpClient _http;
        private readonly JsonSerializerOptions _options;
        private readonly IJSRuntime _js;

        // Constructor
        public KaizenService(HttpClient customHttpClientService, IJSRuntime jSRuntime)
        {
            _http = customHttpClientService;
            _js = jSRuntime;
            _options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        }

        public async Task<Kaizen> CreateKaizen(Kaizen kaizen)
        {
            var response = await _http.PostAsJsonAsync($"kaizen", kaizen);
            var newKaizen = await response.Content.ReadFromJsonAsync<Kaizen>();

            return newKaizen;
        }

        public async Task DeleteKaizen(int kaizenId)
        {
            var response = await _http.DeleteAsync($"kaizen/{kaizenId}");
        }


        public async Task<List<Kaizen>> GetAllKaizen()
        {
            var response = await _http.GetAsync($"kaizen");
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new ApplicationException(content);
            }

            var kaizen = JsonSerializer.Deserialize<List<Kaizen>>(content, _options);

            return kaizen;
        }

        public async Task<Kaizen> GetKaizenById(int kaizenId)
        {
            var response = await _http.GetAsync($"kaizen/{kaizenId}");
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new ApplicationException(content);
            }

            var kaizen = JsonSerializer.Deserialize<Kaizen>(content, _options);

            return kaizen;
        }

        public async Task<Kaizen> GetKaizenByIdWhitFile(int kaizenId)
        {
            var response = await _http.GetAsync($"kaizen/{kaizenId}?includeFile=true");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();

                var kaizen = JsonSerializer.Deserialize<Kaizen>(content, _options);
                return kaizen;
            }
            await _js.InvokeVoidAsync("alert", $"Error : {response.Content.ReadAsStringAsync().Result}");
            return null;

        }

        public async Task<bool> UpdateKaizen(Kaizen kaizen)
        {
            var response = await _http.PutAsJsonAsync($"kaizen/{kaizen.KaizenId}", kaizen);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return true;
            }

            return false;
        }

        public async Task<bool> RemoveEvidence(int kaizenId, int fileUploadId)
        {
            var response = await _http.PostAsJsonAsync($"kaizen/{kaizenId}/evidence/remove", fileUploadId);

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return true;
            }

            return false;
        }

    }
}
