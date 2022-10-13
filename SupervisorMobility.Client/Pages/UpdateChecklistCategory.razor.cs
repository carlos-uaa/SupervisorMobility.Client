namespace SupervisorMobility.Client.Pages
{
    public partial class UpdateChecklistCategory
    {
        [Parameter]
        public int CategoryId { get; set; }

        public ChecklistCategory _checklistCategory { get; set; } = new();

        protected override async Task OnParametersSetAsync()
        {
            ChecklistCategory dbCategory = await ChecklistService.GetCategoryById(CategoryId);
            _checklistCategory = dbCategory;
        }

        void UpdateCategory()
        {
            ChecklistService.UpdateCategory(_checklistCategory);
            NavigationManager.NavigateTo($"checklistcategories");
        }
    }
}
