using DocumentFormat.OpenXml.Office2010.Excel;
using System.Net.Http.Json;

namespace SupervisorMobility.Client.Services.JobObservationService
{
    public class JobObservationService : IJobObservationService
    {
        private readonly HttpClient _http;
        private readonly JsonSerializerOptions _options;

        // Constructor
        public JobObservationService(HttpClient http)
        {
            _http = http;
            _options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        }

        public async Task<JobObservation> CreateJobObservationType(JobObservation jobObservation)
        {
            var response = await _http.PostAsJsonAsync("jobObservations", jobObservation);
            var newJobObservation = await response.Content.ReadFromJsonAsync<JobObservation>();

            return newJobObservation;
        }

        public async Task DeleteJobObservation(int jobObservationId)
        {
            var response = await _http.DeleteAsync($"jobObservations/{jobObservationId}");
        }

        public async Task<List<JobObservation>> GetAllJobObservations()
        {
            var response = await _http.GetAsync("jobObservations");
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new ApplicationException(content);
            }

            var jobObservation = JsonSerializer.Deserialize<List<JobObservation>>(content, _options);

            return jobObservation;
        }

        public async Task<JobObservation> GetJobObservationById(int jobObservationId)
        {
            var response = await _http.GetAsync($"jobObservations/{jobObservationId}");
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new ApplicationException(content);
            }

            var jobObservation = JsonSerializer.Deserialize<JobObservation>(content, _options);

            return jobObservation;
        }

        public async Task UpdateJobObservation(JobObservation jobObservation)
        {
            var response = await _http.PutAsJsonAsync($"jobObservations/{jobObservation.JobObservationId}", jobObservation);
        }

    }
}
