namespace SupervisorMobility.Client.Services.SOSReviewService
{
    public interface ISOSReviewService
    {

        Task<List<SOSReviewProgram>> GetAllSOSReviews(bool includeCollections);

        Task<SOSReviewProgram> GetSOSById(int id, bool includeCollections = false);

        Task<SOSReviewProgram> CreateSOSRevier(SOSReviewProgram product);

        Task<bool> UpdateProduct(SOSReviewProgram product);

        Task DeleteProduct(int id);

    }
}
