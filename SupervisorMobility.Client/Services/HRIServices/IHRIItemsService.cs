using SupervisorMobility.Client.Data;
using SupervisorMobility.Client.Data.Entities.Hri;

namespace SupervisorMobility.Client.Services.HRIServices
{
    public interface IHRIItemsService
    {
        Task<ServiceResponse<List<HRIItem>>> GetAllHRIItemsAsync();
        Task<ServiceResponse<HRIItem>> GetSingleHRIItemAsync(int Id);
        Task<ServiceResponse<HRIItem>> CreateHRIItemAsync(HRIItem Item);
        Task<ServiceResponse<HRIItem>> UpdateHRIItemAsync(HRIItem Item);
        Task<ServiceResponse<bool>> DeleteHRIItemAsync(int Id);
    }
}
