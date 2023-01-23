using Microsoft.JSInterop;
using MudBlazor;
using SupervisorMobility.Client.Data.Entities;
using SupervisorMobility.Client.Pages.Configuration.AssyChartPage;

namespace SupervisorMobility.Client.Pages.Configuration.AssyChartPage
{
    public partial class AssyCharts
    {
        // Breadcrumb links
        private List<BreadcrumbItem> _links = new List<BreadcrumbItem>
        {
            new BreadcrumbItem("Home", href: "#"),
            new BreadcrumbItem("Configuration", href: "/configuration"),
            new BreadcrumbItem("Assy Chart", href: "", disabled: true),
        };

        // Objects
        public List<AssyChart> _assychart { get; set; } = new List<AssyChart>();

        // Initialization
        protected async override Task OnInitializedAsync()
        {
            _assychart.Add(new AssyChart() { Stage = "T1", Distribution = "SET DOOR PROTECTOR LH", GOS = "TX2300-5NA_1", OperationName = "SET-FR FDR PROTECTOR LH", Model = "P71A" });
            _assychart.Add(new AssyChart() { Stage = "T1", Distribution = "SET DOOR PROTECTOR LH", GOS = "TX2400-5NA_1", OperationName = "SET FR DOOR PROTECTOR LH", Model = "P71A" });
            _assychart.Add(new AssyChart() { Stage = "T1", Distribution = "SET DOOR PROTECTOR LH", GOS = "NO GOS", OperationName = "RETIRO DE JIG EN PUERTAS LH", Model = "P71A" });
            _assychart.Add(new AssyChart() { Stage = "T1", Distribution = "SET DOOR PROTECTOR LH", GOS = "NO GOS", OperationName = "SET FR DOOR PROTECTOR LH", Model = "P71A" });
            _assychart.Add(new AssyChart() { Stage = "T1", Distribution = "SET DOOR PROTECTOR LH", GOS = "NO GOS", OperationName = "COLOCACION DE CARNAZA LH", Model = "P71A" });

        }



    }
}
