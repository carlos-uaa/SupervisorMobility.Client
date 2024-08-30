using Microsoft.JSInterop;
using System.Net.Http.Json;

namespace SupervisorMobility.Client.Services.StationService
{
    public class StationService : IStationService
    {
        private readonly HttpClient _http;
        private readonly JsonSerializerOptions _options;

        // Constructor
        public StationService(HttpClient customHttpClientService, IJSRuntime jSRuntime)
        {
            _http = customHttpClientService;
            _options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        }

        public async Task<Station> CreateStation(Station Station)
        {
            var response = await _http.PostAsJsonAsync("Station", Station);
            var newStation = await response.Content.ReadFromJsonAsync<Station>();

            return newStation;
        }

        public async Task DeleteStation(int id)
        {
            var response = await _http.DeleteAsync($"Station/{id}");
        }

        public async Task<Station> GetStationById(int id)
        {
            var response = await _http.GetAsync($"Station/{id}");
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new ApplicationException(content);
            }

            var Station = JsonSerializer.Deserialize<Station>(content, _options);

            return Station;
        }

        public async Task<List<Station>> GetStations()
        {
            var response = await _http.GetAsync("Station");
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new ApplicationException(content);
            }

            var Stations = JsonSerializer.Deserialize<List<Station>>(content, _options);

            return Stations;
        }

        public async Task<bool> UpdateStation(Station Station)
        {
            var response = await _http.PutAsJsonAsync($"Station/{Station.StationId}", Station);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return true;
            }

            return false;
        }
    }
}
