
namespace SupervisorMobility.Client.Services.SOS_Services.SOSSynopticRequirementsService
{
    public interface ISynopticRequirementsService
    {
        Task<SOSSynopticTableofOperatingRequirements> GetSOSSynopticTableofOperatingRequirements(int SOSSynopticTableofOperatingRequirementsId, bool includeLogbooks = false, bool includeSOS = false, bool includeCollections = false);
        Task<List<SOSSynopticTableofOperatingRequirements>> GetAllSOSSynopticTableofOperatingRequirements(bool includeLogbooks = false, bool includeSOS = false, bool includeCollections = false);
        Task<SOSSynopticTableofOperatingRequirements> UpdateSOSSynopticTableofOperatingRequirements(SOSSynopticTableofOperatingRequirementsForUpdateDto SosEntity);
        Task<bool> DeleteSOSSynopticTableofOperatingRequirements(int SosEntity_id);

        Task<FileUpload> AddIllustrationToSOSSynopticTableofOperatingRequirements(MultipartFormDataContent? contentfiles, int SOS_SOSSynopticTableofOperatingRequirements_id);
        Task<string> ShowIlustrationSOSSynopticTableofOperatingRequirements(int idfile);

        Task<bool> RemoveIlustrationFromSOSData(int SOS_SOSSynopticTableofOperatingRequirements_id, int ImageFile_id);
        Task GenerateExcelSTOperatingRequirements(int Id);
    }
}
