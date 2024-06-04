
namespace SupervisorMobility.Client.Services.IS_Services.PartService
{
    public interface IPartServices
    {
        Task<Part?> CreatePart(Part partToCreate);
        Task<List<Part>> GetAllParts(bool includeScketes = false);
        Task<Part?> GetPart(int part_id, bool includeScketes = false);
        Task<Part?> UpdatePart(Part partToUpdate);

        Task<bool> DeletePart(int part_id);

        Task<FileUpload> UploadSketchPart(MultipartFormDataContent? contentfiles, int part_id);
        Task<string> ShowImagePart(int idfile);
    }
}
