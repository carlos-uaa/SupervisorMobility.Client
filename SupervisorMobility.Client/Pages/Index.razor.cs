using MudBlazor;

namespace SupervisorMobility.Client.Pages
{
    public partial class Index
    {
        //[CascadingParameter]
        //public Dictionary<string, Glosary> _glosaryInfo { get; set; }

        private List<BreadcrumbItem> _links = new List<BreadcrumbItem>
        {
            new BreadcrumbItem("Home", href: "#", disabled: true)
        };
    }
}
