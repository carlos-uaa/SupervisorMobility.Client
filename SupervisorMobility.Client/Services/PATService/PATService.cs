using Blazorise.Utilities;
using Microsoft.JSInterop;
using SupervisorMobility.Client.Data.Entities;
using System.Net.Http.Json;
using System.Text.Json.Serialization;

namespace SupervisorMobility.Client.Services.PATService
{
    public class PATService : IPATService
    {
        private readonly HttpClient _http;
        private readonly IJSRuntime _js;
        private readonly JsonSerializerOptions _options;

        // Constructor
        public PATService(HttpClient customHttpClientService, IJSRuntime jSRuntime)
        {
            _http = customHttpClientService;
            _js = jSRuntime;
            _options = new JsonSerializerOptions
            {
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault,
                PropertyNameCaseInsensitive = true,
                NumberHandling = JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.WriteAsString,
            };
            _options.Converters.Add(new IntToStringConverter());
        }

        public async Task<PAT> CreatePat(PAT pat)
        {
            try
            {
                var response = await _http.PostAsJsonAsync($"PAT", pat);

                var content = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var newPat = JsonSerializer.Deserialize<PAT>(content, _options);

                    response.Dispose();

                    return newPat;
                }
            }
            catch (Exception ex)
            {
                // Manejo de excepciones
                Console.WriteLine($"Error al obtener la lista de usuarios: {ex.Message}");
            }

            return null;
        }
        public async Task<PAT?> getPat(int patid)
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

        public async Task<int?> getPatByJob(int Jobid, int areaid, int plantid)
        {
            try
            {
                var response = await _http.GetAsync($"PAT/JOB/{Jobid}?plantid={plantid}&&areaid={areaid}");

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var id = int.Parse(content);

                    response.Dispose();

                    return id;
                }
            }
            catch (Exception ex)
            {
                // Manejo de excepciones
                Console.WriteLine($"Error al obtener el PAT Relacionado: {ex.Message}");
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
                Console.WriteLine($"Error al obtener la lista de PATS: {ex.Message}");
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

        public async Task<bool> DeletePat(int patId)
        {
            var response = await _http.DeleteAsync($"PAT/{patId}");
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                return false;
            }

            return true;
        }

        // Update pat
        public async Task<bool> UpdatePat(PAT pat)
        {
            var response = await _http.PutAsJsonAsync($"PAT/{pat.PATid}", pat);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return true;
            }

            return false;
        }

    }
}
