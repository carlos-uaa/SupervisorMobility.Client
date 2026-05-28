using SupervisorMobility.Client.Data;
using SupervisorMobility.Client.Data.Entities.Hri;
using System.Net.Http.Json;

namespace SupervisorMobility.Client.Services.HRIServices
{
    public class HRIItemsService : IHRIItemsService
    {
        private readonly HttpClient _httpClient;

        public HRIItemsService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<ServiceResponse<List<HRIItem>>> GetAllHRIItemsAsync()
        {
            try
            {
                var response = await _httpClient.GetFromJsonAsync<ServiceResponse<List<HRIItem>>>("HRIItem/GetAllHRIItemsAsync");
                return response!;
            }
            catch (Exception ex)
            {
                return new ServiceResponse<List<HRIItem>>
                {
                    Success = false,
                    Message = $"Error getting all HRI items: {ex.Message}"
                };
            }
        }

        public async Task<ServiceResponse<HRIItem>> GetSingleHRIItemAsync(int Id)
        {
            try
            {
                var response = await _httpClient.GetFromJsonAsync<ServiceResponse<HRIItem>>($"HRIItem/GetSingleHRIItemAsync/{Id}");
                return response!;
            }
            catch (Exception ex)
            {
                return new ServiceResponse<HRIItem>
                {
                    Success = false,
                    Message = $"Error getting HRI item with ID {Id}: {ex.Message}"
                };
            }
        }

        public async Task<ServiceResponse<HRIItem>> CreateHRIItemAsync(HRIItem Line)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("HRIItem/CreateHRIItemAsync", Line);
                return await response.Content.ReadFromJsonAsync<ServiceResponse<HRIItem>>() ?? new ServiceResponse<HRIItem>();
            }
            catch (Exception ex)
            {
                return new ServiceResponse<HRIItem>
                {
                    Success = false,
                    Message = $"Error creating HRI item: {ex.Message}"
                };
            }
        }

        public async Task<ServiceResponse<HRIItem>> UpdateHRIItemAsync(HRIItem Line)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync("HRIItem/UpdateHRIItemAsync", Line);
                return await response.Content.ReadFromJsonAsync<ServiceResponse<HRIItem>>() ?? new ServiceResponse<HRIItem>();
            }
            catch (Exception ex)
            {
                return new ServiceResponse<HRIItem>
                {
                    Success = false,
                    Message = $"Error updating HRI item with ID {Line.Id}: {ex.Message}"
                };
            }
        }

        public async Task<ServiceResponse<bool>> DeleteHRIItemAsync(int Id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"HRIItem/DeleteHRIItemAsync/{Id}");
                return await response.Content.ReadFromJsonAsync<ServiceResponse<bool>>() ?? new ServiceResponse<bool>();
            }
            catch (Exception ex)
            {
                return new ServiceResponse<bool>
                {
                    Success = false,
                    Message = $"Error deleting HRI item with ID {Id}: {ex.Message}"
                };
            }
        }
    }
}
