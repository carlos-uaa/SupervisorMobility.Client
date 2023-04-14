using Microsoft.JSInterop;
using System.Net.Http.Json;

namespace SupervisorMobility.Client.Services.AttendanceService
{
    public class AttendanceService : IAttendanceService
    {

        private readonly HttpClient _http;
        private readonly HttpClient _httpBridge;
        private readonly JsonSerializerOptions _options;
        private readonly IJSRuntime _js;

        // Constructor
        public AttendanceService(CustomHttpClientService customHttpClientService, IJSRuntime jSRuntime)
        {
            _http = customHttpClientService.GetApiHttpClient();
            _httpBridge = customHttpClientService.GetBridgeHttpClient();
            _js = jSRuntime;
            _options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        }
        public async Task<List<Attendance>> GetAttendanceList()
        {
            var response = await _http.GetAsync($"Attendance");
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new ApplicationException(content);
            }

            var attendanceslist = JsonSerializer.Deserialize<List<Attendance>>(content, _options);

            return attendanceslist;
        }
        public async Task<List<Attendance>> AssignEmployes()
        {
            var response = await _http.GetAsync($"Attendance/Assign");
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new ApplicationException(content);
            }

            var attendanceslist = JsonSerializer.Deserialize<List<Attendance>>(content, _options);

            return attendanceslist;
        }
        public async Task<List<Attendance>> UpdateList(List<Attendance> list)
        {
            var response = await _http.PostAsJsonAsync($"Attendance/updatelist", list);
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new ApplicationException(content);
            }

            var attendanceslist = JsonSerializer.Deserialize<List<Attendance>>(content, _options);

            return attendanceslist;
        }
    }
    
}
