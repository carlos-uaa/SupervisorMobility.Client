namespace SupervisorMobility.Client.Pages
{
    public partial class CreateChecklistCategory
    {
        ChecklistCategory _checklistCategory = new();

        async void CreateCategoryAsync()
        {
            var result = await ChecklistService.CreateCategory(_checklistCategory);
            NavigationManager.NavigateTo($"checklistcategories");
        }
    }
}
