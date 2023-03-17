namespace SupervisorMobility.Client.Services.FileUploadAndDownloadService
{
    public interface IFileUploadAndDownloadService
    {


        Task<FileUpload> UploadFile(MultipartFormDataContent contentfile);
        Task<FileUpload> UploadGuide(MultipartFormDataContent contentfile);
        Task<FileUpload> UploadUsers(MultipartFormDataContent contentfile);
        Task<FileUpload> UploadEvidences(MultipartFormDataContent contentfiles, int lupId);
        Task<UploadAssyChartResult> ProccedToUpdateData(FileUpload fileinfo);

        Task DownloadFileFromOnePlant(int idPlant);
        Task DownloadFileFromAllPlants();

        Task DownloadFileGuide(int idfile, string filename);


    }
}
