using MudBlazor;

namespace SupervisorMobility.Client.Pages.Configuration.GroupPage
{
    public partial class UpdateGroup
    {
        // Parameters
        [Parameter]
        public int GroupId { get; set; }

        // Breadcrumb links
        private List<BreadcrumbItem> _links;

        // Objects
        public Group _group { get; set; } = new();

        // Initialization
        protected override async Task OnParametersSetAsync()
        {
            Group dbGroup = await GroupService.GetGroupById(GroupId);
            _group = dbGroup;
            _links = new List<BreadcrumbItem>
            {
                new BreadcrumbItem(text: Localizer["home"], href: "/"),
                new BreadcrumbItem(text: Localizer["configuration"], href: "/configuration"),
                new BreadcrumbItem(text: Localizer["GroupTitle"], href: "/groups"),
                new BreadcrumbItem(text: Localizer0["update"] + " " +  _group.Description, href: "", disabled: true)
            };
            BreadcrumbService.UpdateBreadcrumbs(_links);

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
