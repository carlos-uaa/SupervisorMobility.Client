namespace SupervisorMobility.Client.Services.JobObservationService
{
    public interface IJobObservationService
    {
        // Get all job observations     
        Task<List<JobObservation>> GetAllJobObservations();

        // Get job observation by Id
        Task<JobObservation> GetJobObservationById(int jobObservationId);

        // Create job observation
        Task<JobObservation> CreateJobObservationType(JobObservation jobObservation);

        // Update job observation
        Task UpdateJobObservation(JobObservation jobObservation);

        // Delete job observation
        Task DeleteJobObservation(int jobObservationId);
    }
}
