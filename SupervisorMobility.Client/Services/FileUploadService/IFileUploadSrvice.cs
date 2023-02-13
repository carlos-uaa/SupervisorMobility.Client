namespace SupervisorMobility.Client.Services.FileUploadService
{
    public interface IFileUploadService
    {


        Task<UploadResult> UploadFile(MultipartFormDataContent contentfile);
        Task<UploadDataResult> SetNewData(UploadResult fileinfo);
        // Get all support document types
       
    }
}
