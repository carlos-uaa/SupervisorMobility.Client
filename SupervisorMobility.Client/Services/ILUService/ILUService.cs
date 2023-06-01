using Microsoft.JSInterop;
using SupervisorMobility.Client.Data;
using System.Text.Json.Serialization;

namespace SupervisorMobility.Client.Services.ILUService
{
    public class ILUService : IILUService
    {
        private readonly HttpClient _http;
        private readonly HttpClient _httpBridge;
        private readonly JsonSerializerOptions _options;
        private readonly IJSRuntime _js;

        // Constructor
        public ILUService(CustomHttpClientService customHttpClientService, IJSRuntime jSRuntime)
        {
            _http = customHttpClientService.GetApiHttpClient();
            _httpBridge = customHttpClientService.GetBridgeHttpClient();
            _js = jSRuntime;
            _options = new JsonSerializerOptions
            {
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault,
                PropertyNameCaseInsensitive = true,
                NumberHandling = JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.WriteAsString
            };
            _options.Converters.Add(new IntToStringConverter());
        }


        public async Task<List<ILULevel>> GetLevelsILU()
        {

            try
            {
                var response = await _http.GetAsync("Users");

                var content = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var iLULevelsList = JsonSerializer.Deserialize<List<ILULevel>>(content, _options);

                    response.Dispose();

                    return iLULevelsList;
                }
            }
            catch (Exception ex)
            {
                // Manejo de excepciones
                Console.WriteLine($"Error al obtener la lista de usuarios: {ex.Message}");
            }

            return null;

        }

    }

}
