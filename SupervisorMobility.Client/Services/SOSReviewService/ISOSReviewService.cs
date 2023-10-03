namespace SupervisorMobility.Client.Services.SOSReviewService
{
    public interface ISOSReviewService
    {

        Task<List<SOSReviewProgram>> GetAllSOSReviews(bool includeCollections);

        Task<SOSReviewProgram> GetSOSById(int id, bool includeCollections = false);

        Task<SOSReviewProgram> CreateSOSReview(SOSReviewProgram product);

        Task<bool> UpdateSOSReview(SOSReviewProgram product);

        Task DeleteSOSReview(int id);
        Task<SOSRegisterJobObservation> CreateSOSRegister(int SOSid, int month, int year, JobObservation JobEntity);
        Task<List<SOSRegisterJobObservation>> GetSOSRegisters(int sosid);

        Task<SOSRegUserOperation> CreateSOSRegUserOperation(int SOSid, int SupervisorId, int OperationId);
        Task<List<SOSRegUserOperation>> GetSOSRegUserOperation(int sosid);

    }
}
