using Microsoft.JSInterop;
using System;
using System.Net.Http.Json;

namespace SupervisorMobility.Client.Services.FileUploadAndDownloadService
{
    public class FileUploadAndDownloadService : IFileUploadAndDownloadService
    {
        private readonly HttpClient _http;
        private readonly JsonSerializerOptions _options;
        private readonly IJSRuntime _js;

        // Constructor
        public FileUploadAndDownloadService(HttpClient customHttpClientService, IJSRuntime jSRuntime)
        {
            _http = customHttpClientService;
            _js = jSRuntime;
            _options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        }

        //Plantstructure Upload

        public async Task<FileUpload> UploadPlantStructure(MultipartFormDataContent contentfile, int plantnameid, int userId)
        {
            var response = await _http.PostAsync($"File/MassiveUploadTreeData?plantnameid={plantnameid}&userId={userId}", contentfile);

            if (response.IsSuccessStatusCode)
            {
                //await _js.InvokeVoidAsync("alert", $"Upload Data Succesful");
                return new FileUpload();
            }

            return null;

                 }
        public async Task PlantStructureFormat()
        {
            var response = await _http.GetAsync($"File/MassiveUploadTreeDataExample");

            if (!response.IsSuccessStatusCode)
            {
                await _js.InvokeVoidAsync("alert", "Error File Download");
            }
            else
            {
                var fileStream = response.Content.ReadAsStreamAsync();
                using var streamRef = new DotNetStreamReference(stream: await fileStream);
                await _js.InvokeVoidAsync("downloadFileFromStream", "TreeDataExample.xlsx", streamRef);
            }
        }
        //Paths Upload
        public async Task<FileUpload> UploadPathStructure(MultipartFormDataContent contentfile, int userId)
        {
            var response = await _http.PostAsync($"File/MassivePaths?userId={userId}", contentfile);

            if (response.IsSuccessStatusCode)
            {
                //await _js.InvokeVoidAsync("alert", $"Upload Data Succesful");
                return new FileUpload();
            }

            return null;
        }
        public async Task PathStructureFormat()
        {
            var response = await _http.GetAsync($"File/MassivePathsExample");

            if (!response.IsSuccessStatusCode)
            {
                await _js.InvokeVoidAsync("alert", "Error File Download");
            }
            else
            {
                var fileStream = response.Content.ReadAsStreamAsync();
                using var streamRef = new DotNetStreamReference(stream: await fileStream);
                await _js.InvokeVoidAsync("downloadFileFromStream", "MassivePathsExample.xlsx", streamRef);
            }
        }

        //Assy CHART Upload
        public async Task<FileUpload> UploadFile(MultipartFormDataContent contentfile)
        {
            var response = await _http.PostAsync($"File", contentfile);

            var result = await response.Content.ReadFromJsonAsync<FileUpload>();

            return result;
        }

        //Upload guide file
        public async Task<FileUpload> UploadGuide(MultipartFormDataContent contentfile)
        {
            Console.WriteLine("upload Guide");

            var response = await _http.PostAsync($"File/UploadGuide", contentfile);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<FileUpload>();
                return result;
            }

            await _js.InvokeVoidAsync("alert", $"Error : {response.Content.ReadAsStringAsync().Result}");
            return null;


        }
        //UploadUsers
        public async Task<FileUpload> UploadUsers(MultipartFormDataContent contentfile)
        {
            var response = await _http.PostAsync($"File/UploadUsers", contentfile);

            var result = await response.Content.ReadFromJsonAsync<FileUpload>();

            return result;
        }

        

        //UploadEvidences
        public async Task<FileUpload> UploadEvidences(MultipartFormDataContent? contentfiles, int lupId)
        {
            var response = await _http.PostAsync($"File/UploadEvidences?lupId={lupId}", contentfiles);

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

        //Upload Operator Signature
        public async Task<FileUpload> UploadOperatorSignature(MultipartFormDataContent? contentfiles, int jobObservationId)
        {
            var response = await _http.PostAsync($"File/UploadOperatorSignature?jobObservationId={jobObservationId}", contentfiles);

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

        public async Task<UploadAssyChartResult> ProccedToUpdateData(FileUpload fileinfo)
        {
            var response = await _http.PostAsJsonAsync($"File/Data", fileinfo);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<UploadAssyChartResult>();
                return result;
            }
            else
            {
                await _js.InvokeVoidAsync("alert", $"Error Upload Data error: {response.Content.ReadAsStringAsync().Result}");
            }

            return null;
        }

        public async Task DownloadFileFromOnePlant(int idPlant)
        {
            var response = await _http.GetAsync($"File/Bulk/ByPlantId/{idPlant}");

            if (!response.IsSuccessStatusCode)
            {
                await _js.InvokeVoidAsync("alert", "Error File Download");
            }
            else
            {
                var fileStream = response.Content.ReadAsStreamAsync();
                using var streamRef = new DotNetStreamReference(stream: await fileStream);
                await _js.InvokeVoidAsync("downloadFileFromStream", "Report One Plant.xlsx", streamRef);
            }

        }
        public async Task DownloadFileFromAllPlants()
        {
            var response = await _http.GetAsync($"File/Bulk/ByPlantId");

            if (!response.IsSuccessStatusCode)
            {
                await _js.InvokeVoidAsync("alert", "Error File Download");
            }
            else
            {
                var fileStream = response.Content.ReadAsStreamAsync();
                using var streamRef = new DotNetStreamReference(stream: await fileStream);
                await _js.InvokeVoidAsync("downloadFileFromStream", "Report ALL Plants.xlsx", streamRef);
            }
        }
        public async Task DownloadFileUsers()
        {
            var response = await _http.GetAsync($"File/Bulk/DownloadUsers");

            if (!response.IsSuccessStatusCode)
            {
                await _js.InvokeVoidAsync("alert", "Error File Download");
            }
            else
            {
                var fileStream = response.Content.ReadAsStreamAsync();
                using var streamRef = new DotNetStreamReference(stream: await fileStream);
                await _js.InvokeVoidAsync("downloadFileFromStream", "BulkUsers.xlsx", streamRef);
            }
        }


        public async Task DownloadFileGuide(int idfile, string filename)
        {
            var response = await _http.GetAsync($"File/Guide/{idfile}");

            if (!response.IsSuccessStatusCode)
            {
                await _js.InvokeVoidAsync("alert", "Error File Download");
            }
            else
            {
                var fileStream = response.Content.ReadAsStreamAsync();
                using var streamRef = new DotNetStreamReference(stream: await fileStream);
                await _js.InvokeVoidAsync("downloadFileFromStream", $"{filename}", streamRef);
            }

        }

        public async Task DownloadFileEvidence(int idfile, string filename)
        {
            var response = await _http.GetAsync($"File/Evidence/{idfile}");

            if (!response.IsSuccessStatusCode)
            {
                await _js.InvokeVoidAsync("alert", "Error File Download");
            }
            else
            {
                var fileStream = response.Content.ReadAsStreamAsync();
                using var streamRef = new DotNetStreamReference(stream: await fileStream);
                await _js.InvokeVoidAsync("downloadFileFromStream", $"{filename}", streamRef);
            }

        }


        public async Task<string> ShowImageEvidence(int idfile)
        {
            var response = await _http.GetAsync($"File/Evidence/{idfile}");

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

        public async Task<string> ShowOperatorSignature(int idfile)
        {
            var response = await _http.GetAsync($"File/Signatures/{idfile}");

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



        public async Task DownloadAllUsersFormat()
        {
            var response = await _http.GetAsync($"File/Users/DownloadAllExample");

            if (!response.IsSuccessStatusCode)
            {
                await _js.InvokeVoidAsync("alert", "Error File Download");
            }
            else
            {
                var fileStream = response.Content.ReadAsStreamAsync();
                using var streamRef = new DotNetStreamReference(stream: await fileStream);
                await _js.InvokeVoidAsync("downloadFileFromStream", "AllUsersFormatExample.xlsx", streamRef);
            }
        }

        public async Task DownloadSSVFormat()
        {
            var response = await _http.GetAsync($"File/Users/DownloadSSVExample");

            if (!response.IsSuccessStatusCode)
            {
                await _js.InvokeVoidAsync("alert", "Error File Download");
            }
            else
            {
                var fileStream = response.Content.ReadAsStreamAsync();
                using var streamRef = new DotNetStreamReference(stream: await fileStream);
                await _js.InvokeVoidAsync("downloadFileFromStream", "SSVUsersFormatExample.xlsx", streamRef);
            }
        }

        public async Task DownloadSupervisorsFormat()
        {
            var response = await _http.GetAsync($"File/Users/DownloadSupervisorExample");

            if (!response.IsSuccessStatusCode)
            {
                await _js.InvokeVoidAsync("alert", "Error File Download");
            }
            else
            {
                var fileStream = response.Content.ReadAsStreamAsync();
                using var streamRef = new DotNetStreamReference(stream: await fileStream);
                await _js.InvokeVoidAsync("downloadFileFromStream", "SVUsersFormatExample.xlsx", streamRef);
            }
        }

        public async Task DownloadOperatorsFormat()
        {
            var response = await _http.GetAsync($"File/Users/DownloadOperatorsExample");

            if (!response.IsSuccessStatusCode)
            {
                await _js.InvokeVoidAsync("alert", "Error File Download");
            }
            else
            {
                var fileStream = response.Content.ReadAsStreamAsync();
                using var streamRef = new DotNetStreamReference(stream: await fileStream);
                await _js.InvokeVoidAsync("downloadFileFromStream", "OperatorsUsersFormatExample.xlsx", streamRef);
            }
        }

    }


}