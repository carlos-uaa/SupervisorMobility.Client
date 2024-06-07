using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Office2010.PowerPoint;
using DocumentFormat.OpenXml.Packaging;
using Microsoft.JSInterop;
using System.Net.Http.Json;

namespace SupervisorMobility.Client.Services.IS_Services.PartService
{
    public class PartServices : IPartServices
    {
        private readonly HttpClient _http;
        private readonly JsonSerializerOptions _options;
        private readonly IJSRuntime _js;

        // Constructor
        public PartServices(HttpClient customHttpClientService, IJSRuntime jSRuntime)
        {
            _http = customHttpClientService;
            _js = jSRuntime;
            _options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        }

        public async Task<Part?> CreatePart(Part partToCreate)
        {
            var response = await _http.PostAsJsonAsync($"IS/Part", partToCreate);
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new ApplicationException(content);
            }

            var part = JsonSerializer.Deserialize<Part>(content, _options);

            return part; 

        }
        public async Task<Part?> GetPart(int part_id, bool includeScketes = false, bool includeModel = false)
        {
            var response = await _http.GetAsync($"IS/Part/{part_id}?includeScketes={includeScketes}&includeModel={includeModel}");
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new ApplicationException(content);
            }

            var part = JsonSerializer.Deserialize<Part>(content, _options);

            return part;
        }


        public async Task<List<Part>> GetAllParts(bool includeScketes = false)
        {
            var response = await _http.GetAsync($"IS/Part?includeScketes={includeScketes}");
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new ApplicationException(content);
            }

            var parts = JsonSerializer.Deserialize<List<Part>>(content, _options);

            return parts;
        }

        public async Task<Part?> UpdatePart(Part partToUpdate)
        {
            var response = await _http.PutAsJsonAsync($"IS/Part/{partToUpdate.PartId}", partToUpdate);
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            var part = JsonSerializer.Deserialize<Part>(content, _options);

            return part;
        }
        public async Task<bool> DeletePart(int part_id)
        {
            var response = await _http.DeleteAsync($"IS/Part/{part_id}");
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                return  false;
            }

                return  true;
        }

        public async Task<FileUpload> UploadSketchPart(MultipartFormDataContent? contentfiles, int part_id)
        {
            var response = await _http.PostAsync($"IS/Part/UploadPartSkecth/{part_id}", contentfiles);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();

                var result = JsonSerializer.Deserialize<FileUpload>(content, _options);

                return result;

            }
            else
            {
                await _js.InvokeVoidAsync("alert", $"Error Upload Data error: {response.Content.ReadAsStringAsync().Result}");
            }

            return null;

        }

        public async Task<string> ShowImagePart(int idfile)
        {
            var response = await _http.GetAsync($"IS/Part/PartSketch/{idfile}");

            if (response.IsSuccessStatusCode)
            {
                var contentType = response.Content.Headers.ContentType.MediaType;
                var contentBytes = await response.Content.ReadAsByteArrayAsync();
                var base64Content = Convert.ToBase64String(contentBytes);

                return $"data:{contentType};base64,{base64Content}";
            }
            else
            {
                return "Error Loading Image";
            }
        }

    }
}
