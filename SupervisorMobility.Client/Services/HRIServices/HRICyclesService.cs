using SupervisorMobility.Client.Data;
using SupervisorMobility.Client.Data.Entites.Dtos.HRICyclesDtos;
using SupervisorMobility.Client.Data.Entities.Dtos.HRIRevisionItemsDtos;
using System.Net.Http.Json;

namespace SupervisorMobility.Client.Services.HRIServices
{
    public class HRICyclesService : IHRICyclesService
    {
        private readonly HttpClient _http;

        public HRICyclesService(HttpClient http)
        {
            _http = http;
        }

        public async Task<ServiceResponse<List<GetHRICyclesDto>>> GetAllHRICycles()
        {
            try
            {
                return await _http.GetFromJsonAsync<ServiceResponse<List<GetHRICyclesDto>>>("HRICycles/GetAllHRICycles");
            }
            catch (Exception ex)
            {
                return new ServiceResponse<List<GetHRICyclesDto>>
                {
                    Success = false,
                    Message = $"An error occurred: {ex.Message}",
                    Data = null
                };
            }
        }

        public async Task<ServiceResponse<GetHRICyclesDto>> GetHRICycleById(int id)
        {
            try
            {
                return await _http.GetFromJsonAsync<ServiceResponse<GetHRICyclesDto>>($"HRICycles/GetHRICycleById/{id}");
            }
            catch (Exception ex)
            {
                return new ServiceResponse<GetHRICyclesDto>
                {
                    Success = false,
                    Message = $"An error occurred: {ex.Message}",
                    Data = null
                };
            }
        }

        public async Task<ServiceResponse<GetHRICyclesDto>> CreateHRICycle(CreateHRICyclesDto dto)
        {
            try
            {
                var response = await _http.PostAsJsonAsync("HRICycles/CreateHRICycle", dto);
                return await response.Content.ReadFromJsonAsync<ServiceResponse<GetHRICyclesDto>>();
            }
            catch (Exception ex)
            {
                return new ServiceResponse<GetHRICyclesDto>
                {
                    Success = false,
                    Message = $"An error occurred: {ex.Message}",
                    Data = null
                };
            }
        }

        public async Task<ServiceResponse<bool>> CreateNewDailyRevision(CreateDailyRevisionDto createDaily)
        {
            try
            {
                var response = await _http.PostAsJsonAsync("HRICycles/CreateNewDailyRevision", createDaily);
                return await response.Content.ReadFromJsonAsync<ServiceResponse<bool>>();
            }
            catch (Exception ex)
            {
                return new ServiceResponse<bool>
                {
                    Success = false,
                    Message = $"An error occurred: {ex.Message}",
                    Data = false
                };
            }
        }

        public async Task<ServiceResponse<bool>> DeleteHRICycle(int id)
        {
            try
            {
                var response = await _http.DeleteAsync($"HRICycles/DeleteHRICycle/{id}");
                return await response.Content.ReadFromJsonAsync<ServiceResponse<bool>>();
            }
            catch (Exception ex)
            {
                return new ServiceResponse<bool>
                {
                    Success = false,
                    Message = $"An error occurred: {ex.Message}",
                    Data = false
                };
            }
        }
    }
}
