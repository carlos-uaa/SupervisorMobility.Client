using MudBlazor;

namespace SupervisorMobility.Client.Pages.Configuration.GroupPage
{
    public partial class CreateGroup
    {
        // Breadcrumb links
        private List<BreadcrumbItem> _links;
        protected async override Task OnInitializedAsync()
        {

            _links = new List<BreadcrumbItem>
        {
            new BreadcrumbItem(text: Localizer["home"], href: "#"),
            new BreadcrumbItem(text: Localizer["configuration"], href: "/configuration"),
            new BreadcrumbItem(text: Localizer["GroupTitle"], href: "/groups"),
            new BreadcrumbItem(text: Localizer["GroupNew"], href: "", disabled: true)
        };
        }

        // Objects
        Group _group = new();

        // Create group
        async void CreateGroupAsync()
        {
            _group.IsActive = true;
            var result = await GroupService.CreateGroup(_group);
            NavigationManager.NavigateTo($"groups");
        }
    }
}
