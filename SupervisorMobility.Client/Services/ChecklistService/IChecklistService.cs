namespace SupervisorMobility.Client.Services.ChecklistService
{
    public interface IChecklistService
    {
        // Get all checklist categories
        Task<List<ChecklistCategory>> GetChecklistCategories();

        // Get checklist category by Id
        Task<ChecklistCategory> GetCategoryById(int id);

        // Get checklist category including questions
        Task<ChecklistCategory> GetCategoryIncludingQuestions(int id);

        // Create checklist category
        Task<ChecklistCategory> CreateCategory(ChecklistCategory category);

        // Update checklist category
        Task UpdateCategory(ChecklistCategory category);

        // Delete checklist category
        Task DeleteCategory(int id);

        // Update checklist category sequence
        Task UpdateCategorySequence(int categoryId, ChecklistCategory checklistCategory);
    }
}
