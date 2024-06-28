using SupervisorMobility.Client.Data.Entities.SOSAnalysis_Process;

namespace SupervisorMobility.Client.Services.SOS_Services.SOSAnaysisServices
{
    public interface ISOSAnaysisService
    {
        Task<SOSAnalysis> CreateSOSAnalysis(SOSAnalysis SOS_EntityToCreate, string InternalControlNumber, string ProcessNumber);
        Task<SOSAnalysis> GetSOSAnalysis(int SOSAnalysisId, bool includeImages = false, bool includeNotes = false, bool includeLogbooks = false, bool includeSpecialCases = false, bool includeSOS = false);
        Task<List<SOSAnalysis>> GetAllSOSAnalysis(bool includeImages = false, bool includeNotes = false, bool includeLogbooks = false, bool includeSpecialCases = false, bool includeSOS = false);
        Task<SOSAnalysis> UpdateSOSAnalysis(SOSAnalysis SosEntity);
        Task<SOSAnalysis> DeleteSOSAnalysis(int SosEntity_id);

        Task<FileUpload> AddIllustrationToSOSAnalysis(MultipartFormDataContent? contentfiles, int SOS_SOSAnalysis_id);
        Task<string> ShowIlustrationSOSAnalysis(int idfile);

        Task<bool> RemoveIlustrationFromSOSData(int SOS_SOSAnalysis_id, int ImageFile_id);
    }
}
