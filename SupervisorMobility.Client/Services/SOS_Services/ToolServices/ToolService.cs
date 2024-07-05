using Microsoft.JSInterop;
using System.Net.Http.Json;

namespace SupervisorMobility.Client.Services.SOS_Services.ToolServices
{
    public class ToolService : IToolService
    {
        private readonly HttpClient _http;
        private readonly JsonSerializerOptions _options;

        // Constructor
        public ToolService(HttpClient customHttpClientService, IJSRuntime jSRuntime)
        {
            _http = customHttpClientService;
            _options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        }

        public async Task<Tool> CreateTool(Tool Tool)
        {
            var response = await _http.PostAsJsonAsync("Analysis_Process/Tools", Tool);
            var newTool = await response.Content.ReadFromJsonAsync<Tool>();

            return newTool;
        }

        public async Task DeleteTool(int id)
        {
            var response = await _http.DeleteAsync($"Analysis_Process/Tools/{id}");
        }

        public async Task<Tool> GetToolById(int id)
        {
            var response = await _http.GetAsync($"Analysis_Process/Tools/{id}");
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new ApplicationException(content);
            }

            var Tool = JsonSerializer.Deserialize<Tool>(content, _options);

            return Tool;
        }

        public async Task<List<Tool>> GetTools()
        {
            var response = await _http.GetAsync("Analysis_Process/Tools");
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new ApplicationException(content);
            }

            var Tools = JsonSerializer.Deserialize<List<Tool>>(content, _options);

            return Tools;
        }

        public async Task<bool> UpdateTool(Tool Tool)
        {
            var response = await _http.PutAsJsonAsync($"Analysis_Process/Tools/{Tool.ToolId}", Tool);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return true;
            }

            return false;
        }
    }
}
