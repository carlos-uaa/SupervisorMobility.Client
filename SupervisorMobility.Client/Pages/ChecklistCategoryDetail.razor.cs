using MudBlazor;

namespace SupervisorMobility.Client.Pages
{
    public partial class ChecklistCategoryDetail
    {
        [Parameter]
        public int CategoryId { get; set; }

        ChecklistCategory _checklistCategory = new();

        private List<BreadcrumbItem> _links = new List<BreadcrumbItem>
        {
            new BreadcrumbItem("Home", href: "#"),
            new BreadcrumbItem("Configuration", href: "/configuration"),
            new BreadcrumbItem("Checklist categories", href: "/checklistcategories"),
            new BreadcrumbItem("CategoryDetail", href: "", disabled: true),
        };

        protected override async Task OnParametersSetAsync()
        {
            _checklistCategory = await ChecklistService.GetCategoryById(CategoryId);
        }
    }
}
