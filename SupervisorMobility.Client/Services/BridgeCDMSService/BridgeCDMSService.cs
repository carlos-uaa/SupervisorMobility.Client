using Microsoft.AspNetCore.Routing;
using Microsoft.JSInterop;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SupervisorMobility.Client.Data.Entities.CDMS;
using System;
using System.IO;
using System.Net.Http.Json;
using System.Text;

namespace SupervisorMobility.Client.Services.BridgeCDMSService
{
    public class BridgeCDMSService : IBridgeCDMSService
    {
        private readonly HttpClient _http;
        private readonly IJSRuntime _js;
        private readonly JsonSerializerOptions _options;

        // Constructor
        public BridgeCDMSService(HttpClient customHttpClientService, IJSRuntime jSRuntime)
        {
            _http = customHttpClientService;
            _js = jSRuntime;
            _options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        }

       
        //CCP
        public async Task<CDMS_CCP_Directory> GetFoldersCCP()
        {
           
            try
            {
                var response = await _http.GetAsync("BridgeCDMS/SMCcp/GetDirectoryPathsCcp");

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<CDMS_CCP_Directory>();
                    return result;
                }
                else
                {
                    //await _js.InvokeVoidAsync("alert", $"Error get folders: {response.Content.ReadAsStringAsync().Result}");
                    Console.WriteLine($"GET FOLDERS CCP, Status Code {response.StatusCode} : {response.Content.ReadAsStringAsync().Result}");
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

            var json = JsonConvert.SerializeObject(parameters);

            var content = new StringContent(json, Encoding.UTF8, "application/json");

            try
            {
                var response = await _http.PostAsync("BridgeCDMS/SMCcp/PostArchivesDirectoryCcp", content);

                if (response.IsSuccessStatusCode)
                {

                    var result = await response.Content.ReadFromJsonAsync<CDMS_CCP_Archives>();
                    return result;
                }
                else
                {
                    //await _js.InvokeVoidAsync("alert", $"Error get folders: {response.Content.ReadAsStringAsync().Result}");
                    Console.WriteLine($"GET FILES CCP, Status Code {response.StatusCode} : {response.Content.ReadAsStringAsync().Result}");
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
            catch (Exception ex)
            {
                Console.WriteLine($"Error of Json GOS: {ex.Message}");

                var response = await _http.PostAsync("BridgeCDMS/SMGos/PostArchivesDirectoryGos", content);
                var contentString = await response.Content.ReadAsStringAsync();

                // Deserializar el contenido en un objeto JObject
                var responseObject = JObject.Parse(contentString);

                // Acceder a la propiedad "operation" del objeto JObject
                var operation = (string)responseObject["operation"];

                if (operation == "NO FILES IN DIRECTORY")
                {
                    Console.WriteLine($"No Files or Directories");
                    CDMS_CCP_Archives toreturn = new CDMS_CCP_Archives();
                    toreturn.success = false;
                    toreturn.message = operation;
                    return toreturn;
                }
                else if (operation == "NO FILES OR DIRECTORIES")
                {
                    Console.WriteLine($"No Files or Directories");
                    CDMS_CCP_Archives toreturn = new CDMS_CCP_Archives();
                    toreturn.success = false;
                    toreturn.message = operation;
                    return toreturn;
                }
            }

            return null;

        }

        public async Task<CDMS_DownloadFile> GetDownloadLinkCCP(string URL)
        {
            var parameters = new Dictionary<string, string>
            {
                { "route", URL }
            };

            var json = JsonConvert.SerializeObject(parameters);

            var content = new StringContent(json, Encoding.UTF8, "application/json");
            try
            {
                var response = await _http.PostAsync("BridgeCDMS/SMCcp/PostDownloadfileCcp", content);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<CDMS_DownloadFile>();
                    return result;
                }
                else
                {
                    //await _js.InvokeVoidAsync("alert", $"Error get folders: {response.Content.ReadAsStringAsync().Result}");
                    Console.WriteLine($"GET LINK CCP, Status Code {response.StatusCode} : {response.Content.ReadAsStringAsync().Result}");
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

            var json = JsonConvert.SerializeObject(parameters);

            var content = new StringContent(json, Encoding.UTF8, "application/json");

            try
            {
               
                var uri = new Uri(_http.BaseAddress, "BridgeCDMS/SMCcp/DeleteFileTempCcp");

                var request = new HttpRequestMessage(HttpMethod.Delete, uri)
                {
                    Content = content
                };

                var response = await _http.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<CDMS_General>();
                    return result;
                }
                else
                {
                    //await _js.InvokeVoidAsync("alert", $"Error get folders: {response.Content.ReadAsStringAsync().Result}");
                    Console.WriteLine($"DELETE TEMP CCP FILE, Status Code {response.StatusCode} : {response.Content.ReadAsStringAsync().Result}");
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
                var response = await _http.GetAsync("BridgeCDMS/SMHoe/GetDirectoryPaths");

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<CDMS_HOE_Directory>();
                    return result;
                }
                else
                {
                    //await _js.InvokeVoidAsync("alert", $"Error get folders: {response.Content.ReadAsStringAsync().Result}");
                    Console.WriteLine($"GET FOLDERS HOE, Status Code {response.StatusCode} : {response.Content.ReadAsStringAsync().Result}");
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

          
            var json = JsonConvert.SerializeObject(parameters);

            var content = new StringContent(json, Encoding.UTF8, "application/json");

            try
            {
                var response = await _http.PostAsync("BridgeCDMS/SMHoe/PostArchivesDirectoryHOE", content);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<CDMS_HOE_Archives>();
                    return result;
                }
                else
                {
                    //await _js.InvokeVoidAsync("alert", $"Error get folders: {response.Content.ReadAsStringAsync().Result}");
                    Console.WriteLine($"GET FILES HOE, Status Code {response.StatusCode} : {response.Content.ReadAsStringAsync().Result}");
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
            catch (Exception ex)
            {
                Console.WriteLine($"Error of Json HOE: {ex.Message}");

                var response = await _http.PostAsync("BridgeCDMS/SMHoe/PostArchivesDirectoryHOE", content);
                var contentString = await response.Content.ReadAsStringAsync();

                // Deserializar el contenido en un objeto JObject
                var responseObject = JObject.Parse(contentString);

                // Acceder a la propiedad "operation" del objeto JObject
                var operation = (string)responseObject["operation"];
                Console.WriteLine($"REAL ERRO [{operation}]");

                if (operation == "NO FILES IN DIRECTORY")
                {
                    Console.WriteLine($"No Files in Directories");
                    CDMS_HOE_Archives toreturn = new CDMS_HOE_Archives();
                    toreturn.success = false;
                    toreturn.message = operation;
                    return toreturn;
                }
                else if (operation == "NO FILES OR DIRECTORIES")
                {
                    Console.WriteLine($"No Files or Directories");
                    CDMS_HOE_Archives toreturn = new CDMS_HOE_Archives();
                    toreturn.success = false;
                    toreturn.message = operation;
                    return toreturn;
                }
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
        //        var response = await _http.PostAsync("SMGos/PostDownloadfileGos", content);

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
                var response = await _http.GetAsync("BridgeCDMS/SMGos/GetDirectoryPathsGos");

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<CDMS_GOS_Directory>();
                    return result;
                }
                else
                {
                    //await _js.InvokeVoidAsync("alert", $"Error get folders: {response.Content.ReadAsStringAsync().Result}");
                    Console.WriteLine($"GET FOLDERS GOS, Status Code {response.StatusCode} : {response.Content.ReadAsStringAsync().Result}");
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

            var json = JsonConvert.SerializeObject(parameters);

            var content = new StringContent(json, Encoding.UTF8, "application/json");

            try
            {
                var response = await _http.PostAsync("BridgeCDMS/SMGos/PostArchivesDirectoryGos", content);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<CDMS_GOS_Archives>();
                    return result;
                }
                else
                {
                    //await _js.InvokeVoidAsync("alert", $"Error get folders: {response.Content.ReadAsStringAsync().Result}");
                    Console.WriteLine($"GET FILES GOS, Status Code {response.StatusCode} : {response.Content.ReadAsStringAsync().Result}");
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
            catch (Exception ex)
            {
                Console.WriteLine($"Error of Json GOS: {ex.Message}");

                var response = await _http.PostAsync("BridgeCDMS/SMGos/PostArchivesDirectoryGos", content);
                var contentString = await response.Content.ReadAsStringAsync();

                // Deserializar el contenido en un objeto JObject
                var responseObject = JObject.Parse(contentString);

                // Acceder a la propiedad "operation" del objeto JObject
                var operation = (string)responseObject["operation"];

                if (operation == "NO FILES IN DIRECTORY")
                {
                    Console.WriteLine($"No Files or Directories");
                    CDMS_GOS_Archives toreturn = new CDMS_GOS_Archives();
                    toreturn.success = false;
                    toreturn.message = operation;
                    return toreturn;
                }else if(operation == "NO FILES OR DIRECTORIES")
                {
                    Console.WriteLine($"No Files or Directories");
                    CDMS_GOS_Archives toreturn = new CDMS_GOS_Archives();
                    toreturn.success = false;
                    toreturn.message = operation;
                    return toreturn;
                }
            }

            return null;
        }

        public async Task<CDMS_DownloadFile> GetDownloadLinkGOS(string URL)
        {
            var parameters = new Dictionary<string, string>
            {
                { "route", URL }
            };

            var json = JsonConvert.SerializeObject(parameters);

            var content = new StringContent(json, Encoding.UTF8, "application/json");

            try
            {
                var response = await _http.PostAsync("BridgeCDMS/SMGos/PostDownloadfileGos", content);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<CDMS_DownloadFile>();
                    return result;
                }
                else
                {
                    //await _js.InvokeVoidAsync("alert", $"Error get folders: {response.Content.ReadAsStringAsync().Result}");
                    Console.WriteLine($"GET LINK GOS, Status Code {response.StatusCode} : {response.Content.ReadAsStringAsync().Result}");
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
                var uri = new Uri(_http.BaseAddress, "BridgeCDMS/SMGos/DeleteFileTempGos");

                var request = new HttpRequestMessage(HttpMethod.Delete, uri)
                {
                    Content = content
                };

                var response = await _http.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<CDMS_General>();
                    return result;
                }
                else
                {
                    //await _js.InvokeVoidAsync("alert", $"Error get folders: {response.Content.ReadAsStringAsync().Result}");
                    Console.WriteLine($"DELETE TEMP GOS, Status Code {response.StatusCode} : {response.Content.ReadAsStringAsync().Result}");
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
