using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Http;
using SupervisorMobility.Client.Data;
using SupervisorMobility.Client.Data.Entites.Dtos.HRIDtos;
using SupervisorMobility.Client.Data.Entities.Hri;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace SupervisorMobility.Client.Services.HRIServices
{
    public class HRIService: IHRIService
    {
        private const long MaxImageUploadBytes = 10 * 1024 * 1024; // 10 MB
        private readonly HttpClient _httpClient;

        public HRIService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<ServiceResponse<List<HRI>>> GetAllHRI()
        {
            var serviceResponse = new ServiceResponse<List<HRI>>();

            try
            {
                var response = await _httpClient.GetFromJsonAsync<ServiceResponse<List<GetHRIDto>>>(
                    "HRI/GetAllHRI"
                );

                if (response == null || response.Data == null)
                {
                    serviceResponse.Success = false;
                    serviceResponse.Message = response?.Message ?? "Empty response while getting HRI list.";
                    serviceResponse.Data = new List<HRI>();
                    return serviceResponse;
                }

                // Mapear DTO → Entidad
                serviceResponse.Data = response.Data.Select(dto => new HRI
                {
                    HriId = dto.HriId,
                    HRILinesId = dto.HRILinesId,
                    Line = dto.Line,
                    HRIItemId = dto.HRIItemId,
                    NameOfItem = dto.NameOfItem,
                    ControlNumber = dto.ControlNumber,
                    HRIDockId = dto.HRIDockId,
                    Dock = dto.Dock,
                    Department = dto.Department,
                    Images = dto.Images,
                    ItemsRevised = dto.ItemsRevised,
                    WeeklyRevisions = dto.WeeklyRevisions,
                    HriCycles = dto.HriCycles,
                    IsActive = dto.IsActive,
                    CreationDate = dto.CreationDate,
                    HourmeterRevision = dto.HourmeterRevision,
                    UserId = dto.UserId,
                    Supervisor = dto.Supervisor
                }).ToList();

                serviceResponse.Success = true;
                serviceResponse.Message = response.Message ?? "HRI list retrieved successfully.";
            }
            catch (Exception ex)
            {
                serviceResponse.Success = false;
                serviceResponse.Data = new List<HRI>();
                serviceResponse.Message = $"Exception getting HRI list: {ex.Message}";
            }

            return serviceResponse;
        }

        public async Task<ServiceResponse<HRI>> GetHRIById(int id)
        {
            var serviceResponse = new ServiceResponse<HRI>();

            try
            {
                var response = await _httpClient.GetFromJsonAsync<ServiceResponse<GetHRIDto>>(
                    $"HRI/GetHRIById/{id}"
                );

                if (response == null || response.Data == null)
                {
                    serviceResponse.Success = false;
                    serviceResponse.Message = response?.Message ?? "Empty response while getting HRI by id.";
                    serviceResponse.Data = null;
                    return serviceResponse;
                }

                // Mapear DTO → Entidad
                serviceResponse.Data = new HRI
                {
                    HriId = response.Data.HriId,
                    HRILinesId = response.Data.HRILinesId,
                    Line = response.Data.Line,
                    HRIItemId = response.Data.HRIItemId,
                    NameOfItem = response.Data.NameOfItem,
                    ControlNumber = response.Data.ControlNumber,
                    HRIDockId = response.Data.HRIDockId,
                    Dock = response.Data.Dock,
                    Department = response.Data.Department,
                    Images = response.Data.Images,
                    ItemsRevised = response.Data.ItemsRevised,
                    WeeklyRevisions = response.Data.WeeklyRevisions,
                    HriCycles = response.Data.HriCycles,
                    IsActive = response.Data.IsActive,
                    CreationDate = response.Data.CreationDate,
                    HourmeterRevision = response.Data.HourmeterRevision,
                    UserId = response.Data.UserId,
                    Supervisor = response.Data.Supervisor
                };

                serviceResponse.Success = true;
                serviceResponse.Message = response.Message ?? "HRI retrieved successfully.";
            }
            catch (Exception ex)
            {
                serviceResponse.Success = false;
                serviceResponse.Data = null;
                serviceResponse.Message = $"Exception getting HRI by id: {ex.Message}";
            }

            return serviceResponse;
        }

        public async Task<ServiceResponse<GetHRIDto>> CreateHRI(CreateHRIDto dto)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("HRI/CreateHRI", dto);
                var result = await response.Content.ReadFromJsonAsync<ServiceResponse<GetHRIDto>>();
                return result ?? new ServiceResponse<GetHRIDto>
                {
                    Success = false,
                    Data = new GetHRIDto(),
                    Message = "Empty response while creating HRI."
                };
            }
            catch (Exception ex)
            {
                return new ServiceResponse<GetHRIDto>
                {
                    Success = false,
                    Data = new GetHRIDto(),
                    Message = $"Exception creating HRI: {ex.Message}"
                };
            }
        }

        public async Task<ServiceResponse<bool>> DeleteHRI(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"HRI/DeleteHRI/{id}");
                var result = await response.Content.ReadFromJsonAsync<ServiceResponse<bool>>();
                return result ?? new ServiceResponse<bool>
                {
                    Success = false,
                    Data = false,
                    Message = "Empty response while deleting HRI."
                };
            }
            catch (Exception ex)
            {
                return new ServiceResponse<bool>
                {
                    Success = false,
                    Data = false,
                    Message = $"Exception deleting HRI: {ex.Message}"
                };
            }
        }

        public async Task<string> SaveImageInTempFolderAsync(IBrowserFile imageFile)
        {
            if (imageFile is null || imageFile.Size == 0 || imageFile.Size > MaxImageUploadBytes)
            {
                return string.Empty;
            }

            var streamContent = new StreamContent(imageFile.OpenReadStream(MaxImageUploadBytes));
            streamContent.Headers.ContentType = new MediaTypeHeaderValue(imageFile.ContentType);

            var response = await _httpClient.PostAsync("HRImages/SaveImageInTempFolderAsync", new MultipartFormDataContent
            {
                { streamContent, "image", imageFile.Name }
            });

            if (!response.IsSuccessStatusCode)
            {
                return string.Empty;
            }

            var result = await response.Content.ReadFromJsonAsync<ServiceResponse<string>>();
            if(result is null || !result.Success)
            {
                return string.Empty;
            }
            return result.Data ?? string.Empty;
        }
    }
}