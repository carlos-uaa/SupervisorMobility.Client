using Microsoft.JSInterop;
using MudBlazor;
using SupervisorMobility.Client.Data.Entities;

namespace SupervisorMobility.Client.Pages.Configuration.MaterialPage
{
    public partial class MaterialForm
    {
        [Parameter]
        public int? MaterialID { get; set; }

        [Parameter]
        public string? MaterialName { get; set; }

        [Parameter]
        public EventCallback<bool> OnMaterialCreated { get; set; }

        int PageState = 0;


        // Breadcrumb links
        private List<BreadcrumbItem> _links = new List<BreadcrumbItem>();

        // Objects
        public Material _Material { get; set; } = new();

        //User
        private string json = string.Empty;
        public User user = new();
        public bool logged = false;



        public bool ExecBtnPress = false;

        protected async override Task OnInitializedAsync()
        {
            if (!string.IsNullOrEmpty(MaterialName))
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
                new BreadcrumbItem(text: Localizer["Materials"], href: "Materials")
            };

            switch (PageState)
            {
                case 1:
                    _Material = new Material();
                    _Material.IsActive =  true;
                    if (!string.IsNullOrEmpty(MaterialName))
                    {
                        _Material.MaterialName = MaterialName;
                    }
                    _links.Add(new BreadcrumbItem(text: Localizer["create"], href: "", disabled: true));
                    break;
                case 2:
                    _Material = await MaterialsServices.GetMaterialById((int)MaterialID);
                    _links.Add(new BreadcrumbItem(text: Localizer["details"], href: "", disabled: true));
                    break;
                case 3:
                    _Material = await MaterialsServices.GetMaterialById((int)MaterialID);
                    _links.Add(new BreadcrumbItem(text: Localizer["update"], href: "", disabled: true));
                    break;
            }


            BreadcrumbService.UpdateBreadcrumbs(_links);
        }

        private void EditMaterial(int depId)
        {
           NavigationManager.NavigateTo($"Material/Update/{depId}", forceLoad: true);
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

        private async void ExectMaterialAsync()
        {
            ExecBtnPress = true;
            switch (PageState)
            {
                case 1:

                    var result = await MaterialsServices.CreateMaterial(_Material);

                    if (result != null)
                    {
                        if (!string.IsNullOrEmpty(MaterialName))
                        {
                            await OnMaterialCreated.InvokeAsync(true);
                        }
                        else
                        {
                            NavigationManager.NavigateTo($"Material");
                            Snackbar.Clear();
                            Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                            Snackbar.Add($"{Localizer1["MaterialCreateSucces"]}", Severity.Info);
                        }
                    }
                    else
                    {
                        Snackbar.Clear();
                        Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                        Snackbar.Add($"{Localizer1["MaterialCreateError"]}", Severity.Error);
                        ExecBtnPress = false;
                    }
                    break;
                case 3:
                    var resultUpdate = await MaterialsServices.UpdateMaterial(_Material);

                    if (resultUpdate != null)
                    {
                        NavigationManager.NavigateTo($"Material");
                        Snackbar.Clear();
                        Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                        Snackbar.Add($"{Localizer1["MaterialUpdateSucces"]}", Severity.Info);
                    }
                    else
                    {
                        Snackbar.Clear();
                        Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                        Snackbar.Add($"{Localizer1["MaterialUpdateError"]}", Severity.Error);
                        ExecBtnPress = false;
                    }

                    break;
                }
        }


    }
}