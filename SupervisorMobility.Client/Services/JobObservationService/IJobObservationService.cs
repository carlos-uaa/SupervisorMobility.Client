namespace SupervisorMobility.Client.Services.JobObservationService
{
    public interface IJobObservationService
    {
        // Get all job observations     
        Task<List<JobObservationHistoryVersion>> GetAllHistoryJobObservations(int jobObservationId);
        Task<JobObservationHistoryVersion> GetOneHistoryJobObservation(int jobObservationId, int HistoryId);

        Task<List<JobObservation>> GetAllJobObservations(bool includeTree = false, bool includePeople = false, bool includeLup = false, bool includeHistory = false, bool includeCkAnswers = false, bool ForSosProgram = false, int year = 0, int userId = 0);

        // Get job observation by Id
        Task<JobObservation> GetJobObservationById(int jobObservationId, bool includeTree = false, bool includePeople = false, bool includeLup = false, bool includeHistory = false, bool includeCkAnswers = false);

        // Create job observation
        Task<JobObservation> CreateJobObservation(JobObservation jobObservation);
        Task<JobObservation> CreateJobObservationWithLup(JobObservation jobObservationWithLup);

        // Update job observation
        Task<bool> UpdateJobObservation(JobObservation jobObservation, string loggedUser);

        // Delete job observation
        Task DeleteJobObservation(int jobObservationId);
    }
}
