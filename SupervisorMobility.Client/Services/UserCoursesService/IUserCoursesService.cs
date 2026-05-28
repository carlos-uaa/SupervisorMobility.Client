using SupervisorMobility.Client.Data;

namespace SupervisorMobility.Client.Services.UserCoursesService
{
    public interface IUserCoursesService
    {
        Task<ServiceResponse<List<UserCourse>>> GetUserCoursesAsync(string payrol);
    }
}
