using SupervisorMobility.Client.Data.Entities.SOS_Process;

namespace SupervisorMobility.Client.Services.SOS_Services.SOSAnalysisServices
{
    public interface ISOSAnalysisService
    {
       
        Task<SOSAnalysis> GetSOSAnalysis(int SOSAnalysisId, bool includeImages = false, bool includeNotes = false, bool includeLogbooks = false, bool includeSpecialCases = false, bool includeSOS = false, bool includeImagesSOS = false);
        Task<List<SOSAnalysis>> GetAllSOSAnalysis(bool includeImages = false, bool includeNotes = false, bool includeLogbooks = false, bool includeSpecialCases = false, bool includeSOS = false);
        Task<List<SOSAnalysis>> GetAllSOSAnalysisByDistribution(int Distribution_Id, bool includeImages = false, bool includeNotes = false, bool includeLogbooks = false, bool includeSpecialCases = false, bool includeSOS = false);
        Task<SOSAnalysis> UpdateSOSAnalysis(SOSAnalysis SosEntity);
        Task<bool> DeleteSOSAnalysis(int SosEntity_id);

        Task<FileUpload> AddIllustrationToSOSAnalysis(MultipartFormDataContent? contentfiles, int SOS_SOSAnalysis_id);
        Task<string> ShowIlustrationSOSAnalysis(int idfile);

        Task<bool> RemoveIlustrationFromSOSData(int SOS_SOSAnalysis_id, int ImageFile_id);
    }
}
