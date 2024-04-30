using static SupervisorMobility.Client.Pages.Inicio.SOSProgramPage.SOS_Details;

namespace SupervisorMobility.Client.Services.SOSReviewService
{
    public interface ISOSReviewService
    {

        Task<List<SOSReviewProgram>> GetAllSOSReviews( bool includeNavigation = false, bool includeUsers = false, bool includeSuggestions = false);

        Task<SOSReviewProgram> GetSOSById(int id, bool includeNavigation = false, bool includeUsers = false, bool includeSuggestions = false);

        Task<SOSReviewProgram> CreateSOSReview(SOSReviewProgram product);

        Task<bool> UpdateSOSReview(SOSReviewProgram product);

        Task DeleteSOSReview(int id);
        Task<SOSRegisterJobObservation> CreateSOSRegister(int SOSid, int month, int year, JobObservation JobEntity);
        Task<List<SOSRegisterJobObservation>> GetSOSRegisters(int sosid);

        Task<bool> ApplyMassiveSuggest(int SOS_Id, List<JobObservationNulls> Jobs, List<DistSelect> distribuciones);

       Task<SOSRegUserOperation> CreateSOSRegUserOperation(int SOSid, int SupervisorId, int OperationId);
        Task<List<SOSRegUserOperation>> GetSOSRegUserOperation(int sosid);
        Task<SOSRegUserOperation> UpdateSOSRegUserOperation(SOSRegUserOperation UpdateReg, int option);

    }
}
