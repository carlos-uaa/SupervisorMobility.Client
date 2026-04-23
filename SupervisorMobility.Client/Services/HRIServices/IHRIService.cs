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
        Task<ServiceResponse<List<GetHRIDto>>> GetAllHRI();
        Task<ServiceResponse<GetHRIDto>> GetHRIById(int id);
        Task<ServiceResponse<GetHRIDto>> CreateHRI(CreateHRIDto dto);
        Task<ServiceResponse<bool>> DeleteHRI(int id);
        Task<ServiceResponse<List<HRIToTableDto>>> GetHRISoftInfoList();

        // Images
        Task<string> SaveImageInTempFolderAsync(IBrowserFile imageFile);
    }
}