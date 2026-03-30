using SupervisorMobility.Client.Data;
using System.Net.Http.Json;

namespace SupervisorMobility.Client.Services.LocalUserCoursesService
{
    public class LocalUserCoursesService : ILocalUserCoursesService
    {
        private readonly HttpClient _httpClient;

        public LocalUserCoursesService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<ServiceResponse<LocalUserCourses>> UpdateLocalUserCourse(LocalUserCourses course)
        {
            var response = await _httpClient.PutAsJsonAsync("api/LocalUserCourses/update", course);

            if (!response.IsSuccessStatusCode)
            {
                return new ServiceResponse<LocalUserCourses>
                {
                    Success = false,
                    Message = $"Error al actualizar el curso: {response.ReasonPhrase}"
                };
            }

            var result = await response.Content.ReadFromJsonAsync<ServiceResponse<LocalUserCourses>>();
            return result!;
        }

        public async Task<ServiceResponse<LocalUserCourses>> DeleteLocalUserCourse(int courseId)
        {
            var response = await _httpClient.DeleteAsync($"api/LocalUserCourses/delete/{courseId}");

            if (!response.IsSuccessStatusCode)
            {
                return new ServiceResponse<LocalUserCourses>
                {
                    Success = false,
                    Message = $"Error al eliminar el curso: {response.ReasonPhrase}"
                };
            }

            var result = await response.Content.ReadFromJsonAsync<ServiceResponse<LocalUserCourses>>();
            return result!;
        }

    }
}
