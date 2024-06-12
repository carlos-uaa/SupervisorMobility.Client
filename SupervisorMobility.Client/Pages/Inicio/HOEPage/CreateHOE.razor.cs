using Microsoft.JSInterop;
using MudBlazor;
using SupervisorMobility.Client.Data.Entities;
using System.Globalization;

namespace SupervisorMobility.Client.Pages.Inicio.HOEPage
{
    public partial class CreateHOE
    {
        private List<BreadcrumbItem> _links;
        Appearance _appearance { get; set; } = new();
        public List<DataPanel> _dataPanelsCategories { get; set; } = new();
        //User
        private string json = string.Empty;
        public User user = new();
        public bool logged = false;

        public string signatureUser = string.Empty;
        public bool isHeader = false;
        public int userType = 0;

        string partNumber = "";
        string partModel = "";
        string programmed = "500";
        string inspector = "A. GARCIA";
        string hour = "22:45";
        string date = "14/02/24";

        string ssvImage = string.Empty;
        string svImage = string.Empty;
        string operatorImage = string.Empty;

        public string hour1 { get; set; }
        TimeSpan? startHour = new TimeSpan(00, 00, 00);
        DateTime newDate1;


        protected async override Task OnInitializedAsync()
        {

            _links = new List<BreadcrumbItem>
                {
                    new BreadcrumbItem(text: Localizer["home"], href: "/"),
                    new BreadcrumbItem(text: Localizer["hoe"], href: "", disabled: true)
                };
            BreadcrumbServices.UpdateBreadcrumbs(_links);
            await GetUserAsync();
            logged = await HasPropertyAsync();
            if (!logged)
            {
                Snackbar.Clear();
                Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                Snackbar.Add($"Error You have to log in", Severity.Error);
                NavigationManager.NavigateTo($"/");
            }




   
            StateHasChanged();
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








    }
}