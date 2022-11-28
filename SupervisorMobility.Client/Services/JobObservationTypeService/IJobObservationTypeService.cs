namespace SupervisorMobility.Client.Services.JobObservationTypeService
{
    public interface IJobObservationTypeService
    {
        // Get all job observation types    
        Task<List<JobObservationType>> GetJobObservationTypes();

        // Get job observation type by Id
        Task<JobObservationType> GetJobObservationTypeById(int id);

        // Create job observation type
        Task<JobObservationType> CreateJobObservationType(JobObservationType jobObservationType);

        // Update job observation type
        Task UpdateJobObservationType(JobObservationType jobObservationType);

        // Delete job observation type
        Task DeleteJobObservationType(int id);
    }
}
