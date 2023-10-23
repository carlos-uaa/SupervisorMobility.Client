using AutoMapper;
using Microsoft.JSInterop;
using SupervisorMobility.Client.Data.Entities;
using System.Net.Http.Json;
using System.Text.Json.Serialization;

namespace SupervisorMobility.Client.Services.JobObservationService
{
    public class JobObservationService : IJobObservationService
    {
        private readonly HttpClient _http;
        private readonly IMapper _mapper;
        private readonly JsonSerializerOptions _options;

        // Constructor
        public JobObservationService(IMapper mapper, HttpClient customHttpClientService, IJSRuntime jSRuntime)
        {
            _mapper = mapper;
            _http = customHttpClientService;
            _options = new JsonSerializerOptions
            {
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault,
                PropertyNameCaseInsensitive = true,
                NumberHandling = JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.WriteAsString
            };
        }

        public async Task<JobObservation> CreateJobObservation(JobObservation _jobObservation)
        {
            var response = await _http.PostAsJsonAsync($"jobobservations", _jobObservation);
            var newJobObservation = await response.Content.ReadFromJsonAsync<JobObservationNulls>();

            return  _mapper.Map<JobObservation>(newJobObservation);
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

            var jobObservation = JsonSerializer.Deserialize<List<JobObservationNulls>>(content, _options);

            return _mapper.Map<List<JobObservation>>(jobObservation);
        }
        public async Task<List<JobObservationVersion>> GetAllHistoryJobObservations(int jobObservationId)
        {
            var response = await _http.GetAsync($"jobobservations/{jobObservationId}/history");
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new ApplicationException(content);
            }

            var historyJobObservation = JsonSerializer.Deserialize<List<JobObservationVersion>>(content, _options);

            return historyJobObservation;
        }


        public async Task<JobObservationVersion> GetOneHistoryJobObservation(int jobObservationId, int HistoryId)
        {
            var response = await _http.GetAsync($"jobobservations/{jobObservationId}/history/{HistoryId}/detail");
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new ApplicationException(content);
            }

            var jobObservation = JsonSerializer.Deserialize<JobObservationVersion>(content, _options);

            return jobObservation;
        }

        public async Task<List<JobObservation>> GetAllJobObservationsWithLup()
        {
            var response = await _http.GetAsync($"jobobservations?IncludeLup=true");
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new ApplicationException(content);
            }

            var jobObservation = JsonSerializer.Deserialize<List<JobObservationNulls>>(content, _options);

            return _mapper.Map<List<JobObservation>>(jobObservation); ;
        }

        public async Task<JobObservation> GetJobObservationWithLup(int jobObservationId)
        {
            var response = await _http.GetAsync($"jobobservations/{jobObservationId}?IncludeLup=true");
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new ApplicationException(content);
            }

            var jobObservation = JsonSerializer.Deserialize<JobObservationNulls>(content, _options);

            return _mapper.Map<JobObservation>(jobObservation);
        }


        public async Task<JobObservation> GetJobObservationById(int jobObservationId)
        {
            var response = await _http.GetAsync($"jobobservations/{jobObservationId}");
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new ApplicationException(content);
            }

            var jobObservation = JsonSerializer.Deserialize<JobObservationNulls>(content, _options);

            return _mapper.Map<JobObservation>(jobObservation); ;
        }

        public async Task<bool> UpdateJobObservation(JobObservation jobObservation, string loggedUser)
        {
            RequestJobObservationADuser requestJobObservationADuser = new RequestJobObservationADuser() { 
                JobObservation = jobObservation,
                LoggedUser = loggedUser
            };



        var response = await _http.PutAsJsonAsync($"jobobservations/{jobObservation.JobObservationId}", requestJobObservationADuser);

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return true;
            }

            return false;
        }

}
}
