using DocumentFormat.OpenXml.Bibliography;
using Microsoft.JSInterop;
using MudBlazor;
using System.Globalization;
using System.Timers;

namespace SupervisorMobility.Client.Pages.JobObservationPage
{
    public partial class UpdateJobObservation
    {

        [Parameter]
        public int JobObservationId { get; set; }
        public JobObservation _jobObservation { get; set; } = new();

        public string hour1 { get; set; }
        public string hour2 { get; set; }

        DateTime newDate1;
        DateTime newDate2;

        //Objects
        private bool dense = false;
        private bool hover = false;
        private bool ronly = false;
        TimeSpan? endHour { get; set; }
        TimeSpan? startHour { get; set; }

        List<Plant> _plants { get; set; } = new();
        List<Area> _areas = new();
        List<Distribution> _distributions = new();
        List<Operation> _operations = new();
        List<Product> _products { get; set; } = new();

        public int plantId;
        public int areaId;
        public int distributionId;
        public int operationId;

        int[] models = new int[5];
        string[] cicles = new string[5];

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
            new BreadcrumbItem("Update Job Observation", href: "", disabled: true)
        };

        protected async override Task OnInitializedAsync()
        {

            _jobObservation = await JobObservationService.GetJobObservationById(JobObservationId);
  
            startHour = _jobObservation.DateStart?.TimeOfDay;
            endHour = _jobObservation.DateEnd?.TimeOfDay;

            _plants = await PlantServices.GetPlants();
            _products = await ProductService.GetProducts();
            _areas = await AreaServices.GetAreas(_jobObservation.PlantId);
            _distributions = await DistributionService.GetDistributions(_jobObservation.PlantId, _jobObservation.AreaId);
            _operations = await OperationService.GetOperations(_jobObservation.PlantId, _jobObservation.AreaId, _jobObservation.DistributionId);

            var prod = _jobObservation.Models.Split('|');
            models[0] = Int32.Parse(prod[0]);
            models[1] = Int32.Parse(prod[1]);
            models[2] = Int32.Parse(prod[2]);
            models[3] = Int32.Parse(prod[3]);
            models[4] = Int32.Parse(prod[4]);
            cicles = _jobObservation.Cicles.Split('|');
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
        private async Task EditJobObservation()
        {

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
            _jobObservation.Cicles = cicles[0] + "|" + cicles[1] + "|" + cicles[2] + "|" + cicles[3] + "|" + cicles[4];
            _jobObservation.DateStart = newDate1;
            _jobObservation.DateEnd = newDate2;

            var result = await JobObservationService.UpdateJobObservation(_jobObservation);

            if (result)
            {
                await JSRuntime.InvokeVoidAsync("alert", "Job Observation Succesful Update!"); // Alert
                NavigationManager.NavigateTo("/jobobservation");
            }
            else
                await JSRuntime.InvokeVoidAsync("alert", "Update failed!"); // Alert
        }

        void CancelUpdateJobObservation()
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
            elapsedTime = $"{currentTime.Subtract(startTime)}".Substring(0, 12);
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
