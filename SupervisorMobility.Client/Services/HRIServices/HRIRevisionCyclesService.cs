using SupervisorMobility.Client.Data;
using SupervisorMobility.Client.Data.Entites.Dtos.HRIRevisionCycles;
using SupervisorMobility.Client.Data.Entities.Dtos.HRIRevisionItemsDtos;
using System.Net.Http.Json;

namespace SupervisorMobility.Client.Services.HRIServices
{
    public class HRIRevisionCyclesService : IHRIRevisionCyclesService
    {
        private readonly HttpClient _http;

        public HRIRevisionCyclesService(HttpClient http)
        {
            _http = http;
        }

        public async Task<ServiceResponse<List<GetRevisionCyclesDto>>> GetAllRevisionCycles()
        {
            try
            {
                var response = await _http.GetFromJsonAsync<ServiceResponse<List<GetRevisionCyclesDto>>>("HRIRevisionCycles/GetAllRevisionCycles");
                return response ?? new ServiceResponse<List<GetRevisionCyclesDto>> { Success = false, Message = "No se obtuvo respuesta del servidor." };
            }
            catch (Exception ex)
            {
                return new ServiceResponse<List<GetRevisionCyclesDto>> { Success = false, Message = $"Error: {ex.Message}" };
            }
        }

        public async Task<ServiceResponse<List<GetRevisionCyclesDto>>> GetAllRevisionCyclesByRevisionItemId(int itemId)
        {
            try
            {
                var response = await _http.GetFromJsonAsync<ServiceResponse<List<GetRevisionCyclesDto>>>($"HRIRevisionCycles/GetAllRevisionCyclesByRevisionItemId/{itemId}");
                return response ?? new ServiceResponse<List<GetRevisionCyclesDto>> { Success = false, Message = "No se obtuvo respuesta del servidor." };
            }
            catch (Exception ex)
            {
                return new ServiceResponse<List<GetRevisionCyclesDto>> { Success = false, Message = $"Error: {ex.Message}" };
            }
        }

        public async Task<ServiceResponse<GetRevisionCyclesDto>> GetRevisionCycleById(int id)
        {
            try
            {
                var response = await _http.GetFromJsonAsync<ServiceResponse<GetRevisionCyclesDto>>($"HRIRevisionCycles/GetRevisionCycleById/{id}");
                return response ?? new ServiceResponse<GetRevisionCyclesDto> { Success = false, Message = "No se encontró el ciclo." };
            }
            catch (Exception ex)
            {
                return new ServiceResponse<GetRevisionCyclesDto> { Success = false, Message = $"Error: {ex.Message}" };
            }
        }

        public async Task<ServiceResponse<GetRevisionCyclesDto>> CreateRevisionCycle(int itemId, CreateRevisionCyclesDto dto)
        {
            try
            {
                var response = await _http.PostAsJsonAsync($"HRIRevisionCycles/CreateRevisionCycle/{itemId}", dto);
                var apiResponse = await response.Content.ReadFromJsonAsync<ServiceResponse<GetRevisionCyclesDto>>();
                return apiResponse ?? new ServiceResponse<GetRevisionCyclesDto> { Success = false, Message = "Error al crear ciclo." };
            }
            catch (Exception ex)
            {
                return new ServiceResponse<GetRevisionCyclesDto> { Success = false, Message = $"Error: {ex.Message}" };
            }
        }

        public async Task<ServiceResponse<bool>> CreateRevisionCyclesByRevisionItemId(int itemId, List<CreateRevisionCyclesDto> listOfRevisionsCycles)
        {
            try
            {
                var response = await _http.PostAsJsonAsync($"HRIRevisionCycles/CreateRevisionCyclesByRevisionItemId/{itemId}", listOfRevisionsCycles);
                var apiResponse = await response.Content.ReadFromJsonAsync<ServiceResponse<bool>>();
                return apiResponse ?? new ServiceResponse<bool> { Success = false, Message = "Error al crear ciclos." };
            }
            catch (Exception ex)
            {
                return new ServiceResponse<bool> { Success = false, Message = $"Error: {ex.Message}" };
            }
        }

        public async Task<ServiceResponse<bool>> CreateNewDailyRevision(CreateDailyRevisionDto createDaily)
        {
            try
            {
                var response = await _http.PostAsJsonAsync("HRIRevisionCycles/CreateNewDailyRevisionsForRevisionCycle", createDaily);
                var apiResponse = await response.Content.ReadFromJsonAsync<ServiceResponse<bool>>();
                return apiResponse ?? new ServiceResponse<bool> { Success = false, Message = "Error al crear revisión diaria." };
            }
            catch (Exception ex)
            {
                return new ServiceResponse<bool> { Success = false, Message = $"Error: {ex.Message}" };
            }
        }

        public async Task<ServiceResponse<GetRevisionCyclesDto>> UpdateRevisionCycle(int id, UpdateRevisionCycleDto dto)
        {
            try
            {
                var response = await _http.PutAsJsonAsync($"HRIRevisionCycles/UpdateRevisionCycle/{id}", dto);
                var apiResponse = await response.Content.ReadFromJsonAsync<ServiceResponse<GetRevisionCyclesDto>>();
                return apiResponse ?? new ServiceResponse<GetRevisionCyclesDto> { Success = false, Message = "Error al actualizar ciclo." };
            }
            catch (Exception ex)
            {
                return new ServiceResponse<GetRevisionCyclesDto> { Success = false, Message = $"Error: {ex.Message}" };
            }
        }

        public async Task<ServiceResponse<bool>> DeleteRevisionCycle(int id)
        {
            try
            {
                var response = await _http.DeleteAsync($"HRIRevisionCycles/DeleteRevisionCycle/{id}");
                var apiResponse = await response.Content.ReadFromJsonAsync<ServiceResponse<bool>>();
                return apiResponse ?? new ServiceResponse<bool> { Success = false, Message = "Error al eliminar ciclo." };
            }
            catch (Exception ex)
            {
                return new ServiceResponse<bool> { Success = false, Message = $"Error: {ex.Message}" };
            }
        }
    }
}
