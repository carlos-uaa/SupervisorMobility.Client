using Microsoft.JSInterop;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using System;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Net.Http.Json;
using System.Net;
using SupervisorMobility.Client.Data.Entities;

namespace SupervisorMobility.Client.Services.LoginService
{
    public class LoginService : ILoginService
    {
        private readonly HttpClient _http;
        private readonly HttpClient _httpBridge;
        private readonly HttpClient _httpAD;
        private readonly JsonSerializerOptions _options;
        private readonly IJSRuntime _js;

        // Constructor
        public LoginService(CustomHttpClientService customHttpClientService, IJSRuntime jSRuntime)
        {
            _http = customHttpClientService.GetApiHttpClient();
            _httpBridge = customHttpClientService.GetBridgeHttpClient();
            _httpAD = customHttpClientService.GetADHttpClient();
            _js = jSRuntime;
            _options = new JsonSerializerOptions
            {
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault,
                PropertyNameCaseInsensitive = true,
                NumberHandling = JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.WriteAsString
            };
            _options.Converters.Add(new IntToStringConverter());
        }

        public async Task<AD_User> LoginAD(string username, string password)
        {


            var data = new
            {
                username = username,
                password = password
            };

            var json = JsonConvert.SerializeObject(data); 

            var content = new StringContent(json, Encoding.UTF8, "application/json"); 

            try
            {
                var response = await _http.PostAsync($"login?username={username}&password={password}", content);

                if (response.StatusCode != HttpStatusCode.OK)
                {
                    return null;
                }

                var result = await response.Content.ReadFromJsonAsync<AD_User>(); // Leemos la respuesta


                return result;
            }catch(Exception ex)
            {
                Console.WriteLine("Error: " + ex.ToString());
                return null;
            }

        }
    }
}
