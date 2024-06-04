using Microsoft.JSInterop;
using MudBlazor;
namespace SupervisorMobility.Client.Pages.Inicio.ISPage.QualityAppearancePage
{
    public partial class CreateQualityAppearance
    {
        private List<BreadcrumbItem> _links;
        //User
        private string json = string.Empty;
        public User user = new();
        public bool logged = false;
        string partName = "FR DOOR INR RH";
        string partNumber = "A2477221001D";
        string programmed = "500";
        string inspector = "A. GARCIA";
        string hour = "22:45";
        string date = "14/02/24";
        string materialSpecification = "CR5";
        string partThickness = "0.77";
        string boreholesQuantity = "540";
        string laminate = "G1";
        string partNumberReleased = "01";

        string fracture = "OK";
        string radiusMalformation = "OK";
        string strikes = "OK";
        string thinning = "0.77";
        string lackOfMaterial = "OK";
        string corrosionControl = "v3";
        string fitting = "OK";
        string generalGrate = "0.18";
        string gap = "OK";
        string buttonMark = "V3";
        string mercedesMark = "V3";
        string burringHole = "V3";

        List<Product> _products { get; set; } = new();
        int productId = 0;

        protected async override Task OnInitializedAsync()
        {
            _links = new List<BreadcrumbItem>
                {
                    new BreadcrumbItem(text: Localizer["home"], href: "/"),
                    new BreadcrumbItem(text: Localizer["Appearance"], href: "", disabled: true)
                };
            BreadcrumbService.UpdateBreadcrumbs(_links);
            await GetUserAsync();
            logged = await HasPropertyAsync();
            if (!logged)
            {
                Snackbar.Clear();
                Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                Snackbar.Add($"Error You have to log in", Severity.Error);
                NavigationManager.NavigateTo($"/");
            }
            _products = await ProductsService.GetProducts();
            _products = _products.OrderBy(p => p.Description).ToList();
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