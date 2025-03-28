using DocumentFormat.OpenXml.Presentation;
using Microsoft.JSInterop;
using SupervisorMobility.Client.Data.Entities;
using System.Net.Http.Json;
using System.Runtime.CompilerServices;

namespace SupervisorMobility.Client.Services.SOS_Services.SOSFlowServices
{
    public class SOSFlowService : ISOSFlowService
    {
        private readonly HttpClient _http;
        private readonly JsonSerializerOptions _options;
        private readonly IJSRuntime _js;

        // Constructor
        public SOSFlowService(HttpClient HttpClientService, IJSRuntime jSRuntime)
        {
            _http = HttpClientService;
            _js = jSRuntime;
            _options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        }

      


        public async Task<List<SOSFlow>> GetAllSOSFlow(bool includeImages = false, bool includeNotes = false, bool includeLogbooks = false, bool includeSpecialCases = false, bool includeSOS = false)
        {
            var response = await _http.GetAsync($"SOS/Flow/all?includeImages={includeImages}&includeNotes={includeNotes}&includeLogbooks={includeLogbooks}&includeSpecialCases={includeSpecialCases}&includeSOS={includeSOS}");
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new ApplicationException(content);
            }
            var SOSFlowsRetorned = JsonSerializer.Deserialize<List<SOSFlow>>(content, _options);

            return SOSFlowsRetorned;
        }

        public async Task<SOSFlow> GetSOSFlow(int SOSFlowId, bool includeImages = false, bool includeNotes = false, bool includeLogbooks = false, bool includeSpecialCases = false, bool includeSOS = false, bool includePeople = false)
        {
            var response = await _http.GetAsync($"SOS/Flow/{SOSFlowId}?includeImages={includeImages}&includeNotes={includeNotes}&includeLogbooks={includeLogbooks}&includeSpecialCases={includeSpecialCases}&includeSOS={includeSOS}&includePeople={includePeople}");
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new ApplicationException(content);
            }
            var SOSFlowsRetorned = JsonSerializer.Deserialize<SOSFlow>(content, _options);

            return SOSFlowsRetorned;
        }

        public async Task<SOSFlow> UpdateSOSFlow(SOSFlow SosEntity)
        {
            var response = await _http.PutAsJsonAsync($"SOS/Flow/{SosEntity.SOSFlowId}", SosEntity);
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            var SOSFlowUpdated = JsonSerializer.Deserialize<SOSFlow>(content, _options);

            return SOSFlowUpdated;
        }


        public async Task<bool> DeleteSOSFlow(int SosEntity_id)
        {
            var response = await _http.DeleteAsync($"SOS/Flow/{SosEntity_id}");
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                return false;
            }

            return true;
        }

      

        //public async Task<FileUpload> AddIllustrationToSOSFlow(MultipartFormDataContent? contentfiles, int SOS_SOSFlow_id)
        //{
        //    var response = await _http.PostAsync($"SOS/Flow/Ilustrations/{SOS_SOSFlow_id}", contentfiles);

        //    if (response.IsSuccessStatusCode)
        //    {
        //        var content = await response.Content.ReadAsStringAsync();

        //        var result = JsonSerializer.Deserialize<FileUpload>(content, _options);

        //        return result;

        //    }
        //    else
        //    {
        //        await _js.InvokeVoidAsync("alert", $"Error Upload Data error: {response.Content.ReadAsStringAsync().Result}");
        //    }

        //    return null;
        //}
     

        //public async Task<string> ShowIlustrationSOSFlow(int idfile)
        //{
        //    var response = await _http.GetAsync($"SOS/Flow/Ilustrations/{idfile}");

        //    if (response.IsSuccessStatusCode)
        //    {
        //        var contentType = response.Content.Headers.ContentType.MediaType;
        //        var contentBytes = await response.Content.ReadAsByteArrayAsync();
        //        var base64Content = Convert.ToBase64String(contentBytes);

        //        return $"data:{contentType};base64,{base64Content}";
        //    }
        //    else
        //    {
        //        return "Error Loading Image";
        //    }
        //}

        //public async Task<bool> RemoveIlustrationFromSOSData(int SOS_SOSFlow_id, int ImageFile_id)
        //{
        //    var response = await _http.DeleteAsync($"SOS/Flow/Ilustrations/{SOS_SOSFlow_id}/remove/{ImageFile_id}");
        //    var content = await response.Content.ReadAsStringAsync();

        //    if (!response.IsSuccessStatusCode)
        //    {
        //        return false;
        //    }

        //    return true;
        //}




    }
}
