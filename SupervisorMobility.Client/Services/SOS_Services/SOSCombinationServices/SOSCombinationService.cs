using DocumentFormat.OpenXml.Presentation;
using Microsoft.JSInterop;
using SupervisorMobility.Client.Data.Entities;
using System.Net.Http.Json;
using System.Runtime.CompilerServices;

namespace SupervisorMobility.Client.Services.SOS_Services.SOSCombinationServices
{
    public class SOSCombinationService : ISOSCombinationService
    {
        private readonly HttpClient _http;
        private readonly JsonSerializerOptions _options;
        private readonly IJSRuntime _js;

        // Constructor
        public SOSCombinationService(HttpClient HttpClientService, IJSRuntime jSRuntime)
        {
            _http = HttpClientService;
            _js = jSRuntime;
            _options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        }

      


        public async Task<List<SOSCombination>> GetAllSOSCombination(bool includeImages = false, bool includeNotes = false, bool includeLogbooks = false, bool includeSpecialCases = false, bool includeSOS = false)
        {
            var response = await _http.GetAsync($"SOS/Combination/all?includeImages={includeImages}&includeNotes={includeNotes}&includeLogbooks={includeLogbooks}&includeSpecialCases={includeSpecialCases}&includeSOS={includeSOS}");
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new ApplicationException(content);
            }
            var SOSCombinationsRetorned = JsonSerializer.Deserialize<List<SOSCombination>>(content, _options);

            return SOSCombinationsRetorned;
        }

        public async Task<SOSCombination> GetSOSCombination(int SOSCombinationId, bool includeImages = false, bool includeNotes = false, bool includeLogbooks = false, bool includeSpecialCases = false, bool includeSOS = false, bool includeProcess = false)
        {
            var response = await _http.GetAsync($"SOS/Combination/{SOSCombinationId}?includeImages={includeImages}&includeNotes={includeNotes}&includeLogbooks={includeLogbooks}&includeSpecialCases={includeSpecialCases}&includeSOS={includeSOS}&includeProcess={includeProcess}");
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new ApplicationException(content);
            }
            var SOSCombinationsRetorned = JsonSerializer.Deserialize<SOSCombination>(content, _options);

            return SOSCombinationsRetorned;
        }

        public async Task<SOSCombination> UpdateSOSCombination(SOSCombination SosEntity)
        {
            SosEntity.SOSHub.SOSCombination.Clear();
            var response = await _http.PutAsJsonAsync($"SOS/Combination/{SosEntity.SOSCombinationId}", SosEntity);
            var content = await response.Content.ReadAsStringAsync();
         
            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            var SOSCombinationUpdated = JsonSerializer.Deserialize<SOSCombination>(content, _options);

            return SOSCombinationUpdated;
        }


        public async Task<bool> DeleteSOSCombination(int SosEntity_id)
        {
            var response = await _http.DeleteAsync($"SOS/Combination/{SosEntity_id}");
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                return false;
            }

            
            return true;
        }



        public async Task<FileUpload> AddIllustrationToSOSCombination(MultipartFormDataContent? contentfiles, int SOS_SOSCombination_id)
        {
            var response = await _http.PostAsync($"SOS/Combination/Ilustrations/{SOS_SOSCombination_id}", contentfiles);

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


        public async Task<string> ShowIlustrationSOSCombination(int idfile)
        {
            var response = await _http.GetAsync($"SOS/Combination/Ilustrations/{idfile}");

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

        public async Task<bool> RemoveIlustrationFromSOSData(int SOS_SOSCombination_id, int ImageFile_id)
        {
            var response = await _http.DeleteAsync($"SOS/Combination/Ilustrations/{SOS_SOSCombination_id}/remove/{ImageFile_id}");
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                return false;
            }

            return true;
        }




    }
}
