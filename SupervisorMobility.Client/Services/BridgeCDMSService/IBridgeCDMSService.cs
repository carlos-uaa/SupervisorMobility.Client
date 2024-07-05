using SupervisorMobility.Client.Data.Entities;
using System.Runtime.CompilerServices;

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

        Task<AsyncVoidMethodBuilder> GetDownloadLinkGOS(int ID, string namefile);
        Task<CDMS_General> DeleteFileTempGOS(string FileName, string pathFile);

        Task<AsyncVoidMethodBuilder> GetDownloadLinkCCP(int ID, string namefile);
        Task<CDMS_General> DeleteFileTempCCP(string FileName, string pathFile);

        Task<AsyncVoidMethodBuilder> Download_DeleteFileTempHOE(string FileName, string pathFile);

    }
}
