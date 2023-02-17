using MudBlazor;

namespace SupervisorMobility.Client.Pages.Configuration.GroupPage
{
    public partial class UpdateGroup
    {
        // Parameters
        [Parameter]
        public int GroupId { get; set; }

        // Breadcrumb links
        private List<BreadcrumbItem> _links = new List<BreadcrumbItem>
        {
            new BreadcrumbItem("Home", href: "#"),
            new BreadcrumbItem("Configuration", href: "/configuration"),
            new BreadcrumbItem("Groups", href: "/groups"),
            new BreadcrumbItem("UpdateGroup", href: "", disabled: true)
        };

        // Objects
        public Group _group { get; set; } = new();

        // Initialization
        protected override async Task OnParametersSetAsync()
        {
            Group dbGroup = await GroupService.GetGroupById(GroupId);
            _group = dbGroup;
        }

        // Update group
        async void UpdateGroupAsync()
        {

            var result = await GroupService.UpdateGroup(_group);

            if (result)
            {
                NavigationManager.NavigateTo($"groups");
            }
        }
    }
}
