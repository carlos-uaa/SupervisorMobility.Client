using SupervisorMobility.Client.Data.Entities.PaginationEntities;

namespace SupervisorMobility.Client.Services.JobObservationService
{
    public interface IJobObservationService
    {
        // Get all job observations     
        Task<List<JobObservationHistoryVersion>> GetAllHistoryJobObservations(int jobObservationId);
        Task<JobObservationHistoryVersion> GetOneHistoryJobObservation(int jobObservationId, int HistoryId);

        Task<List<JobObservation>> GetAllJobObservations(bool includeTree = false, bool includePeople = false,
            bool includeLup = false, bool includeHistory = false,
            bool includeCkAnswers = false, int idPlant = 0, int idArea = 0, bool ForSosProgram = false, int year = 0, int month = 0,
            int SOSAnualId = 0, int idUser = 0);

        Task<List<JobObservation>> GetAllFinishedJobObservations(bool includeTree = false, bool includePeople = false,
            bool includeLup = false, bool includeHistory = false,
            bool includeCkAnswers = false, int idPlant = 0, int idArea = 0, bool ForSosProgram = false, int year = 0, int month = 0,
            int SOSAnualId = 0, int idUser = 0);

        Task<List<JobObservation>> GetAllNextYearJobsObservations(int plantId, int areaId, int year);
        Task<List<JobObservation>> GetAllTrainingJobsObservations(int plantId, int areaId, int month);
        Task<(int Total, List<JobObservation>JobObservations, JOCountPaginationDto Count)> GetAllJobObservationsByFilters(DateTime startDate, DateTime endDate, int JobObsId, int plantId,
            int areaId, int distributionId, int operationId, int supervisorId, int status, int userId, int typeId,
            string searchString, int page, int entries, int? sortO, string? sortL);

        // Get job observation by Id
        Task<JobObservation> GetJobObservationById(int jobObservationId, bool includeTree = false, bool includePeople = false, bool includeLup = false, bool includeHistory = false, bool includeCkAnswers = false);

        // Create job observation
        Task<JobObservation> CreateJobObservation(JobObservation jobObservation);
        Task<JobObservation> CreateJobObservationWithLup(JobObservation jobObservationWithLup);

        // Update job observation
        Task<bool> UpdateJobObservation(JobObservation jobObservation, string loggedUser);

        // Delete job observation
        Task DeleteJobObservation(int jobObservationId);
        Task<JobObservation> CreateOperatorSignature(MultipartFormDataContent checklistAnswer);
    }
}
