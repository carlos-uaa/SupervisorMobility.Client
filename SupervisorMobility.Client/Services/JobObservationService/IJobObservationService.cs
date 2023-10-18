namespace SupervisorMobility.Client.Services.JobObservationService
{
    public interface IJobObservationService
    {
        // Get all job observations     
        Task<List<JobObservationVersion>> GetAllHistoryJobObservations(int jobObservationId);
        Task<JobObservationVersion> GetOneHistoryJobObservation(int jobObservationId, int HistoryId);

        Task<List<JobObservation>> GetAllJobObservations();
        Task<List<JobObservation>> GetAllJobObservationsWithLup();

        // Get job observation by Id
        Task<JobObservation> GetJobObservationById(int jobObservationId);
        Task<JobObservation> GetJobObservationWithLup(int jobObservationId);

        // Create job observation
        Task<JobObservation> CreateJobObservation(JobObservation jobObservation);

        // Update job observation
        Task<bool> UpdateJobObservation(JobObservation jobObservation, string loggedUser);

        // Delete job observation
        Task DeleteJobObservation(int jobObservationId);
    }
}
