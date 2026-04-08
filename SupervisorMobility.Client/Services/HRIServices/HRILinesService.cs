using SupervisorMobility.Client.Data;
using SupervisorMobility.Client.Data.Entities.Hri;
using System.Net.Http.Json;

namespace SupervisorMobility.Client.Services.HRIServices
{
    public class HRILinesService : IHRILinesService
    {
        private readonly HttpClient _httpClient;

        public HRILinesService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<ServiceResponse<List<HRILines>>> GetAllHRILinesAsync()
        {
            try
            {
                var response = await _httpClient.GetFromJsonAsync<ServiceResponse<List<HRILines>>>("api/HRILines/GetAllHRILinesAsync");
                return response!;
            }
            catch (Exception ex)
            {
                return new ServiceResponse<List<HRILines>>
                {
                    Success = false,
                    Message = $"Error getting all HRI lines: {ex.Message}"
                };
            }
        }

        public async Task<ServiceResponse<HRILines>> GetSingleHRILineAsync(int Id)
        {
            try
            {
                var response = await _httpClient.GetFromJsonAsync<ServiceResponse<HRILines>>($"api/HRILines/GetSingleHRILineAsync/{Id}");
                return response!;
            }
            catch (Exception ex)
            {
                return new ServiceResponse<HRILines>
                {
                    Success = false,
                    Message = $"Error getting HRI line with ID {Id}: {ex.Message}"
                };
            }
        }

        public async Task<ServiceResponse<HRILines>> CreateHRILineAsync(HRILines Line)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/HRILines/CreateHRILineAsync", Line);
                return await response.Content.ReadFromJsonAsync<ServiceResponse<HRILines>>() ?? new ServiceResponse<HRILines>();
            }
            catch (Exception ex)
            {
                return new ServiceResponse<HRILines>
                {
                    Success = false,
                    Message = $"Error creating HRI line: {ex.Message}"
                };
            }
        }

        public async Task<ServiceResponse<HRILines>> UpdateHRILineAsync(HRILines Line)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync("api/HRILines/UpdateHRILineAsync", Line);
                return await response.Content.ReadFromJsonAsync<ServiceResponse<HRILines>>() ?? new ServiceResponse<HRILines>();
            }
            catch (Exception ex)
            {
                return new ServiceResponse<HRILines>
                {
                    Success = false,
                    Message = $"Error updating HRI line: {ex.Message}"
                };
            }
        }

        public async Task<ServiceResponse<bool>> DeleteHRILineAsync(int Id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"api/HRILines/DeleteHRILineAsync/{Id}");
                return await response.Content.ReadFromJsonAsync<ServiceResponse<bool>>() ?? new ServiceResponse<bool>();
            }
            catch (Exception ex)
            {
                return new ServiceResponse<bool>
                {
                    Success = false,
                    Message = $"Error deleting HRI line with ID {Id}: {ex.Message}"
                };
            }
        }
    }
}
