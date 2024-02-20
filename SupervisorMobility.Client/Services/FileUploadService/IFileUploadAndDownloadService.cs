namespace SupervisorMobility.Client.Services.FileUploadAndDownloadService
{
    public interface IFileUploadAndDownloadService
    {

        Task<FileUpload> UploadPlantStructure(MultipartFormDataContent contentfile, int plantnameid, int userId);
        Task PlantStructureFormat();

        Task<FileUpload> UploadPathStructure(MultipartFormDataContent contentfile, int userId);
        Task PathStructureFormat();

        Task<FileUpload> UploadFile(MultipartFormDataContent contentfile);
        Task<FileUpload> UploadGuide(MultipartFormDataContent contentfile);
        Task<FileUpload> UploadUsers(MultipartFormDataContent contentfile);
        Task<FileUpload> UploadEvidences(MultipartFormDataContent contentfiles, int lupId);
        Task<FileUpload> UploadOperatorSignature(MultipartFormDataContent contentfiles, int jobObservationId);
        Task<UploadAssyChartResult> ProccedToUpdateData(FileUpload fileinfo);

        Task DownloadFileFromOnePlant(int idPlant);
        Task DownloadFileFromAllPlants();
        Task DownloadFileUsers();

        Task DownloadFileGuide(int idfile, string filename);
        Task DownloadFileEvidence(int idfile, string filename);
        Task<string> ShowImageEvidence(int idfile);

        Task DownloadAllUsersFormat();
        Task DownloadSSVFormat();
        Task DownloadSupervisorsFormat();
        Task DownloadOperatorsFormat();

    }
}
