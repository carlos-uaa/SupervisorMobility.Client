using DocumentFormat.OpenXml.Presentation;
using Microsoft.JSInterop;
using MudBlazor;
using SupervisorMobility.Client.Data.Entities;
using SupervisorMobility.Client.Data.Entities.SOS_Process;
using System.Net.Http.Json;
using System.Runtime.CompilerServices;

namespace SupervisorMobility.Client.Services.SOS_Services.SOSSynopticRequirementsService
{
    public class SynopticRequirementsService : ISynopticRequirementsService
    {
        private readonly HttpClient _http;
        private readonly ISnackbar snackbar;
        private readonly JsonSerializerOptions _options;
        private readonly IJSRuntime _js;

        // Constructor
        public SynopticRequirementsService(HttpClient HttpClientService, IJSRuntime jSRuntime, ISnackbar snackbar)
        {
            _http = HttpClientService;
            _js = jSRuntime;
            _options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            this.snackbar = snackbar;
        }




        public async Task<List<SOSSynopticTableofOperatingRequirements>> GetAllSOSSynopticTableofOperatingRequirements(bool includeLogbooks = false, bool includeSOS = false, bool includeCollections = false)
        {
            var response = await _http.GetAsync($"SOS/SynopticTableofOperatingRequirements/all?includeLogbooks={includeLogbooks}&includeSOS={includeSOS}&includeCollections={includeCollections}");
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new ApplicationException(content);
            }
            var SOSSynopticTableofOperatingRequirementssRetorned = JsonSerializer.Deserialize<List<SOSSynopticTableofOperatingRequirements>>(content, _options);

            return SOSSynopticTableofOperatingRequirementssRetorned;
        }

        public async Task<SOSSynopticTableofOperatingRequirements> GetSOSSynopticTableofOperatingRequirements(int SOSSynopticTableofOperatingRequirementsId, bool includeLogbooks = false, bool includeSOS = false, bool includeCollections = false)
        {
            var response = await _http.GetAsync($"SOS/SynopticTableofOperatingRequirements/{SOSSynopticTableofOperatingRequirementsId}?includeLogbooks={includeLogbooks}&includeSOS={includeSOS}&includeCollections={includeCollections}");
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new ApplicationException(content);
            }
            var SOSSynopticTableofOperatingRequirementssRetorned = JsonSerializer.Deserialize<SOSSynopticTableofOperatingRequirements>(content, _options);

            return SOSSynopticTableofOperatingRequirementssRetorned;
        }
        public async Task<SOSSynopticTableofOperatingRequirements> UpdateSOSSynopticTableofOperatingRequirements(SOSSynopticTableofOperatingRequirementsForUpdateDto SosEntity)
        {
            var response = await _http.PutAsJsonAsync($"SOS/SynopticTableofOperatingRequirements/{SosEntity.SOSSynopticTableofOperatingRequirementsId}", SosEntity);
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            var SOSSynopticTableofOperatingRequirementsUpdated = JsonSerializer.Deserialize<SOSSynopticTableofOperatingRequirements>(content, _options);

            return SOSSynopticTableofOperatingRequirementsUpdated;
        }


        public async Task<bool> DeleteSOSSynopticTableofOperatingRequirements(int SosEntity_id)
        {
            var response = await _http.DeleteAsync($"SOS/SynopticTableofOperatingRequirements/{SosEntity_id}");
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                return false;
            }

            return true;
        }



        public async Task<FileUpload> AddIllustrationToSOSSynopticTableofOperatingRequirements(MultipartFormDataContent? contentfiles, int SOS_SOSSynopticTableofOperatingRequirements_id)
        {
            var response = await _http.PostAsync($"SOS/SynopticTableofOperatingRequirements/Ilustrations/{SOS_SOSSynopticTableofOperatingRequirements_id}", contentfiles);

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


        public async Task<string> ShowIlustrationSOSSynopticTableofOperatingRequirements(int idfile)
        {
            var response = await _http.GetAsync($"SOS/SynopticTableofOperatingRequirements/Ilustrations/{idfile}");

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
        public async Task<bool> RemoveIlustrationFromSOSData(int SOS_SOSSynopticTableofOperatingRequirements_id, int ImageFile_id)
        {
            var response = await _http.DeleteAsync($"SOS/SynopticTableofOperatingRequirements/Ilustrations/{SOS_SOSSynopticTableofOperatingRequirements_id}/remove/{ImageFile_id}");
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                return false;
            }

            return true;
        }

        public async Task GenerateExcelSTOperatingRequirements(int Id)
        {
            var response = await _http.GetAsync($"SOS/SynopticTableofOperatingRequirements/GenerateExcelSTOperatingRequirements/{Id}");

            if (!response.IsSuccessStatusCode)
            {
                snackbar.Add("Error while exporting, could not download file", Severity.Error);
            }
            else
            {
                var filename = response.Content.Headers.ContentDisposition.FileName.Replace("\"", string.Empty);
                var fileStream = response.Content.ReadAsStreamAsync();
                using var streamRef = new DotNetStreamReference(stream: await fileStream);
                await _js.InvokeVoidAsync("downloadFileFromStream", filename, streamRef);
            }
        }



    }
}
