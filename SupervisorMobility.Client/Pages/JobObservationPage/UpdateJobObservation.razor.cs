using MudBlazor;

namespace SupervisorMobility.Client.Pages.JobObservationPage
{
    public partial class UpdateJobObservation
    {

        [Parameter]
        public int JobObservationId { get; set; }
        public JobObservation _jobObservation { get; set; } = new();

        //Objects
        private bool dense = false;
        private bool hover = true;
        private bool ronly = false;

        List<Plant> _plants { get; set; } = new();
        List<Area> _areas = new();
        List<Distribution> _distributions = new();
        List<Operation> _operations = new();

        public int plantId;
        public int areaId;
        public int distributionId;
        public int operationId;

        public DateTime? dateStart = DateTime.Today;
        public DateTime? dateEnd = DateTime.Today;

        public string observer { get; set; } = "Juan";
        public string operator1 { get; set; } = "Pedro";

        public int option { get; set; } = 1;
        public string anomaly { get; set; }

      
        public string time1HOE { get; set; } = "10 min";
        public string time2HOE { get; set; } = "20 min";
        string[] models = new string[5] { "P71A", "X247", "P71A", "X247", "P71A" };
        string[] cicles = new string[5] { "1 min", "2 min", "3 min", "4 min", "5 min" };

        public string models2;

        public string sArea;
        public string qArea;
        public string dArea;
        public string cArea;
        public string othersArea;

        public string identifiedActivity = "Actividad Identificada";
        public string ssvCommentary = "ssv comentario";
        public string operatorCommentary = "operator comment";
        public string ssvSignature = "Juan";
        public string operatorSignature ="Pedro";


        public string placeholder = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, " +
          "sed do eiusmod tempor incididuntut labore et dolore magna aliqua. Ut enim ad minim " +
          "veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo coe velit esse cillum";

        // Breadcrumb links
        private List<BreadcrumbItem> _links = new List<BreadcrumbItem>
        {
            new BreadcrumbItem("Home", href: "#"),
            new BreadcrumbItem("Job Observation", href: "", disabled: true)
        };

        protected async override Task OnInitializedAsync()
        {
            _jobObservation = await JobObservationService.GetJobObservationById(JobObservationId);
            _plants = await PlantServices.GetPlants();
            models2 = models[0] + "|" + models[1] + "|" + models[2] + "|" + models[3] + "|" + models[4] + "|";
        }
        private async void ShowAreas()
        {
            _areas = await AreaServices.GetAreas(plantId);
        }

        private async void ShowDistributions()
        {
            _distributions = await DistributionService.GetDistributions(plantId, areaId);
        }
        private async void ShowOperations()
        {
            _operations = await OperationService.GetOperations(plantId, areaId, distributionId);
        }

        private async Task CreateNewJobObservation()
        {
            models2 = models[0] + "|" + models[1] + "|" + models[2] + "|" + models[3] + "|" + models[4] + "|";

            Task.Delay(10);
        }


    }
}
