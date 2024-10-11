using Microsoft.AspNetCore.Routing;
using Microsoft.JSInterop;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SupervisorMobility.Client.Data.Entities.CDMS;
using System;
using System.IO;
using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.XPath;

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
        #region CCP

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
                //Console.WriteLine($"Error al hacer la solicitud: {ex.Message}");
            }
            catch (TaskCanceledException ex)
            {
                //Console.WriteLine($"La solicitud ha sido cancelada: {ex.Message}");
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
                    //Console.WriteLine($"GET FILES CCP, Status Code {response.StatusCode} : {response.Content.ReadAsStringAsync().Result}");
                }
            }
            catch (HttpRequestException ex)
            {
                //Console.WriteLine($"Error al hacer la solicitud: {ex.Message}");
            }
            catch (TaskCanceledException ex)
            {
                //Console.WriteLine($"La solicitud ha sido cancelada: {ex.Message}");
            }
            catch (Exception ex)
            {
                //Console.WriteLine($"Error of Json GOS: {ex.Message}");

                var response = await _http.PostAsync("BridgeCDMS/SMGos/PostArchivesDirectoryGos", content);
                var contentString = await response.Content.ReadAsStringAsync();

                // Deserializar el contenido en un objeto JObject
                var responseObject = JObject.Parse(contentString);

                // Acceder a la propiedad "operation" del objeto JObject
                var operation = (string)responseObject["operation"];

                if (operation == "NO FILES IN DIRECTORY")
                {
                    //Console.WriteLine($"No Files or Directories");
                    CDMS_CCP_Archives toreturn = new CDMS_CCP_Archives();
                    toreturn.success = false;
                    toreturn.message = operation;
                    return toreturn;
                }
                else if (operation == "NO FILES OR DIRECTORIES")
                {
                    //Console.WriteLine($"No Files or Directories");
                    CDMS_CCP_Archives toreturn = new CDMS_CCP_Archives();
                    toreturn.success = false;
                    toreturn.message = operation;
                    return toreturn;
                }
            }

            return null;

        }

        public async Task<AsyncVoidMethodBuilder> GetDownloadLinkCCP(int ID, string namefile)
        {
            var parameters = new Dictionary<string, int>
            {
                { "ID", ID }
            };

            var json = JsonConvert.SerializeObject(parameters);

            var content = new StringContent(json, Encoding.UTF8, "application/json");

            try
            {
                var response = await _http.PostAsync("BridgeCDMS/SMCcp/PostDownloadfileCcp", content);

                string KeyDocument = "";
                string PathDocument = "";

                if (response.Headers.TryGetValues("KeyDocument", out IEnumerable<string> keyValue))
                {
                    string headerValue = keyValue.FirstOrDefault();
                    KeyDocument = headerValue;
                }
                else
                {
                    //Console.WriteLine($"El encabezado 'KeyDocument' no está presente en la respuesta HTTP.");
                }

                if (response.Headers.TryGetValues("PathDocument", out IEnumerable<string> pathValue))
                {
                    string headerValue = pathValue.FirstOrDefault();
                    PathDocument = headerValue;
                }
                else
                {
                    //Console.WriteLine($"El encabezado 'PathDocument' no está presente en la respuesta HTTP.");
                }

                if (response.IsSuccessStatusCode)
                {

                    // Obtener el contenido de la respuesta como un Stream
                    using (var stream = await response.Content.ReadAsStreamAsync())
                    {
                        // Leer los bytes del Stream
                        byte[] fileBytes;
                        using (var memoryStream = new MemoryStream())
                        {
                            await stream.CopyToAsync(memoryStream);
                            fileBytes = memoryStream.ToArray();
                        }

                        var result = await _js.InvokeAsync<string>("triggerFileDownloadAndWaitForConfirmation", namefile, fileBytes);

                        //Console.WriteLine($"Download CCP - fileDownlaod Succes");
                        if (result == "File downloaded successfully")
                        {
                            var DeleteTemp = await DeleteFileTempCCP(KeyDocument, PathDocument);
                            if (DeleteTemp is not null)
                            {
                                //Console.WriteLine($"Delete File TempCCP successfully");
                            }
                        }
                        else
                        {
                            //Console.WriteLine("Error durante la descarga del archivo.");
                        }


                    }//end using

                    return new AsyncVoidMethodBuilder();
                }
                else
                {
                    //await _js.InvokeVoidAsync("alert", $"Error get folders: {response.Content.ReadAsStringAsync().Result}");
                    //Console.WriteLine($"GET LINK GOS, Status Code {response.StatusCode} : {response.Content.ReadAsStringAsync().Result}");
                }
            }
            catch (HttpRequestException ex)
            {
                //Console.WritkeLine($"Error al hacer la solicitud: {ex.Message}");
            }
            catch (TaskCanceledException ex)
            {
                //Console.WriteLine($"La solicitud ha sido cancelada: {ex.Message}");
            }

            return new AsyncVoidMethodBuilder();
        }

        public async Task<CDMS_General> DeleteFileTempCCP(string FileName, string pathFile)
        {
            var parameters = new Dictionary<string, string>
            {
                { "routeDelete", FileName },
                { "documentDelete", pathFile }
            };

            var json = JsonConvert.SerializeObject(parameters);

            var content = new StringContent(json, Encoding.UTF8, "application/json");

            try
            {
                var response = await _http.PostAsync("BridgeCDMS/SMCcp/DeleteFileTempCcp", content);

                if (response.IsSuccessStatusCode)
                {
                    //var result = await response.Content.ReadFromJsonAsync<CDMS_General>();
                    //return result;
                    //Console.WriteLine($"Delete File TempCCP successfully");
                }
                else
                {
                    //await _js.InvokeVoidAsync("alert", $"Error get folders: {response.Content.ReadAsStringAsync().Result}");
                    //Console.WriteLine($"DELETE TEMP CCP, Status Code {response.StatusCode} : {response.Content.ReadAsStringAsync().Result}");
                }
            }
            catch (HttpRequestException ex)
            {
                //Console.WriteLine($"Error al hacer la solicitud: {ex.Message}");
            }
            catch (TaskCanceledException ex)
            {
                //Console.WriteLine($"La solicitud ha sido cancelada: {ex.Message}");
            }

            return null;
        }

        #endregion
        //HOE
        #region hoe
        public async Task<CDMS_HOE_Directory> GetFoldersHOE()
        {
            try
            {
                var response = await _http.GetAsync("BridgeCDMS/SMHoe/GetDirectoryPathsHoe");

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<CDMS_HOE_Directory>();
                    return result;
                }
                else
                {
                    //await _js.InvokeVoidAsync("alert", $"Error get folders: {response.Content.ReadAsStringAsync().Result}");
                    //Console.WriteLine($"GET FOLDERS HOE, Status Code {response.StatusCode} : {response.Content.ReadAsStringAsync().Result}");
                }
            }
            catch (HttpRequestException ex)
            {
                //Console.WriteLine($"Error al hacer la solicitud: {ex.Message}");
            }
            catch (TaskCanceledException ex)
            {
                //Console.WriteLine($"La solicitud ha sido cancelada: {ex.Message}");
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
                    //Console.WriteLine($"GET FILES HOE, Status Code {response.StatusCode} : {response.Content.ReadAsStringAsync().Result}");
                }
            }
            catch (HttpRequestException ex)
            {
                //Console.WriteLine($"Error al hacer la solicitud: {ex.Message}");
            }
            catch (TaskCanceledException ex)
            {
                //Console.WriteLine($"La solicitud ha sido cancelada: {ex.Message}");
            }
            catch (Exception ex)
            {
                //Console.WriteLine($"Error of Json HOE: {ex.Message}");

                var response = await _http.PostAsync("BridgeCDMS/SMHoe/PostArchivesDirectoryHOE", content);
                var contentString = await response.Content.ReadAsStringAsync();

                // Deserializar el contenido en un objeto JObject
                var responseObject = JObject.Parse(contentString);

                // Acceder a la propiedad "operation" del objeto JObject
                var operation = (string)responseObject["operation"];
                //Console.WriteLine($"REAL ERRO [{operation}]");

                if (operation == "NO FILES IN DIRECTORY")
                {
                    //Console.WriteLine($"No Files in Directories");
                    CDMS_HOE_Archives toreturn = new CDMS_HOE_Archives();
                    toreturn.success = false;
                    toreturn.message = operation;
                    return toreturn;
                }
                else if (operation == "NO FILES OR DIRECTORIES")
                {
                    //Console.WriteLine($"No Files or Directories");
                    CDMS_HOE_Archives toreturn = new CDMS_HOE_Archives();
                    toreturn.success = false;
                    toreturn.message = operation;
                    return toreturn;
                }
            }

            return null;
        }

        public async Task<AsyncVoidMethodBuilder> Download_DeleteFileTempHOE(string FileName, string pathFile)
        {
            var parameters = new Dictionary<string, string>
            {
                { "route", pathFile }
            };

            var json = JsonConvert.SerializeObject(parameters);

            var content = new StringContent(json, Encoding.UTF8, "application/json");

            try
            {
                var response = await _http.PostAsync("BridgeCDMS/SMHoe/PostDownloadHOE", content);

                string PathDocument = "";

                if (response.Headers.TryGetValues("PathDocument", out IEnumerable<string> pathValue))
                {
                    string headerValue = pathValue.FirstOrDefault();
                    PathDocument = headerValue;
                }
                else
                {
                    //Console.WriteLine($"El encabezado 'PathDocument' no está presente en la respuesta HTTP.");
                }

                if (response.IsSuccessStatusCode)
                {

                    // Obtener el contenido de la respuesta como un Stream
                    using (var stream = await response.Content.ReadAsStreamAsync())
                    {
                        // Leer los bytes del Stream
                        byte[] fileBytes;
                        using (var memoryStream = new MemoryStream())
                        {
                            await stream.CopyToAsync(memoryStream);
                            fileBytes = memoryStream.ToArray();
                        }

                        var result = await _js.InvokeAsync<string>("triggerFileDownloadAndWaitForConfirmation", FileName, fileBytes);

                        //Console.WriteLine($"Download GOS - fileDownlaod Succes");
                        if (result == "File downloaded successfully")
                        {
                            var DELETEparameters = new Dictionary<string, string>
                                {
                                    { "documentDelete", PathDocument }
                                };

                            var deletejson = JsonConvert.SerializeObject(DELETEparameters);

                            var deletecontent = new StringContent(deletejson, Encoding.UTF8, "application/json");

                            try
                            {
                                var DeleteTemp = await _http.PostAsync("BridgeCDMS/SMHoe/DeleteFileTempHoe", deletecontent);

                                if (response.IsSuccessStatusCode)
                                {
                                    //Console.WriteLine($"Delete File TempHOE successfully");
                                }
                                else
                                {
                                    //Console.WriteLine($"DELETE TEMP hoe, Status Code {response.StatusCode} : {response.Content.ReadAsStringAsync().Result}");
                                }
                            }
                            catch (HttpRequestException ex)
                            {
                                //Console.WriteLine($"Error al hacer la solicitud: {ex.Message}");
                            }
                            catch (TaskCanceledException ex)
                            {
                                //Console.WriteLine($"La solicitud ha sido cancelada: {ex.Message}");
                            }
                          
                        }
                        else
                        {
                            //Console.WriteLine("Error durante la descarga del archivo.");
                        }

                    }//end using

                    return new AsyncVoidMethodBuilder();
                }
                else
                {
                    //Console.WriteLine($"GET FILE HOE, Status Code {response.StatusCode} : {response.Content.ReadAsStringAsync().Result}");
                }
            }
            catch (HttpRequestException ex)
            {
                //Console.WriteLine($"Error al hacer la solicitud: {ex.Message}");
            }
            catch (TaskCanceledException ex)
            {
                //Console.WriteLine($"La solicitud ha sido cancelada: {ex.Message}");
            }

            return new AsyncVoidMethodBuilder();
        }
        #endregion

        #region GOS
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
                    //Console.WriteLine($"GET FOLDERS GOS, Status Code {response.StatusCode} : {response.Content.ReadAsStringAsync().Result}");
                }
            }
            catch (HttpRequestException ex)
            {
                //Console.WriteLine($"Error al hacer la solicitud: {ex.Message}");
            }
            catch (TaskCanceledException ex)
            {
                //Console.WriteLine($"La solicitud ha sido cancelada: {ex.Message}");
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
                    //Console.WriteLine($"GET FILES GOS, Status Code {response.StatusCode} : {response.Content.ReadAsStringAsync().Result}");
                }
            }
            catch (HttpRequestException ex)
            {
                //Console.WriteLine($"Error al hacer la solicitud: {ex.Message}");
            }
            catch (TaskCanceledException ex)
            {
                //Console.WriteLine($"La solicitud ha sido cancelada: {ex.Message}");
            }
            catch (Exception ex)
            {
                //Console.WriteLine($"Error of Json GOS: {ex.Message}");

                var response = await _http.PostAsync("BridgeCDMS/SMGos/PostArchivesDirectoryGos", content);
                var contentString = await response.Content.ReadAsStringAsync();

                // Deserializar el contenido en un objeto JObject
                var responseObject = JObject.Parse(contentString);

                // Acceder a la propiedad "operation" del objeto JObject
                var operation = (string)responseObject["operation"];

                if (operation == "NO FILES IN DIRECTORY")
                {
                    //Console.WriteLine($"No Files or Directories");
                    CDMS_GOS_Archives toreturn = new CDMS_GOS_Archives();
                    toreturn.success = false;
                    toreturn.message = operation;
                    return toreturn;
                }else if(operation == "NO FILES OR DIRECTORIES")
                {
                    //Console.WriteLine($"No Files or Directories");
                    CDMS_GOS_Archives toreturn = new CDMS_GOS_Archives();
                    toreturn.success = false;
                    toreturn.message = operation;
                    return toreturn;
                }
            }

            return null;
        }

        public async Task<AsyncVoidMethodBuilder> GetDownloadLinkGOS(int ID, string namefile)
        {
            var parameters = new Dictionary<string, string>
            {
                { "ID", ID.ToString() }
            };

            var json = JsonConvert.SerializeObject(parameters);

            var content = new StringContent(json, Encoding.UTF8, "application/json");

            try
            {
                var response = await _http.PostAsync("BridgeCDMS/SMGos/PostDownloadfileGos", content);

                string KeyDocument = "";
                string PathDocument = "";

                if (response.Headers.TryGetValues("KeyDocument", out IEnumerable<string> keyValue))
                {
                    string headerValue = keyValue.FirstOrDefault();
                    KeyDocument = headerValue;
                }
                else
                {
                    //Console.WriteLine($"El encabezado 'KeyDocument' no está presente en la respuesta HTTP.");
                }

                if (response.Headers.TryGetValues("PathDocument", out IEnumerable<string> pathValue))
                {
                    string headerValue = pathValue.FirstOrDefault();
                    PathDocument = headerValue;
                }
                else
                {
                    //Console.WriteLine($"El encabezado 'PathDocument' no está presente en la respuesta HTTP.");
                }

                if (response.IsSuccessStatusCode)
                {

                    // Obtener el contenido de la respuesta como un Stream
                    using (var stream = await response.Content.ReadAsStreamAsync())
                    {
                        // Leer los bytes del Stream
                        byte[] fileBytes;
                        using (var memoryStream = new MemoryStream())
                        {
                            await stream.CopyToAsync(memoryStream);
                            fileBytes = memoryStream.ToArray();
                        }

                        var result = await _js.InvokeAsync<string>("triggerFileDownloadAndWaitForConfirmation", namefile, fileBytes);

                        Console.WriteLine($"Download GOS - fileDownlaod Succes");
                        if (result == "File downloaded successfully")
                        {
                            var DeleteTemp = await DeleteFileTempGOS(KeyDocument, PathDocument);
                            if (DeleteTemp is not null)
                            {
                                //Console.WriteLine($"Delete File TempGOS successfully");
                            }
                        }
                        else
                        {
                            //Console.WriteLine("Error durante la descarga del archivo.");
                        }

                      
                    }//end using

                    return new AsyncVoidMethodBuilder();
                }
                else
                {
                    //await _js.InvokeVoidAsync("alert", $"Error get folders: {response.Content.ReadAsStringAsync().Result}");
                    //Console.WriteLine($"GET LINK GOS, Status Code {response.StatusCode} : {response.Content.ReadAsStringAsync().Result}");
                }
            }
            catch (HttpRequestException ex)
            {
                //Console.WriteLine($"Error al hacer la solicitud: {ex.Message}");
            }
            catch (TaskCanceledException ex)
            {
                //Console.WriteLine($"La solicitud ha sido cancelada: {ex.Message}");
            }

            return new AsyncVoidMethodBuilder();
        }

        public async Task<CDMS_General> DeleteFileTempGOS(string FileName, string pathFile)
        {
            var parameters = new Dictionary<string, string>
            {
                { "routeDelete", FileName },
                { "documentDelete", pathFile }
            };

            var json = JsonConvert.SerializeObject(parameters);

            var content = new StringContent(json, Encoding.UTF8, "application/json");

            try
            {
                var response = await _http.PostAsync("BridgeCDMS/SMGos/DeleteFileTempGos", content);

                if (response.IsSuccessStatusCode)
                {
                    //Console.WriteLine($"Delete File TempGOS successfully");

                    //var result = await response.Content.ReadFromJsonAsync<CDMS_General>();
                    //return result;
                }
                else
                {
                    //await _js.InvokeVoidAsync("alert", $"Error get folders: {response.Content.ReadAsStringAsync().Result}");
                    //Console.WriteLine($"DELETE TEMP GOS, Status Code {response.StatusCode} : {response.Content.ReadAsStringAsync().Result}");
                }
            }
            catch (HttpRequestException ex)
            {
                //Console.WriteLine($"Error al hacer la solicitud: {ex.Message}");
            }
            catch (TaskCanceledException ex)
            {
                //Console.WriteLine($"La solicitud ha sido cancelada: {ex.Message}");
            }

            return null;
        }
        #endregion 
    }

}
