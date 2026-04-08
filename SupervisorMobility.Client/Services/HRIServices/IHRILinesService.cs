using SupervisorMobility.Client.Data;
using SupervisorMobility.Client.Data.Entities.Hri;

namespace SupervisorMobility.Client.Services.HRIServices
{
    public interface IHRILinesService
    {
        Task<ServiceResponse<List<HRILines>>> GetAllHRILinesAsync();
        Task<ServiceResponse<HRILines>> GetSingleHRILineAsync(int Id);
        Task<ServiceResponse<HRILines>> CreateHRILineAsync(HRILines Line);
        Task<ServiceResponse<HRILines>> UpdateHRILineAsync(HRILines Line);
        Task<ServiceResponse<bool>> DeleteHRILineAsync(int Id);
    }
}
