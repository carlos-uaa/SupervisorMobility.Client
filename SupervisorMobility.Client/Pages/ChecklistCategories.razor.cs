namespace SupervisorMobility.Client.Pages
{
    public partial class ChecklistCategories
    {
        public List<ChecklistCategory> _checklistCategories { get; set; } = new();

        protected async override Task OnInitializedAsync()
        {
            _checklistCategories = await ChecklistService.GetChecklistCategories();
        }

        void CategoryDetails(int categoryId)
        {
            NavigationManager.NavigateTo($"/checklistcategories/category/{categoryId}");
        }

        void EditCategory(int categoryId)
        {
            NavigationManager.NavigateTo($"checklistcategories/category/updatecategory/{categoryId}");
        }
    }
}
