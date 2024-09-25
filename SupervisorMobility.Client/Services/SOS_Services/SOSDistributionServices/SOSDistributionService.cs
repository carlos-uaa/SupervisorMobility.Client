using DocumentFormat.OpenXml.Presentation;
using Microsoft.JSInterop;
using SupervisorMobility.Client.Data.Entities;
using SupervisorMobility.Client.Data.Entities.SOS_Process;
using System.Net.Http.Json;
using System.Runtime.CompilerServices;

namespace SupervisorMobility.Client.Services.SOS_Services.SOSDistributionServices
{
    public class SOSDistributionService : ISOSDistributionService
    {
        private readonly HttpClient _http;
        private readonly JsonSerializerOptions _options;
        private readonly IJSRuntime _js;

        // Constructor
        public SOSDistributionService(HttpClient HttpClientService, IJSRuntime jSRuntime)
        {
            _http = HttpClientService;
            _js = jSRuntime;
            _options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        }




        public async Task<List<SOSDistribution>> GetAllSOSDistribution(bool includeImages = false, bool includeNotes = false, bool includeLogbooks = false, bool includeSpecialCases = false, bool includeSOS = false)
        {
            var response = await _http.GetAsync($"SOS/Distribution/all?includeImages={includeImages}&includeNotes={includeNotes}&includeLogbooks={includeLogbooks}&includeSpecialCases={includeSpecialCases}&includeSOS={includeSOS}");
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new ApplicationException(content);
            }
            var SOSDistributionsRetorned = JsonSerializer.Deserialize<List<SOSDistribution>>(content, _options);

            return SOSDistributionsRetorned;
        }

        public async Task<SOSDistribution> GetSOSDistribution(int SOSDistributionId, bool includeImages = false, bool includeNotes = false, bool includeLogbooks = false, bool includeSpecialCases = false, bool includeSOS = false, bool includeImagesSOS = false, bool includeTurns = false, bool includeTimes = false)
        {
            var response = await _http.GetAsync($"SOS/Distribution/{SOSDistributionId}?includeImages={includeImages}&includeNotes={includeNotes}&includeLogbooks={includeLogbooks}&includeSpecialCases={includeSpecialCases}&includeSOS={includeSOS}&includeImagesSOS={includeImagesSOS}&includeTurns={includeTurns}&includeTimes={includeTimes}");
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new ApplicationException(content);
            }
            var SOSDistributionsRetorned = JsonSerializer.Deserialize<SOSDistribution>(content, _options);

            return SOSDistributionsRetorned;
        }
        public async Task<SOSDistribution> UpdateSOSDistribution(SOSDistribution SosEntity)
        {
            var response = await _http.PutAsJsonAsync($"SOS/Distribution/{SosEntity.SOSDistributionId}", SosEntity);
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            var SOSDistributionUpdated = JsonSerializer.Deserialize<SOSDistribution>(content, _options);

            return SOSDistributionUpdated;
        }


        public async Task<bool> DeleteSOSDistribution(int SosEntity_id)
        {
            var response = await _http.DeleteAsync($"SOS/Distribution/{SosEntity_id}");
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                return false;
            }

            return true;
        }



        public async Task<FileUpload> AddIllustrationToSOSDistribution(MultipartFormDataContent? contentfiles, int SOS_SOSDistribution_id)
        {
            var response = await _http.PostAsync($"SOS/Distribution/Ilustrations/{SOS_SOSDistribution_id}", contentfiles);

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


        public async Task<string> ShowIlustrationSOSDistribution(int idfile)
        {
            var response = await _http.GetAsync($"SOS/Distribution/Ilustrations/{idfile}");

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
        public async Task<bool> RemoveIlustrationFromSOSData(int SOS_SOSDistribution_id, int ImageFile_id)
        {
            var response = await _http.DeleteAsync($"SOS/Distribution/Ilustrations/{SOS_SOSDistribution_id}/remove/{ImageFile_id}");
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                return false;
            }

            return true;
        }



    }
}
