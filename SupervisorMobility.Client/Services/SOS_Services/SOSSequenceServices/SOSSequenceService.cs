using DocumentFormat.OpenXml.Presentation;
using Microsoft.JSInterop;
using SupervisorMobility.Client.Data.Entities;
using SupervisorMobility.Client.Data.Entities.SOS_Process;
using System.Net.Http.Json;
using System.Runtime.CompilerServices;

namespace SupervisorMobility.Client.Services.SOS_Services.SOSSequenceServices
{
    public class SOSSequenceService : ISOSSequenceService
    {
        private readonly HttpClient _http;
        private readonly JsonSerializerOptions _options;
        private readonly IJSRuntime _js;

        // Constructor
        public SOSSequenceService(HttpClient HttpClientService, IJSRuntime jSRuntime)
        {
            _http = HttpClientService;
            _js = jSRuntime;
            _options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        }




        public async Task<List<SOSSequence>> GetAllSOSSequence(bool includeImages = false, bool includeNotes = false, bool includeLogbooks = false, bool includeSpecialCases = false, bool includeSOS = false)
        {
            var response = await _http.GetAsync($"SOS/Sequence/all?includeImages={includeImages}&includeNotes={includeNotes}&includeLogbooks={includeLogbooks}&includeSpecialCases={includeSpecialCases}&includeSOS={includeSOS}");
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new ApplicationException(content);
            }
            var SOSSequencesRetorned = JsonSerializer.Deserialize<List<SOSSequence>>(content, _options);

            return SOSSequencesRetorned;
        }

        public async Task<SOSSequence> GetSOSSequence(int SOSSequenceId, bool includeImages = false, bool includeNotes = false, bool includeLogbooks = false, bool includeSpecialCases = false, bool includeSOS = false, bool includeImagesSOS = false)
        {
            var response = await _http.GetAsync($"SOS/Sequence/{SOSSequenceId}?includeImages={includeImages}&includeNotes={includeNotes}&includeLogbooks={includeLogbooks}&includeSpecialCases={includeSpecialCases}&includeSOS={includeSOS}&includeImagesSOS={includeImagesSOS}");
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new ApplicationException(content);
            }
            var SOSSequencesRetorned = JsonSerializer.Deserialize<SOSSequence>(content, _options);

            return SOSSequencesRetorned;
        }
        public async Task<SOSSequence> UpdateSOSSequence(SOSSequence SosEntity)
        {
            var response = await _http.PutAsJsonAsync($"SOS/Sequence/{SosEntity.SOSSequenceId}", SosEntity);
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            var SOSSequenceUpdated = JsonSerializer.Deserialize<SOSSequence>(content, _options);

            return SOSSequenceUpdated;
        }


        public async Task<SOSSequence> DeleteSOSSequence(int SosEntity_id)
        {
            var response = await _http.DeleteAsync($"SOS/Sequence/{SosEntity_id}");
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            var SOSHubsRetorned = JsonSerializer.Deserialize<SOSSequence>(content, _options);

            return SOSHubsRetorned;
        }



        public async Task<FileUpload> AddIllustrationToSOSSequence(MultipartFormDataContent? contentfiles, int SOS_SOSSequence_id)
        {
            var response = await _http.PostAsync($"SOS/Sequence/Ilustrations/{SOS_SOSSequence_id}", contentfiles);

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


        public async Task<string> ShowIlustrationSOSSequence(int idfile)
        {
            var response = await _http.GetAsync($"SOS/Sequence/Ilustrations/{idfile}");

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
        public async Task<bool> RemoveIlustrationFromSOSData(int SOS_SOSSequence_id, int ImageFile_id)
        {
            var response = await _http.DeleteAsync($"SOS/Sequence/Ilustrations/{SOS_SOSSequence_id}/remove/{ImageFile_id}");
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                return false;
            }

            return true;
        }


    }
}
