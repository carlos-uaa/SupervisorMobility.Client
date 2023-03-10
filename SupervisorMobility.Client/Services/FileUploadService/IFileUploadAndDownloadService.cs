namespace SupervisorMobility.Client.Services.FileUploadAndDownloadService
{
    public interface IFileUploadAndDownloadService
    {


        Task<FileUpload> UploadFile(MultipartFormDataContent contentfile);
        Task<FileUpload> UploadGuide(MultipartFormDataContent contentfile);
        Task<FileUpload> UploadUsers(MultipartFormDataContent contentfile);
        Task<List<FileUpload>> UploadEvidences(MultipartFormDataContent contentfile);
        Task<UploadAssyChartResult> ProccedToUpdateData(FileUpload fileinfo);

        Task DownloadFileFromOnePlant(int idPlant);
        Task DownloadFileFromAllPlants();

        Task DownloadFileGuide(int idfile, string filename);


    }
}
