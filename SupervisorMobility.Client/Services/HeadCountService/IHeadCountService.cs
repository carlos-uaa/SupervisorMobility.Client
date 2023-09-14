namespace SupervisorMobility.Client.Services.HeadCountService
{
    public interface IHeadCountService
    {
        Task<FileUpload> UploadHeadCount(MultipartFormDataContent contentfile, int userid);

        Task<List<HeadCount>> GetAllHeadCout();

        Task<bool> UpdateHeadCount(HeadCount ToUpdate, int HeadId);
    }
}
