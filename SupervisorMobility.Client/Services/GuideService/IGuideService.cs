namespace SupervisorMobility.Client.Services.GuideService
{
    public interface IGuideService
    {

        Task<Guide> CreateGuide(Guide guideData);

        Task<List<Guide>> GetAllGuides();
        Task<List<Guide>> GetAllGuidesWhitFile();

        // Get product by Id
        Task<Guide> GetGuideById(int id);
        Task<Guide> GetGuideByIdWhitFile(int id);
    }
}
