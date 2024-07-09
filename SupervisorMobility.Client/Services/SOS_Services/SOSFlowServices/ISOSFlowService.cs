
namespace SupervisorMobility.Client.Services.SOS_Services.SOSFlowServices
{
    public interface ISOSFlowService
    {
       
        Task<SOSFlow> GetSOSFlow(int SOSFlowId, bool includeImages = false, bool includeNotes = false, bool includeLogbooks = false, bool includeSpecialCases = false, bool includeSOS = false);
        Task<List<SOSFlow>> GetAllSOSFlow(bool includeImages = false, bool includeNotes = false, bool includeLogbooks = false, bool includeSpecialCases = false, bool includeSOS = false);
        Task<SOSFlow> UpdateSOSFlow(SOSFlow SosEntity);
        Task<SOSFlow> DeleteSOSFlow(int SosEntity_id);

        //Task<FileUpload> AddIllustrationToSOSFlow(MultipartFormDataContent? contentfiles, int SOS_SOSFlow_id);
        //Task<string> ShowIlustrationSOSFlow(int idfile);

        //Task<bool> RemoveIlustrationFromSOSData(int SOS_SOSFlow_id, int ImageFile_id);
    }
}
