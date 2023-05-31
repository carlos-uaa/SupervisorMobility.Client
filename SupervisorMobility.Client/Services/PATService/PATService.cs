using Microsoft.JSInterop;

namespace SupervisorMobility.Client.Services.PATService
{
    public class PATService : IPATService
    {
        private readonly HttpClient _http;
        private readonly HttpClient _httpBridge;
        private readonly IJSRuntime _js;
        private readonly JsonSerializerOptions _options;

        // Constructor
        public PATService(CustomHttpClientService customHttpClientService, IJSRuntime jSRuntime)
        {
            _http = customHttpClientService.GetApiHttpClient();
            _httpBridge = customHttpClientService.GetBridgeHttpClient();
            _js = jSRuntime;
            _options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        }

        public async Task<PAT> getPat(int patid)
        {
            try
            {
                var response = await _http.GetAsync($"PAT/{patid}?includeCollections=true");

                var content = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var Pat = JsonSerializer.Deserialize<PAT>(content, _options);

                    response.Dispose();

                    return Pat;
                }
            }
            catch (Exception ex)
            {
                // Manejo de excepciones
                Console.WriteLine($"Error al obtener la lista de PAT: {ex.Message}");
            }

            return null;
        }

        public async Task<List<PAT>> GetAllPATs()
        {

            try
            {
                var response = await _http.GetAsync($"PAT?includeCollections=true");

                var content = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var PatList = JsonSerializer.Deserialize<List<PAT>>(content, _options);

                    response.Dispose();

                    return PatList;
                }
            }
            catch (Exception ex)
            {
                // Manejo de excepciones
                Console.WriteLine($"Error al obtener la lista de PATS SSV: {ex.Message}");
            }

            return null;

        }

        public async Task<List<PAT>> GetAllPATSforSSV(int ssvid)
        {

            try
            {
                var response = await _http.GetAsync($"PAT/SSV/{ssvid}?includeCollections=true");

                var content = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var PatList = JsonSerializer.Deserialize<List<PAT>>(content, _options);

                    response.Dispose();

                    return PatList;
                }
            }
            catch (Exception ex)
            {
                // Manejo de excepciones
                Console.WriteLine($"Error al obtener la lista de PATS SSV: {ex.Message}");
            }

            return null;

        }

        public async Task<List<PAT>> GetAllPATSforSV(int svid)
        {

            try
            {
                var response = await _http.GetAsync($"PAT/SV/{svid}?includeCollections=true");

                var content = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var PatList = JsonSerializer.Deserialize<List<PAT>>(content, _options);

                    response.Dispose();

                    return PatList;
                }
            }
            catch (Exception ex)
            {
                // Manejo de excepciones
                Console.WriteLine($"Error al obtener la lista de PATS SSV: {ex.Message}");
            }

            return null;

        }

    }
}
