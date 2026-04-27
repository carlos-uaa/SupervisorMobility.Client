using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SupervisorMobility.Client.Data;
using SupervisorMobility.Client.Data.Entites.Dtos.HRIDtos;
using SupervisorMobility.Client.Data.Entities.Dtos.HRIDtos;
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
        Task<ServiceResponse<bool>> UpdateHRI(int HriId, UpdateHRIDto dto);

        // Weekly Revisions
        Task<ServiceResponse<bool>> CreateNewWeeklyRevision(List<CreateWeeklyRevisionDto> weeklyRevisions);

        // Images
        Task<string> SaveImageInTempFolderAsync(IBrowserFile imageFile);
        Task<byte[]?> GetImageContentAsync(string path);
    }
}