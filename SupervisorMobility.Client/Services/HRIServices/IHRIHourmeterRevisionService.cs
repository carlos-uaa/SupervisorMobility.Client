using SupervisorMobility.Client.Data;
using SupervisorMobility.Client.Data.Entities.Dtos;
using SupervisorMobility.Client.Data.Entities.Dtos.HRIDtos.HRIHourmeterRevisionDto;
using SupervisorMobility.Client.Data.Entities.Dtos.HRIRevisionItemsDtos;

namespace SupervisorMobility.Client.Services.HRIServices
{
    public interface IHRIHourmeterRevisionService
    {
        Task<ServiceResponse<List<GetHourmeterRevisionDto>>> GetAllHourmeterRevisions();
        Task<ServiceResponse<GetHourmeterRevisionDto>> GetHourmeterRevisionByHRIId(int hriId);
        Task<ServiceResponse<GetHourmeterRevisionDto>> GetHourmeterRevisionById(int id);
        Task<ServiceResponse<GetHourmeterRevisionDto>> AddHourmeterRevision(CreateHourMeterRevisionDto newHourmeterRevision);
        Task<ServiceResponse<bool>> AddDailyRevisionToHourmeterRevision(CreateDailyRevisionDto createDaily);
        Task<ServiceResponse<bool>> DeleteHourmeterRevision(int id);
    }
}
