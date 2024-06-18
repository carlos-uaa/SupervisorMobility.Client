using Microsoft.JSInterop;
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
        public async Task<SOSHub> GetSOSHub(int HubId, bool includeImages = false, bool includeVideos = false, bool includeTools = false, bool includeEquipments = false, bool includeMaterials = false, bool includeInformation = false, bool includePeople = false, bool includeDocuments = false)
        {
            var response = await _http.GetAsync($"SOS/DataPool/{HubId}?includeImages={includeImages}&includeVideos={includeVideos}&includeTools={includeTools}&includeEquipments={includeEquipments}&includeMaterials={includeMaterials}&includeInformation={includeInformation}&includePeople={includePeople}&includeDocuments={includeDocuments}");
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new ApplicationException(content);
            }

            var SOSHubReturned = JsonSerializer.Deserialize<SOSHub>(content, _options);

            return SOSHubReturned;
        }
        public async Task<IEnumerable<SOSHub>> GetAllSOSHub(bool includeImages = false, bool includeVideos = false, bool includeTools = false, bool includeEquipments = false, bool includeMaterials = false, bool includeInformation = false, bool includePeople = false, bool includeDocuments = false)
        {
            var response = await _http.GetAsync($"SOS/DataPool?includeImages={includeImages}&includeVideos={includeVideos}&includeTools={includeTools}&includeEquipments={includeEquipments}&includeMaterials={includeMaterials}&includeInformation={includeInformation}&includePeople={includePeople}&includeDocuments={includeDocuments}");
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
        public  async Task<SOSHub> DeleteSOSHub(SOSHub SosEntity)
        {
            var response = await _http.DeleteAsync($"SOS/DataPool/{SosEntity.SOSHubId}");
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            var SOSHubsRetorned = JsonSerializer.Deserialize<SOSHub>(content, _options);

            return SOSHubsRetorned;
        }
        public Task<FileUpload> AddImageToSOSHub(MultipartFormDataContent? contentfiles, int SOS_DataPool_id)
        {
            throw new NotImplementedException();
        }
        public Task<FileUpload> AddVideoToSOSHub(MultipartFormDataContent? contentfiles, int SOS_DataPool_id)
        {
            throw new NotImplementedException();
        }
        public Task<FileUpload> AddCDToSOSHub(MultipartFormDataContent? contentfiles, int SOS_DataPool_id)
        {
            throw new NotImplementedException();
        }
        public Task<string> ShowImageSosHub(int idfile)
        {
            throw new NotImplementedException();
        }
        //Aun no se como hacer esto XD
        //Task<> ShowVideoSosHub(int idfile);
        public Task DownloadFileCD(int idfile, string filename)
        {
            throw new NotImplementedException();
        }
        public Task<bool> RemoveImageFromSOSData(int SOS_DataPool_id, int ImageFile_id)
        {
            throw new NotImplementedException();
        }
        public Task<bool> RemoveVideoFromSOSData(int SOS_DataPool_id, int VideoFile_id)
        {
            throw new NotImplementedException();
        }
        public Task<bool> RemoveCDFromSOSData(int SOS_DataPool_id, int CDFile_id)
        {
            throw new NotImplementedException();
        }

    }
}
