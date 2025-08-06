using Microsoft.JSInterop;
using SupervisorMobility.Client.Data.Helpers;
using System.Net.Http.Json;

namespace SupervisorMobility.Client.Services.MetricsService
{
    public class MetricsService : IMetricsService
    {
        private readonly HttpClient _http;
        private readonly JsonSerializerOptions _options;

        // Constructor
        public MetricsService(HttpClient customHttpClientService)
        {
            _http = customHttpClientService;
            _options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        }

        public async Task<int> GetTotalJobObs(MetricsFiltersDto filters)
        {
            string urlQueryParams = $"?{QueryHelper.ToQueryString(filters)}";

            var response = await _http.GetAsync($"reports/totaljobs{urlQueryParams}");
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new ApplicationException(content);
            }

            int total = JsonSerializer.Deserialize<int>(content, _options);

            return total;
        }

        public async Task<Dictionary<string, int>> GetJobsStatusChartData(MetricsFiltersDto filters)
        {
            string urlQueryParams = $"?{QueryHelper.ToQueryString(filters)}";

            var response = await _http.GetAsync($"reports/jobsstatusdata{urlQueryParams}");
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new ApplicationException(content);
            }

            Dictionary<string, int> total = JsonSerializer.Deserialize<Dictionary<string, int>>(content, _options)!;

            return total;
        }

        public async Task<Dictionary<string, int>> GetJobsTypeChartData(MetricsFiltersDto filters)
        {
            string urlQueryParams = $"?{QueryHelper.ToQueryString(filters)}";

            var response = await _http.GetAsync($"reports/jobstypedata{urlQueryParams}");
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new ApplicationException(content);
            }

            Dictionary<string, int> total = JsonSerializer.Deserialize<Dictionary<string, int>>(content, _options)!;

            return total;
        }

        public async Task<int> GetTotalLUPs(MetricsFiltersDto filters)
        {
            string urlQueryParams = $"?{QueryHelper.ToQueryString(filters)}";

            var response = await _http.GetAsync($"reports/totallups{urlQueryParams}");
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new ApplicationException(content);
            }

            int total = JsonSerializer.Deserialize<int>(content, _options);

            return total;
        }

        public async Task<Dictionary<string, int>> GetLUPData(MetricsFiltersDto filters)
        {
            string urlQueryParams = $"?{QueryHelper.ToQueryString(filters)}";

            var response = await _http.GetAsync($"reports/lupdata{urlQueryParams}");
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new ApplicationException(content);
            }

            Dictionary<string, int> total = JsonSerializer.Deserialize<Dictionary<string, int>>(content, _options)!;

            return total;
        }

        public async Task<Dictionary<string, int>> GetLUPProgressData(MetricsFiltersDto filters)
        {
            string urlQueryParams = $"?{QueryHelper.ToQueryString(filters)}";

            var response = await _http.GetAsync($"reports/lupProgressData{urlQueryParams}");
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new ApplicationException(content);
            }

            Dictionary<string, int> total = JsonSerializer.Deserialize<Dictionary<string, int>>(content, _options)!;

            return total;
        }
    }

    public class MetricsFiltersDto
    {
        public int? plantId { get; set; }
        public int? areaId { get; set; }
        public int? distributionId { get; set; }
        public int? operationId { get; set; }
        public DateTime? inferiorDate { get; set; }
        public DateTime? superiorDate { get; set; }
        public DateTime? today { get; set; }
    }
}
