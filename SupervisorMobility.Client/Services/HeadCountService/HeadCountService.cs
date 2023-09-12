using Microsoft.JSInterop;
using System.Net.Http.Json;
using System.Text.Json.Serialization;

namespace SupervisorMobility.Client.Services.HeadCountService
{
    public class HeadCountService : IHeadCountService
    {
        private readonly HttpClient _http;
        private readonly HttpClient _httpBridge;
        private readonly HttpClient _httpExtends;
        private readonly JsonSerializerOptions _options;
        private readonly IJSRuntime _js;

        // Constructor
        public HeadCountService(CustomHttpClientService customHttpClientService, IJSRuntime jSRuntime)
        {
            _http = customHttpClientService.GetApiHttpClient();
            _httpBridge = customHttpClientService.GetBridgeHttpClient();
            _httpExtends = customHttpClientService.GetApiExtendsHttpClient();
            _js = jSRuntime;
            _options = new JsonSerializerOptions
            {
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault,
                PropertyNameCaseInsensitive = true,
                NumberHandling = JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.WriteAsString
            };
            _options.Converters.Add(new IntToStringConverter());
        }

        public async Task<FileUpload> UploadHeadCount(MultipartFormDataContent contentfile, int userid)
        {
            Console.WriteLine("upload HeadCountFile");

            var response = await _httpExtends.PostAsync($"HeadCount/Upload?UserIdUpload={userid}", contentfile);

            if (response.IsSuccessStatusCode)
            {
                await _js.InvokeVoidAsync("alert", $"Upload Data Succesful");
                return new FileUpload();
            }

            return null;


        }
    }
}
