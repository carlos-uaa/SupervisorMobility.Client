using DocumentFormat.OpenXml.Presentation;
using Microsoft.JSInterop;
using SupervisorMobility.Client.Data.Entities;
using SupervisorMobility.Client.Data.Entities.SOS_Process;
using System.Net.Http.Json;
using System.Runtime.CompilerServices;

namespace SupervisorMobility.Client.Services.SOS_Services.SOSSynopticControlPointsService
{
    public class SynopticControlPointsService : ISynopticControlPointsService
    {
        private readonly HttpClient _http;
        private readonly JsonSerializerOptions _options;
        private readonly IJSRuntime _js;

        // Constructor
        public SynopticControlPointsService(HttpClient HttpClientService, IJSRuntime jSRuntime)
        {
            _http = HttpClientService;
            _js = jSRuntime;
            _options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        }

        public async Task<List<SOSSynopticTableofControlPoints>> GetAllSOSSynopticTableofControlPoints(bool includeLogbooks = false, bool includeSOS = false, bool includeCollections = false)
        {
            var response = await _http.GetAsync($"SOS/SynopticTableofControlPoints/all?includeLogbooks={includeLogbooks}&includeSOS={includeSOS}&includeCollections={includeCollections}");
            var content = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
            {
                throw new ApplicationException(content);
            }
            var SOSSynopticTableofControlPointssRetorned = JsonSerializer.Deserialize<List<SOSSynopticTableofControlPoints>>(content, _options);
            return SOSSynopticTableofControlPointssRetorned;
        }

        public async Task<SOSSynopticTableofControlPoints> GetSOSSynopticTableofControlPoints(int SOSSynopticTableofControlPointsId, bool includeLogbooks = false, bool includeSOS = false, bool includeCollections = false)
        {
            var response = await _http.GetAsync($"SOS/SynopticTableofControlPoints/{SOSSynopticTableofControlPointsId}?includeLogbooks={includeLogbooks}&includeSOS={includeSOS}&includeCollections={includeCollections}");
            var content = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
            {
                throw new ApplicationException(content);
            }
            var SOSSynopticTableofControlPointssRetorned = JsonSerializer.Deserialize<SOSSynopticTableofControlPoints>(content, _options);
            return SOSSynopticTableofControlPointssRetorned;
        }

        public async Task<SOSSynopticTableofControlPoints> UpdateSOSSynopticTableofControlPoints(SOSSynopticTableofControlPoints SosEntity)
        {
            var response = await _http.PutAsJsonAsync($"SOS/SynopticTableofControlPoints/{SosEntity.SOSSynopticTableofControlPointsId}", SosEntity);
            var content = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
            {
                return null;
            }
            var SOSSynopticTableofControlPointsUpdated = JsonSerializer.Deserialize<SOSSynopticTableofControlPoints>(content, _options);
            return SOSSynopticTableofControlPointsUpdated;
        }

        public async Task<bool> DeleteSOSSynopticTableofControlPoints(int SosEntity_id)
        {
            var response = await _http.DeleteAsync($"SOS/SynopticTableofControlPoints/{SosEntity_id}");
            var content = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
            {
                return false;
            }
            return true;
        }

        public async Task<FileUpload> AddIllustrationToSOSSynopticTableofControlPoints(MultipartFormDataContent? contentfiles, int SOS_SOSSynopticTableofControlPoints_id)
        {
            var response = await _http.PostAsync($"SOS/SynopticTableofControlPoints/Ilustrations/{SOS_SOSSynopticTableofControlPoints_id}", contentfiles);
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

        public async Task<string> ShowIlustrationSOSSynopticTableofControlPoints(int idfile)
        {
            var response = await _http.GetAsync($"SOS/SynopticTableofControlPoints/Ilustrations/{idfile}");
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

        public async Task<bool> RemoveIlustrationFromSOSData(int SOS_SOSSynopticTableofControlPoints_id, int ImageFile_id)
        {
            var response = await _http.DeleteAsync($"SOS/SynopticTableofControlPoints/Ilustrations/{SOS_SOSSynopticTableofControlPoints_id}/remove/{ImageFile_id}");
            var content = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
            {
                return false;
            }
            return true;
        }
    }
}
