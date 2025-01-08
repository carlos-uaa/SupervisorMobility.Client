namespace SupervisorMobility.Client.Services.HeadCountService
{
    public interface IHeadCountService
    {
        Task<FileUpload> UploadHeadCount(MultipartFormDataContent contentfile, int userid);

        Task<List<HeadCount>> GetAllHeadCout();

        Task<bool> UpdateHeadCount(HeadCount ToUpdate, int HeadId);

        Task DownloadFormat();
        //HeadCount Process
        Task<HeadCountProcess> CreateHeadCountPorcess(HeadCountProcess ToCreate);
        Task<List<HeadCountProcess>> GetAllHeadCountProcesses();
        Task<bool> UpdateHeadCountProcesses(HeadCountProcess ToUpdate, int HeadId);
        Task<bool> DeleteHeadCountProcess(int HeadId);
    }
}
