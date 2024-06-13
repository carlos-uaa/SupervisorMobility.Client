using Microsoft.JSInterop;
using static System.Net.WebRequestMethods;

namespace SupervisorMobility.Client.Services.TestServices
{
    public class TestService : ITestService
    {
        private readonly HttpClient _http;
        private readonly JsonSerializerOptions _options;
        private readonly IJSRuntime _js;

        // Constructor
        public TestService(HttpClient customHttpClientService, IJSRuntime jSRuntime)
        {
            _http = customHttpClientService;
            _js = jSRuntime;
            _options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        }
        public async Task<(int,string)> UploadVideoFiles(MultipartFormDataContent contentfile)
        {
            var response = await _http.PostAsync($"File/VideoTestUpload", contentfile);

            return ((int)response.StatusCode, response.Content.Headers.ToString());
        }
    }
}
