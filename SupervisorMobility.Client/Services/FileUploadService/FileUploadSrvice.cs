using System.Net.Http.Json;

namespace SupervisorMobility.Client.Services.FileUploadService
{
    public class FileUploadService : IFileUploadService
    {
        private readonly HttpClient _http;
        private readonly JsonSerializerOptions _options;

        // Constructor
        public FileUploadService(HttpClient http)
        {
            _http = http;
            _options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        }

        //
        public async Task<UploadResult> UploadFile(MultipartFormDataContent contentfile)
        {
            var response = await _http.PostAsync("File", contentfile);
           
            var result = await response.Content.ReadFromJsonAsync<UploadResult>();

            return result;
        }
        public async Task<UploadDataResult> SetNewData(UploadResult fileinfo)
        {
            var response = await _http.PostAsJsonAsync("File/Data", fileinfo);

            var result = await response.Content.ReadFromJsonAsync<UploadDataResult>();

            return result;
        }
    }


}