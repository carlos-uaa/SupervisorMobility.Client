using Microsoft.JSInterop;
using SupervisorMobility.Client.Data.Entities;
using System.Net.Http.Json;

namespace SupervisorMobility.Client.Services.GuideService
{
    public class GuideService : IGuideService
    {
        private readonly HttpClient _http;
        private readonly JsonSerializerOptions _options;
        private readonly IFileUploadAndDownloadService FilesServices;
        private readonly IJSRuntime _js;


        // Constructor
        public GuideService(HttpClient http, IJSRuntime jSRuntime)
        {
            _http = http;
            _js = jSRuntime;
            _options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        }

        public async Task<Guide> CreateGuide(Guide guideData)
        {
      
            var response = await _http.PostAsJsonAsync("guides", guideData);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadFromJsonAsync<Guide>();
                return content;
            }
            await _js.InvokeVoidAsync("alert", $"Error : {response.Content.ReadAsStringAsync().Result}");
            return null;

        }


        public async Task<List<Guide>> GetAllGuides()
        {
            var response = await _http.GetAsync($"guides");


            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();

                var Guides = JsonSerializer.Deserialize<List<Guide>>(content, _options);
                return Guides;
            }
            await _js.InvokeVoidAsync("alert", $"Error : {response.Content.ReadAsStringAsync().Result}");
            return null;


        }
        public async Task<List<Guide>> GetAllGuidesWhitFile()
        {
            var response = await _http.GetAsync($"guides?includeFiles=true");


            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();

                var Guides = JsonSerializer.Deserialize<List<Guide>>(content, _options);
                return Guides;
            }
            await _js.InvokeVoidAsync("alert", $"Error : {response.Content.ReadAsStringAsync().Result}");
            return null;

        }

        // Get product by Id
        public async  Task<Guide> GetGuideById(int guideid)
        {
            var response = await _http.GetAsync($"guides/{guideid}");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();

                var guide = JsonSerializer.Deserialize<Guide>(content, _options);
                return guide;
            }
            await _js.InvokeVoidAsync("alert", $"Error : {response.Content.ReadAsStringAsync().Result}");
            return null;

        }
        public async Task<Guide> GetGuideByIdWhitFile(int guideid)
        {
            var response = await _http.GetAsync($"guides/{guideid}?includeFile=true");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();

                var guide = JsonSerializer.Deserialize<Guide>(content, _options);
                return guide;
            }
            await _js.InvokeVoidAsync("alert", $"Error : {response.Content.ReadAsStringAsync().Result}");
            return null;

        }

    }
}
