using Microsoft.JSInterop;
using MudBlazor;
using SupervisorMobility.Client.Data.Entities;
using SupervisorMobility.Client.Pages.Configuration.PlantPage;

namespace SupervisorMobility.Client.Pages.Configuration.AssyChartPage
{
    public partial class AssyCharts
    {
        // Breadcrumb links
        private List<BreadcrumbItem> _links = new List<BreadcrumbItem>
        {
            new BreadcrumbItem("Home", href: "#"),
            new BreadcrumbItem("Configuration", href: "/configuration"),
            new BreadcrumbItem("Assy Chart", href: "/assychart", disabled: true),
        };


        //Objects
        private bool dense = false;
        private bool hover = true;
        private bool ronly = false;
        private string searchString = "";

        // Objects
        public List<AssyChart> _assychart { get; set; } = new();


        protected async override Task OnInitializedAsync()
        {

            _assychart = await AssyChartServices.GetAssyCharts();
            Console.WriteLine(_assychart);
        }


        //Filtering

        private bool FilterFunc(AssyChart element)
        {
            //editar filtros aun falta informacion
            if (string.IsNullOrWhiteSpace(searchString))
                return true;
            if (element.GOS.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                return true;
            if (element.CCP.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                return true;
            if ($"{element.GOS} {element.CCP} {element.OperationDescription}".Contains(searchString))
                return true;
            return false;
        }

        void GoToUpdateAssyChart(int assychartid)
        {
            NavigationManager.NavigateTo($"assychart/updateassychart/{assychartid}");
        }

        async Task DeleteAssyChart(int assychartid)
        {

            bool confirm = await JSRuntime.InvokeAsync<bool>("confirm", $"Are you sure you want to delete this AssyChart?");

            if (confirm)
            {

                await AssyChartServices.DeleteAssyChart(assychartid);
                NavigationManager.NavigateTo($"assychart");
            }



        }




    }
}
