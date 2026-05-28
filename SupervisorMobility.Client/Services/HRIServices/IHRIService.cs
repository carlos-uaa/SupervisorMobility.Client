using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SupervisorMobility.Client.Data;
using SupervisorMobility.Client.Data.Entites.Dtos.HRIDtos;
using SupervisorMobility.Client.Data.Entities.Dtos.HRIDtos;
using SupervisorMobility.Client.Data.Entities.Dtos.HRIDtos.HRIMetricsDtos;
using SupervisorMobility.Client.Data.Entities.Hri;
using SupervisorMobility.Clinet.Data.Entities.Dtos.HRIHistory;

namespace SupervisorMobility.Client.Services.HRIServices
{
    public interface IHRIService
    {
        // HRI
        Task<ServiceResponse<List<GetHRIDto>>> GetAllHRI();
        Task<ServiceResponse<GetHRIDto>> GetHRIById(int id);
        Task<ServiceResponse<GetHRIDto>> GetDailyByMonthAndYear(int hriId, int month, int year);
        Task<ServiceResponse<GetHRIDto>> CreateHRI(CreateHRIDto dto);
        Task<ServiceResponse<bool>> DeleteHRI(int id);
        Task<ServiceResponse<List<HRIToTableDto>>> GetHRISoftInfoList();
        Task<ServiceResponse<bool>> UpdateHRI(int HriId, UpdateHRIDto dto);

        // Weekly Revisions
        Task<ServiceResponse<bool>> CreateNewWeeklyRevision(List<CreateWeeklyRevisionDto> weeklyRevisions);

        // Images
        Task<string> SaveImageInTempFolderAsync(IBrowserFile imageFile);
        Task<byte[]?> GetImageContentAsync(string path);

        // History
        Task<ServiceResponse<List<GetHRIHistoryActionDto>>> GetHistoryByHRIId(int hriId);

        // Endpoints para el Dashboard del HRI
        Task<ServiceResponse<HriKpis>> GetHriKPIs();
        Task<ServiceResponse<LinesChartData>> GetLinesChartData(int areaId);
        Task<ServiceResponse<GeneralStatusChartData>> GetGeneralStatusChartData(int areaId);
        Task<ServiceResponse<List<HriRecentRevisionsDto>>> GetRecentRevisions(int areaId, string? filter = null);


        // Excel Report
        Task<byte[]> GetExcelReport(int hriId, int month, int year);
    }
}