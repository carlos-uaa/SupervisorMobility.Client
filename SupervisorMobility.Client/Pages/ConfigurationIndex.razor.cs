using MudBlazor;

namespace SupervisorMobility.Client.Pages
{
    public partial class ConfigurationIndex
    {
        // Breadcrumb links
        private List<BreadcrumbItem> _links = new List<BreadcrumbItem>
        {
            new BreadcrumbItem("Home", href: "#"),
            new BreadcrumbItem("Configuration", href: "", disabled: true)
        };
    }
}
