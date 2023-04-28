using Microsoft.JSInterop;
using MudBlazor;
using SupervisorMobility.Client.Data.Entities;
using SupervisorMobility.Client.Pages.Configuration.PlantPage;
using System;
using System.Globalization;
using System.Timers;

namespace SupervisorMobility.Client.Pages.JobObservationPage
{
    public partial class PlanJobObservation
    {

        [Parameter]
        public string date { get; set; }
        public string hour1 { get; set; }
        public string hour2 { get; set; }
        TimeSpan? startHour = new TimeSpan(00, 00, 00);
        TimeSpan? endHour = new TimeSpan(00, 00, 00);
        DateTime newDate1;
        DateTime newDate2;

        List<JobObservation> _jobObservations;
        List<Plant> _plants { get; set; } = new();
        List<Product> _products { get; set; } = new();
        List<Area> _areas = new();
        List<Distribution> _distributions = new();
        List<Operation> _operations = new();
        List<Lup> _tempLup { get; set; } = new();
        Lup lup { get; set; } = new();
        List<Lup> _lup { get; set; } = new();

        public JobObservation _jobObservation { get; set; } = new();

        public string areaS;
        public string areaQ;
        public string areaD;
        public string areaC;
        public string areaOther;

        int[] models = new int[5];
        string[] cycles = new string[5] { "", "", "", "", "" };

        public string placeholder = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, " +
          "sed do eiusmod tempor incididuntut labore et dolore magna aliqua. Ut enim ad minim " +
          "veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo coe velit esse cillum";

        //timer
        const string DEFAULT_TIME = "00:00:00.000";
        string elapsedTime = DEFAULT_TIME;
        System.Timers.Timer timer = new System.Timers.Timer(1);
        DateTime startTime = DateTime.Now;
        bool isRunning = false;

        //Glosary
        private List<Glosary> glosary = new();
        private Dictionary<string, Glosary> _glosaryInfo;

        //Past Job observation
        //Lup Modal
        private bool visiblePast = false;
        private bool visibleLup = false;
        private int lupId;

        private DialogOptions dialogLup = new() { CloseOnEscapeKey = true, MaxWidth = MaxWidth.Large, FullWidth = true };
        private DialogOptions dialogPastJobObservations = new() { CloseOnEscapeKey = true, MaxWidth = MaxWidth.Large, FullWidth = true };

        //Past job observation
        public List<JobObservation> pastJobs = new();
        public List<JobObservation> pastjobObservations = new();
        public List<Lup> pastLup = new();
        public JobObservation pastJob = new();

        public Distribution distribution= new Distribution();
        public Operation operation = new();

        public bool flag = false;


        // Breadcrumb links
        private List<BreadcrumbItem> _links = new List<BreadcrumbItem>
        {
            new BreadcrumbItem("Home", href: "/"),
            new BreadcrumbItem("Job Observation", href: "/jobobservation"),
            new BreadcrumbItem("Plan Job Observation", href: "", disabled: true)
        };


        //User
        private string json = string.Empty;
        public User user = new();

        //Operator user
        public List<User> users = new();
        public List<User> operatorUsers = new();

        protected async override Task OnInitializedAsync()
        {
            _jobObservation.Supervisor = new();

            await GetUserAsync();

            if (user != null)
            {
                _plants = await PlantServices.GetPlants();
                _jobObservation.PlantId = (int)user.PlantId;

                _jobObservation.AreaId = (int)user.AreaId;

                _areas = await AreaServices.GetAreas((int)user.PlantId);
                _jobObservation.SupervisorId = user.UserId;
                _jobObservation.Supervisor = await UsersService.GetUser(user.UserId);

                _distributions = await DistributionService.GetDistributionsWithCollections(_jobObservation.PlantId, _jobObservation.AreaId);
                StateHasChanged();
            }



            //operator User
            users = await UsersService.GetUsers();
            foreach (var operatorUser in users)
            {
                if (user != null && operatorUser.AreaId == user.AreaId && operatorUser.UserType == 4)
                {
                    operatorUsers.Add(operatorUser);
                }
            }



            glosary = await GlosaryService.GetGlosary();
            _glosaryInfo = glosary.ToDictionary(x => x.Name, x => x);

            date = date.Replace("-", "/");

            _jobObservation.IsActive = true;
            _jobObservation.StartDate = DateTime.ParseExact(date, "d/M/yyyy", null);
            _jobObservation.EndDate = DateTime.ParseExact(date, "d/M/yyyy", null);
            _jobObservation.Option = 1;

            _plants = await PlantServices.GetPlants();
            //_products = await ProductService.GetProducts();


        }


        //Local storage user
        private async Task GetUserAsync()
        {
            if (!await TryGetAsync())
                user = new();
        }

        private async Task<bool> TryGetAsync()
        {
            bool hasProperty = await HasPropertyAsync();
            if (hasProperty)
            {
                json = await JSRuntime.InvokeAsync<string>("localStorage.getItem", "user");
                user = JsonSerializer.Deserialize<User>(json) ?? new();
            }
            return hasProperty;
        }

        private async Task<bool> HasPropertyAsync()
            => await JSRuntime.InvokeAsync<bool>("localStorage.hasOwnProperty", "user");


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
            _distributions = await DistributionService.GetDistributionsWithCollections(_jobObservation.PlantId, _jobObservation.AreaId);
        }
        private async void ShowOperations()
        {

            _products = _distributions[_distributions.FindIndex(d => d.DistributionId == _jobObservation.DistributionId)].Products;
            _jobObservation.OperationId = 0;
            _operations = _distributions[_distributions.FindIndex(d => d.DistributionId == _jobObservation.DistributionId)].Operations;

            distribution = await DistributionService.GetDistributionById(_jobObservation.PlantId, _jobObservation.AreaId, _jobObservation.DistributionId);
        }

        private async void ShowPastJobObservations()
        {
            flag = true;
            operation = await OperationService.GetOperationById(_jobObservation.PlantId, _jobObservation.AreaId, _jobObservation.DistributionId, _jobObservation.OperationId);
            pastjobObservations = new();
            pastLup = new();
            if (user != null)
            {
                pastJobs = await JobObservationService.GetAllJobObservations();

                foreach (var job in pastJobs)
                {
                    if (job.Supervisor.Name == user.Name && Convert.ToDateTime(job.StartDate?.ToShortDateString()).Date <= Convert.ToDateTime(_jobObservation.StartDate?.ToShortDateString()).Date
                        && job.DistributionId == _jobObservation.DistributionId && job.OperationId == _jobObservation.OperationId)
                    {

                        pastjobObservations.Add(job);

                        pastJob = await JobObservationService.GetJobObservationWithLup(job.JobObservationId);
                        foreach (var lups in pastJob.Lup)
                        {
                            pastLup.Add(lups);
                        }
                    }

                }


            }
            pastjobObservations = pastjobObservations.OrderBy(x => x.StartDate).ToList();
            StateHasChanged();
        }

        private async Task PlanNewJobObservation()
        {
            //Planned
            _jobObservation.Type = 1;

            Console.WriteLine(startHour);
            hour1 = _jobObservation.StartDate?.ToShortDateString() + $" {startHour}";
            hour2 = _jobObservation.EndDate?.ToShortDateString() + $" {endHour}";

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
            _jobObservation.Cicles = cycles[0] + "|" + cycles + "|" + cycles[2] + "|" + cycles[3] + "|" + cycles[4];
            _jobObservation.StartDate = newDate1;
            _jobObservation.EndDate = newDate2;

            _jobObservation.PlannedStartDate = newDate1;
            _jobObservation.PlannedEndDate = newDate2;

            _jobObservation.Status = 1;

            var result = await JobObservationService.CreateJobObservation(_jobObservation);
            if (result != null)
            {
                Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                Snackbar.Add($"Job Observation Created", Severity.Info);

                if(_tempLup.Count > 0)
                {
                    _jobObservations = await JobObservationService.GetAllJobObservations();
                    foreach(var temp in _tempLup)
                    {
                        temp.JobObservationId = _jobObservations.Last().JobObservationId;
                        var result2 = await LupService.CreateLup(temp);
                        if (result2 != null)
                        {
                            Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                            Snackbar.Add($"Job observation Lup item Created", Severity.Info);
                        }
                        else
                        {
                            Snackbar.Clear();
                            Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                            Snackbar.Add($"Error in Lup", Severity.Error);
                        }
                    }
                }

                NavigationManager.NavigateTo("/jobobservation");
            }
            else
                await JSRuntime.InvokeVoidAsync("alert", "Error en los datos!"); // Alert

        }

        void CancelCreateJobObservation()
        {
            NavigationManager.NavigateTo("/jobobservation");
        }

        private void OpenDialogLup(int id)
        {
            lupId = id;
            visibleLup = true;
        }

        private void OpenDialogPastJobObservations()
        {
            if (!flag)
            {
                Snackbar.Clear();
                Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                Snackbar.Add($"Select a distribution and an operation", Severity.Error);
                return;
            }
            visiblePast = true;
        }

        void CloseLup() => visibleLup = false;
        void CloseOverdue() => visiblePast = false;

        void GoToJobObservation(int jobObservationId)
        {
            NavigationManager.NavigateTo($"/");
            NavigationManager.NavigateTo($"jobobservation/updatejobobservation/{jobObservationId}");
        }

        //Lup
        void Closed(MudChip chip)
        {
            // react to chip closed
        }
    }
}
