using Microsoft.JSInterop;
using SupervisorMobility.Client.Data.Entities;
using System.Net.Http.Json;

namespace SupervisorMobility.Client.Services.ChecklistAnswerService
{
    public class ChecklistAnswerService : IChecklistAnswerService
    {
        private readonly HttpClient _http;
        private readonly JsonSerializerOptions _options;
        private readonly IJSRuntime _js;

        // Constructor
        public ChecklistAnswerService(HttpClient customHttpClientService, IJSRuntime jSRuntime)
        {
            _http = customHttpClientService;
            _js = jSRuntime;
            _options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        }

        public async Task<ChecklistAnswer> CreateChecklistAnswer(MultipartFormDataContent checklistAnswer)
        {
            var response = await _http.PostAsJsonAsync($"checklistAnswers", checklistAnswer);
            var newChecklistAnswer = await response.Content.ReadFromJsonAsync<ChecklistAnswer>();

            return newChecklistAnswer;
        }

        public async Task<ChecklistAnswer> CreateEvidencesChecklistAnswer(MultipartFormDataContent checklistAnswer)
        {
            var response = await _http.PostAsJsonAsync($"checklistAnswers/evidences", checklistAnswer);
            var newChecklistAnswer = await response.Content.ReadFromJsonAsync<ChecklistAnswer>();

            return newChecklistAnswer;
        }

        public async Task DeleteChecklistAnswer(int checklistAnswerId)
        {
            var response = await _http.DeleteAsync($"checklistAnswers/{checklistAnswerId}");
        }


        public async Task<List<ChecklistAnswer>> GetAllChecklistAnswers()
        {
            var response = await _http.GetAsync($"checklistAnswers");
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new ApplicationException(content);
            }

            var checklistAnswer = JsonSerializer.Deserialize<List<ChecklistAnswer>>(content, _options);

            return checklistAnswer;
        }

        public async Task<List<ChecklistAnswer>> GetAllChecklistAnswersByJobObservationId(int jobObservationId)
        {
            var response = await _http.GetAsync($"checklistAnswers/JobObservationId/{jobObservationId}");
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new ApplicationException(content);
            }

            var checklistAnswer = JsonSerializer.Deserialize<List<ChecklistAnswer>>(content, _options);

            return checklistAnswer;
        }

        public async Task<ChecklistAnswer> GetChecklistAnswerById(int checklistAnswerId)
        {
            var response = await _http.GetAsync($"checklistAnswers/{checklistAnswerId}");
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new ApplicationException(content);
            }

            var checklistAnswer = JsonSerializer.Deserialize<ChecklistAnswer>(content, _options);

            return checklistAnswer;
        }


        public async Task<bool> UpdateChecklistAnswer(ChecklistAnswer checklistAnswer)
        {
            var response = await _http.PutAsJsonAsync($"checklistAnswers/{checklistAnswer}", checklistAnswer);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return true;
            }

            return false;
        }


    }
}
