using Microsoft.JSInterop;
using System.Net.Http.Json;

namespace SupervisorMobility.Client.Services.JobObservationService
{
    public class JobObservationService : IJobObservationService
    {
        private readonly HttpClient _http;
        private readonly HttpClient _httpBridge;
        private readonly JsonSerializerOptions _options;

        // Constructor
        public JobObservationService(CustomHttpClientService customHttpClientService, IJSRuntime jSRuntime)
        {
            _http = customHttpClientService.GetApiHttpClient();
            _httpBridge = customHttpClientService.GetBridgeHttpClient();
            _options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        }

        public async Task<JobObservation> CreateJobObservation(JobObservation _jobObservation)
        {
            var response = await _http.PostAsJsonAsync($"jobobservations", _jobObservation);
            var newJobObservation = await response.Content.ReadFromJsonAsync<JobObservation>();

            return newJobObservation;
        }

        public async Task DeleteJobObservation(int jobObservationId)
        {
            var response = await _http.DeleteAsync($"jobobservations/{jobObservationId}");
        }

        public async Task<List<JobObservation>> GetAllJobObservations()
        {
            var response = await _http.GetAsync($"jobobservations");
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new ApplicationException(content);
            }

            var jobObservation = JsonSerializer.Deserialize<List<JobObservation>>(content, _options);

            return jobObservation;
        }

        public async Task<JobObservation> GetJobObservationWithLup(int jobObservationId)
        {
            var response = await _http.GetAsync($"jobobservations/{jobObservationId}?IncludeLup=true");
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new ApplicationException(content);
            }

            var jobObservation = JsonSerializer.Deserialize<JobObservation>(content, _options);

            return jobObservation;
        }


        public async Task<JobObservation> GetJobObservationById(int jobObservationId)
        {
            var response = await _http.GetAsync($"jobobservations/{jobObservationId}");
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new ApplicationException(content);
            }

            var jobObservation = JsonSerializer.Deserialize<JobObservation>(content, _options);

            return jobObservation;
        }

        public async Task<bool> UpdateJobObservation(JobObservation jobObservation)
        {
            var response = await _http.PutAsJsonAsync($"jobobservations/{jobObservation.JobObservationId}", jobObservation);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return true;
            }

            return false;
        }

    }
}
