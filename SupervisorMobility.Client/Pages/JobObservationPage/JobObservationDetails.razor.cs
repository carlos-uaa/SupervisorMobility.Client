using MudBlazor;

namespace SupervisorMobility.Client.Pages.JobObservationPage
{
    public partial class JobObservationDetails
    {

        [Parameter]
        public int JobObservationId { get; set; }
        public JobObservation _jobObservation { get; set; } = new();

        //Objects
        private bool dense = false;
        private bool hover = false;
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


        string[] models = new string[5];
        string[] cicles = new string[5];


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
            models = _jobObservation.Models.Split('|');
            cicles = _jobObservation.Cicles.Split('|');
            
            _plants = await PlantServices.GetPlants();
        }



    }
}
