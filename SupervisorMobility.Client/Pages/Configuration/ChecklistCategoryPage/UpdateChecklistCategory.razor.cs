using MudBlazor;

namespace SupervisorMobility.Client.Pages.Configuration.ChecklistCategoryPage
{
    public partial class UpdateChecklistCategory
    {
        // Parameters
        [Parameter]
        public int CategoryId { get; set; }

        // Breadcrumb links 
        private List<BreadcrumbItem> _links = new List<BreadcrumbItem>
        {
            new BreadcrumbItem("Home", href: "#"),
            new BreadcrumbItem("Configuration", href: "/configuration"),
            new BreadcrumbItem("Checklist categories", href: "/checklistcategories"),
            new BreadcrumbItem("UpdateCategory", href: "", disabled: true),
        };

        // Objects
        public ChecklistCategory _checklistCategory { get; set; } = new();

        // Initialization
        protected override async Task OnParametersSetAsync()
        {
            ChecklistCategory dbCategory = await ChecklistService.GetCategoryById(CategoryId);
            _checklistCategory = dbCategory;
        }

        // Update category
        void UpdateCategory()
        {
            ChecklistService.UpdateCategory(_checklistCategory);
            NavigationManager.NavigateTo($"checklistcategories");
        }
    }
}
