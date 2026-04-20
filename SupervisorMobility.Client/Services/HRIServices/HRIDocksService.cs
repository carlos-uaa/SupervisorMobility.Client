using SupervisorMobility.Client.Data;
using SupervisorMobility.Client.Data.Entities.Hri;
using System.Net.Http.Json;

namespace SupervisorMobility.Client.Services.HRIServices
{
    public class HRIDocksService : IHRIDocksService
    {
        private readonly HttpClient _httpClient;

        public HRIDocksService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<ServiceResponse<List<HRIDock>>> GetAllHRIDocksAsync()
        {
            try
            {
                var response = await _httpClient.GetFromJsonAsync<ServiceResponse<List<HRIDock>>>("HRIDocks/GetAllHRIDocksAsync");
                return response!;
            }
            catch (Exception ex)
            {
                return new ServiceResponse<List<HRIDock>>
                {
                    Success = false,
                    Message = $"Error getting all HRI docks: {ex.Message}"
                };
            }
        }

        public async Task<ServiceResponse<HRIDock>> GetSingleHRIDockAsync(int Id)
        {
            try
            {
                var response = await _httpClient.GetFromJsonAsync<ServiceResponse<HRIDock>>($"HRIDocks/GetSingleHRIDockAsync/{Id}");
                return response!;
            }
            catch (Exception ex)
            {
                return new ServiceResponse<HRIDock>
                {
                    Success = false,
                    Message = $"Error getting HRI dock with ID {Id}: {ex.Message}"
                };
            }
        }

        public async Task<ServiceResponse<HRIDock>> CreateHRIDockAsync(HRIDock Line)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("HRIDocks/CreateHRIDockAsync", Line);
                return await response.Content.ReadFromJsonAsync<ServiceResponse<HRIDock>>() ?? new ServiceResponse<HRIDock>();
            }
            catch (Exception ex)
            {
                return new ServiceResponse<HRIDock>
                {
                    Success = false,
                    Message = $"Error creating HRI dock: {ex.Message}"
                };
            }
        }

        public async Task<ServiceResponse<HRIDock>> UpdateHRIDockAsync(HRIDock Line)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync("HRIDocks/UpdateHRIDockAsync", Line);
                return await response.Content.ReadFromJsonAsync<ServiceResponse<HRIDock>>() ?? new ServiceResponse<HRIDock>();
            }
            catch (Exception ex)
            {
                return new ServiceResponse<HRIDock>
                {
                    Success = false,
                    Message = $"Error updating HRI dock: {ex.Message}"
                };
            }
        }

        public async Task<ServiceResponse<bool>> DeleteHRIDockAsync(int Id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"HRIDocks/DeleteHRIDockAsync/{Id}");
                return await response.Content.ReadFromJsonAsync<ServiceResponse<bool>>() ?? new ServiceResponse<bool>();
            }
            catch (Exception ex)
            {
                return new ServiceResponse<bool>
                {
                    Success = false,
                    Message = $"Error deleting HRI dock with ID {Id}: {ex.Message}"
                };
            }
        }
    }
}
