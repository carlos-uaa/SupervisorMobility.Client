using Microsoft.JSInterop;
using MudBlazor;
using System.Globalization;
using System.Timers;

namespace SupervisorMobility.Client.Pages.JobObservationPage
{
    public partial class CreateJobObservation
    {

        [Parameter]
        public string date { get; set; }
        public string hour1 { get; set; }
        public string hour2 { get; set; }
        TimeSpan? startHour = new TimeSpan(12, 00, 00);
        TimeSpan? endHour = new TimeSpan(13, 00, 00);
        DateTime newDate1;
        DateTime newDate2;

        List<Plant> _plants { get; set; } = new();
        List<Product> _products { get; set; } = new();
        List<Area> _areas = new();
        List<Distribution> _distributions = new();
        List<Operation> _operations = new();
        public JobObservation _jobObservation { get; set; } = new();

        int[] models = new int[5];
        string[] cicles = new string[5] { "00:00:00", "00:00:00", "00:00:00", "00:00:00", "00:00:00" };

        public string placeholder = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, " +
          "sed do eiusmod tempor incididuntut labore et dolore magna aliqua. Ut enim ad minim " +
          "veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo coe velit esse cillum";

        //timer
        const string DEFAULT_TIME = "00:00:00";
        string elapsedTime = DEFAULT_TIME;
        System.Timers.Timer timer = new System.Timers.Timer(1);
        DateTime startTime = DateTime.Now;
        bool isRunning = false;
        public int opt = 1;

        // Breadcrumb links
        private List<BreadcrumbItem> _links = new List<BreadcrumbItem>
        {
            new BreadcrumbItem("Home", href: "/"),
            new BreadcrumbItem("Job Observation", href: "/jobobservation"),
            new BreadcrumbItem("New Job Observation", href: "", disabled: true)
        };

        protected async override Task OnInitializedAsync()
        {
            date = date.Replace("-", "/");

            _jobObservation.IsActive= true;
            _jobObservation.DateStart = DateTime.ParseExact(date, "d/M/yyyy", null);
            _jobObservation.DateEnd = DateTime.ParseExact(date, "d/M/yyyy", null);
            _jobObservation.Option = 3;
            _plants = await PlantServices.GetPlants();
            _products = await ProductService.GetProducts();
        }
        private async void ShowAreas()
        {
            _jobObservation.AreaId = 0;
            _jobObservation.DistributionId = 0;
            _jobObservation.OperationId = 0;
            _areas = await AreaServices.GetAreas(_jobObservation.PlantId);
        }

        private async void ShowDistributions()
        {
            _jobObservation.DistributionId = 0;
            _jobObservation.OperationId = 0;
            _distributions = await DistributionService.GetDistributions(_jobObservation.PlantId, _jobObservation.AreaId);
        }
        private async void ShowOperations()
        {
            _jobObservation.OperationId = 0;
            _operations = await OperationService.GetOperations(_jobObservation.PlantId, _jobObservation.AreaId, _jobObservation.DistributionId);
        }

        private async Task CreateNewJobObservation()
        {
            Console.WriteLine(startHour);
            hour1 = _jobObservation.DateStart?.ToShortDateString() + $" {startHour}";
            hour2 = _jobObservation.DateEnd?.ToShortDateString() + $" {endHour}";

            if (DateTime.TryParseExact(hour1, $"d/M/yyyy HH:mm:ss", null, DateTimeStyles.None, out newDate1))
            {
                Console.WriteLine(newDate1);
            }
            else
                Console.WriteLine("Unable to parse '{0}'", hour1);


            if (DateTime.TryParseExact(hour2, $"d/M/yyyy HH:mm:ss", null, DateTimeStyles.None, out newDate2))
            {
                Console.WriteLine(newDate2);
            }
            else
                Console.WriteLine("Unable to parse '{0}'", hour2);
 
            _jobObservation.Models = models[0] + "|" + models[1] + "|" + models[2] + "|" + models[3] + "|" + models[4];
            _jobObservation.Cicles= cicles[0] + "|" + cicles[1] + "|" + cicles[2] + "|" + cicles[3] + "|" + cicles[4];
            _jobObservation.DateStart = newDate1;
            _jobObservation.DateEnd = newDate2;


           //Console.WriteLine(_jobObservation.PlantId);
            //Console.WriteLine(_jobObservation.AreaId);
            //Console.WriteLine(_jobObservation.DistributionId);
            //Console.WriteLine(_jobObservation.OperationId);
            //Console.WriteLine(_jobObservation.IsActive);
     
            //Console.WriteLine(_jobObservation.DateStart);
            //Console.WriteLine(_jobObservation.DateEnd);
            //Console.WriteLine(_jobObservation.DateEnd);
            //Console.WriteLine(_jobObservation.Observer);
            //Console.WriteLine(_jobObservation.Operator);
            //Console.WriteLine(_jobObservation.Option);
            //Console.WriteLine(_jobObservation.Anomaly);
            //Console.WriteLine(_jobObservation.Time1HOE);
            //Console.WriteLine(_jobObservation.Time2HOE);
            //Console.WriteLine(_jobObservation.Models);
            //Console.WriteLine(_jobObservation.Cicles);
            //Console.WriteLine(_jobObservation.SArea);
            //Console.WriteLine(_jobObservation.QArea);
            //Console.WriteLine(_jobObservation.DArea);
            //Console.WriteLine(_jobObservation.CArea);
            //Console.WriteLine(_jobObservation.OthersArea);
            //Console.WriteLine(_jobObservation.IdentifiedActivity);
            //Console.WriteLine(_jobObservation.SsvCommentary);
            //Console.WriteLine(_jobObservation.OperatorCommentary);
            //Console.WriteLine(_jobObservation.SsvSignature);
            //Console.WriteLine(_jobObservation.OperatorSignature);

            var result = await JobObservationService.CreateJobObservation(_jobObservation);
            if (result != null)
            {
                NavigationManager.NavigateTo("/jobobservation");
            }
            else
                await JSRuntime.InvokeVoidAsync("alert", "Error en los datos!"); // Alert

        }

        void CancelCreateJobObservation()
        {
            NavigationManager.NavigateTo("/jobobservation");
        }

        //timer
        private void OnTimedEvent(Object source, ElapsedEventArgs e)
        {
            switch (opt)
            {
                case 1:
                    cicles[0] = elapsedTime; break;
                case 2:
                    cicles[1] = elapsedTime; break;
                case 3:
                    cicles[2] = elapsedTime; break;
                case 4:
                    cicles[3] = elapsedTime; break;
                case 5:
                    cicles[4] = elapsedTime; break;
            }
            DateTime currentTime = e.SignalTime;
            elapsedTime = $"{currentTime.Subtract(startTime)}".Substring(0,12);
            StateHasChanged();
        }

        void StartTimer()
        {
            startTime = DateTime.Now;
            timer = new System.Timers.Timer(1);
            timer.Elapsed += OnTimedEvent;
            timer.AutoReset = true;
            timer.Enabled = true;
            isRunning = true;
        }

        void StopTimer()
        {
            isRunning = false;
            Console.WriteLine($"Elapsed Time: {elapsedTime}");
            timer.Enabled = false;
            elapsedTime = DEFAULT_TIME;
        }

        void OnTimerChanged()
        {
            if (!isRunning)
                StartTimer();
            else
                StopTimer();
        }

    }
}
