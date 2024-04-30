using static System.Net.WebRequestMethods;
using System.Text.Json.Serialization;

namespace SupervisorMobility.Client.Services.SOS_Data_Service
{
    public class SOSDataService : ISOSDataService
    {
        private readonly HttpClient _http;
        private readonly JsonSerializerOptions _options;

        private List<JobObservationNulls> _SuggestionJobs = new List<JobObservationNulls>();
        private List<JobObservationNulls> _SosJobs = new List<JobObservationNulls>();

        public event Action<List<JobObservation>> JobsChanges;
        public event Action<List<JobObservationNulls>> SuggestionJobsChanged;

        public List<JobObservationNulls> SuggestionJobs => _SuggestionJobs;
        public List<JobObservationNulls> SosJobs => _SosJobs;


        public SOSDataService(HttpClient http)
        {
            _http = http;
            _options = new JsonSerializerOptions
            {
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault,
                PropertyNameCaseInsensitive = true,
                NumberHandling = JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.WriteAsString
            };
            _options.Converters.Add(new IntToStringConverter());
        }

        public void SetSosJobObservation(int sos)
        {
            throw new NotImplementedException();
        }

        public void SetSugestionJobObservation()
        {
            throw new NotImplementedException();
        }

        public List<JobObservationNulls> GetSosByMonth(int month)
        {
            throw new NotImplementedException();
        }

        public List<JobObservationNulls> GetSosSuggestionByMonth(int month)
        {
            throw new NotImplementedException();
        }

        public List<JobObservationNulls> GetSosByDistribution(int dis_Id)
        {
            throw new NotImplementedException();
        }

        public List<JobObservationNulls> GetSosSuggestionByDistribution(int dis_Id)
        {
            throw new NotImplementedException();
        }
    }
}
