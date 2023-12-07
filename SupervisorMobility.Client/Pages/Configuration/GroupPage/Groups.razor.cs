using Microsoft.JSInterop;
using MudBlazor;

namespace SupervisorMobility.Client.Pages.Configuration.GroupPage
{
    public partial class Groups
    {
        //Breadcrumb links
        private List<BreadcrumbItem> _links;

        // Objects
        public List<Group> _groups { get; set; } = new();
        Group _group = new();

        // Initialization
        protected async override Task OnInitializedAsync()
        {
            _groups = await GroupService.GetGroups();
            _links = new List<BreadcrumbItem>
        {
            new BreadcrumbItem(text: Localizer["home"], href: "/"),
            new BreadcrumbItem(text: Localizer["configuration"], href: "/configuration"),
            new BreadcrumbItem(text: Localizer["GroupTitle"], href: "", disabled: true)
        };
        }

        // Create group
        void CreateGroup()
        {
            NavigationManager.NavigateTo($"groups/creategroup");
        }

        // Delete group
        async Task DeleteGroup(int groupId)
        {
            _groups.RemoveAll(group => group.GroupId == groupId);
            await GroupService.DeleteGroup(groupId);
            
            visibleDelete = false;
        }

        // Update group
        void UpdateGroup(int groupId)
        {
            NavigationManager.NavigateTo($"groups/updategroup/{groupId}");
        }
        private string searchString = "";

        private bool FilterFunc(Group element)
        {
            if (string.IsNullOrWhiteSpace(searchString))
                return true;
            if (element.GroupId.ToString().Contains(searchString, StringComparison.OrdinalIgnoreCase))
                return true;
            if (element.Code.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                return true;
            if (element.Description.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                return true;
            if ($"{element.GroupId} {element.Code} {element.Description}".Contains(searchString))
                return true;
            return false;
        }


        //Delete Group
        private bool visibleDelete = false;
        public int deleteGroupId = 0;
        private void OpenDeleteDialog(int deleteId)
        {
            deleteGroupId = deleteId;
            visibleDelete = true;
        }
        void CloseDeleteModal() => visibleDelete = false;
        
        DialogOptions dialogDeleteOptions =  new() { CloseOnEscapeKey = true, MaxWidth = MaxWidth.ExtraSmall, FullWidth = true, Position = DialogPosition.TopCenter, DisableBackdropClick = true, CloseButton = true };


    }
}
