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
