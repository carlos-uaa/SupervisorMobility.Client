namespace SupervisorMobility.Client.Services.FileUploadService
{
    public interface IFileUploadService
    {


        Task<UploadResult> UploadFile(MultipartFormDataContent contentfile);
        // Get all support document types
       
    }
}
