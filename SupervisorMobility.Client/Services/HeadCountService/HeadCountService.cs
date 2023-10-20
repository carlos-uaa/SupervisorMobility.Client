using Microsoft.JSInterop;
using System.Net.Http.Json;
using System.Text.Json.Serialization;

namespace SupervisorMobility.Client.Services.HeadCountService
{
    public class HeadCountService : IHeadCountService
    {
        private readonly HttpClient _http;
        private readonly JsonSerializerOptions _options;
        private readonly IJSRuntime _js;

        // Constructor
        public HeadCountService(HttpClient customHttpClientService, IJSRuntime jSRuntime)
        {
            _http = customHttpClientService;
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

            var response = await _http.PostAsync($"HeadCount/Upload?UserIdUpload={userid}", contentfile);

            if (response.IsSuccessStatusCode)
            {
                await _js.InvokeVoidAsync("alert", $"Upload Data Succesful");
                return new FileUpload();
            }

            return null;


        }

        public async Task<bool> UpdateHeadCount(HeadCount ToUpdate, int HeadId)
        {
            var response = await _http.PutAsJsonAsync($"HeadCount/{HeadId}", ToUpdate);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return true;
            }

            return false;
        }

        public async Task<List<HeadCount>> GetAllHeadCout()
        {
            Console.WriteLine("Get All HeadCountFile");

            var response = await _http.GetAsync($"HeadCount");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();

                var HeadCount = JsonSerializer.Deserialize<List<HeadCount>>(content, _options);

                response.Dispose();

                return HeadCount;

            }

            return null;
        }
    }
}
