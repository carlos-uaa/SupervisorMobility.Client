namespace SupervisorMobility.Client.Services.DepartmentService
{
    public interface IDepartmentService
    {
        Task<List<Department>> GetDepartments();

        Task<Department> GetDepartmentById(int id);

        Task<Department> CreateDepartment(Department department);

        Task<bool> UpdateDepartment(Department department);

        Task DeleteDepartment(int id);
    }
}
