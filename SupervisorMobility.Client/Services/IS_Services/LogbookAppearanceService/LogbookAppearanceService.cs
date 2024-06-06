using Microsoft.JSInterop;
using SupervisorMobility.Client.Data.Entities.IS;
using System.Net.Http.Json;

namespace SupervisorMobility.Client.Services.IS_Services.LogbookAppearanceService
{
    public class LogbookAppearanceService : ILogbookAppearanceService
    {
        private readonly HttpClient _http;
        private readonly JsonSerializerOptions _options;
        private readonly IJSRuntime _js;

        // Constructor
        public LogbookAppearanceService(HttpClient customHttpClientService, IJSRuntime jSRuntime)
        {
            _http = customHttpClientService;
            _js = jSRuntime;
            _options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        }

        public async Task<LogbookAppearance?> CreateLogbookAppearance(LogbookAppearance logbookLogbookAppearanceToCreate)
        {
            var response = await _http.PostAsJsonAsync($"IS/Appearance/Logbook", logbookLogbookAppearanceToCreate);
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new ApplicationException(content);
            }

            var logbookLogbookAppearance = JsonSerializer.Deserialize<LogbookAppearance>(content, _options);

            return logbookLogbookAppearance;

        }
        public async Task<LogbookAppearance?> GetLogbookAppearance(int logbookLogbookAppearance_id, bool includePanelResults = false, bool includeProblemDefectResults = false)
        {
            var response = await _http.GetAsync($"IS/Appearance/Logbook/{logbookLogbookAppearance_id}?includePanelResults={includePanelResults}?includeProblemDefectResults={includeProblemDefectResults}");
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new ApplicationException(content);
            }

            var logbookLogbookAppearance = JsonSerializer.Deserialize<LogbookAppearance>(content, _options);

            return logbookLogbookAppearance;
        }


        public async Task<List<LogbookAppearance>> GetAllLogbookAppearances(bool includePanelResults = false, bool includeProblemDefectResults = false)
        {
            var response = await _http.GetAsync($"IS/Appearance/Logbook?includePanelResults={includePanelResults}?includeProblemDefectResults={includeProblemDefectResults}");
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new ApplicationException(content);
            }

            var logbookLogbookAppearances = JsonSerializer.Deserialize<List<LogbookAppearance>>(content, _options);

            return logbookLogbookAppearances;
        }

        public async Task<LogbookAppearance?> UpdateLogbookAppearance(LogbookAppearance logbookLogbookAppearanceToUpdate)
        {
            var response = await _http.PutAsJsonAsync($"IS/Appearance/Logbook/{logbookLogbookAppearanceToUpdate.LogbookAppearanceId}", logbookLogbookAppearanceToUpdate);
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            var logbookLogbookAppearance = JsonSerializer.Deserialize<LogbookAppearance>(content, _options);

            return logbookLogbookAppearance;
        }
        public async Task<bool> DeleteLogbookAppearance(int logbookLogbookAppearance_id)
        {
            var response = await _http.DeleteAsync($"IS/Appearance/Logbook/{logbookLogbookAppearance_id}");
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                return false;
            }

            return true;
        }

    }
}
