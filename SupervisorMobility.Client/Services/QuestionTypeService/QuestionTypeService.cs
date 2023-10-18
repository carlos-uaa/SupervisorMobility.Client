using Microsoft.JSInterop;

namespace SupervisorMobility.Client.Services.QuestionTypeService
{
    public class QuestionTypeService : IQuestionTypeService
    {
        private readonly HttpClient _http;
        private readonly HttpClient _httpBridge;
        private readonly JsonSerializerOptions _options;

        // Constructor
        public QuestionTypeService(CustomHttpClientService customHttpClientService, IJSRuntime jSRuntime)
        {
            _http = customHttpClientService.GetApiHttpClient();
            _httpBridge = customHttpClientService.GetBridgeHttpClient();
            _options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        }

        // Get all question types
        public async Task<List<QuestionType>> GetQuestionTypes()
        {
            var response = await _http.GetAsync("QuestionTypes");
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new ApplicationException(content);
            }

            var questionTypes = JsonSerializer.Deserialize<List<QuestionType>>(content, _options);

            return questionTypes;
        }
    }
}
