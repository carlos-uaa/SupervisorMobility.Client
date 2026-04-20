using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Http;
using SupervisorMobility.Client.Data;
using SupervisorMobility.Client.Data.Entites.Dtos.HRIDtos;
using SupervisorMobility.Client.Data.Entities.Hri;

namespace SupervisorMobility.Client.Services.HRIServices
{
    public interface IHRIService
    {
        // HRI
        Task<ServiceResponse<List<HRI>>> GetAllHRI();
        Task<ServiceResponse<GetHRIDto>> GetHRIById(int id);
        Task<ServiceResponse<GetHRIDto>> CreateHRI(CreateHRIDto dto);
        Task<ServiceResponse<bool>> DeleteHRI(int id);

        // Images
        Task<string> SaveImageInTempFolderAsync(IBrowserFile imageFile);
    }
}