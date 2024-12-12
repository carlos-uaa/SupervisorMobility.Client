
namespace SupervisorMobility.Client.Services.SOS_Services.SOSSequenceServices
{
    public interface ISOSSequenceService
    {

        Task<SOSSequence> GetSOSSequence(int SOSSequenceId, bool includeImages = false, bool includeNotes = false, bool includeLogbooks = false, bool includeSpecialCases = false, bool includeSOS = false, bool includeImagesSOS = false);
        Task<List<SOSSequence>> GetAllSOSSequence(bool includeImages = false, bool includeNotes = false, bool includeLogbooks = false, bool includeSpecialCases = false, bool includeSOS = false);
        Task<List<SOSSequence>> GetAllSOSSequenceByDistribution(int Distribution_Id, bool includeImages = false, bool includeNotes = false, bool includeLogbooks = false, bool includeSpecialCases = false, bool includeSOS = false);
        Task<SOSSequence> UpdateSOSSequence(SOSSequence SosEntity);
        Task<bool> DeleteSOSSequence(int SosEntity_id);

        Task<FileUpload> AddIllustrationToSOSSequence(MultipartFormDataContent? contentfiles, int SOS_SOSSequence_id);
        Task<string> ShowIlustrationSOSSequence(int idfile);

        Task<bool> RemoveIlustrationFromSOSData(int SOS_SOSSequence_id, int ImageFile_id);
    }
}
