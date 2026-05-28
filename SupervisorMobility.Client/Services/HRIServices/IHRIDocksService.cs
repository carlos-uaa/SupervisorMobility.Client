using SupervisorMobility.Client.Data;
using SupervisorMobility.Client.Data.Entities.Hri;

namespace SupervisorMobility.Client.Services.HRIServices
{
    public interface IHRIDocksService
    {
        Task<ServiceResponse<List<HRIDock>>> GetAllHRIDocksAsync();
        Task<ServiceResponse<HRIDock>> GetSingleHRIDockAsync(int Id);
        Task<ServiceResponse<HRIDock>> CreateHRIDockAsync(HRIDock Line);
        Task<ServiceResponse<HRIDock>> UpdateHRIDockAsync(HRIDock Line);
        Task<ServiceResponse<bool>> DeleteHRIDockAsync(int Id);
    }
}
