using Microsoft.JSInterop;
using System.Net.Http.Json;

namespace SupervisorMobility.Client.Services.AttendanceService
{
    public class AttendanceService : IAttendanceService
    {

        private readonly HttpClient _http;
        private readonly JsonSerializerOptions _options;
        private readonly IJSRuntime _js;

        // Constructor
        public AttendanceService(HttpClient customHttpClientService, IJSRuntime jSRuntime)
        {
            _http = customHttpClientService;
            _js = jSRuntime;
            _options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        }
        public async Task<List<Attendance>> GetAttendanceList(int superiorId)
        {
            try
            {
                var response = await _http.GetAsync($"Attendance?idsuperior={superiorId}");
                var content = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    //var result = await response.Content.ReadFromJsonAsync<List<Attendance>>();
                    var attendanceslist = JsonSerializer.Deserialize<List<Attendance>>(content, _options);

                    return attendanceslist;
                }
                else
                {
                    await _js.InvokeVoidAsync("alert", $"Error Bar Result: {response.Content.ReadAsStringAsync().Result}");
                }
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Error al hacer la solicitud: {ex.Message}");
            }
            catch (TaskCanceledException ex)
            {
                Console.WriteLine($"La solicitud ha sido cancelada: {ex.Message}");
            }

            return null;

        }
        public async Task<List<Attendance>> AssignEmployes()
        {
        
            try
            {
                var response = await _http.GetAsync($"Attendance/Assign");
                var content = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    //var result = await response.Content.ReadFromJsonAsync<List<Attendance>>();
                    var attendanceslist = JsonSerializer.Deserialize<List<Attendance>>(content, _options);

                    return attendanceslist;
                }
                else
                {
                    await _js.InvokeVoidAsync("alert", $"Error Bar Result: {response.Content.ReadAsStringAsync().Result}");
                }
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Error al hacer la solicitud: {ex.Message}");
            }
            catch (TaskCanceledException ex)
            {
                Console.WriteLine($"La solicitud ha sido cancelada: {ex.Message}");
            }

            return null;


        }
        public async Task<List<Attendance>> UpdateList(List<Attendance> list)
        {
           
            try
            {
                var response = await _http.PostAsJsonAsync($"Attendance/updatelist", list);

                var content = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    //var result = await response.Content.ReadFromJsonAsync<List<Attendance>>();
                    var attendanceslist = JsonSerializer.Deserialize<List<Attendance>>(content, _options);

                    return attendanceslist;
                }
                else
                {
                    await _js.InvokeVoidAsync("alert", $"Error Bar Result: {response.Content.ReadAsStringAsync().Result}");
                }
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Error al hacer la solicitud: {ex.Message}");
            }
            catch (TaskCanceledException ex)
            {
                Console.WriteLine($"La solicitud ha sido cancelada: {ex.Message}");
            }

            return null;
        }
    }
    
}
