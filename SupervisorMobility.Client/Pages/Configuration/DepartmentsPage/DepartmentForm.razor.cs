using Microsoft.JSInterop;
using MudBlazor;
using SupervisorMobility.Client.Data.Entities;

namespace SupervisorMobility.Client.Pages.Configuration.DepartmentsPage
{
    public partial class DepartmentForm
    {
        [Parameter]
        public int? DepartmentID { get; set; }
        int PageState = 0;


        // Breadcrumb links
        private List<BreadcrumbItem> _links = new List<BreadcrumbItem>();

        // Objects
        public Department _Department { get; set; } = new();

        //User
        private string json = string.Empty;
        public User user = new();
        public bool logged = false;



        public bool ExecBtnPress = false;

        protected async override Task OnInitializedAsync()
        {
            var currentUrl = NavigationManager.Uri;
            PageState = currentUrl.Contains("Create", StringComparison.OrdinalIgnoreCase) ? 1 : 0;
            
            if (PageState == 0) { 
                PageState = currentUrl.Contains("Details", StringComparison.OrdinalIgnoreCase) ? 2 : 0;
            }
            if (PageState == 0) { 
                PageState = currentUrl.Contains("Update", StringComparison.OrdinalIgnoreCase) ? 3 : 0;
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
                new BreadcrumbItem(text: Localizer["Departments"], href: "Departments")
            };

            switch (PageState)
            {
                case 1:
                    _Department = new Department();
                    _Department.IsActive =  true;
                    _links.Add(new BreadcrumbItem(text: Localizer["create"], href: "", disabled: true));
                    break;
                case 2:
                    _Department = await DepartmentsServices.GetDepartmentById((int)DepartmentID);
                    _links.Add(new BreadcrumbItem(text: Localizer["details"], href: "", disabled: true));
                    break;
                case 3:
                    _Department = await DepartmentsServices.GetDepartmentById((int)DepartmentID);
                    _links.Add(new BreadcrumbItem(text: Localizer["update"], href: "", disabled: true));
                    break;
            }


            BreadcrumbService.UpdateBreadcrumbs(_links);
        }

        private void EditDepartment(int depId)
        {
           NavigationManager.NavigateTo($"Department/Update/{depId}", forceLoad: true);
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

        private async void ExectDepartmentAsync()
        {
            ExecBtnPress = true;
            switch (PageState)
                            {
                                case 1:

                    var result = await DepartmentsServices.CreateDepartment(_Department);

                    if (result != null)
                    {
                        NavigationManager.NavigateTo($"Department");
                        Snackbar.Clear();
                        Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                        Snackbar.Add($"{Localizer1["DepartmentCreateSucces"]}", Severity.Info);
                    }
                    else
                    {
                        Snackbar.Clear();
                        Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                        Snackbar.Add($"{Localizer1["DepartmentCreateError"]}", Severity.Error);
                        ExecBtnPress = false;
                    }
                    break;
                case 3:
                    var resultUpdate = await DepartmentsServices.UpdateDepartment(_Department);

                    if (resultUpdate != null)
                    {
                        NavigationManager.NavigateTo($"Department");
                        Snackbar.Clear();
                        Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                        Snackbar.Add($"{Localizer1["DepartmentUpdateSucces"]}", Severity.Info);
                    }
                    else
                    {
                        Snackbar.Clear();
                        Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                        Snackbar.Add($"{Localizer1["DepartmentUpdateError"]}", Severity.Error);
                        ExecBtnPress = false;
                    }

                    break;
                }
        }


    }
}