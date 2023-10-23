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
using DocumentFormat.OpenXml.Spreadsheet;

namespace SupervisorMobility.Client.Services.LoginService
{
    public class LoginService : ILoginService
    {
        private readonly HttpClient _http;
        private readonly JsonSerializerOptions _options;
        private readonly IJSRuntime _js;

        // Constructor
        public LoginService(HttpClient customHttpClientService, IJSRuntime jSRuntime)
        {
            _http = customHttpClientService;
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

            var parameters = new Dictionary<string, string>
            {
                { "user", username },
                { "pass", password }
            };

            //var content = new FormUrlEncodedContent(parameters);
            var json = JsonConvert.SerializeObject(parameters); 

            var content = new StringContent(json, Encoding.UTF8, "application/json"); 

            try
            {
                var response = await _http.PostAsync($"login", content);

                if(response.StatusCode != HttpStatusCode.OK)
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
