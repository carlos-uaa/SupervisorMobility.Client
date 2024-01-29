namespace SupervisorMobility.Client.Services.ChecklistService
{
    public interface IChecklistService
    {
        // Get all checklist categories
        Task<List<JobCategoryStructure>> GetChecklistCategories(bool includeChecklistQuestions = false);

        // Get checklist category by Id
        Task<JobCategoryStructure> GetCategoryById(int id);

        // Get checklist category including questions
        Task<JobCategoryStructure> GetCategoryIncludingQuestions(int id);

        // Create checklist category
        Task<JobCategoryStructure> CreateCategory(JobCategoryStructure category);

        // Update checklist category
        Task UpdateCategory(JobCategoryStructure category);

        // Delete checklist category
        Task DeleteCategory(int id);

        // Update checklist category sequence
        Task UpdateCategorySequence(int categoryId, JobCategoryStructure checklistCategory);

        // Get all checklist questions by category Id
        Task<List<ChecklistQuestion>> GetChecklistQuestionsByCategoryId(int categoryId);

        // Get checklist question by Id
        Task<ChecklistQuestion> GetQuestionById(int categoryId, int questionId);

        // Create checklist question 
        Task<ChecklistQuestion> CreateQuestion(int categoryId, ChecklistQuestion question);

        // Update checklist question
        Task UpdateQuestion(int categoryId, ChecklistQuestion question);

        // Delete checklist question
        Task DeleteQuestion(int categoryId, int questionId);
        // Update checklist question sequence
        Task UpdateChecklistQuestionSequence(int categoryId, int checklisQuestionId, ChecklistQuestion checklistQuestion);
    }
}
