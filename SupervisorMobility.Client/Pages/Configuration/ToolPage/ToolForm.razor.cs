using Microsoft.JSInterop;
using MudBlazor;
using SupervisorMobility.Client.Data.Entities;

namespace SupervisorMobility.Client.Pages.Configuration.ToolPage
{
    public partial class ToolForm
    {
        [Parameter]
        public int? ToolID { get; set; }

        [Parameter]
        public string? ToolName { get; set; }

        [Parameter]
        public EventCallback<bool> OnToolCreated { get; set; }

        int PageState = 0;


        // Breadcrumb links
        private List<BreadcrumbItem> _links = new List<BreadcrumbItem>();

        // Objects
        public Tool _Tool { get; set; } = new();

        //User
        private string json = string.Empty;
        public User user = new();
        public bool logged = false;



        public bool ExecBtnPress = false;

        protected async override Task OnInitializedAsync()
        {

            if(!string.IsNullOrEmpty(ToolName))
            {
                PageState = 1;
            }
            else
            {

                var currentUrl = NavigationManager.Uri;
                PageState = currentUrl.Contains("Create", StringComparison.OrdinalIgnoreCase) ? 1 : 0;
            
                if (PageState == 0) { 
                    PageState = currentUrl.Contains("Details", StringComparison.OrdinalIgnoreCase) ? 2 : 0;
                }
                if (PageState == 0) { 
                    PageState = currentUrl.Contains("Update", StringComparison.OrdinalIgnoreCase) ? 3 : 0;
                }

            }

            await GetUserAsync();
            logged = await HasPropertyAsync();
            if (!logged)
            {
                Snackbar.Clear();
                Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                Snackbar.Add($"Error You have to log in", Severity.Error);
                NavigationManager.NavigateTo($"/");
            }
            else
            {

            }

            _links = new List<BreadcrumbItem>
            {
                new BreadcrumbItem(text: Localizer["home"], href: "/"),
                new BreadcrumbItem(text: Localizer["configuration"], href: "/configuration"),
                new BreadcrumbItem(text: Localizer["Tools"], href: "Tool")
            };

            switch (PageState)
            {
                case 1:
                    _Tool = new Tool();
                    _Tool.IsActive =  true;
                    if (!string.IsNullOrEmpty(ToolName))
                    {
                        _Tool.ToolName = ToolName;
                    }
                    _links.Add(new BreadcrumbItem(text: Localizer["create"], href: "", disabled: true));
                    break;
                case 2:
                    _Tool = await ToolsServices.GetToolById((int)ToolID);
                    _links.Add(new BreadcrumbItem(text: Localizer["details"], href: "", disabled: true));
                    break;
                case 3:
                    _Tool = await ToolsServices.GetToolById((int)ToolID);
                    _links.Add(new BreadcrumbItem(text: Localizer["update"], href: "", disabled: true));
                    break;
            }


            BreadcrumbService.UpdateBreadcrumbs(_links);
        }

        private void EditTool(int depId)
        {
           NavigationManager.NavigateTo($"Tool/Update/{depId}", forceLoad: true);
        }

        //Local storage user
        private async Task GetUserAsync()
        {
            if (!await TryGetAsync())
                user = new();
        }

        private async Task<bool> TryGetAsync()
        {
            bool hasProperty = await HasPropertyAsync();
            if (hasProperty)
            {
                json = await JSRuntime.InvokeAsync<string>("localStorage.getItem", "user");
                user = JsonSerializer.Deserialize<User>(json) ?? new();
            }
            return hasProperty;
        }

        private async Task<bool> HasPropertyAsync()
            => await JSRuntime.InvokeAsync<bool>("localStorage.hasOwnProperty", "user");

        private async void ExectToolAsync()
        {
            ExecBtnPress = true;
            switch (PageState)
                            {
                                case 1:

                    var result = await ToolsServices.CreateTool(_Tool);

                    if (result != null)
                    {
                        if (!string.IsNullOrEmpty(ToolName))
                        {
                            await OnToolCreated.InvokeAsync(true);
                        }
                        else
                        {
                            Snackbar.Clear();
                            Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                            Snackbar.Add($"{Localizer1["ToolCreateSucces"]}", Severity.Info);
                            NavigationManager.NavigateTo($"Tool");
                        }
                    }
                    else
                    {
                        Snackbar.Clear();
                        Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                        Snackbar.Add($"{Localizer1["ToolCreateError"]}", Severity.Error);
                        ExecBtnPress = false;
                    }
                    break;
                case 3:
                    var resultUpdate = await ToolsServices.UpdateTool(_Tool);

                    if (resultUpdate != null)
                    {
                        NavigationManager.NavigateTo($"Tool");
                        Snackbar.Clear();
                        Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                        Snackbar.Add($"{Localizer1["ToolUpdateSucces"]}", Severity.Info);
                    }
                    else
                    {
                        Snackbar.Clear();
                        Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                        Snackbar.Add($"{Localizer1["ToolUpdateError"]}", Severity.Error);
                        ExecBtnPress = false;
                    }

                    break;
                }
        }


    }
}