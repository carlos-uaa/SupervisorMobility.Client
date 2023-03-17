using Microsoft.AspNetCore.Routing;
using Microsoft.JSInterop;
using System.IO;
using System.Net.Http.Json;

namespace SupervisorMobility.Client.Services.BridgeCDMSService
{
    public class BridgeCDMSService : IBridgeCDMSService
    {
        private readonly HttpClient _http;
        private readonly HttpClient _httpBridge;
        private readonly IJSRuntime _js;
        private readonly JsonSerializerOptions _options;

        // Constructor
        public BridgeCDMSService(CustomHttpClientService customHttpClientService, IJSRuntime jSRuntime)
        {
            _http = customHttpClientService.GetApiHttpClient();
            _httpBridge = customHttpClientService.GetBridgeHttpClient();
            _js = jSRuntime;
            _options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        }
        //CCP
        public async Task<CDMS_CCP_Folder> GetFoldersCCP()
        {
            var response = await _httpBridge.GetAsync("SMCcp/GetDirectoryPathsCcp");

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<CDMS_CCP_Folder>();
                return result;
            }
            else
            {
                await _js.InvokeVoidAsync("alert", $"Error get folders: {response.Content.ReadAsStringAsync().Result}");
            }

            return null;
        }
        //HOE
        public async Task<CDMS_HOE_Folder> GetFoldersHOE()
        {
            var response = await _httpBridge.GetAsync("SMHoe/GetDirectoryPaths");

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<CDMS_HOE_Folder>();
                return result;
            }
            else
            {
                await _js.InvokeVoidAsync("alert", $"Error get folders: {response.Content.ReadAsStringAsync().Result}");
            }

            return null;
        }
        //GOS
        public async Task<CDMS_GOS_Folder> GetFoldersGOS()
        {
            var response = await _httpBridge.GetAsync("SMGos/GetDirectoryPathsGos");

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<CDMS_GOS_Folder>();
                return result;
            }
            else
            {
                await _js.InvokeVoidAsync("alert", $"Error get folders: {response.Content.ReadAsStringAsync().Result}");
            }

            return null;
        }
        //get Assy Chart by Id
        public async Task<CDMS_CCP_Document> GetFilesCCP(string ruta)
        {

            var parameters = new Dictionary<string, string>
        {
            { "route", ruta }
        };

            var content = new FormUrlEncodedContent(parameters);

            var response = await _httpBridge.PostAsJsonAsync("/SMCcp/PostArchivesDirectoryCcp", content);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<CDMS_CCP_Document>();
                return result;
            }
            else
            {
                await _js.InvokeVoidAsync("alert", $"Error get files: {response.Content.ReadAsStringAsync().Result}");
            }

            return null;
        }
        public async Task<CDMS_HOE_Document> GetFilesHOE(string route)
        {


            var parameters = new Dictionary<string, string>
        {
            { "route", route }
        };

            var content = new FormUrlEncodedContent(parameters);


            var response = await _httpBridge.PostAsync("/SMHoe/PostArchivesDirectoryHOE", content);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<CDMS_HOE_Document>();
                return result;
            }
            else
            {
                await _js.InvokeVoidAsync("alert", $"Error get files: {response.Content.ReadAsStringAsync().Result}");
            }

            return null;
        }
        public async Task<CDMS_GOS_Document> GetFilesGOS(string ruta)
        {

            var parameters = new Dictionary<string, string>
        {
            { "route", ruta }
        };

            var content = new FormUrlEncodedContent(parameters);
            var response = await _httpBridge.PostAsJsonAsync("/SMGos/PostArchivesDirectoryGos", content);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<CDMS_GOS_Document>();
                return result;
            }
            else
            {
                await _js.InvokeVoidAsync("alert", $"Error get files: {response.Content.ReadAsStringAsync().Result}");
            }

            return null;
        }


    }

}
