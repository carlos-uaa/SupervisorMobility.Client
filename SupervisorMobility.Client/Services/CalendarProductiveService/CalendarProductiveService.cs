using Microsoft.JSInterop;
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

        public List<Holiday> GetHolidaysInService()
        {
            return this.holidays;
        }
        public bool AddHolidaysInService(List<Holiday> toAdd)
        {

            return true;
        }
        
        public bool AddHolidayToService(Holiday toAdd)
        {
            this.holidays.Add(toAdd);
            return true;
        }
        public bool UpdateHolidayInService(Holiday toUpdate)
        {

            return false;
        }


    }
}
