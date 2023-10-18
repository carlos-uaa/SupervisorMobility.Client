using SupervisorMobility.Client.Data.Entities;

namespace SupervisorMobility.Client.Services.BridgeCDMSService
{
    public interface IBridgeCDMSService
    {

        Task<CDMS_CCP_Directory> GetFoldersCCP();
        Task<CDMS_HOE_Directory> GetFoldersHOE();
        Task<CDMS_GOS_Directory> GetFoldersGOS();
        //get Assy Chart by Id
        Task<CDMS_CCP_Archives> GetFilesCCP(string route);
        Task<CDMS_HOE_Archives> GetFilesHOE(string route);
        Task<CDMS_GOS_Archives> GetFilesGOS(string route);

        Task<CDMS_DownloadFile> GetDownloadLinkGOS(string URL);
        //HOE DESCARGA ULTIMA VERSION por ahora
        //Task<CDMS_DownloadFile> GetDownloadLinkHOE(string URL);
        Task<CDMS_DownloadFile> GetDownloadLinkCCP(string URL);
        Task<CDMS_General> DeleteFileTempGOS(string FileName);
        //Hoe No tiene este metodo
        //Task<CDMS_General> DeleteFileTempHOE(string URL);
        Task<CDMS_General> DeleteFileTempCCP(string FileName);

    }
}
