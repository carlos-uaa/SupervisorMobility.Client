using MudBlazor;

namespace SupervisorMobility.Client.Pages
{
    public partial class UpdateChecklistCategory
    {
        [Parameter]
        public int CategoryId { get; set; }

        public ChecklistCategory _checklistCategory { get; set; } = new();

        private List<BreadcrumbItem> _links = new List<BreadcrumbItem>
        {
            new BreadcrumbItem("Home", href: "#"),
            new BreadcrumbItem("Configuration", href: "/configuration"),
            new BreadcrumbItem("Checklist categories", href: "/checklistcategories"),
            new BreadcrumbItem("UpdateCategory", href: "", disabled: true),
        };

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
