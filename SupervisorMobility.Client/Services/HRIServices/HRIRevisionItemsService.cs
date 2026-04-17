using SupervisorMobility.Client.Data;
using SupervisorMobility.Client.Data.Entites.Dtos.HRIRevisionItemsDtos;
using System.Net.Http.Json;

namespace SupervisorMobility.Client.Services.HRIServices
{
    public class HRIRevisionItemsService : IHRIRevisionItemsService
    {
        private readonly HttpClient _http;

        public HRIRevisionItemsService(HttpClient http)
        {
            _http = http;
        }

        // Revision Items
        public async Task<ServiceResponse<List<GetHRIRevisionItemDto>>> GetAllHRIRevisionItems() =>
            await _http.GetFromJsonAsync<ServiceResponse<List<GetHRIRevisionItemDto>>>("api/HRIRevisionItem/GetAllHRIRevisionItems");

        public async Task<ServiceResponse<GetHRIRevisionItemDto>> GetHRIRevisionItemById(int id) =>
            await _http.GetFromJsonAsync<ServiceResponse<GetHRIRevisionItemDto>>($"api/HRIRevisionItem/GetHRIRevisionItemById/{id}");

        public async Task<ServiceResponse<GetHRIRevisionItemDto>> CreateHRIRevisionItem(CreateHRIRevisionItemDto dto)
        {
            var response = await _http.PostAsJsonAsync("api/HRIRevisionItem/CreateHRIRevisionItem", dto);
            return await response.Content.ReadFromJsonAsync<ServiceResponse<GetHRIRevisionItemDto>>();
        }

        public async Task<ServiceResponse<GetHRIRevisionItemDto>> UpdateHRIRevisionItem(int id, UpdateHRIRevisionItemDto dto)
        {
            var response = await _http.PutAsJsonAsync($"api/HRIRevisionItem/UpdateHRIRevisionItem/{id}", dto);
            return await response.Content.ReadFromJsonAsync<ServiceResponse<GetHRIRevisionItemDto>>();
        }

        public async Task<ServiceResponse<bool>> DeleteHRIRevisionItem(int id)
        {
            var response = await _http.DeleteAsync($"api/HRIRevisionItem/DeleteHRIRevisionItem/{id}");
            return await response.Content.ReadFromJsonAsync<ServiceResponse<bool>>();
        }

        // Frequencies
        public async Task<ServiceResponse<List<GetFrequencyDto>>> GetAllFrequencies() =>
            await _http.GetFromJsonAsync<ServiceResponse<List<GetFrequencyDto>>>("api/HRIRevisionItem/GetAllFrequencies");

        public async Task<ServiceResponse<GetFrequencyDto>> GetFrequencyById(int id) =>
            await _http.GetFromJsonAsync<ServiceResponse<GetFrequencyDto>>($"api/HRIRevisionItem/GetFrequencyById/{id}");

        public async Task<ServiceResponse<GetFrequencyDto>> CreateFrequency(CreateFrequencyDto dto)
        {
            var response = await _http.PostAsJsonAsync("api/HRIRevisionItem/CreateFrequency", dto);
            return await response.Content.ReadFromJsonAsync<ServiceResponse<GetFrequencyDto>>();
        }

        public async Task<ServiceResponse<GetFrequencyDto>> UpdateFrequency(int id, UpdateFrequencyDto dto)
        {
            var response = await _http.PutAsJsonAsync($"api/HRIRevisionItem/UpdateFrequency/{id}", dto);
            return await response.Content.ReadFromJsonAsync<ServiceResponse<GetFrequencyDto>>();
        }

        public async Task<ServiceResponse<bool>> DeleteFrequency(int id)
        {
            var response = await _http.DeleteAsync($"api/HRIRevisionItem/DeleteFrequency/{id}");
            return await response.Content.ReadFromJsonAsync<ServiceResponse<bool>>();
        }

        // Veredicts
        public async Task<ServiceResponse<List<GetVeredictDto>>> GetAllVeredicts() =>
            await _http.GetFromJsonAsync<ServiceResponse<List<GetVeredictDto>>>("api/HRIRevisionItem/GetAllVeredicts");

        public async Task<ServiceResponse<GetVeredictDto>> GetVeredictById(int id) =>
            await _http.GetFromJsonAsync<ServiceResponse<GetVeredictDto>>($"api/HRIRevisionItem/GetVeredictById/{id}");

        public async Task<ServiceResponse<GetVeredictDto>> CreateVeredict(CreateVeredictDto dto)
        {
            var response = await _http.PostAsJsonAsync("api/HRIRevisionItem/CreateVeredict", dto);
            return await response.Content.ReadFromJsonAsync<ServiceResponse<GetVeredictDto>>();
        }

        public async Task<ServiceResponse<GetVeredictDto>> UpdateVeredict(int id, UpdateVeredictDto dto)
        {
            var response = await _http.PutAsJsonAsync($"api/HRIRevisionItem/UpdateVeredict/{id}", dto);
            return await response.Content.ReadFromJsonAsync<ServiceResponse<GetVeredictDto>>();
        }

        public async Task<ServiceResponse<bool>> DeleteVeredict(int id)
        {
            var response = await _http.DeleteAsync($"api/HRIRevisionItem/DeleteVeredict/{id}");
            return await response.Content.ReadFromJsonAsync<ServiceResponse<bool>>();
        }

        // Revision Methods
        public async Task<ServiceResponse<List<GetRevisionMethodDto>>> GetAllRevisionMethods() =>
            await _http.GetFromJsonAsync<ServiceResponse<List<GetRevisionMethodDto>>>("api/HRIRevisionItem/GetAllRevisionMethods");

        public async Task<ServiceResponse<GetRevisionMethodDto>> GetRevisionMethodById(int id) =>
            await _http.GetFromJsonAsync<ServiceResponse<GetRevisionMethodDto>>($"api/HRIRevisionItem/GetRevisionMethodById/{id}");

        public async Task<ServiceResponse<GetRevisionMethodDto>> CreateRevisionMethod(CreateRevisionMethodDto dto)
        {
            var response = await _http.PostAsJsonAsync("api/HRIRevisionItem/CreateRevisionMethod", dto);
            return await response.Content.ReadFromJsonAsync<ServiceResponse<GetRevisionMethodDto>>();
        }

        public async Task<ServiceResponse<GetRevisionMethodDto>> UpdateRevisionMethod(int id, UpdateRevisionMethodDto dto)
        {
            var response = await _http.PutAsJsonAsync($"api/HRIRevisionItem/UpdateRevisionMethod/{id}", dto);
            return await response.Content.ReadFromJsonAsync<ServiceResponse<GetRevisionMethodDto>>();
        }

        public async Task<ServiceResponse<bool>> DeleteRevisionMethod(int id)
        {
            var response = await _http.DeleteAsync($"api/HRIRevisionItem/DeleteRevisionMethod/{id}");
            return await response.Content.ReadFromJsonAsync<ServiceResponse<bool>>();
        }

    }
}
