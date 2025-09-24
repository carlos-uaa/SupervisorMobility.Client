using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.Presentation;
using Microsoft.JSInterop;
using SupervisorMobility.Client.Data.Entities;
using SupervisorMobility.Client.Data.Entities.SOS_Process;
using System.Buffers;
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
        public async Task<SOSHub> GetSOSHub(int HubId, bool includeAnalysesBkup = false, bool includeSections = false, bool includeImages = false, bool includeVideos = false, bool includeCommentaries = false, bool includeTools = false, bool includeEquipments = false, bool includeMaterials = false, bool includeInformation = false, bool includePeople = false, bool includeDocuments = false, bool includeModel = false, bool includeCollections = false, bool includePeopleCollections = false, bool includePats = false)
        {
            var response = await _http.GetAsync($"SOS/DataPool/{HubId}?includeAnalysesBkup={includeAnalysesBkup}&includeSections={includeSections}&includeImages={includeImages}&includeVideos={includeVideos}&includeCommentaries={includeCommentaries}&includeTools={includeTools}&includeEquipments={includeEquipments}&includeMaterials={includeMaterials}&includeInformation={includeInformation}&includePeople={includePeople}&includeDocuments={includeDocuments}&includeModel={includeModel}&includeCollections={includeCollections}&includePeopleCollections={includePeopleCollections}&includePats={includePats}");
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new ApplicationException(content);
            }

            var SOSHubReturned = JsonSerializer.Deserialize<SOSHub>(content, _options);

            return SOSHubReturned;
        }
        public async Task<List<SOSHub>> GetAllSOSHub(bool includeAnalysesBkup = false, bool includeSections = false, bool includeImages = false, bool includeVideos = false, bool includeCommentaries = false, bool includeTools = false, bool includeEquipments = false, bool includeMaterials = false, bool includeInformation = false, bool includePeople = false, bool includeDocuments = false, bool includeSOSDistribution = false)
        {
            var response = await _http.GetAsync($"SOS/DataPool?includeAnalysesBkup={includeAnalysesBkup}&includeSections={includeSections}&includeImages={includeImages}&includeVideos={includeVideos}&includeCommentaries={includeCommentaries}&includeTools={includeTools}&includeEquipments={includeEquipments}&includeMaterials={includeMaterials}&includeInformation={includeInformation}&includePeople={includePeople}&includeDocuments={includeDocuments}&includeSOSDistribution={includeSOSDistribution}");
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
        public async Task<SOSHub> DeleteSOSHub(int SosEntity_id)
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
        public async Task<string> ShowImageSosHub(int idfile)
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

            if (!response.IsSuccessStatusCode)
            {
                return false;
            }

            return true;
        }
        public async Task<bool> RemoveVideoFromSOSData(int SOS_DataPool_id, int VideoFile_id)
        {
            var response = await _http.DeleteAsync($"SOS/DataPool/Video/{SOS_DataPool_id}/remove/{VideoFile_id}");

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


        public async Task<int> GeneratePat(int SOS_DataPool_id, PAT pat)
        {
            var response = await _http.PostAsJsonAsync($"PAT/sosHub?SOSHubCollection_Id={SOS_DataPool_id}", pat);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();

                var patCreated = JsonSerializer.Deserialize<PAT>(content, _options);

                return patCreated.PATid;
            }

            return 0;
        }


        public async Task<int> GenerateAnalysis(int SOS_DataPool_id, SOSAnalysis analysis)
        {
            var response = await _http.PostAsJsonAsync($"SOS/Analysis?SOSHubCollection_Id={SOS_DataPool_id}", analysis);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();

                var analysisCreated = JsonSerializer.Deserialize<SOSAnalysis>(content, _options);

                return analysisCreated.SOSAnalysisId;
            }

            return 0;
        }

        public async Task<int> GenerateCombination(int SOS_DataPool_id, SOSCombination combination)
        {
            var response = await _http.PostAsJsonAsync($"SOS/Combination?SOSHubCollection_Id={SOS_DataPool_id}", combination);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();

                var CombinationCreated = JsonSerializer.Deserialize<SOSCombination>(content, _options);

                return CombinationCreated.SOSCombinationId;
            }

            return 0;
        }

        public async Task<int> GenerateFlow(int SOS_DataPool_id, SOSFlow flow)
        {
            var response = await _http.PostAsJsonAsync($"SOS/Flow?SOSHubCollection_Id={SOS_DataPool_id}", flow);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();

                var FlowCreated = JsonSerializer.Deserialize<SOSFlow>(content, _options);

                return FlowCreated.SOSFlowId;
            }

            return 0;
        }

        public async Task<int> GenerateDistribution(int SOS_DataPool_id, SOSDistribution distribution)
        {
            var response = await _http.PostAsJsonAsync($"SOS/Distribution?SOSHubCollection_Id={SOS_DataPool_id}", distribution);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();

                var DistributionCreated = JsonSerializer.Deserialize<SOSDistribution>(content, _options);

                return DistributionCreated.SOSDistributionId;
            }

            return 0;
        }

        public async Task<int> GenerateSequence(int SOS_DataPool_id, SOSSequence sequence)
        {
            var response = await _http.PostAsJsonAsync($"SOS/Sequence?SOSHubCollection_Id={SOS_DataPool_id}", sequence);
            var content = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                SOSSequence? sequenceCreated = JsonSerializer.Deserialize<SOSSequence>(content, _options);

                Console.WriteLine("sequence cre: " + sequence.SOSSequenceId);
                return sequenceCreated.SOSSequenceId;
            }

            return 0;
        }

        public async Task<List<SOSHub>> GetAllHistorySOSHub(int HubId, bool includeAnalysesBkup = false, bool includeSections = false, bool includeImages = false, bool includeVideos = false, bool includeCommentaries = false, bool includeTools = false, bool includeEquipments = false, bool includeMaterials = false, bool includeInformation = false, bool includePeople = false, bool includeDocuments = false)
        {
            var response = await _http.GetAsync($"SOS/DataPool/{HubId}/History?includeAnalysesBkup={includeAnalysesBkup}&includeSections={includeSections}&includeImages={includeImages}&includeVideos={includeVideos}&includeCommentaries={includeCommentaries}&includeTools={includeTools}&includeEquipments={includeEquipments}&includeMaterials={includeMaterials}&includeInformation={includeInformation}&includePeople={includePeople}&includeDocuments={includeDocuments}");
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new ApplicationException(content);
            }

            var SOSHubReturned = JsonSerializer.Deserialize<List<SOSHub>>(content, _options);

            return SOSHubReturned;
        }

        public async Task<int> GenerateSynopticRequirements(int SOS_DataPool_id, SOSSynopticTableofOperatingRequirements SynopticRequirements)
        {
            var response = await _http.PostAsJsonAsync<SOSSynopticTableofOperatingRequirements>($"SOS/SynopticTableofOperatingRequirements?SOSHubCollection_Id={SOS_DataPool_id}", SynopticRequirements);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();

                var SynopticRequirementsCreated = JsonSerializer.Deserialize<SOSSynopticTableofOperatingRequirements>(content, _options);

                return SynopticRequirementsCreated.SOSSynopticTableofOperatingRequirementsId;
            }

            return 0;
        }

        public  async Task<int> GenerateSynopticControlPoints(int SOS_DataPool_id, SOSSynopticTableofControlPoints SynopticControlPoints)
        {
            var response = await _http.PostAsJsonAsync<SOSSynopticTableofControlPoints>($"SOS/SynopticTableofControlPoints?SOSHubCollection_Id={SOS_DataPool_id}", SynopticControlPoints);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();

                var SynoptiControlPoints = JsonSerializer.Deserialize<SOSSynopticTableofControlPoints>(content, _options);

                return SynoptiControlPoints.SOSSynopticTableofControlPointsId;
            }

            return 0;
        }
    }
}
