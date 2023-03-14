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
        public async Task<FoldersCDMS> GetFoldersCCP()
        {
            var response = await _httpBridge.GetAsync("SMCcp/GetDirectoryPathsCcp");

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<FoldersCDMS>();
                return result;
            }
            else
            {
                await _js.InvokeVoidAsync("alert", $"Error get folders: {response.Content.ReadAsStringAsync().Result}");
            }

            return null;
        }
        //HOE
        public async Task<FoldersCDMS> GetFoldersHOE()
        {
            var response = await _httpBridge.GetAsync("SMHoe/GetDirectoryPaths");

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<FoldersCDMS>();
                return result;
            }
            else
            {
                await _js.InvokeVoidAsync("alert", $"Error get folders: {response.Content.ReadAsStringAsync().Result}");
            }

            return null;
        }
        //GOS
        public async Task<FoldersCDMS> GetFoldersGOS()
        {
            var response = await _httpBridge.GetAsync("SMGos/GetDirectoryPathsGos");

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<FoldersCDMS>();
                return result;
            }
            else
            {
                await _js.InvokeVoidAsync("alert", $"Error get folders: {response.Content.ReadAsStringAsync().Result}");
            }

            return null;
        }
        //get Assy Chart by Id
       public async Task<FilesCDMS> GetFilesCCP(string ruta)
        {
            var response = await _httpBridge.PostAsJsonAsync("/SMCcp/PostArchivesDirectoryCcp", ruta);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<FilesCDMS>();
                return result;
            }
            else
            {
                await _js.InvokeVoidAsync("alert", $"Error get files: {response.Content.ReadAsStringAsync().Result}");
            }

            return null;
        }
        public async Task<FilesCDMS> GetFilesHOE(string ruta)
        {
            var response = await _httpBridge.PostAsJsonAsync("/SMHoe/PostArchivesDirectoryHOE", ruta);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<FilesCDMS>();
                return result;
            }
            else
            {
                await _js.InvokeVoidAsync("alert", $"Error get files: {response.Content.ReadAsStringAsync().Result}");
            }

            return null;
        }
        public async Task<FilesCDMS> GetFilesGOS(string ruta)
        {
            var response = await _httpBridge.PostAsJsonAsync("/SMGos/PostArchivesDirectoryGos", ruta);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<FilesCDMS>();
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
