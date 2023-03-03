using MudBlazor;

namespace SupervisorMobility.Client.Pages
{
    public partial class Index
    {

        private List<BreadcrumbItem> _links = new List<BreadcrumbItem>
        {
            new BreadcrumbItem("Home", href: "#", disabled: true)
        };
    }
}
