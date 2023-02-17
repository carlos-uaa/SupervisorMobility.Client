using Microsoft.JSInterop;
using System;
using System.Net.Http.Json;

namespace SupervisorMobility.Client.Services.FileUploadAndDownloadService
{
    public class FileUploadAndDownloadService : IFileUploadAndDownloadService
    {
        private readonly HttpClient _http;
        private readonly JsonSerializerOptions _options;
        private readonly IJSRuntime _js;

        // Constructor
        public FileUploadAndDownloadService(HttpClient http, IJSRuntime jSRuntime)
        {
            _http = http;
            _js = jSRuntime;
            _options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        }

        //
        public async Task<UploadResult> UploadFile(MultipartFormDataContent contentfile)
        {
            var response = await _http.PostAsync("File", contentfile);
           
            var result = await response.Content.ReadFromJsonAsync<UploadResult>();

            return result;
        }
        public async Task<UploadDataResult> ProccedToUpdateData(UploadResult fileinfo)
        {
            var response = await _http.PostAsJsonAsync("File/Data", fileinfo);

            if (response.IsSuccessStatusCode)
            {
                var  result = await response.Content.ReadFromJsonAsync<UploadDataResult>();
                return result;
            }

            return null;
        }

        public async Task DownloadFileFromOnePlant(int idPlant)
        {
            var response = await _http.GetAsync($"File/Bulk/ByPlantId/{idPlant}");

            if (!response.IsSuccessStatusCode)
            {
                await _js.InvokeVoidAsync("alert", "Error File Download");
            }
            else
            {
                var fileStream = response.Content.ReadAsStreamAsync();
                using var streamRef = new DotNetStreamReference(stream: await fileStream);
                await _js.InvokeVoidAsync("downloadFileFromStream", "Report One Plant.xlsx", streamRef);
            }

        }
        public async Task DownloadFileFromAllPlants()
        {
            var response = await _http.GetAsync($"File/Bulk/ByPlantId");

            if (!response.IsSuccessStatusCode)
            {
                await _js.InvokeVoidAsync("alert", "Error File Download");
            }
            else
            {
                var fileStream = response.Content.ReadAsStreamAsync();
                using var streamRef = new DotNetStreamReference(stream: await fileStream);
                await _js.InvokeVoidAsync("downloadFileFromStream", "Report ALL Plants.xlsx", streamRef);
            }
        }
    }


}