namespace SupervisorMobility.Client.Services.HeadCountService
{
    public interface IHeadCountService
    {
        Task<FileUpload> UploadHeadCount(MultipartFormDataContent contentfile, int userid);
    }
}
