using SupervisorMobility.Client.Services.HRIServices;
using SupervisorMobility.Client.Data;
using SupervisorMobility.Client.Data.Entites.Dtos.HRIDtos;
using SupervisorMobility.Client.Data.Entities.Dtos.HRIDtos;
using Microsoft.AspNetCore.Components.Forms;
using SupervisorMobility.Client.Data.Entities.Dtos.HRIDtos.HRIMetricsDtos;
using SupervisorMobility.Clinet.Data.Entities.Dtos.HRIHistory;

namespace TestSupervisorMobility.Client.Services
{
    public sealed class FakeHRIService : IHRIService
    {
        public Task<ServiceResponse<List<GetHRIDto>>> GetAllHRI() => throw new NotImplementedException();
        public Task<ServiceResponse<GetHRIDto>> GetHRIById(int id) => throw new NotImplementedException();
        public Task<ServiceResponse<GetHRIDto>> GetDailyByMonthAndYear(int hriId, int month, int year) => throw new NotImplementedException();
        public Task<ServiceResponse<GetHRIDto>> CreateHRI(CreateHRIDto dto) => throw new NotImplementedException();
        public Task<ServiceResponse<bool>> DeleteHRI(int id) => throw new NotImplementedException();
        public Task<ServiceResponse<List<HRIToTableDto>>> GetHRISoftInfoList() => throw new NotImplementedException();
        public Task<ServiceResponse<bool>> UpdateHRI(int HriId, UpdateHRIDto dto) => throw new NotImplementedException();
        public Task<ServiceResponse<bool>> CreateNewWeeklyRevision(List<CreateWeeklyRevisionDto> weeklyRevisions) => throw new NotImplementedException();
        public Task<string> SaveImageInTempFolderAsync(IBrowserFile imageFile) => Task.FromResult(string.Empty);
        public Task<byte[]?> GetImageContentAsync(string path) => Task.FromResult<byte[]?>(null);
        public Task<ServiceResponse<List<GetHRIHistoryActionDto>>> GetHistoryByHRIId(int hriId) => throw new NotImplementedException();
        public Task<ServiceResponse<HriKpis>> GetHriKPIs() => throw new NotImplementedException();
        public Task<ServiceResponse<LinesChartData>> GetLinesChartData(int areaId) => throw new NotImplementedException();
        public Task<ServiceResponse<GeneralStatusChartData>> GetGeneralStatusChartData(int areaId) => throw new NotImplementedException();
        public Task<ServiceResponse<List<HriRecentRevisionsDto>>> GetRecentRevisions(int areaId, string? filter = null) => throw new NotImplementedException();
        public Task<byte[]> GetExcelReport(int hriId, int month, int year) => throw new NotImplementedException();
    }
}