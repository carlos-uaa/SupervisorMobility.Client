
using Microsoft.JSInterop;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using static System.Net.WebRequestMethods;

namespace SupervisorMobility.Client.Services.HCIService
{
    public class HCIService : IHCIService
    {
        private readonly HttpClient _http;
        private readonly JsonSerializerOptions _options;

        HCIService(HttpClient http)
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

        public async Task<bool> CreateHCI(HCI content)
        {
            var response = await _http.PostAsJsonAsync($"/HCI", content);

            return response.IsSuccessStatusCode;
        }

        public async Task<List<HCI>> GetHCI(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<List<HCI>> GetHCIs(bool includeNavigation = false, bool includePeople = false, bool includeEvidences = false, bool includeTransactions = false)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> UpdateHCI(HCI content)
        {
            throw new NotImplementedException();
        }
    }
}
