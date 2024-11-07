using AutoMapper;
using Microsoft.JSInterop;
using SupervisorMobility.Client.Data.Entities;
using SupervisorMobility.Client.Data.Entities.PaginationEntities;
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

        public async Task<JobObservation> CreateJobObservationWithLup(JobObservation _jobObservationWithLup)
        {
            var response = await _http.PostAsJsonAsync($"jobobservations/WithLup", _jobObservationWithLup);
            var newJobObservation = await response.Content.ReadFromJsonAsync<JobObservationNulls>();

            return _mapper.Map<JobObservation>(newJobObservation);
        }

        public async Task DeleteJobObservation(int jobObservationId)
        {
            var response = await _http.DeleteAsync($"jobobservations/{jobObservationId}");
        }

        public async Task<List<JobObservation>> GetAllJobObservations(bool includeTree = false, bool includePeople = false, bool includeLup = false, 
            bool includeHistory = false, bool includeCkAnswers = false, int idPlant = 0, int idArea = 0, bool ForSosProgram = false,
            int year = 0, int month = 0, int SOSAnualId = 0, int idUser = 0)
        {
            var response = await _http.GetAsync($"jobobservations?includeTree={includeTree}&includePeople={includePeople}&includeLup={includeLup}&includeHistory={includeHistory}&includeCkAnswers={includeCkAnswers}&idPlant={idPlant}&idArea={idArea}&ForSosProgram={ForSosProgram}&year={year}&month={month}&SOSAnualId={SOSAnualId}&idUser={idUser}");
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new ApplicationException(content);
            }

            var jobObservation = JsonSerializer.Deserialize<List<JobObservationNulls>>(content, _options);

            return _mapper.Map<List<JobObservation>>(jobObservation);
        }
          public async Task<List<JobObservation>> GetAllNextYearJobsObservations(int plantId, int areaId, int year)
        {
            var response = await _http.GetAsync($"jobobservations/NextYear?plantId={plantId}&areaId={areaId}&year={year}");
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new ApplicationException(content);
            }

            var jobObservation = JsonSerializer.Deserialize<List<JobObservationNulls>>(content, _options);

            return _mapper.Map<List<JobObservation>>(jobObservation);
        }

        public async Task<(int Total, List<JobObservation>JobObservations, JOCountPaginationDto Count)> GetAllJobObservationsByFilters(DateTime startDate, DateTime endDate, int JobObsId, int plantId,
            int areaId, int distributionId, int operationId, int supervisorId, int status, int userId, int typeId,
            string searchString, int page, int entries, int? sortO, string? sortL)
        {
            var response = await _http.GetFromJsonAsync<JOPaginationDto>($"jobobservations/filters?startDate={startDate}&endDate={endDate}&jobObsId={JobObsId}&plantId={plantId}&areaId={areaId}&distributionId={distributionId}&operationId={operationId}&supervisorId={supervisorId}&status={status}&userId={userId}&typeId={typeId}&searchString={searchString}&page={page}&entries={entries}&sortO={sortO}&sortL={sortL}");
            return (response.Total, _mapper.Map<List<JobObservation>>(response.JobObservations), response.CountPagination);
        }

        public async Task<List<JobObservationHistoryVersion>> GetAllHistoryJobObservations(int jobObservationId)
        {
            var response = await _http.GetAsync($"jobobservations/{jobObservationId}/history");
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new ApplicationException(content);
            }

            var historyJobObservation = JsonSerializer.Deserialize<List<JobObservationHistoryVersion>>(content, _options);

            return historyJobObservation;
        }


        public async Task<JobObservationHistoryVersion> GetOneHistoryJobObservation(int jobObservationId, int HistoryId)
        {
            var response = await _http.GetAsync($"jobobservations/{jobObservationId}/history/{HistoryId}/detail");
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new ApplicationException(content);
            }

            var jobObservation = JsonSerializer.Deserialize<JobObservationHistoryVersion>(content, _options);

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


        public async Task<JobObservation> GetJobObservationById(int jobObservationId, bool includeTree = false, bool includePeople = false, bool includeLup = false, bool includeHistory = false, bool includeCkAnswers = false)
        {
            var response = await _http.GetAsync($"jobobservations/{jobObservationId}?includeTree={includeTree}&includePeople={includePeople}&includeLup={includeLup}&includeHistory={includeHistory}&includeCkAnswers={includeCkAnswers}");
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                return null;
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

        public async Task<JobObservation> CreateOperatorSignature(MultipartFormDataContent operatorSignature)
        {
            var response = await _http.PostAsync("jobobservations/operatorSignature", operatorSignature);

            var newJobObservation = await response.Content.ReadFromJsonAsync<JobObservationNulls>();

            return _mapper.Map<JobObservation>(newJobObservation);
        }

    }
}
