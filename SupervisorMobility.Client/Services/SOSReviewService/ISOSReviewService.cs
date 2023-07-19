namespace SupervisorMobility.Client.Services.SOSReviewService
{
    public interface ISOSReviewService
    {

        Task<List<SOSReviewProgram>> GetAllSOSReviews(bool includeCollections);

        Task<SOSReviewProgram> GetSOSById(int id, bool includeCollections = false);

        Task<SOSReviewProgram> CreateSOSReview(SOSReviewProgram product);

        Task<bool> UpdateSOSReview(SOSReviewProgram product);

        Task DeleteSOSReview(int id);

    }
}
