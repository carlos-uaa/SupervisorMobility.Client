using DocumentFormat.OpenXml.Presentation;
using Microsoft.JSInterop;
using SupervisorMobility.Client.Data.Entities;
using SupervisorMobility.Client.Data.Entities.SOS_Process;
using System.Net.Http.Json;
using System.Runtime.CompilerServices;

namespace SupervisorMobility.Client.Services.SOS_Services.SOSAnalysisServices
{
    public class SOSAnalysisService : ISOSAnalysisService
    {
        private readonly HttpClient _http;
        private readonly JsonSerializerOptions _options;
        private readonly IJSRuntime _js;

        // Constructor
        public SOSAnalysisService(HttpClient HttpClientService, IJSRuntime jSRuntime)
        {
            _http = HttpClientService;
            _js = jSRuntime;
            _options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        }




        public async Task<List<SOSAnalysis>> GetAllSOSAnalysis(bool includeImages = false, bool includeNotes = false, bool includeLogbooks = false, bool includeSpecialCases = false, bool includeSOS = false)
        {
            var response = await _http.GetAsync($"SOS/Analysis/all?includeImages={includeImages}&includeNotes={includeNotes}&includeLogbooks={includeLogbooks}&includeSpecialCases={includeSpecialCases}&includeSOS={includeSOS}");
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new ApplicationException(content);
            }
            var SOSAnalysissRetorned = JsonSerializer.Deserialize<List<SOSAnalysis>>(content, _options);

            return SOSAnalysissRetorned;
        }

        public async Task<SOSAnalysis> GetSOSAnalysis(int SOSAnalysisId, bool includeImages = false, bool includeNotes = false, bool includeLogbooks = false, bool includeSpecialCases = false, bool includeSOS = false, bool includeImagesSOS = false)
        {
            var response = await _http.GetAsync($"SOS/Analysis/{SOSAnalysisId}?includeImages={includeImages}&includeNotes={includeNotes}&includeLogbooks={includeLogbooks}&includeSpecialCases={includeSpecialCases}&includeSOS={includeSOS}&includeImagesSOS={includeImagesSOS}");
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new ApplicationException(content);
            }
            var SOSAnalysissRetorned = JsonSerializer.Deserialize<SOSAnalysis>(content, _options);

            return SOSAnalysissRetorned;
        }
        public async Task<SOSAnalysis> UpdateSOSAnalysis(SOSAnalysis SosEntity)
        {
            var response = await _http.PutAsJsonAsync($"SOS/Analysis/{SosEntity.SOSAnalysisId}", SosEntity);
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            var SOSAnalysisUpdated = JsonSerializer.Deserialize<SOSAnalysis>(content, _options);

            return SOSAnalysisUpdated;
        }


        public async Task<SOSAnalysis> DeleteSOSAnalysis(int SosEntity_id)
        {
            var response = await _http.DeleteAsync($"SOS/Analysis/{SosEntity_id}");
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            var SOSHubsRetorned = JsonSerializer.Deserialize<SOSAnalysis>(content, _options);

            return SOSHubsRetorned;
        }



        public async Task<FileUpload> AddIllustrationToSOSAnalysis(MultipartFormDataContent? contentfiles, int SOS_SOSAnalysis_id)
        {
            var response = await _http.PostAsync($"SOS/Analysis/Ilustrations/{SOS_SOSAnalysis_id}", contentfiles);

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


        public async Task<string> ShowIlustrationSOSAnalysis(int idfile)
        {
            var response = await _http.GetAsync($"SOS/Analysis/Ilustrations/{idfile}");

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
        public async Task<bool> RemoveIlustrationFromSOSData(int SOS_SOSAnalysis_id, int ImageFile_id)
        {
            var response = await _http.DeleteAsync($"SOS/Analysis/Ilustrations/{SOS_SOSAnalysis_id}/remove/{ImageFile_id}");
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                return false;
            }

            return true;
        }


    }
}
