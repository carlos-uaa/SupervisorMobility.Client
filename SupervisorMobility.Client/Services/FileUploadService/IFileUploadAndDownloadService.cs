namespace SupervisorMobility.Client.Services.FileUploadAndDownloadService
{
    public interface IFileUploadAndDownloadService
    {


        Task<UploadResult> UploadFile(MultipartFormDataContent contentfile);
        Task<UploadDataResult> ProccedToUpdateData(UploadResult fileinfo);

        Task DownloadFileFromOnePlant(int idPlant);
        Task DownloadFileFromAllPlants();

       
    }
}
