using SupervisorMobility.Client.Data;
using SupervisorMobility.Client.Data.Entites.Dtos.HRICyclesDtos;

namespace SupervisorMobility.Client.Services.HRIServices
{
    public interface IHRICyclesService
    {
        Task<ServiceResponse<List<GetHRICyclesDto>>> GetAllHRICycles();
        Task<ServiceResponse<GetHRICyclesDto>> GetHRICycleById(int id);
        Task<ServiceResponse<GetHRICyclesDto>> CreateHRICycle(CreateHRICyclesDto dto);
        Task<ServiceResponse<bool>> DeleteHRICycle(int id);

    }
}
