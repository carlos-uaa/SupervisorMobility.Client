using SupervisorMobility.Client.Data;
using System.Net.Http.Json;

namespace SupervisorMobility.Client.Services.UserCoursesService
{
    public class UserCoursesService : IUserCoursesService 
    {
        private readonly HttpClient _http;
        public UserCoursesService(HttpClient http)
        {
            _http = http;
        }
        public async Task<ServiceResponse<List<UserCourse>>> GetUserCoursesAsync(string payrol)
        {
            try
            {
                var response = await _http.GetFromJsonAsync<ServiceResponse<List<UserCourse>>>($"UserCourses/GetUserCoursesAsync/{payrol}");
                if (response != null)
                {
                    return response;
                }
                else
                {
                    return new ServiceResponse<List<UserCourse>>
                    {
                        Success = false,
                        Message = "No se encontraron cursos para el usuario."
                    };
                }
            }
            catch (Exception ex)
            {
                return new ServiceResponse<List<UserCourse>>
                {
                    Success = false,
                    Message = $"Error al obtener los cursos del usuario: {ex.Message}"
                };
            }
        }
    }
}
