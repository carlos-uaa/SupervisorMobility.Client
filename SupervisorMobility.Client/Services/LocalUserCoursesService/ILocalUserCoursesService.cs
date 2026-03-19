using SupervisorMobility.Client.Data;

namespace SupervisorMobility.Client.Services.LocalUserCoursesService
{
    public interface ILocalUserCoursesService
    {
        Task<ServiceResponse<LocalUserCourses>> UpdateLocalUserCourse(LocalUserCourses course);
        Task<ServiceResponse<LocalUserCourses>> DeleteLocalUserCourse(int courseId);
    }
}
