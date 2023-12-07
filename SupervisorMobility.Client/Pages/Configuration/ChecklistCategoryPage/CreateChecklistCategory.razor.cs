using MudBlazor;

namespace SupervisorMobility.Client.Pages.Configuration.ChecklistCategoryPage
{
    public partial class CreateChecklistCategory
    {
        // Breadcrumb links
        private List<BreadcrumbItem> _links = new List<BreadcrumbItem>
        {
            new BreadcrumbItem("Home", href: "/"),
            new BreadcrumbItem("Configuration", href: "/configuration"),
            new BreadcrumbItem("Checklist categories", href: "/checklistcategories"),
            new BreadcrumbItem("New category", href: "", disabled: true),
        };

        // Objects
        ChecklistCategory _checklistCategory = new();

        // Create checklist category
        async void CreateCategoryAsync()
        {
            _checklistCategory.IsActive = true;
            var result = await ChecklistService.CreateCategory(_checklistCategory);
            NavigationManager.NavigateTo($"checklistcategories");
        }
    }
}
