using SupervisorMobility.Client.Data;
using SupervisorMobility.Client.Data.Entities.Hri;
using SupervisorMobility.Client.Services.HRIServices;

namespace TestSupervisorMobility.Client.Services
{
    public sealed class FakeHRIItemsService : IHRIItemsService
    {
        public Task<ServiceResponse<List<HRIItem>>> GetAllHRIItemsAsync() => Task.FromResult(new ServiceResponse<List<HRIItem>>
        {
            Data = new List<HRIItem>
                {
                    new() { Id = 1, Name = "Item de prueba", IsActive = true }
                }
        });

        public Task<ServiceResponse<HRIItem>> GetSingleHRIItemAsync(int Id) => throw new NotImplementedException();
        public Task<ServiceResponse<HRIItem>> CreateHRIItemAsync(HRIItem Item) => throw new NotImplementedException();
        public Task<ServiceResponse<HRIItem>> UpdateHRIItemAsync(HRIItem Item) => throw new NotImplementedException();
        public Task<ServiceResponse<bool>> DeleteHRIItemAsync(int Id) => throw new NotImplementedException();
    }
}