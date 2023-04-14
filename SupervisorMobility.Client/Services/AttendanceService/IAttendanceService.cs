namespace SupervisorMobility.Client.Services.AttendanceService
{
    public interface IAttendanceService
    {
        Task<List<Attendance>> GetAttendanceList();
        Task<List<Attendance>> AssignEmployes();
        Task<List<Attendance>> UpdateList(List<Attendance> list);
    }
}
