using Microsoft.JSInterop;
using System.Net.Http.Json;

namespace SupervisorMobility.Client.Services.JobObservationTypeService
{
    public class JobObservationTypeService : IJobObservationTypeService
    {
        private readonly HttpClient _http;
        private readonly JsonSerializerOptions _options;

        // Constructor
        public JobObservationTypeService(HttpClient customHttpClientService, IJSRuntime jSRuntime)
        {
            _http = customHttpClientService;
            _options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        }

        // Create job observation type
        public async Task<JobObservationType> CreateJobObservationType(JobObservationType jobObservationType)
        {
            var response = await _http.PostAsJsonAsync("jobObservationTypes", jobObservationType);
            var newJobObservationType = await response.Content.ReadFromJsonAsync<JobObservationType>();

            return newJobObservationType;
        }

        // Delete job observation type
        public async Task DeleteJobObservationType(int id)
        {
            var response = await _http.DeleteAsync($"jobObservationTypes/{id}");
        }

        // Get job observation type by Id
        public async Task<JobObservationType> GetJobObservationTypeById(int id)
        {
            var response = await _http.GetAsync($"jobObservationTypes/{id}");
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new ApplicationException(content);
            }

            var jobObservationType = JsonSerializer.Deserialize<JobObservationType>(content, _options);

            return jobObservationType;
        }

        // Get all job observation types
        public async Task<List<JobObservationType>> GetJobObservationTypes()
        {
            var response = await _http.GetAsync("jobObservationTypes");
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new ApplicationException(content);
            }

            var jobObservationTypes = JsonSerializer.Deserialize<List<JobObservationType>>(content, _options);

            return jobObservationTypes;
        }

        // Update job observation type
        public async Task UpdateJobObservationType(JobObservationType jobObservationType)
        {
            var response = await _http.PutAsJsonAsync($"jobObservationTypes/{jobObservationType.JobObservationTypeId}", jobObservationType);
        }
    }
}
