using Microsoft.JSInterop;
using System.Net.Http.Json;

namespace SupervisorMobility.Client.Services.SOS_Services.MaterialServices
{
    public class MaterialService : IMaterialService
    {
        private readonly HttpClient _http;
        private readonly JsonSerializerOptions _options;

        // Constructor
        public MaterialService(HttpClient customHttpClientService, IJSRuntime jSRuntime)
        {
            _http = customHttpClientService;
            _options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        }

        public async Task<Material> CreateMaterial(Material Material)
        {
            var response = await _http.PostAsJsonAsync("Analysis_Process/Materials", Material);
            var newMaterial = await response.Content.ReadFromJsonAsync<Material>();

            return newMaterial;
        }

        public async Task DeleteMaterial(int id)
        {
            var response = await _http.DeleteAsync($"Analysis_Process/Materials/{id}");
        }

        public async Task<Material> GetMaterialById(int id)
        {
            var response = await _http.GetAsync($"Analysis_Process/Materials/{id}");
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new ApplicationException(content);
            }

            var Material = JsonSerializer.Deserialize<Material>(content, _options);

            return Material;
        }

        public async Task<List<Material>> GetMaterials()
        {
            var response = await _http.GetAsync("Analysis_Process/Materials");
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new ApplicationException(content);
            }

            var Materials = JsonSerializer.Deserialize<List<Material>>(content, _options);

            return Materials;
        }

        public async Task<bool> UpdateMaterial(Material Material)
        {
            var response = await _http.PutAsJsonAsync($"Analysis_Process/Materials/{Material.MaterialId}", Material);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return true;
            }

            return false;
        }
    }
}
