using DocumentFormat.OpenXml.Presentation;
using Microsoft.JSInterop;
using SupervisorMobility.Client.Data.Entities;
using SupervisorMobility.Client.Data.Entities.SOSAnalysis_Process;
using System.Net.Http.Json;
using System.Runtime.CompilerServices;

namespace SupervisorMobility.Client.Services.SOS_Services.SOSHubService
{
    public class SOSHubService : ISOSHubService
    {
        private readonly HttpClient _http;
        private readonly JsonSerializerOptions _options;
        private readonly IJSRuntime _js;

        // Constructor
        public SOSHubService(HttpClient HttpClientService, IJSRuntime jSRuntime)
        {
            _http = HttpClientService;
            _js = jSRuntime;
            _options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        }

        public async Task<SOSHub> CreateSOScollection(SOSHub SOS_EntityToCreate)
        {
            var response = await _http.PostAsJsonAsync($"SOS/DataPool", SOS_EntityToCreate);
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new ApplicationException(content);
            }

            var SOSHubCreated = JsonSerializer.Deserialize<SOSHub>(content, _options);

            return SOSHubCreated;
        }
        public async Task<SOSHub> GetSOSHub(int HubId, bool includeAnalysesBkup = false, bool includeSections = false, bool includeImages = false, bool includeVideos = false, bool includeCommentaries = false, bool includeTools = false, bool includeEquipments = false, bool includeMaterials = false, bool includeInformation = false, bool includePeople = false, bool includeDocuments = false)
        {
            var response = await _http.GetAsync($"SOS/DataPool/{HubId}?includeAnalysesBkup={includeAnalysesBkup}&includeSections={includeSections}&includeImages={includeImages}&includeVideos={includeVideos}&includeCommentaries={includeCommentaries}&includeTools={includeTools}&includeEquipments={includeEquipments}&includeMaterials={includeMaterials}&includeInformation={includeInformation}&includePeople={includePeople}&includeDocuments={includeDocuments}");
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new ApplicationException(content);
            }

            var SOSHubReturned = JsonSerializer.Deserialize<SOSHub>(content, _options);

            return SOSHubReturned;
        }
        public async Task<List<SOSHub>> GetAllSOSHub(bool includeAnalysesBkup = false, bool includeSections = false, bool includeImages = false, bool includeVideos = false, bool includeCommentaries = false, bool includeTools = false, bool includeEquipments = false, bool includeMaterials = false, bool includeInformation = false, bool includePeople = false, bool includeDocuments = false)
        {
            var response = await _http.GetAsync($"SOS/DataPool?includeAnalysesBkup={includeAnalysesBkup}&includeSections={includeSections}&includeImages={includeImages}&includeVideos={includeVideos}&includeCommentaries={includeCommentaries}&includeTools={includeTools}&includeEquipments={includeEquipments}&includeMaterials={includeMaterials}&includeInformation={includeInformation}&includePeople={includePeople}&includeDocuments={includeDocuments}");
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new ApplicationException(content);
            }
            var SOSHubsRetorned = JsonSerializer.Deserialize<List<SOSHub>>(content, _options);

            return SOSHubsRetorned;
        }
        public async Task<SOSHub> UpdateSOSHub(SOSHub SosEntity)
        {
            var response = await _http.PutAsJsonAsync($"SOS/DataPool/{SosEntity.SOSHubId}", SosEntity);
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            var SOSHubUpdated = JsonSerializer.Deserialize<SOSHub>(content, _options);

            return SOSHubUpdated;
        }
        public  async Task<SOSHub> DeleteSOSHub(int SosEntity_id)
        {
            var response = await _http.DeleteAsync($"SOS/DataPool/{SosEntity_id}");
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            var SOSHubsRetorned = JsonSerializer.Deserialize<SOSHub>(content, _options);

            return SOSHubsRetorned;
        }
        public async Task<FileUpload> AddImageToSOSHub(MultipartFormDataContent? contentfiles, int SOS_DataPool_id)
        {
            var response = await _http.PostAsync($"SOS/DataPool/Image/{SOS_DataPool_id}", contentfiles);

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
        public async Task<FileUpload> AddVideoToSOSHub(MultipartFormDataContent? contentfiles, int SOS_DataPool_id)
        {
            var response = await _http.PostAsync($"SOS/DataPool/Video/{SOS_DataPool_id}", contentfiles);

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
        public async Task<FileUpload> AddCDToSOSHub(MultipartFormDataContent? contentfile, int SOS_DataPool_id)
        {
            var response = await _http.PostAsync($"SOS/DataPool/CD/{SOS_DataPool_id}", contentfile);

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
        public  async Task<string> ShowImageSosHub(int idfile)
        {
            var response = await _http.GetAsync($"SOS/DataPool/Image/{idfile}");

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
        //Aun no se como hacer esto XD
        public async Task<string> ShowVideoSosHub(int idfile)
        {
            var response = await _http.GetAsync($"SOS/DataPool/Video/{idfile}");

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
        public async Task DownloadFileCD(int idfile, string filename)
        {
            var response = await _http.GetAsync($"SOS/DataPool/CD/{idfile}");

            if (!response.IsSuccessStatusCode)
            {
                await _js.InvokeVoidAsync("alert", "Error File Download");
            }
            else
            {
                var fileStream = response.Content.ReadAsStreamAsync();
                using var streamRef = new DotNetStreamReference(stream: await fileStream);
                await _js.InvokeVoidAsync("downloadFileFromStream", filename, streamRef);
            }

        }
        public async Task<bool> RemoveImageFromSOSData(int SOS_DataPool_id, int ImageFile_id)
        {
            var response = await _http.DeleteAsync($"SOS/DataPool/Image/{SOS_DataPool_id}/remove/{ImageFile_id}");
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                return false;
            }

            return true;
        }
        public  async  Task<bool> RemoveVideoFromSOSData(int SOS_DataPool_id, int VideoFile_id)
        {
            var response = await _http.DeleteAsync($"SOS/DataPool/Video/{SOS_DataPool_id}/remove/{VideoFile_id}");
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                return false;
            }

            return true;
        }
        public async Task<bool> RemoveCDFromSOSData(int SOS_DataPool_id, int CDFile_id)
        {
            var response = await _http.DeleteAsync($"SOS/DataPool/CD/{SOS_DataPool_id}/remove/{CDFile_id}");
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                return false;
            }

            return true;
        }

        public async Task<bool> GenerateAnalysis(int SOS_DataPool_id, SOSAnalysis analysis)
        {
            var response = await _http.PostAsJsonAsync($"SOS/Analysis?SOSHubCollection_Id={SOS_DataPool_id}", analysis);
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                return false;
            }


            return true;
        }

        public Task<bool> GenerateCombination(int SOS_DataPool_id)
        {
            throw new NotImplementedException();
        }

        public Task<bool> GenerateFlow(int SOS_DataPool_id)
        {
            throw new NotImplementedException();
        }

        public Task<bool> GenerateDistribution(int SOS_DataPool_id)
        {
            throw new NotImplementedException();
        }

        public Task<bool> GenerateSequence(int SOS_DataPool_id)
        {
            throw new NotImplementedException();
        }
    }
}
