using MudBlazor;

namespace SupervisorMobility.Client.Pages.Configuration.AssyChartPage
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
    }
}
