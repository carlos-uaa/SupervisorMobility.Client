using DocumentFormat.OpenXml.Presentation;
using Microsoft.JSInterop;
using SupervisorMobility.Client.Data.Entities;
using SupervisorMobility.Client.Data.Entities.SOSAnalysis_Process;
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


        //public  async Task<SOSAnalysis> DeleteSOSAnalysis(int SosEntity_id)
        //{
        //    var response = await _http.DeleteAsync($"SOS/Analysis/{SosEntity_id}");
        //    var content = await response.Content.ReadAsStringAsync();

        //    if (!response.IsSuccessStatusCode)
        //    {
        //        return null;
        //    }

        //    var SOSAnalysissRetorned = JsonSerializer.Deserialize<SOSAnalysis>(content, _options);

        //    return SOSAnalysissRetorned;
        //}
        //public async Task<FileUpload> AddImageToSOSAnalysis(MultipartFormDataContent? contentfiles, int SOS_Analysis_id)
        //{
        //    var response = await _http.PostAsync($"SOS/Analysis/Image/{SOS_Analysis_id}", contentfiles);

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
        //public async Task<FileUpload> AddVideoToSOSAnalysis(MultipartFormDataContent? contentfiles, int SOS_Analysis_id)
        //{
        //    var response = await _http.PostAsync($"SOS/Analysis/Video/{SOS_Analysis_id}", contentfiles);

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
        //public async Task<FileUpload> AddCDToSOSAnalysis(MultipartFormDataContent? contentfile, int SOS_Analysis_id)
        //{
        //    var response = await _http.PostAsync($"SOS/Analysis/CD/{SOS_Analysis_id}", contentfile);

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
        //public  async Task<string> ShowImageSOSAnalysis(int idfile)
        //{
        //    var response = await _http.GetAsync($"SOS/Analysis/Image/{idfile}");

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
        ////Aun no se como hacer esto XD
        ////Task<> ShowVideoSOSAnalysis(int idfile);
        //public async Task DownloadFileCD(int idfile, string filename)
        //{
        //    var response = await _http.GetAsync($"SOS/Analysis/CD/{idfile}");

        //    if (!response.IsSuccessStatusCode)
        //    {
        //        await _js.InvokeVoidAsync("alert", "Error File Download");
        //    }
        //    else
        //    {
        //        var fileStream = response.Content.ReadAsStreamAsync();
        //        using var streamRef = new DotNetStreamReference(stream: await fileStream);
        //        await _js.InvokeVoidAsync("downloadFileFromStream", filename, streamRef);
        //    }

        //}
        //public async Task<bool> RemoveImageFromSOSData(int SOS_Analysis_id, int ImageFile_id)
        //{
        //    var response = await _http.DeleteAsync($"SOS/Analysis/Image/{SOS_Analysis_id}/remove/{ImageFile_id}");
        //    var content = await response.Content.ReadAsStringAsync();

        //    if (!response.IsSuccessStatusCode)
        //    {
        //        return false;
        //    }

        //    return true;
        //}
        //public  async  Task<bool> RemoveVideoFromSOSData(int SOS_Analysis_id, int VideoFile_id)
        //{
        //    var response = await _http.DeleteAsync($"SOS/Analysis/Video/{SOS_Analysis_id}/remove/{VideoFile_id}");
        //    var content = await response.Content.ReadAsStringAsync();

        //    if (!response.IsSuccessStatusCode)
        //    {
        //        return false;
        //    }

        //    return true;
        //}
        //public async Task<bool> RemoveCDFromSOSData(int SOS_Analysis_id, int CDFile_id)
        //{
        //    var response = await _http.DeleteAsync($"SOS/Analysis/CD/{SOS_Analysis_id}/remove/{CDFile_id}");
        //    var content = await response.Content.ReadAsStringAsync();

        //    if (!response.IsSuccessStatusCode)
        //    {
        //        return false;
        //    }

        //    return true;
        //}

    }
}
