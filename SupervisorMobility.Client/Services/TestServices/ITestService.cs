namespace SupervisorMobility.Client.Services.TestServices
{
    public interface ITestService
    {
        Task<(int, string)> UploadVideoFiles(MultipartFormDataContent contentfile);
    }
}
