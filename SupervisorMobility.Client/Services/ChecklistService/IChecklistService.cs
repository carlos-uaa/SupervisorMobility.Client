namespace SupervisorMobility.Client.Services.ChecklistService
{
    public interface IChecklistService
    {
        // Get all checklist categories
        Task<List<ChecklistCategory>> GetChecklistCategories();

        // Create checklist category
        Task<ChecklistCategory> CreateCategory(ChecklistCategory category);
    }
}
