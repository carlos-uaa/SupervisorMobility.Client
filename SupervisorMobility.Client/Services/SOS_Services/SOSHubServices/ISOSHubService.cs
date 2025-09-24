using DocumentFormat.OpenXml.Wordprocessing;
using SupervisorMobility.Client.Data.Entities.SOS_Process;

namespace SupervisorMobility.Client.Services.SOS_Services.SOSHubService
{
    public interface ISOSHubService
    {
        Task<SOSHub> CreateSOScollection(SOSHub SOS_EntityToCreate);
        Task<SOSHub> GetSOSHub(int HubId, bool includeAnalysesBkup = false, bool includeSections = false, bool includeImages = false, bool includeVideos = false, bool includeCommentaries = false, bool includeTools = false, bool includeEquipments = false, bool includeMaterials = false, bool includeInformation = false, bool includePeople = false, bool includeDocuments = false, bool includeModel = false, bool includeCollections = false, bool includePeopleCollections = false, bool includePats = false);
        Task<List<SOSHub>> GetAllSOSHub(bool includeAnalysesBkup = false, bool includeSections = false, bool includeImages = false, bool includeVideos = false, bool includeCommentaries = false, bool includeTools = false, bool includeEquipments = false, bool includeMaterials = false, bool includeInformation = false, bool includePeople = false, bool includeDocuments = false, bool includeSOSDistribution = false);
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

        Task<int> GenerateAnalysis(int SOS_DataPool_id, SOSAnalysis analysis);
        Task<int> GenerateCombination(int SOS_DataPool_id, SOSCombination combination);
        Task<int> GenerateFlow(int SOS_DataPool_id, SOSFlow flow);
        Task<int> GenerateDistribution(int SOS_DataPool_id, SOSDistribution distribution);
        Task<int> GenerateSequence(int SOS_DataPool_id, SOSSequence sequence);
        Task<int> GeneratePat(int SOS_DataPool_id, PAT pat);

        Task<int> GenerateSynopticRequirements(int SOS_DataPool_id, SOSSynopticTableofOperatingRequirements SynopticRequirements);
        Task<int> GenerateSynopticControlPoints(int SOS_DataPool_id, SOSSynopticTableofControlPoints SynopticControlPoints);


        //Histoy
        Task<List<SOSHub>> GetAllHistorySOSHub(int HubId, bool includeAnalysesBkup = false, bool includeSections = false, bool includeImages = false, bool includeVideos = false, bool includeCommentaries = false, bool includeTools = false, bool includeEquipments = false, bool includeMaterials = false, bool includeInformation = false, bool includePeople = false, bool includeDocuments = false);
    }
}
