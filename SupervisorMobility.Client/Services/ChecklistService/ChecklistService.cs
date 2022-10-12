using System.Net.Http.Json;

namespace SupervisorMobility.Client.Services.ChecklistService
{
    public class ChecklistService : IChecklistService
    {
        private readonly HttpClient _http;
        private readonly JsonSerializerOptions _options;

        // Constructor
        public ChecklistService(HttpClient http)
        {
            _http = http;
            _options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        }

        // Create checklist category
        public async Task<ChecklistCategory> CreateCategory(ChecklistCategory category)
        {
            var response = await _http.PostAsJsonAsync("checklistcategories", category);
            var newCategory = await response.Content.ReadFromJsonAsync<ChecklistCategory>();

            return newCategory;
        }

        // Get all checklist categories
        public async Task<List<ChecklistCategory>> GetChecklistCategories()
        {
            var response = await _http.GetAsync("checklistcategories");
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new ApplicationException(content);
            }

            var categories = JsonSerializer.Deserialize<List<ChecklistCategory>>(content, _options);

            return categories;
        }
    }
}
