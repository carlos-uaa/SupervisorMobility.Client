using Microsoft.JSInterop;
using MudBlazor;
using SupervisorMobility.Client.Data.Entities;
using SupervisorMobility.Client.Pages.Configuration.PlantPage;
using static MudBlazor.CategoryTypes;

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
            if (string.IsNullOrWhiteSpace(searchString))
                return true;
            if (element.Plant.Description.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                return true;
            if (element.Area.Description.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                return true;
            if (element.GOS.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                return true;
            if (element.CCP.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                return true;
            if (element.HOE.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                return true;
            if (element.Operation.Description.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                return true;
            if (element.Product.Description.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                return true;
            if (element.CreationDate.ToString().Contains(searchString, StringComparison.OrdinalIgnoreCase))
                return true;
            if (element.ModificationDate.ToString().Contains(searchString, StringComparison.OrdinalIgnoreCase))
                return true;
            if ($"{element.GOS} {element.CCP} {element.OperationDescription}".Contains(searchString))
                return true;
            return false;
        }

        private TableGroupDefinition<AssyChart> _groupDefinition = new()
        {
            GroupName = "Group",
            Selector = (e) => e.Product.Description
        };

        void GoToUpdateAssyChart(int assychartid)
        {
            NavigationManager.NavigateTo($"assychart/updateassychart/{assychartid}");
        }

        async Task DeleteAssyChart(int assychartid)
        {

            bool confirm = await JSRuntime.InvokeAsync<bool>("confirm", $"Are you sure you want to delete this AssyChart?");

            if (confirm)
            {
                _assychart.RemoveAll(assychart => assychart.AssyChardId == assychartid);
                await AssyChartServices.DeleteAssyChart(assychartid);
            }



        }


        private bool visible = false;
        private void OpenDialogGOS(string ruta)
        {
            visible = true;
        }
        void Close() => visible = false;

        private DialogOptions dialogOptions = new() { CloseOnEscapeKey = true, MaxWidth = MaxWidth.Large, FullWidth = true };

    }
}
