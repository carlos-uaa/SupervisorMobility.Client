
namespace SupervisorMobility.Client.Services.SOS_Services.SOSDistributionServices
{
    public interface ISOSDistributionService
    {
        Task<SOSDistribution> GetSOSDistribution(int SOSDistributionId, bool includeImages = false, bool includeNotes = false, bool includeLogbooks = false, bool includeSpecialCases = false, bool includeSOS = false, bool includeImagesSOS = false, bool includeTurns = false, bool includeTimes = false, bool includeCollections =false);
        Task<List<SOSDistribution>> GetAllSOSDistribution(bool includeImages = false, bool includeNotes = false, bool includeLogbooks = false, bool includeSpecialCases = false, bool includeSOS = false);
        Task<SOSDistribution> UpdateSOSDistribution(SOSDistribution SosEntity);
        Task<bool> DeleteSOSDistribution(int SosEntity_id);

        Task<FileUpload> AddIllustrationToSOSDistribution(MultipartFormDataContent? contentfiles, int SOS_SOSDistribution_id);
        Task<string> ShowIlustrationSOSDistribution(int idfile);

        Task<bool> RemoveIlustrationFromSOSData(int SOS_SOSDistribution_id, int ImageFile_id);
    }
}
