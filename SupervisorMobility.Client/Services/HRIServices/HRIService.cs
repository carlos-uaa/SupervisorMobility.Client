using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Http;
using SupervisorMobility.Client.Data;
using SupervisorMobility.Client.Data.Entites.Dtos.HRIDtos;
using SupervisorMobility.Client.Data.Entities.Dtos.HRIDtos;
using SupervisorMobility.Client.Data.Entities.Hri;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using static System.Net.WebRequestMethods;

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

        public async Task<ServiceResponse<List<GetHRIDto>>> GetAllHRI()
        {
            var response = await _httpClient.GetFromJsonAsync<ServiceResponse<List<GetHRIDto>>>(
                "HRI/GetAllHRI"
            );

            return response;
        }

        public async Task<ServiceResponse<GetHRIDto>> GetHRIById(int id)
        {
            var response = await _httpClient.GetFromJsonAsync<ServiceResponse<GetHRIDto>>(
                $"HRI/GetHRIById/{id}"
            );

            return response;
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

        public async Task<ServiceResponse<bool>> CreateNewWeeklyRevision(List<CreateWeeklyRevisionDto> weeklyRevisions)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("HRI/CreateNewWeeklyRevision", weeklyRevisions);
                var result = await response.Content.ReadFromJsonAsync<ServiceResponse<bool>>();
                return result ?? new ServiceResponse<bool>
                {
                    Success = false,
                    Data = false,
                    Message = "Empty response while creating weekly revision."
                };
            }
            catch (Exception ex)
            {
                return new ServiceResponse<bool>
                {
                    Success = false,
                    Data = false,
                    Message = $"Exception creating weekly revision: {ex.Message}"
                };
            }
        }


        public async Task<ServiceResponse<List<HRIToTableDto>>> GetHRISoftInfoList()
        {
            var response = await _httpClient.GetFromJsonAsync<ServiceResponse<List<HRIToTableDto>>>(
                "HRI/GetHRISoftInfoList"
            );

            return response;
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
            if (result is null || !result.Success)
            {
                return string.Empty;
            }
            return result.Data ?? string.Empty;
        }

        public async Task<byte[]?> GetImageContentAsync(string path)
        {
            try
            {
                // Llamada al endpoint con query string
                var response = await _httpClient.GetAsync($"api/HRIImages/content?path={Uri.EscapeDataString(path)}");

                if (!response.IsSuccessStatusCode)
                {
                    // Manejo de errores: 404, 400, etc.
                    return null;
                }

                // Leer el contenido como byte[]
                return await response.Content.ReadAsByteArrayAsync();
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}