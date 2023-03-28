using SupervisorMobility.Client.Data.Entities;

namespace SupervisorMobility.Client.Services.BridgeCDMSService
{
    public interface IBridgeCDMSService
    {

        Task<CDMS_CCP_Folder> GetFoldersCCP();
        Task<CDMS_HOE_Folder> GetFoldersHOE();
        Task<CDMS_GOS_Folder> GetFoldersGOS();
        //get Assy Chart by Id
        Task<CDMS_CCP_Document> GetFilesCCP(string route);
        Task<CDMS_HOE_Document> GetFilesHOE(string route);
        Task<CDMS_GOS_Document> GetFilesGOS(string route);

    }
}
