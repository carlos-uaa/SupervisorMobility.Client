using Microsoft.AspNetCore.Routing;
using Microsoft.JSInterop;
using SupervisorMobility.Client.Data.Entities.CDMS;
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
        public async Task<CDMS_CCP_Directory> GetFoldersCCP()
        {
           
            try
            {
                var response = await _httpBridge.GetAsync("SMCcp/GetDirectoryPathsCcp");

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<CDMS_CCP_Directory>();
                    return result;
                }
                else
                {
                    await _js.InvokeVoidAsync("alert", $"Error get folders: {response.Content.ReadAsStringAsync().Result}");
                }
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Error al hacer la solicitud: {ex.Message}");
            }
            catch (TaskCanceledException ex)
            {
                Console.WriteLine($"La solicitud ha sido cancelada: {ex.Message}");
            }


            return null;
        }

        public async Task<CDMS_CCP_Archives> GetFilesCCP(string route)
        {
            var parameters = new Dictionary<string, string>
            {
                { "route", route }
            };

            var content = new FormUrlEncodedContent(parameters);


            try
            {
                var response = await _httpBridge.PostAsync("SMCcp/PostArchivesDirectoryCcp", content);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<CDMS_CCP_Archives>();
                    return result;
                }
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Error al hacer la solicitud: {ex.Message}");
            }
            catch (TaskCanceledException ex)
            {
                Console.WriteLine($"La solicitud ha sido cancelada: {ex.Message}");
            }

            return null;

        }

        public async Task<CDMS_DownloadFile> GetDownloadLinkCCP(string URL)
        {
            var parameters = new Dictionary<string, string>
            {
                { "route", URL }
            };

            var content = new FormUrlEncodedContent(parameters);


            try
            {
                var response = await _httpBridge.PostAsync("SMCcp/PostDownloadfileCcp", content);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<CDMS_DownloadFile>();
                    return result;
                }
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Error al hacer la solicitud: {ex.Message}");
            }
            catch (TaskCanceledException ex)
            {
                Console.WriteLine($"La solicitud ha sido cancelada: {ex.Message}");
            }

            return null;
        }
        public async Task<CDMS_General> DeleteFileTempCCP(string FileName)
        {
            var parameters = new Dictionary<string, string>
            {
                { "routeDelete", FileName }
            };

            var content = new FormUrlEncodedContent(parameters);


            try
            {
               
                var uri = new Uri(_httpBridge.BaseAddress, "SMCcp/DeleteFileTempCcp");

                var request = new HttpRequestMessage(HttpMethod.Delete, uri)
                {
                    Content = content
                };

                var response = await _httpBridge.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<CDMS_General>();
                    return result;
                }
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Error al hacer la solicitud: {ex.Message}");
            }
            catch (TaskCanceledException ex)
            {
                Console.WriteLine($"La solicitud ha sido cancelada: {ex.Message}");
            }

            return null;
        }
        //HOE
        public async Task<CDMS_HOE_Directory> GetFoldersHOE()
        {
            try
            {
                var response = await _httpBridge.GetAsync("SMHoe/GetDirectoryPaths");

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<CDMS_HOE_Directory>();
                    return result;
                }
                else
                {
                    await _js.InvokeVoidAsync("alert", $"Error get folders: {response.Content.ReadAsStringAsync().Result}");
                }
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Error al hacer la solicitud: {ex.Message}");
            }
            catch (TaskCanceledException ex)
            {
                Console.WriteLine($"La solicitud ha sido cancelada: {ex.Message}");
            }
            return null;
        }

        public async Task<CDMS_HOE_Archives> GetFilesHOE(string route)
        {


            var parameters = new Dictionary<string, string>
        {
            { "route", route }
        };

            var content = new FormUrlEncodedContent(parameters);


            try
            {
                var response = await _httpBridge.PostAsync("SMHoe/PostArchivesDirectoryHOE", content);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<CDMS_HOE_Archives>();
                    return result;
                }
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Error al hacer la solicitud: {ex.Message}");
            }
            catch (TaskCanceledException ex)
            {
                Console.WriteLine($"La solicitud ha sido cancelada: {ex.Message}");
            }

            return null;
        }

        //public async Task<CDMS_DownloadFile> GetDownloadLinkHOE(string URL)
        //{
        //    var parameters = new Dictionary<string, string>
        //    {
        //        { "route", URL }
        //    };

        //    var content = new FormUrlEncodedContent(parameters);


        //    try
        //    {
        //        var response = await _httpBridge.PostAsync("SMGos/PostDownloadfileGos", content);

        //        if (response.IsSuccessStatusCode)
        //        {
        //            var result = await response.Content.ReadFromJsonAsync<CDMS_DownloadFile>();
        //            return result;
        //        }
        //    }
        //    catch (HttpRequestException ex)
        //    {
        //        Console.WriteLine($"Error al hacer la solicitud: {ex.Message}");
        //    }
        //    catch (TaskCanceledException ex)
        //    {
        //        Console.WriteLine($"La solicitud ha sido cancelada: {ex.Message}");
        //    }

        //    return null;
        //}

        //GOS
        public async Task<CDMS_GOS_Directory> GetFoldersGOS()
        {
            

            try
            {
                var response = await _httpBridge.GetAsync("SMGos/GetDirectoryPathsGos");

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<CDMS_GOS_Directory>();
                    return result;
                }
                else
                {
                    await _js.InvokeVoidAsync("alert", $"Error get folders: {response.Content.ReadAsStringAsync().Result}");
                }
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Error al hacer la solicitud: {ex.Message}");
            }
            catch (TaskCanceledException ex)
            {
                Console.WriteLine($"La solicitud ha sido cancelada: {ex.Message}");
            }

            return null;
        }
       
        
        public async Task<CDMS_GOS_Archives> GetFilesGOS(string route)
        {
            var parameters = new Dictionary<string, string>
            {
                { "route", route }
            };

            var content = new FormUrlEncodedContent(parameters);

            try
            {
                var response = await _httpBridge.PostAsync("SMGos/PostArchivesDirectoryGos", content);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<CDMS_GOS_Archives>();
                    return result;
                }
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Error al hacer la solicitud: {ex.Message}");
            }
            catch (TaskCanceledException ex)
            {
                Console.WriteLine($"La solicitud ha sido cancelada: {ex.Message}");
            }

            return null;
        }

        public async Task<CDMS_DownloadFile> GetDownloadLinkGOS(string URL)
        {
            var parameters = new Dictionary<string, string>
            {
                { "route", URL }
            };

            var content = new FormUrlEncodedContent(parameters);


            try
            {
                var response = await _httpBridge.PostAsync("SMGos/PostDownloadfileGos", content);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<CDMS_DownloadFile>();
                    return result;
                }
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Error al hacer la solicitud: {ex.Message}");
            }
            catch (TaskCanceledException ex)
            {
                Console.WriteLine($"La solicitud ha sido cancelada: {ex.Message}");
            }

            return null;
        }

        public async Task<CDMS_General> DeleteFileTempGOS(string FileName)
        {
            var parameters = new Dictionary<string, string>
            {
                { "routeDelete", FileName }
            };

            var content = new FormUrlEncodedContent(parameters);

            try
            {
                var uri = new Uri(_httpBridge.BaseAddress, "SMGos/DeleteFileTempGos");

                var request = new HttpRequestMessage(HttpMethod.Delete, uri)
                {
                    Content = content
                };

                var response = await _httpBridge.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<CDMS_General>();
                    return result;
                }
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Error al hacer la solicitud: {ex.Message}");
            }
            catch (TaskCanceledException ex)
            {
                Console.WriteLine($"La solicitud ha sido cancelada: {ex.Message}");
            }

            return null;
        }

    }

}
