using SupervisorMobility.Client.Data;
using SupervisorMobility.Client.Data.Entities.Hri;
using SupervisorMobility.Client.Services.HRIServices;

namespace TestSupervisorMobility.Client.Services
{
    public sealed class FakeHRIDocksService : IHRIDocksService
    {
        public Task<ServiceResponse<List<HRIDock>>> GetAllHRIDocksAsync() => Task.FromResult(new ServiceResponse<List<HRIDock>>
        {
            Data = new List<HRIDock>
            {
                new() { Id = 1, DockName = "Dock de prueba", IsActive = true }
            }
        });

        public Task<ServiceResponse<HRIDock>> GetSingleHRIDockAsync(int Id) => throw new NotImplementedException();
        public Task<ServiceResponse<HRIDock>> CreateHRIDockAsync(HRIDock Line) => throw new NotImplementedException();
        public Task<ServiceResponse<HRIDock>> UpdateHRIDockAsync(HRIDock Line) => throw new NotImplementedException();
        public Task<ServiceResponse<bool>> DeleteHRIDockAsync(int Id) => throw new NotImplementedException();
    }
}