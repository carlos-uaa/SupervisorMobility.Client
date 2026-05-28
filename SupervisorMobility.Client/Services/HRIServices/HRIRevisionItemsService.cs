using SupervisorMobility.Client.Data;
using SupervisorMobility.Client.Data.Entites.Dtos.HRIRevisionItemsDtos;
using SupervisorMobility.Client.Data.Entities.Hri;
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
        public async Task<ServiceResponse<List<GetHRIRevisionItemDto>>> GetAllHRIRevisionItems()
        {
            try
            {
                return await _http.GetFromJsonAsync<ServiceResponse<List<GetHRIRevisionItemDto>>>("HRIRevisionItem/GetAllHRIRevisionItems");
            }catch (Exception ex)
            {
                return new ServiceResponse<List<GetHRIRevisionItemDto>>
                {
                    Success = false,
                    Message = $"Error al obtener HRI Revision Items: {ex.Message}",
                    Data = new List<GetHRIRevisionItemDto>()
                };
            }
        }

        public async Task<ServiceResponse<GetHRIRevisionItemDto>> GetHRIRevisionItemById(int id) =>
            await _http.GetFromJsonAsync<ServiceResponse<GetHRIRevisionItemDto>>($"HRIRevisionItem/GetHRIRevisionItemById/{id}");

        public async Task<ServiceResponse<List<GetHRIRevisionItemDto>>> GetHRIRevisionItemsByHRIId(int id) =>
            await _http.GetFromJsonAsync<ServiceResponse<List<GetHRIRevisionItemDto>>>($"HRIRevisionItem/GetHRIRevisionItemsByHRIId/{id}");

        public async Task<ServiceResponse<GetHRIRevisionItemDto>> CreateHRIRevisionItem(CreateHRIRevisionItemDto dto)
        {
            var response = await _http.PostAsJsonAsync("HRIRevisionItem/CreateHRIRevisionItem", dto);
            return await response.Content.ReadFromJsonAsync<ServiceResponse<GetHRIRevisionItemDto>>();
        }

        public async Task<ServiceResponse<GetHRIRevisionItemDto>> UpdateHRIRevisionItem(int id, UpdateHRIRevisionItemDto dto)
        {
            var response = await _http.PutAsJsonAsync($"HRIRevisionItem/UpdateHRIRevisionItem/{id}", dto);
            return await response.Content.ReadFromJsonAsync<ServiceResponse<GetHRIRevisionItemDto>>();
        }

        public async Task<ServiceResponse<bool>> DeleteHRIRevisionItem(int id)
        {
            var response = await _http.DeleteAsync($"HRIRevisionItem/DeleteHRIRevisionItem/{id}");
            return await response.Content.ReadFromJsonAsync<ServiceResponse<bool>>();
        }

        // Frequencies
        public async Task<ServiceResponse<List<Frequency>>> GetAllFrequencies()
        {
            var serviceResponse = new ServiceResponse<List<Frequency>>();

            try
            {
                // Llamada al API
                var response = await _http.GetFromJsonAsync<ServiceResponse<List<GetFrequencyDto>>>(
                    "HRIRevisionItem/GetAllFrequencies"
                );

                // Validar respuesta
                if (response == null || !response.Success)
                {
                    serviceResponse.Success = false;
                    serviceResponse.Message = "No se recibió información del servidor.";
                    serviceResponse.Data = new List<Frequency>();
                    return serviceResponse;
                }

                // Mapear DTOs a entidades
                serviceResponse.Data = response.Data.Select(dto => new Frequency
                {
                    Id = dto.Id,
                    Code = dto.Code,
                    Description = dto.Description,
                    IsActive = dto.IsActive
                }).ToList();

                serviceResponse.Success = response.Success;
                serviceResponse.Message = response.Message;
            }
            catch (Exception ex)
            {
                // Manejo de errores
                serviceResponse.Success = false;
                serviceResponse.Message = $"Error al obtener frequencies: {ex.Message}";
                serviceResponse.Data = new List<Frequency>();
            }

            return serviceResponse;
        }

        public async Task<ServiceResponse<Frequency>> GetFrequencyById(int id)
        {
            var serviceResponse = new ServiceResponse<Frequency>();

            try
            {
                // Llamada al API
                var response = await _http.GetFromJsonAsync<ServiceResponse<GetFrequencyDto>>(
                    $"HRIRevisionItem/GetFrequencyById/{id}"
                );

                // Validar respuesta
                if (response == null || !response.Success)
                {
                    serviceResponse.Success = false;
                    serviceResponse.Message = "No se recibió información del servidor.";
                    serviceResponse.Data = null;
                    return serviceResponse;
                }

                // Mapear DTO a entidad
                serviceResponse.Data = new Frequency
                {
                    Id = response.Data.Id,
                    Code = response.Data.Code,
                    Description = response.Data.Description,
                    IsActive = response.Data.IsActive
                };

                serviceResponse.Success = true;
                serviceResponse.Message = response.Message;
            }
            catch (Exception ex)
            {
                // Manejo de errores
                serviceResponse.Success = false;
                serviceResponse.Message = $"Error al obtener frequency: {ex.Message}";
                serviceResponse.Data = null;
            }

            return serviceResponse;
        }

        public async Task<ServiceResponse<Frequency>> CreateFrequency(Frequency data)
        {
            var serviceResponse = new ServiceResponse<Frequency>();

            try
            {
                var dto = new CreateFrequencyDto
                {
                    Code = data.Code,
                    Description = data.Description,
                    IsActive = data.IsActive
                };

                var response = await _http.PostAsJsonAsync("HRIRevisionItem/CreateFrequency", dto);

                var apiResponse = await response.Content.ReadFromJsonAsync<ServiceResponse<GetFrequencyDto>>();

                if (apiResponse == null || apiResponse.Data == null)
                {
                    serviceResponse.Success = false;
                    serviceResponse.Message = "No se recibió información del servidor.";
                    return serviceResponse;
                }

                serviceResponse.Data = new Frequency
                {
                    Id = apiResponse.Data.Id,
                    Code = apiResponse.Data.Code,
                    Description = apiResponse.Data.Description,
                    IsActive = apiResponse.Data.IsActive
                };

                serviceResponse.Success = true;
                serviceResponse.Message = "Frequency creada correctamente.";
            }
            catch (Exception ex)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = $"Error al crear frequency: {ex.Message}";
            }

            return serviceResponse;
        }

        public async Task<ServiceResponse<Frequency>> UpdateFrequency(int id, Frequency data)
        {
            var serviceResponse = new ServiceResponse<Frequency>();

            try
            {
                var dto = new UpdateFrequencyDto
                {
                    Code = data.Code,
                    Description = data.Description
                };

                var response = await _http.PutAsJsonAsync($"HRIRevisionItem/UpdateFrequency/{id}", dto);

                var apiResponse = await response.Content.ReadFromJsonAsync<ServiceResponse<GetFrequencyDto>>();

                if (apiResponse == null || apiResponse.Data == null)
                {
                    serviceResponse.Success = false;
                    serviceResponse.Message = "No se recibió información del servidor.";
                    return serviceResponse;
                }

                serviceResponse.Data = new Frequency
                {
                    Id = apiResponse.Data.Id,
                    Code = apiResponse.Data.Code,
                    Description = apiResponse.Data.Description,
                    IsActive = apiResponse.Data.IsActive
                };

                serviceResponse.Success = true;
                serviceResponse.Message = "Frequency actualizada correctamente.";
            }
            catch (Exception ex)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = $"Error al actualizar frequency: {ex.Message}";
            }

            return serviceResponse;
        }


        public async Task<ServiceResponse<bool>> DeleteFrequency(int id)
        {
            var response = await _http.DeleteAsync($"HRIRevisionItem/DeleteFrequency/{id}");
            return await response.Content.ReadFromJsonAsync<ServiceResponse<bool>>();
        }

        // Veredicts
        public async Task<ServiceResponse<List<Veredict>>> GetAllVeredicts()
        {
            var serviceResponse = new ServiceResponse<List<Veredict>>();

            try
            {
                var response = await _http.GetFromJsonAsync<ServiceResponse<List<GetVeredictDto>>>(
                    "HRIRevisionItem/GetAllVeredicts"
                );

                if (response == null || !response.Success)
                {
                    serviceResponse.Success = false;
                    serviceResponse.Message = response?.Message ?? "No se recibió información del servidor.";
                    serviceResponse.Data = new List<Veredict>();
                    return serviceResponse;
                }

                serviceResponse.Data = response.Data.Select(dto => new Veredict
                {
                    Id = dto.Id,
                    Code = dto.Code,
                    Description = dto.Description,
                    IsActive = dto.IsActive
                }).ToList();

                serviceResponse.Success = true;
                serviceResponse.Message = response.Message;
            }
            catch (Exception ex)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = $"Error al obtener veredicts: {ex.Message}";
                serviceResponse.Data = new List<Veredict>();
            }

            return serviceResponse;
        }

        public async Task<ServiceResponse<Veredict>> GetVeredictById(int id)
        {
            var serviceResponse = new ServiceResponse<Veredict>();

            try
            {
                var response = await _http.GetFromJsonAsync<ServiceResponse<GetVeredictDto>>(
                    $"HRIRevisionItem/GetVeredictById/{id}"
                );

                if (response == null || response.Success)
                {
                    serviceResponse.Success = false;
                    serviceResponse.Message = response.Message ?? "Error al obtener veredict.";
                    return serviceResponse;
                }

                serviceResponse.Data = new Veredict
                {
                    Id = response.Data.Id,
                    Code = response.Data.Code,
                    Description = response.Data.Description,
                    IsActive = response.Data.IsActive
                };

                serviceResponse.Success = true;
                serviceResponse.Message = response.Message;
            }
            catch (Exception ex)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = $"Error al obtener veredict: {ex.Message}";
            }

            return serviceResponse;
        }

        public async Task<ServiceResponse<Veredict>> CreateVeredict(Veredict data)
        {
            var serviceResponse = new ServiceResponse<Veredict>();

            var dto = new CreateVeredictDto
            {
                Code = data.Code,
                Description = data.Description,
                IsActive = data.IsActive
            };

            try
            {
                var response = await _http.PostAsJsonAsync("HRIRevisionItem/CreateVeredict", dto);
                var apiResponse = await response.Content.ReadFromJsonAsync<ServiceResponse<GetVeredictDto>>();

                if (apiResponse == null || !apiResponse.Success)
                {
                    serviceResponse.Success = false;
                    serviceResponse.Message = apiResponse?.Message ?? "No se recibió información del servidor.";
                    return serviceResponse;
                }

                serviceResponse.Data = new Veredict
                {
                    Id = apiResponse.Data.Id,
                    Code = apiResponse.Data.Code,
                    Description = apiResponse.Data.Description,
                    IsActive = apiResponse.Data.IsActive
                };

                serviceResponse.Success = true;
                serviceResponse.Message = apiResponse.Message;
            }
            catch (Exception ex)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = $"Error al crear veredict: {ex.Message}";
            }

            return serviceResponse;
        }

        public async Task<ServiceResponse<Veredict>> UpdateVeredict(int id, Veredict data)
        {
            var serviceResponse = new ServiceResponse<Veredict>();

            var dto = new UpdateVeredictDto
            {
                Code = data.Code,
                Description = data.Description,
            };

            try
            {
                var response = await _http.PutAsJsonAsync($"HRIRevisionItem/UpdateVeredict/{id}", dto);
                var apiResponse = await response.Content.ReadFromJsonAsync<ServiceResponse<GetVeredictDto>>();

                if (apiResponse == null || !apiResponse.Success)
                {
                    serviceResponse.Success = false;
                    serviceResponse.Message = apiResponse?.Message ?? "No se recibió información del servidor.";
                    return serviceResponse;
                }

                serviceResponse.Data = new Veredict
                {
                    Id = apiResponse.Data.Id,
                    Code = apiResponse.Data.Code,
                    Description = apiResponse.Data.Description,
                    IsActive = apiResponse.Data.IsActive
                };

                serviceResponse.Success = true;
                serviceResponse.Message = apiResponse.Message;
            }
            catch (Exception ex)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = $"Error al actualizar veredict: {ex.Message}";
            }

            return serviceResponse;
        }

        public async Task<ServiceResponse<bool>> DeleteVeredict(int id)
        {
            var response = await _http.DeleteAsync($"HRIRevisionItem/DeleteVeredict/{id}");
            return await response.Content.ReadFromJsonAsync<ServiceResponse<bool>>();
        }

        // Revision Methods
        public async Task<ServiceResponse<List<RevisionMethod>>> GetAllRevisionMethods()
        {
            var serviceResponse = new ServiceResponse<List<RevisionMethod>>();

            try
            {
                var response = await _http.GetFromJsonAsync<ServiceResponse<List<GetRevisionMethodDto>>>(
                    "HRIRevisionItem/GetAllRevisionMethods"
                );

                if (response == null || response.Data == null)
                {
                    serviceResponse.Success = false;
                    serviceResponse.Message = response?.Message ?? "No se recibió información del servidor.";
                    serviceResponse.Data = new List<RevisionMethod>();
                    return serviceResponse;
                }

                serviceResponse.Data = response.Data.Select(dto => new RevisionMethod
                {
                    Id = dto.Id,
                    Code = dto.Code,
                    Description = dto.Description,
                    IsActive = dto.IsActive
                }).ToList();

                serviceResponse.Success = true;
                serviceResponse.Message = response.Message ?? "RevisionMethods obtenidos correctamente.";
            }
            catch (Exception ex)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = $"Error al obtener revision methods: {ex.Message}";
                serviceResponse.Data = new List<RevisionMethod>();
            }

            return serviceResponse;
        }

        public async Task<ServiceResponse<RevisionMethod>> GetRevisionMethodById(int id)
        {
            var serviceResponse = new ServiceResponse<RevisionMethod>();

            try
            {
                var response = await _http.GetFromJsonAsync<ServiceResponse<GetRevisionMethodDto>>(
                    $"HRIRevisionItem/GetRevisionMethodById/{id}"
                );

                if (response == null || response.Data == null)
                {
                    serviceResponse.Success = false;
                    serviceResponse.Message = response?.Message ?? "No se recibió información del servidor.";
                    return serviceResponse;
                }

                serviceResponse.Data = new RevisionMethod
                {
                    Id = response.Data.Id,
                    Code = response.Data.Code,
                    Description = response.Data.Description,
                    IsActive = response.Data.IsActive
                };

                serviceResponse.Success = true;
                serviceResponse.Message = response.Message ?? "RevisionMethod obtenido correctamente.";
            }
            catch (Exception ex)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = $"Error al obtener revision method: {ex.Message}";
            }

            return serviceResponse;
        }

        public async Task<ServiceResponse<RevisionMethod>> CreateRevisionMethod(RevisionMethod data)
        {
            var serviceResponse = new ServiceResponse<RevisionMethod>();
            var dto = new CreateRevisionMethodDto
            {
                Code = data.Code,
                Description = data.Description,
                IsActive = data.IsActive
            };

            try
            {
                var response = await _http.PostAsJsonAsync("HRIRevisionItem/CreateRevisionMethod", dto);
                var apiResponse = await response.Content.ReadFromJsonAsync<ServiceResponse<GetRevisionMethodDto>>();

                if (apiResponse == null || apiResponse.Data == null)
                {
                    serviceResponse.Success = false;
                    serviceResponse.Message = apiResponse?.Message ?? "No se recibió información del servidor.";
                    return serviceResponse;
                }

                serviceResponse.Data = new RevisionMethod
                {
                    Id = apiResponse.Data.Id,
                    Code = apiResponse.Data.Code,
                    Description = apiResponse.Data.Description,
                    IsActive = apiResponse.Data.IsActive
                };

                serviceResponse.Success = true;
                serviceResponse.Message = apiResponse.Message ?? "RevisionMethod creado correctamente.";
            }
            catch (Exception ex)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = $"Error al crear revision method: {ex.Message}";
            }

            return serviceResponse;
        }

        public async Task<ServiceResponse<RevisionMethod>> UpdateRevisionMethod(int id, RevisionMethod data)
        {
            var serviceResponse = new ServiceResponse<RevisionMethod>();
            var dto = new UpdateRevisionMethodDto
            {
                Code = data.Code,
                Description = data.Description,
            };

            try
            {
                var response = await _http.PutAsJsonAsync($"HRIRevisionItem/UpdateRevisionMethod/{id}", dto);
                var apiResponse = await response.Content.ReadFromJsonAsync<ServiceResponse<GetRevisionMethodDto>>();

                if (apiResponse == null || apiResponse.Data == null)
                {
                    serviceResponse.Success = false;
                    serviceResponse.Message = apiResponse?.Message ?? "No se recibió información del servidor.";
                    return serviceResponse;
                }

                serviceResponse.Data = new RevisionMethod
                {
                    Id = apiResponse.Data.Id,
                    Code = apiResponse.Data.Code,
                    Description = apiResponse.Data.Description,
                    IsActive = apiResponse.Data.IsActive
                };

                serviceResponse.Success = true;
                serviceResponse.Message = apiResponse.Message ?? "RevisionMethod actualizado correctamente.";
            }
            catch (Exception ex)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = $"Error al actualizar revision method: {ex.Message}";
            }

            return serviceResponse;
        }


        public async Task<ServiceResponse<bool>> DeleteRevisionMethod(int id)
        {
            var response = await _http.DeleteAsync($"HRIRevisionItem/DeleteRevisionMethod/{id}");
            return await response.Content.ReadFromJsonAsync<ServiceResponse<bool>>();
        }

    }
}
