using MudBlazor;

namespace SupervisorMobility.Client.Pages
{
    public partial class CreateChecklistCategory
    {
        ChecklistCategory _checklistCategory = new();

        private List<BreadcrumbItem> _links = new List<BreadcrumbItem>
        {
            new BreadcrumbItem("Home", href: "#"),
            new BreadcrumbItem("Configuration", href: "/configuration"),
            new BreadcrumbItem("Checklist categories", href: "/checklistcategories"),
            new BreadcrumbItem("New category", href: "", disabled: true),
        };

        async void CreateCategoryAsync()
        {
            var result = await ChecklistService.CreateCategory(_checklistCategory);
            NavigationManager.NavigateTo($"checklistcategories");
        }
    }
}
