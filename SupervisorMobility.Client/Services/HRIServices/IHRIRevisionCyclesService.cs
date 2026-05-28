using SupervisorMobility.Client.Data;
using SupervisorMobility.Client.Data.Entites.Dtos.HRIRevisionCycles;
using SupervisorMobility.Client.Data.Entities.Dtos.HRIRevisionItemsDtos;

namespace SupervisorMobility.Client.Services.HRIServices
{
    public interface IHRIRevisionCyclesService
    {
        Task<ServiceResponse<List<GetRevisionCyclesDto>>> GetAllRevisionCycles();
        Task<ServiceResponse<List<GetRevisionCyclesDto>>> GetAllRevisionCyclesByRevisionItemId(int itemId);
        Task<ServiceResponse<GetRevisionCyclesDto>> GetRevisionCycleById(int id);
        Task<ServiceResponse<GetRevisionCyclesDto>> CreateRevisionCycle(int itemId, CreateRevisionCyclesDto dto);
        Task<ServiceResponse<bool>> CreateRevisionCyclesByRevisionItemId(int itemId, List<CreateRevisionCyclesDto> listOfRevisionsCycles);
        Task<ServiceResponse<bool>> CreateNewDailyRevision(CreateDailyRevisionDto createDaily);
        Task<ServiceResponse<GetRevisionCyclesDto>> UpdateRevisionCycle(int id, UpdateRevisionCycleDto dto);
        Task<ServiceResponse<bool>> DeleteRevisionCycle(int id);
    }
}
