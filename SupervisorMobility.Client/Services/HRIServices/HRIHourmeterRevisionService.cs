using SupervisorMobility.Client.Data;
using SupervisorMobility.Client.Data.Entities.Dtos;
using SupervisorMobility.Client.Data.Entities.Dtos.HRIDtos.HRIHourmeterRevisionDto;
using SupervisorMobility.Client.Data.Entities.Dtos.HRIRevisionItemsDtos;
using System.Net.Http.Json;

namespace SupervisorMobility.Client.Services.HRIServices
{
    public class HRIHourmeterRevisionService : IHRIHourmeterRevisionService
    {
        private readonly HttpClient _http;

        public HRIHourmeterRevisionService(HttpClient http)
        {
            _http = http;
        }

        public async Task<ServiceResponse<List<GetHourmeterRevisionDto>>> GetAllHourmeterRevisions()
        {
            var response = await _http.GetFromJsonAsync<ServiceResponse<List<GetHourmeterRevisionDto>>>("HRIHourmeterRevision/GetAllHourmeterRevisions");

            return response ?? new ServiceResponse<List<GetHourmeterRevisionDto>> { Success = false, Message = "Error en la petición" };
        }

        public async Task<ServiceResponse<GetHourmeterRevisionDto>> GetHourmeterRevisionByHRIId(int hriId)
        {
            var response = await _http.GetFromJsonAsync<ServiceResponse<GetHourmeterRevisionDto>>($"HRIHourmeterRevision/GetHourmeterRevisionByHRIId/{hriId}");

            return response ?? new ServiceResponse<GetHourmeterRevisionDto> { Success = false, Message = "Error en la petición" };
        }

        public async Task<ServiceResponse<GetHourmeterRevisionDto>> GetHourmeterRevisionById(int id)
        {
            var response = await _http.GetFromJsonAsync<ServiceResponse<GetHourmeterRevisionDto>>($"HRIHourmeterRevision/GetHourmeterRevisionById/{id}");

            return response ?? new ServiceResponse<GetHourmeterRevisionDto> { Success = false, Message = "Error en la petición" };
        }

        public async Task<ServiceResponse<GetHourmeterRevisionDto>> AddHourmeterRevision(CreateHourMeterRevisionDto newHourmeterRevision)
        {
            var httpResponse = await _http.PostAsJsonAsync("HRIHourmeterRevision/AddHourmeterRevision", newHourmeterRevision);
            var response = await httpResponse.Content.ReadFromJsonAsync<ServiceResponse<GetHourmeterRevisionDto>>();

            return response ?? new ServiceResponse<GetHourmeterRevisionDto> { Success = false, Message = "Error en la petición" };
        }

        public async Task<ServiceResponse<bool>> AddDailyRevisionToHourmeterRevision(CreateDailyRevisionDto createDaily)
        {
            var httpResponse = await _http.PostAsJsonAsync("HRIHourmeterRevision/AddDailyRevisionToHourmeterRevision", createDaily);
            var response = await httpResponse.Content.ReadFromJsonAsync<ServiceResponse<bool>>();

            return response ?? new ServiceResponse<bool> { Success = false, Message = "Error en la petición" };
        }

        public async Task<ServiceResponse<bool>> DeleteHourmeterRevision(int id)
        {
            var httpResponse = await _http.DeleteAsync($"HRIHourmeterRevision/DeleteHourmeterRevision/{id}");
            var response = await httpResponse.Content.ReadFromJsonAsync<ServiceResponse<bool>>();

            return response ?? new ServiceResponse<bool> { Success = false, Message = "Error en la petición" };
        }
    }
}
