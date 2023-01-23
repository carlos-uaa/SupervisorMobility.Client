using MudBlazor;

namespace SupervisorMobility.Client.Pages.Configuration.GroupPage
{
    public partial class CreateGroup
    {
        // Breadcrumb links
        private List<BreadcrumbItem> _links = new List<BreadcrumbItem>
        {
            new BreadcrumbItem("Home", href: "#"),
            new BreadcrumbItem("Configuration", href: "/configuration"),
            new BreadcrumbItem("Groups", href: "/groups"),
            new BreadcrumbItem("New group", href: "", disabled: true)
        };

        // Objects
        Group _group = new();

        // Create group
        async void CreateGroupAsync()
        {
            var result = await GroupService.CreateGroup(_group);
            NavigationManager.NavigateTo($"groups");
        }
    }
}
