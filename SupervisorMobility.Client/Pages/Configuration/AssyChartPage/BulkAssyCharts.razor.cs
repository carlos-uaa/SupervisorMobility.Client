using MudBlazor;
using SpreadsheetLight;
using Microsoft.JSInterop;
using System.IO;
using DocumentFormat.OpenXml.Bibliography;
using OfficeOpenXml;

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

        //List Plants to do bulk
        private List<Plant> _plants = new List<Plant>();
        private int plantId = 0;
        private bool AllPlantsSwitch = true;

        private async Task DoBulk()
        {

            if (AllPlantsSwitch)
            {
                //query get all plants
                MemoryStream ms = new MemoryStream();
                using (SLDocument sl = new SLDocument())
                {
                    sl.SetCellValue("B3", "I love ASP.NET MVC");
                    sl.SaveAs(ms);
                }
                // this is important. Otherwise you get an empty file
                // (because you'd be at EOF after the stream is written to, I think...).
                ms.Position = 0;

                //using var streamRef = new DotNetStreamReference(ms);

                await JS.InvokeVoidAsync("saveAsFile", "Reportall.xlsx", ms);
            }
            else
            {
                List<AssyChart> _assychartsInPlant = new List<AssyChart>();

                _assychartsInPlant = await AssyChartServices.GetAssyChartsByPlant(plantId);

                var indexplant = _plants.FindIndex(plant => plant.PlantId == plantId);

                if (indexplant != -1)
                {
                  

                }
                //input in document



            }
        }

    }//end UploadAndBulk


}