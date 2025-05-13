using Microsoft.JSInterop;
using SupervisorMobility.Client.Data.Entities;
using System.Net.Http.Json;

namespace SupervisorMobility.Client.Services.CalendarProductiveService
{
    public class CalendarProductiveService : ICalendarProductiveService
    {
        private readonly HttpClient _http;
        private readonly JsonSerializerOptions _options;

        public List<Holiday> holidays { get; set; } = new List<Holiday>();

        // Constructor
        public CalendarProductiveService(HttpClient customHttpClientService, IJSRuntime jSRuntime)
        {
            _http = customHttpClientService;
            _options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        }

        public async Task<List<Holiday>> GetHolidaysInService(int year)
        {
            if(this.holidays.Any(d=> d.Date.Year == year))
                return this.holidays.Where(d => d.Date.Year == year).ToList();

            await GetHolidaysInYear(year);
            
            return this.holidays.Where(d => d.Date.Year == year).ToList();
        }
       
        
        public bool AddHolidayToService(Holiday toAdd)
        {
            this.holidays.Add(toAdd);
            return true;
        }
         

        public async Task<List<Holiday>> GetHolidaysInYear(int year)
            {
            var response = await _http.GetAsync($"ProductiveCalendar/GetHolidays/{year}");
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new ApplicationException(content);
            }

            var holidays = JsonSerializer.Deserialize<List<Holiday>>(content, _options);

            this.holidays = holidays;

            return holidays;
        }
        public async Task<List<Holiday>> UpdateHolidaysInYear(int year, List<Holiday>? holiday = null)
        {

            var response = await _http.PutAsJsonAsync($"ProductiveCalendar/UpdateOrCreateHolidays/{year}",  holiday != null ? holiday : this.holidays);
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new ApplicationException(content);
            }

            var holidays = JsonSerializer.Deserialize<List<Holiday>>(content, _options);

            this.holidays = holidays;

            return holidays;
        }

    }
}
