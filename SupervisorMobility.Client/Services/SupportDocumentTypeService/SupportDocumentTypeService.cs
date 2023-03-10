using Microsoft.JSInterop;
using System.Net.Http.Json;

namespace SupervisorMobility.Client.Services.SupportDocumentTypeService
{
    public class SupportDocumentTypeService : ISupportDocumentTypeService
    {
        private readonly HttpClient _http;
        private readonly HttpClient _httpBridge;
        private readonly JsonSerializerOptions _options;

        // Constructor
        public SupportDocumentTypeService(CustomHttpClientService customHttpClientService, IJSRuntime jSRuntime)
        {
            _http = customHttpClientService.GetApiHttpClient();
            _httpBridge = customHttpClientService.GetBridgeHttpClient();
            _options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        }

        // Create support document type
        public async Task<SupportDocumentType> CreateSupportDocumentType(SupportDocumentType supportDocumentType)
        {
            var response = await _http.PostAsJsonAsync("supportdocumenttypes", supportDocumentType);
            var newSupportDocumentType = await response.Content.ReadFromJsonAsync<SupportDocumentType>();

            return newSupportDocumentType;
        }

        // Delete support document type
        public async Task DeleteSupportDocumentType(int id)
        {
            var response = await _http.DeleteAsync($"supportdocumenttypes/{id}");
        }

        // Get support document type by Id
        public async Task<SupportDocumentType> GetSupportDocumentTypeById(int id)
        {
            var response = await _http.GetAsync($"supportdocumenttypes/{id}");
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new ApplicationException(content);
            }

            var supportDocumentType = JsonSerializer.Deserialize<SupportDocumentType>(content, _options);

            return supportDocumentType;
        }

        // Get all support document types
        public async Task<List<SupportDocumentType>> GetSupportDocumentTypes()
        {
            var response = await _http.GetAsync("supportdocumenttypes");
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new ApplicationException(content);
            }

            var supportDocumentTypes = JsonSerializer.Deserialize<List<SupportDocumentType>>(content, _options);

            return supportDocumentTypes;
        }

        // Update support document type
        public async Task<bool> UpdateSupportDocumentType(SupportDocumentType supportDocumentType)
        {
            var response = await _http.PutAsJsonAsync($"supportdocumenttypes/{supportDocumentType.SupportDocumentTypeId}", supportDocumentType);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return true;
            }

            return false;
        }
    }
}
