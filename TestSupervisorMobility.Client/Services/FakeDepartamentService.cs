using SupervisorMobility.Client.Data.Entities;
using SupervisorMobility.Client.Services.DepartmentService;

namespace TestSupervisorMobility.Client.Services
{
    public sealed class FakeDepartmentService : IDepartmentService
    {
        public Task<List<Department>> GetDepartments() => Task.FromResult(new List<Department>
        {
            new() { DepartmentId = 1, Code = "D01", Description = "Departamento de prueba", IsActive = true }
        });

        public Task<Department> GetDepartmentById(int id) => throw new NotImplementedException();
        public Task<Department> CreateDepartment(Department department) => throw new NotImplementedException();
        public Task<bool> UpdateDepartment(Department department) => throw new NotImplementedException();
        public Task DeleteDepartment(int id) => throw new NotImplementedException();
    }
}