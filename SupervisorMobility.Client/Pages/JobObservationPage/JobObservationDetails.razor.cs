using MudBlazor;

namespace SupervisorMobility.Client.Pages.JobObservationPage
{
    public partial class JobObservationDetails
    {

        [Parameter]
        public int JobObservationId { get; set; }
        public JobObservation _jobObservation { get; set; } = new();
        List<Product> _products { get; set; } = new();

        //Objects
        private bool dense = false;
        private bool hover = false;
        private bool ronly = false;

        public string hour1 { get; set; }
        public string hour2 { get; set; }
        TimeSpan? endHour { get; set; }
        TimeSpan? startHour { get; set; }

        public int plantId;
        public int areaId;
        public int distributionId;
        public int operationId;

        public DateTime? dateStart = DateTime.Today;
        public DateTime? dateEnd = DateTime.Today;

        public string observer { get; set; } = "Juan";
        public string operator1 { get; set; } = "Pedro";

        int[] models = new int[5];
        string[] cicles = new string[5];


        public string placeholder = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, " +
          "sed do eiusmod tempor incididuntut labore et dolore magna aliqua. Ut enim ad minim " +
          "veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo coe velit esse cillum";

        // Breadcrumb links
        private List<BreadcrumbItem> _links = new List<BreadcrumbItem>
        {
            new BreadcrumbItem("Home", href: "/"),
            new BreadcrumbItem("Job Observation", href: "/jobobservation"),
            new BreadcrumbItem("Job Observation Details", href: "", disabled: true)
        };

        protected async override Task OnInitializedAsync()
        {
            _jobObservation = await JobObservationService.GetJobObservationById(JobObservationId);
            _products = await ProductService.GetProducts();
            var prod = _jobObservation.Models.Split('|');
            models[0] = Int32.Parse(prod[0]);
            models[1] = Int32.Parse(prod[1]);
            models[2] = Int32.Parse(prod[2]);
            models[3] = Int32.Parse(prod[3]);
            models[4] = Int32.Parse(prod[4]);
            cicles = _jobObservation.Cicles.Split('|');

            startHour = _jobObservation.DateStart?.TimeOfDay;
            endHour = _jobObservation.DateEnd?.TimeOfDay;
            Console.WriteLine(startHour);

        }



    }
}
