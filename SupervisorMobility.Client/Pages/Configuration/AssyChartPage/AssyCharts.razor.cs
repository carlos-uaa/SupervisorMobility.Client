using MudBlazor;

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


        //Objects
        private bool dense = false;
        private bool hover = true;
        private bool ronly = false;
        private string searchString = "";

        // Objects
        public List<AssyChart> _assychart { get; set; } = new();
        public List<ChecklistCategory> _checklistCategories { get; set; } = new();



        // Initialization
        protected async override Task OnInitializedAsync()
        {
            _assychart.Add(new AssyChart() { Stage = "T1", Distribution = "SET DOOR PROTECTOR LH", GOS = "TX2300-5NA_1", OperationName = "SET-FR FDR PROTECTOR LH", Model = "P71A" });
            _assychart.Add(new AssyChart() { Stage = "T1", Distribution = "SET DOOR PROTECTOR LH", GOS = "TX2400-5NA_1", OperationName = "SET FR DOOR PROTECTOR LH", Model = "P71A" });
            _assychart.Add(new AssyChart() { Stage = "T1", Distribution = "SET DOOR PROTECTOR LH", GOS = "NO GOS", OperationName = "RETIRO DE JIG EN PUERTAS LH", Model = "P71A" });
            _assychart.Add(new AssyChart() { Stage = "T1", Distribution = "SET DOOR PROTECTOR LH", GOS = "NO GOS", OperationName = "SET FR DOOR PROTECTOR LH", Model = "P71A" });
            _assychart.Add(new AssyChart() { Stage = "T1", Distribution = "SET DOOR PROTECTOR LH", GOS = "NO GOS", OperationName = "COLOCACION DE CARNAZA LH", Model = "P71A" });


        }


        //Filtering

        private bool FilterFunc(AssyChart element)
        {
            if (string.IsNullOrWhiteSpace(searchString))
                return true;
            //if (element.Sign.Contains(searchString, StringComparison.OrdinalIgnoreCase))
            //    return true;
            //if (element.Name.Contains(searchString, StringComparison.OrdinalIgnoreCase))
            //    return true;
            //if ($"{element.Number} {element.Position} {element.Molar}".Contains(searchString))
            //    return true;
            return false;
        }

    }
}
