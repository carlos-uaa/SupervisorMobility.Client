using SupervisorMobility.Client.Data.Entities.SOSAnalysis_Process;

namespace SupervisorMobility.Client.Services.SOS_Services.SOSHubService
{
    public interface ISOSHubService
    {
        Task<SOSHub> CreateSOScollection(SOSHub SOS_EntityToCreate);
        Task<SOSHub> GetSOSHub(int HubId, bool includeAnalysesBkup = false, bool includeSections = false, bool includeImages = false, bool includeVideos = false, bool includeCommentaries = false, bool includeTools = false, bool includeEquipments = false, bool includeMaterials = false, bool includeInformation = false, bool includePeople = false, bool includeDocuments = false);
        Task<List<SOSHub>> GetAllSOSHub( bool includeAnalysesBkup = false, bool includeSections = false, bool includeImages = false, bool includeVideos = false, bool includeCommentaries = false, bool includeTools = false, bool includeEquipments = false, bool includeMaterials = false, bool includeInformation = false, bool includePeople = false, bool includeDocuments = false);
        Task<SOSHub> UpdateSOSHub(SOSHub SosEntity);
        Task<SOSHub> DeleteSOSHub(int SosEntity_id);

        Task<FileUpload> AddImageToSOSHub(MultipartFormDataContent? contentfiles, int SOS_DataPool_id);
        Task<FileUpload> AddVideoToSOSHub(MultipartFormDataContent? contentfiles, int SOS_DataPool_id);
        Task<FileUpload> AddCDToSOSHub(MultipartFormDataContent? contentfiles, int SOS_DataPool_id);
        Task<string> ShowImageSosHub(int idfile);

        Task<string> ShowVideoSosHub(int idfile);
        Task DownloadFileCD(int idfile, string filename);
        Task<bool> RemoveImageFromSOSData(int SOS_DataPool_id, int ImageFile_id);
        Task<bool> RemoveVideoFromSOSData(int SOS_DataPool_id, int VideoFile_id);
        Task<bool> RemoveCDFromSOSData(int SOS_DataPool_id, int CDFile_id);

        Task<bool> GenerateAnalysis(int SOS_DataPool_id, SOSAnalysis analysis);
        Task<bool> GenerateCombination(int SOS_DataPool_id);
        Task<bool> GenerateFlow(int SOS_DataPool_id);
        Task<bool> GenerateDistribution(int SOS_DataPool_id);
        Task<bool> GenerateSequence(int SOS_DataPool_id);
    
    }
}
