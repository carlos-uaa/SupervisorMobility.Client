using MudBlazor;
using SpreadsheetLight;
using Microsoft.JSInterop;
using System.IO;
using DocumentFormat.OpenXml.Bibliography;


namespace SupervisorMobility.Client.Pages.Configuration.AssyChartPage
{
    public partial class BulkAssyCharts
    {
        // Breadcrumb links
        private List<BreadcrumbItem> _links = new List<BreadcrumbItem>
        {
            new BreadcrumbItem("Home", href: "#"),
            new BreadcrumbItem("Configuration", href: "/configuration"),
            new BreadcrumbItem("Assy Chart", href: "/assychart"),
            new BreadcrumbItem("Bulk Assy Chart", href: "/BulkAssyCharts", disabled: true),
        };

        protected async override Task OnInitializedAsync()
        {
            _plants = await PlantServices.GetPlants();
        }

        //List Plants to do bulk
        private List<Plant> _plants = new List<Plant>();
        private int plantId = 0;
        private bool AllPlantsSwitch = true;

        private async Task DoBulk()
        {

            if (AllPlantsSwitch)
            {
               //peticion de archivo todas las plantas
               await FileUpDoServices.DownloadFileFromAllPlants();
            }
            else
            {
                await FileUpDoServices.DownloadFileFromOnePlant(plantId);
            }
        }

    }//end UploadAndBulk


}