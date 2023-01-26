using MudBlazor;

namespace SupervisorMobility.Client.Pages.Configuration.AssyChartPage
{
    public partial class CreateAssyChart
    {
        //objects
        AssyChart _newassychart = new();
        List<Plant> _plants { get; set; } = new();
        List<Area> _areas = new();
        List<Distribution> _distributions { get; set; } = new();


        private List<BreadcrumbItem> _links = new List<BreadcrumbItem>
        {
            new BreadcrumbItem("Home", href: "#"),
            new BreadcrumbItem("Configuration", href: "/configuration"),
            new BreadcrumbItem("Assy Chart", href: "/assychart"),
            new BreadcrumbItem("New Assy Chart", href: "", disabled: true),
        };

        //Inizialize
        protected async override Task OnInitializedAsync()
        {
            _plants = await PlantServices.GetPlants();
        }



        //Function Update Selectd

        async void UpdateAreas()
        {
            _areas = await AreaServices.GetAreas(_newassychart.PlantId);
        }

        private async void UpdateDistributions()
        {
            _distributions = await DistributionServices.GetDistributions(_newassychart.PlantId, _newassychart.AreaId);
        }

        async void CreateNewAssyChartAsync()
        {
            var result = await AssyChartService.CreateAssyChart(_newassychart);
            NavigationManager.NavigateTo("/assychart");
        }

        void CancelCreateAssyChart()
        {
            NavigationManager.NavigateTo("/assychart");
        }

    }
}
