namespace SupervisorMobility.Client.Services.ChecklistAnswerService
{
    public interface IChecklistAnswerService
    {
        Task<List<ChecklistAnswer>> GetAllChecklistAnswers();
        Task<List<ChecklistAnswer>> GetAllChecklistAnswersByJobObservationId(int jobObservationId);
        Task<ChecklistAnswer> GetChecklistAnswerById(int checklistAnswerId);
        Task<ChecklistAnswer> CreateChecklistAnswer(ChecklistAnswer checklistAnswer);
        Task<bool> UpdateChecklistAnswer(ChecklistAnswer checklistAnswer);
        Task DeleteChecklistAnswer(int checklistAnswer);
    }
}
