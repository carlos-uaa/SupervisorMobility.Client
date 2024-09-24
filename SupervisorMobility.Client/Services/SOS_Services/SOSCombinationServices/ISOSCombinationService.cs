
namespace SupervisorMobility.Client.Services.SOS_Services.SOSCombinationServices
{
    public interface ISOSCombinationService
    {
       
        Task<SOSCombination> GetSOSCombination(int SOSCombinationId, bool includeImages = false, bool includeNotes = false, bool includeLogbooks = false, bool includeSpecialCases = false, bool includeSOS = false);
        Task<List<SOSCombination>> GetAllSOSCombination(bool includeImages = false, bool includeNotes = false, bool includeLogbooks = false, bool includeSpecialCases = false, bool includeSOS = false);
        Task<SOSCombination> UpdateSOSCombination(SOSCombination SosEntity);
        Task<bool> DeleteSOSCombination(int SosEntity_id);

        //Task<FileUpload> AddIllustrationToSOSCombination(MultipartFormDataContent? contentfiles, int SOS_SOSCombination_id);
        //Task<string> ShowIlustrationSOSCombination(int idfile);

        //Task<bool> RemoveIlustrationFromSOSData(int SOS_SOSCombination_id, int ImageFile_id);
    }
}
