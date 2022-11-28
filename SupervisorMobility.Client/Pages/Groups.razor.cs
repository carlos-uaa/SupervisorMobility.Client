using Microsoft.JSInterop;
using MudBlazor;

namespace SupervisorMobility.Client.Pages
{
    public partial class Groups
    {
        //Breadcrumb links
        private List<BreadcrumbItem> _links = new List<BreadcrumbItem>
        {
            new BreadcrumbItem("Home", href: "#"),
            new BreadcrumbItem("Configuration", href: "/configuration"),
            new BreadcrumbItem("Groups", href: "", disabled: true)
        };

        // Objects
        public List<Group> _groups { get; set; } = new();
        Group _group = new();

        // Initialization
        protected async override Task OnInitializedAsync()
        {
            _groups = await GroupService.GetGroups();
        }

        // Create group
        void CreateGroup()
        {
            NavigationManager.NavigateTo($"groups/creategroup");
        }

        // Delete group
        async Task DeleteGroup(int groupId)
        {
            bool confirm = await JSRuntime.InvokeAsync<bool>("confirm", $"Are you sure you want to delete this group?");

            if (confirm)
            {
                _groups.RemoveAll(group => group.GroupId == groupId);
                await GroupService.DeleteGroup(groupId);
            }
        }

        // Update group
        void UpdateGroup(int groupId)
        {
            NavigationManager.NavigateTo($"groups/updategroup/{groupId}");
        }
    }
}
