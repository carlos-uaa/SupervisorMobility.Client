using Microsoft.JSInterop;
using System.Net.Http.Json;

namespace SupervisorMobility.Client.Services.DepartmentService
{
    public class DepartmentService : IDepartmentService
    {
        private readonly HttpClient _http;
        private readonly HttpClient _httpBridge;
        private readonly JsonSerializerOptions _options;

        // Constructor
        public DepartmentService(CustomHttpClientService customHttpClientService, IJSRuntime jSRuntime)
        {
            _http = customHttpClientService.GetApiHttpClient();
            _httpBridge = customHttpClientService.GetBridgeHttpClient();
            _options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        }

        public async Task<Department> CreateDepartment(Department department)
        {
            var response = await _http.PostAsJsonAsync("department", department);
            var newDepartment = await response.Content.ReadFromJsonAsync<Department>();

            return newDepartment;
        }

        public async Task DeleteDepartment(int id)
        {
            var response = await _http.DeleteAsync($"department/{id}");
        }

        public async Task<Department> GetDepartmentById(int id)
        {
            var response = await _http.GetAsync($"department/{id}");
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new ApplicationException(content);
            }

            var department = JsonSerializer.Deserialize<Department>(content, _options);

            return department;
        }

        public async Task<List<Department>> GetDepartments()
        {
            var response = await _http.GetAsync("department");
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new ApplicationException(content);
            }

            var departments = JsonSerializer.Deserialize<List<Department>>(content, _options);

            return departments;
        }

        public async Task<bool> UpdateDepartment(Department department)
        {
            var response = await _http.PutAsJsonAsync($"department/{department.DepartmentId}", department);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return true;
            }

            return false;
        }
    }
}
