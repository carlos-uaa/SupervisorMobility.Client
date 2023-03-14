using SupervisorMobility.Client.Data.Entities;

namespace SupervisorMobility.Client.Services.BridgeCDMSService
{
    public interface IBridgeCDMSService
    {

        Task<FoldersCDMS> GetFoldersCCP();
        Task<FoldersCDMS> GetFoldersHOE();
        Task<FoldersCDMS> GetFoldersGOS();
        //get Assy Chart by Id
        Task<FilesCDMS> GetFilesCCP(string ruta);
        Task<FilesCDMS> GetFilesHOE(string ruta);
        Task<FilesCDMS> GetFilesGOS(string ruta);

    }
}
