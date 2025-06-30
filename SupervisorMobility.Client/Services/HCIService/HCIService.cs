
using DocumentFormat.OpenXml.Office2010.Excel;
using Microsoft.JSInterop;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using static System.Net.WebRequestMethods;

namespace SupervisorMobility.Client.Services.HCIService
{
    public class HCIService : IHCIService
    {
        private readonly HttpClient _http;
        private readonly JsonSerializerOptions _options;

        public HCIService(HttpClient http)
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
            content.User = null;
            var response = await _http.PostAsJsonAsync($"HCI", content);

            return response.IsSuccessStatusCode;
        }

        public async Task<HCI> GetHCI(int id)
        {
            var response = await _http.GetAsync($"HCI/{id}?includeNavigation=true&includePeople=true");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadFromJsonAsync<HCI>();
                return content;
            }

            return null;
        } 
        
        public async Task<List<User>> GetUsersWithoutHCI()
        {
            var response = await _http.GetAsync($"HCI/NoHciUsers");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadFromJsonAsync<List<User>>();
                return content;
            }

            return null;
        }

        public async Task<List<HCI>> GetHCIs(int LoginUserId, bool includeNavigation = false, bool includePeople = false, bool includeComments = false, bool includeTransactions = false)
        {
            var response = await _http.GetAsync($"HCI/?LoginUserId={LoginUserId}&includeNavigation={includeNavigation}&includePeople={includePeople}&includeTransactions={includeTransactions}&includeComments={includeComments}");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadFromJsonAsync<List<HCI>>();
                return content;
            }

            return null;
        }

        public async Task<bool> UpdateHCI(HCI content)
        {
            var response = await _http.PutAsJsonAsync($"HCI/{content.HCIId}", content);

            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteHCI(int hciId)
        {
            var response = await _http.DeleteAsync($"HCI/{hciId}");

            return response.IsSuccessStatusCode;
        }

        public Task<List<HCICategory>> GetCategories()
        {
            throw new NotImplementedException();
        }

        public Task<HCICategory> GetCategorieById(int id)
        {
            throw new NotImplementedException();
        }

        public Task<HCICategory> CreateCategorie(HCICategory Categorie)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateCategorie(HCICategory Categorie)
        {
            throw new NotImplementedException();
        }

        public Task DeleteCategorie(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<List<HCICategory>> GetHCICategories()
        {
            var response = await _http.GetAsync("HCI/Categories");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadFromJsonAsync<List<HCICategory>>();
                return content;
            }
            return null;
        }
    }
}
