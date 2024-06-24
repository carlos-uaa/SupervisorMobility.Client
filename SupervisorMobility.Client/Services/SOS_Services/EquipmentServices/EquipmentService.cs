using Microsoft.JSInterop;
using System.Net.Http.Json;

namespace SupervisorMobility.Client.Services.SOS_Services.EquipmentServices
{
    public class EquipmentService : IEquipmentService
    {
        private readonly HttpClient _http;
        private readonly JsonSerializerOptions _options;

        // Constructor
        public EquipmentService(HttpClient customHttpClientService, IJSRuntime jSRuntime)
        {
            _http = customHttpClientService;
            _options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        }

        public async Task<Equipment> CreateEquipment(Equipment Equipment)
        {
            var response = await _http.PostAsJsonAsync("Analysis_Process/Equipments", Equipment);
            var newEquipment = await response.Content.ReadFromJsonAsync<Equipment>();

            return newEquipment;
        }

        public async Task DeleteEquipment(int id)
        {
            var response = await _http.DeleteAsync($"Analysis_Process/Equipments/{id}");
        }

        public async Task<Equipment> GetEquipmentById(int id)
        {
            var response = await _http.GetAsync($"Analysis_Process/Equipments/{id}");
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new ApplicationException(content);
            }

            var Equipment = JsonSerializer.Deserialize<Equipment>(content, _options);

            return Equipment;
        }

        public async Task<List<Equipment>> GetEquipments()
        {
            var response = await _http.GetAsync("Analysis_Process/Equipments");
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new ApplicationException(content);
            }

            var Equipments = JsonSerializer.Deserialize<List<Equipment>>(content, _options);

            return Equipments;
        }

        public async Task<bool> UpdateEquipment(Equipment Equipment)
        {
            var response = await _http.PutAsJsonAsync($"Analysis_Process/Equipments/{Equipment.EquipmentId}", Equipment);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return true;
            }

            return false;
        }
    }
}
