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

        // Delete checklist category
        public async Task DeleteCategory(int id)
        {
            var response = await _http.DeleteAsync($"checklistcategories/{id}");
        }

        // Get checklist category by Id
        public async Task<ChecklistCategory> GetCategoryById(int id)
        {
            var response = await _http.GetAsync($"checklistcategories/{id}");
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new ApplicationException(content);
            }

            var category = JsonSerializer.Deserialize<ChecklistCategory>(content, _options);

            return category;
        }

        // Get checklist category including questions
        public async Task<ChecklistCategory> GetCategoryIncludingQuestions(int id)
        {
            var response = await _http.GetAsync($"checklistcategories/{id}?includeChecklistQuestions=true");
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new ApplicationException(content);
            }

            var category = JsonSerializer.Deserialize<ChecklistCategory>(content, _options);

            return category;
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

        // Update checklist category
        public async Task UpdateCategory(ChecklistCategory category)
        {
            var response = await _http.PutAsJsonAsync($"checklistcategories/{category.ChecklistCategoryId}", category);
        }

        // Update checklist category sequence
        public async Task UpdateCategorySequence(int categoryId, ChecklistCategory checklistCategory)
        {
            var response = await _http.PutAsJsonAsync($"checklistcategories/sequence/{categoryId}", checklistCategory);

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine("Failed");
            }
            else
            {
                Console.WriteLine("Ok");
            }
        }

        // Get checklist question by Id
        public async Task<ChecklistQuestion> GetQuestionById(int categoryId, int questionId)
        {
            var response = await _http.GetAsync($"checklistcategories/{categoryId}/checklistquestions/{questionId}");
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new ApplicationException(content);
            }

            var question = JsonSerializer.Deserialize<ChecklistQuestion>(content, _options);

            return question;
        }

        // Create checklist question 
        public async Task<ChecklistQuestion> CreateQuestion(int categoryId, ChecklistQuestion question)
        {
            var response = await _http.PostAsJsonAsync($"checklistcategories/{categoryId}/checklistquestions", question);
            var newQuestion = await response.Content.ReadFromJsonAsync<ChecklistQuestion>();

            return newQuestion;
        }

        // Delete checklist question 
        public async Task DeleteQuestion(int categoryId, int questionId)
        {
            var response = await _http.DeleteAsync($"checklistcategories/{categoryId}/checklistQuestions/{questionId}");
        }

        // Update checklist question 
        public async Task UpdateQuestion(int categoryId, ChecklistQuestion question)
        {
            var response = await _http.PutAsJsonAsync($"checklistcategories/{categoryId}/checklistquestions/{question.QuestionID}", question);
        }

        // Get all checklist questions by category Id
        public async Task<List<ChecklistQuestion>> GetChecklistQuestionsByCategoryId(int categoryId)
        {
            var response = await _http.GetAsync($"checklistcategories/{categoryId}/checklistquestions");
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new ApplicationException(content);
            }

            var questions = JsonSerializer.Deserialize<List<ChecklistQuestion>>(content, _options);

            return questions;
        }
    }
}
