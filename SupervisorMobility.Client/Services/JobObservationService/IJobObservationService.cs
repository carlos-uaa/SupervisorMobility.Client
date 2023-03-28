namespace SupervisorMobility.Client.Services.JobObservationService
{
    public interface IJobObservationService
    {
        // Get all job observations     
        Task<List<JobObservation>> GetAllJobObservations();
        Task<List<JobObservation>> GetAllJobObservationsWithLup();

        // Get job observation by Id
        Task<JobObservation> GetJobObservationById(int jobObservationId);
        Task<JobObservation> GetJobObservationWithLup(int jobObservationId);

        // Create job observation
        Task<JobObservation> CreateJobObservation(JobObservation jobObservation);

        // Update job observation
        Task<bool> UpdateJobObservation(JobObservation jobObservation);

        // Delete job observation
        Task DeleteJobObservation(int jobObservationId);
    }
}
