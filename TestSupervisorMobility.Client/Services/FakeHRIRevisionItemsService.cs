using SupervisorMobility.Client.Data;
using SupervisorMobility.Client.Data.Entites.Dtos.HRIRevisionItemsDtos;
using SupervisorMobility.Client.Data.Entities.Hri;
using SupervisorMobility.Client.Services.HRIServices;

namespace TestSupervisorMobility.Client.Services
{
    public sealed class FakeHRIRevisionItemsService : IHRIRevisionItemsService
    {
        public Task<ServiceResponse<List<GetHRIRevisionItemDto>>> GetAllHRIRevisionItems() => throw new NotImplementedException();
        public Task<ServiceResponse<GetHRIRevisionItemDto>> GetHRIRevisionItemById(int id) => throw new NotImplementedException();
        public Task<ServiceResponse<List<GetHRIRevisionItemDto>>> GetHRIRevisionItemsByHRIId(int id) => throw new NotImplementedException();
        public Task<ServiceResponse<GetHRIRevisionItemDto>> CreateHRIRevisionItem(CreateHRIRevisionItemDto dto) => throw new NotImplementedException();
        public Task<ServiceResponse<GetHRIRevisionItemDto>> UpdateHRIRevisionItem(int id, UpdateHRIRevisionItemDto dto) => throw new NotImplementedException();
        public Task<ServiceResponse<bool>> DeleteHRIRevisionItem(int id) => throw new NotImplementedException();
        public Task<ServiceResponse<List<Frequency>>> GetAllFrequencies() => Task.FromResult(new ServiceResponse<List<Frequency>>
        {
            Data = new List<Frequency>
            {
                new() { Id = 1, Code = "F01", Description = "Frecuencia de prueba", IsActive = true }
            }
        });
        public Task<ServiceResponse<Frequency>> GetFrequencyById(int id) => throw new NotImplementedException();
        public Task<ServiceResponse<Frequency>> CreateFrequency(Frequency dto) => throw new NotImplementedException();
        public Task<ServiceResponse<Frequency>> UpdateFrequency(int id, Frequency dto) => throw new NotImplementedException();
        public Task<ServiceResponse<bool>> DeleteFrequency(int id) => throw new NotImplementedException();
        public Task<ServiceResponse<List<Veredict>>> GetAllVeredicts() => Task.FromResult(new ServiceResponse<List<Veredict>>
        {
            Data = new List<Veredict>
            {
                new() { Id = 1, Code = "V01", Description = "Criterio de prueba", IsActive = true, HRIRevisionItems = new List<HRIRevisionItems>() }
            }
        });
        public Task<ServiceResponse<Veredict>> GetVeredictById(int id) => throw new NotImplementedException();
        public Task<ServiceResponse<Veredict>> CreateVeredict(Veredict dto) => throw new NotImplementedException();
        public Task<ServiceResponse<Veredict>> UpdateVeredict(int id, Veredict dto) => throw new NotImplementedException();
        public Task<ServiceResponse<bool>> DeleteVeredict(int id) => throw new NotImplementedException();
        public Task<ServiceResponse<List<RevisionMethod>>> GetAllRevisionMethods() => Task.FromResult(new ServiceResponse<List<RevisionMethod>>
        {
            Data = new List<RevisionMethod>
            {
                new() { Id = 1, Code = "RM01", Description = "Método de prueba", IsActive = true, HRIRevisionItems = new List<HRIRevisionItems>() }
            }
        });
        public Task<ServiceResponse<RevisionMethod>> GetRevisionMethodById(int id) => throw new NotImplementedException();
        public Task<ServiceResponse<RevisionMethod>> CreateRevisionMethod(RevisionMethod dto) => throw new NotImplementedException();
        public Task<ServiceResponse<RevisionMethod>> UpdateRevisionMethod(int id, RevisionMethod dto) => throw new NotImplementedException();
        public Task<ServiceResponse<bool>> DeleteRevisionMethod(int id) => throw new NotImplementedException();
    }
}