using SupervisorMobility.Client.Data;
using SupervisorMobility.Client.Data.Entities.Hri;
using SupervisorMobility.Client.Services.HRIServices;

namespace TestSupervisorMobility.Client.Services
{
    public sealed class FakeHRILinesService : IHRILinesService
    {
        public Task<ServiceResponse<List<HRILines>>> GetAllHRILinesAsync() => Task.FromResult(new ServiceResponse<List<HRILines>>
        {
            Data = new List<HRILines>
            {
                new() { Id = 1, LineName = "Linea de prueba", IsActive = true }
            }
        });

        public Task<ServiceResponse<HRILines>> GetSingleHRILineAsync(int Id) => throw new NotImplementedException();
        public Task<ServiceResponse<HRILines>> CreateHRILineAsync(HRILines Line) => throw new NotImplementedException();
        public Task<ServiceResponse<HRILines>> UpdateHRILineAsync(HRILines Line) => throw new NotImplementedException();
        public Task<ServiceResponse<bool>> DeleteHRILineAsync(int Id) => throw new NotImplementedException();
    }
}