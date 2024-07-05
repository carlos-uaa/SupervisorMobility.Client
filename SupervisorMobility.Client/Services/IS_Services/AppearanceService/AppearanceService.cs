using Microsoft.JSInterop;
using SupervisorMobility.Client.Data.Entities.IS;
using SupervisorMobility.Client.Services.IS_Services.AppearanceService;
using System.Net.Http.Json;

namespace SupervisorMobility.Client.Services.IS_Services.AppearanceService
{
    public class AppearanceService : IAppearanceService
    {
        private readonly HttpClient _http;
        private readonly JsonSerializerOptions _options;
        private readonly IJSRuntime _js;

        // Constructor
        public AppearanceService(HttpClient customHttpClientService, IJSRuntime jSRuntime)
        {
            _http = customHttpClientService;
            _js = jSRuntime;
            _options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        }

        public async Task<Appearance?> CreateAppearance(Appearance appearanceToCreate)
        {
            var response = await _http.PostAsJsonAsync($"IS/Appearance", appearanceToCreate);
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new ApplicationException(content);
            }

            var appearance = JsonSerializer.Deserialize<Appearance>(content, _options);

            return appearance;

        }
        public async Task<Appearance?> GetAppearance(int appearance_id, bool includeDataPanelItems = false, bool includeProblemDefectItems = false, bool includeLogBookAppearance = false)
        {
            var response = await _http.GetAsync($"IS/Appearance/{appearance_id}?includeDataPanelItems={includeDataPanelItems}&includeProblemDefectItems={includeProblemDefectItems}&includeLogBookAppearance={includeLogBookAppearance}");
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new ApplicationException(content);
            }

            var appearance = JsonSerializer.Deserialize<Appearance>(content, _options);

            return appearance;
        }


        public async Task<List<Appearance>> GetAllAppearances(bool includeDataPanelItems = false, bool includeProblemDefectItems = false, bool includeLogBookAppearance = false)
        {
            var response = await _http.GetAsync($"IS/Appearance?includeDataPanelItems={includeDataPanelItems}&includeProblemDefectItems={includeProblemDefectItems}&includeLogBookAppearance={includeLogBookAppearance}");
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new ApplicationException(content);
            }

            var appearances = JsonSerializer.Deserialize<List<Appearance>>(content, _options);

            return appearances;
        }

        public async Task<Appearance?> UpdateAppearance(Appearance appearanceToUpdate)
        {
            var response = await _http.PutAsJsonAsync($"IS/Appearance/{appearanceToUpdate.AppearanceId}", appearanceToUpdate);
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            var appearance = JsonSerializer.Deserialize<Appearance>(content, _options);

            return appearance;
        }
        public async Task<bool> DeleteAppearance(int appearance_id)
        {
            var response = await _http.DeleteAsync($"IS/Appearance/{appearance_id}");
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                return false;
            }

            return true;
        }

    }
}
