using MudBlazor;

namespace SupervisorMobility.Client.Pages.Configuration.GlosaryPage
{
    public partial class UpdateGlosaryWord
    {
        // Parameters
        [Parameter]
        public int GlosaryWordId { get; set; }

        // Breadcrumb links
        private List<BreadcrumbItem> _links = new List<BreadcrumbItem>
        {
            new BreadcrumbItem("Home", href: "#"),
            new BreadcrumbItem("Configuration", href: "/configuration"),
            new BreadcrumbItem("Glosary", href: "/glosary"),
            new BreadcrumbItem("Update Glosary Word", href: "", disabled: true)
        };

        // Objects
        public Glosary _glosary { get; set; } = new();

        // Initialization
        protected override async Task OnParametersSetAsync()
        {
            Glosary dbGlosary = await GlosaryService.GetGlosaryWordbyId(GlosaryWordId);
            _glosary = dbGlosary;
        }
        async void UpdateGlosaryWordAsync()
        {

            var result = await GlosaryService.UpdateGlosaryWord(_glosary);

            if (result)
            {
                NavigationManager.NavigateTo($"glosary");
            }
        }
    }
}
