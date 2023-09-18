using Blazorise.Extensions;
using Microsoft.JSInterop;
using MudBlazor;
using SupervisorMobility.Client.Data.Entities;
using SupervisorMobility.Client.Pages.Configuration.PlantPage;
using System;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Timers;

namespace SupervisorMobility.Client.Pages.JobObservationPage
{
    public partial class CreateJobObservation
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

        List<User> _supervisors { get; set; } = new();
        List<User> _allSupervisors = new();

        List<Lup> _tempLup { get; set; } = new();
        Lup lup { get; set; } = new();
        List<Lup> _lup { get; set; } = new();

        AssyChart? _assychart { get; set; }

        public JobObservation _jobObservation { get; set; } = new();

        public string areaS;
        public string areaQ;
        public string areaD;
        public string areaC;
        public string areaOther;

        int[] models = new int[5];
        string[] cycles = new string[5] { "", "", "", "", "" };
        int[] HoeTimes = new int[5];

        public string placeholder = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, " +
          "sed do eiusmod tempor incididuntut labore et dolore magna aliqua. Ut enim ad minim " +
          "veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo coe velit esse cillum";

        //timer
        const string DEFAULT_TIME = "00:00:00.000";
        string elapsedTime = DEFAULT_TIME;
        System.Timers.Timer timer = new System.Timers.Timer(1);
        DateTime startTime = DateTime.Now;
        bool isRunning = false;
        bool isRunning2 = false;
        bool isRunning3 = false;
        bool isRunning4 = false;
        bool isRunning5 = false;
        public int opt = 1;

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
        private List<BreadcrumbItem> _links;


        //User
        private string json = string.Empty;
        public User user = new();

        //Operator user
        public List<User> _operators = new();
        public List<User> operatorUsers = new();

        protected async override Task OnInitializedAsync()
        {
            _links = new List<BreadcrumbItem>
            {
                new BreadcrumbItem(text: Localizer["home"], href: "/"),
                new BreadcrumbItem(text: Localizer["jobObservations"], href: "/jobobservation"),
                new BreadcrumbItem(text: Localizer["create"] + " " +  Localizer["jobObservation"], href: "", disabled: true)
            };

            _jobObservation.Supervisor = new();

            await GetUserAsync();

            if (user != null)
            {

                var uri = NavigationManager.ToAbsoluteUri(NavigationManager.Uri);
                var queryString = System.Web.HttpUtility.ParseQueryString(uri.Query);

                // Leer los valores de los parámetros
                var PatPlantId = queryString["PlantId"];
                var PatAreaId = queryString["AreaId"];
                var PatDistributionId = queryString["DistributionId"];
                var PatOperationId = queryString["OperationId"];
                var PatOperatorId = queryString["OperatorId"];
                var PatSupervisorId = queryString["SupervisorId"];

                _plants = await PlantServices.GetPlants();


                if (user.UserType == 1)
                {
                    if (PatPlantId != null)
                    {

                        _jobObservation.PlantId = int.Parse(PatPlantId);

                        _jobObservation.AreaId = int.Parse(PatAreaId);

                        _areas = await AreaServices.GetAreas(_jobObservation.PlantId);
                        _jobObservation.SupervisorId = int.Parse(PatSupervisorId);

                        _jobObservation.Supervisor = await UsersService.GetUser(_jobObservation.SupervisorId);

                        _distributions = await DistributionService.GetDistributionsWithCollections(_jobObservation.PlantId, _jobObservation.AreaId);

                        _jobObservation.DistributionId = int.Parse(PatDistributionId);

                        _products = _distributions[_distributions.FindIndex(d => d.DistributionId == _jobObservation.DistributionId)].Products;
                        _operations = _distributions[_distributions.FindIndex(d => d.DistributionId == _jobObservation.DistributionId)].Operations;

                        _jobObservation.OperationId = int.Parse(PatOperationId);

                        //_allSupervisors = await UsersService.GetUsersByType(3);
                        //foreach (User sv in _allSupervisors)
                        //{
                        //    if (sv.PlantId == _jobObservation.PlantId && sv.AreaId == _jobObservation.AreaId)
                        //    {
                        //        _supervisors.Add(sv);
                        //    }
                        //}

                        _supervisors =  await UsersService.GetUsersByUserTypeInPlantAndArea(_jobObservation.PlantId, _jobObservation.AreaId, 3, false, false);

                        //_operators = await UsersService.GetUsersByType(4);
                        ////operator User
                        //foreach (var operatorUser in _operators)
                        //{
                        //    if (operatorUser.AreaId == _jobObservation.AreaId && operatorUser.SuperiorId == _jobObservation.SupervisorId)
                        //    {
                        //        operatorUsers.Add(operatorUser);
                        //    }
                        //}

                        _operators = await UsersService.GetSubordinates(_jobObservation.SupervisorId);

                        _jobObservation.OperatorId = int.Parse(PatOperatorId);
                    }
                    else
                    {
                        _jobObservation.PlantId = 0;
                        _jobObservation.AreaId = 0;
                        _jobObservation.SupervisorId = 0;
                        _allSupervisors = await UsersService.GetUsersByType(3, true, false);
                        _operators = await UsersService.GetUsersByType(4, true, false);
                    }

                }
                else if (user.UserType == 2)
                {
                    _jobObservation.PlantId = (int)user.PlantId;
                    _jobObservation.AreaId = 0;
                    _jobObservation.SupervisorId = 0;
                    //_allSupervisors = await UsersService.GetUsersByType(3);
                    //_operators = await UsersService.GetUsersByType(4);
                    _supervisors = await UsersService.GetUsersByUserTypeInPlantAndArea(_jobObservation.PlantId, _jobObservation.AreaId, 3, false, false);


                }
                else
                {

                    if (PatPlantId != null)
                    {
                        _jobObservation.PlantId = int.Parse(PatPlantId);

                        _jobObservation.AreaId = int.Parse(PatAreaId);

                        _areas = await AreaServices.GetAreas(_jobObservation.PlantId);
                        _jobObservation.SupervisorId = int.Parse(PatSupervisorId);

                        _jobObservation.Supervisor = await UsersService.GetUser(_jobObservation.SupervisorId);

                        _distributions = await DistributionService.GetDistributionsWithCollections(_jobObservation.PlantId, _jobObservation.AreaId);

                        _jobObservation.DistributionId = int.Parse(PatDistributionId);

                        _products = _distributions[_distributions.FindIndex(d => d.DistributionId == _jobObservation.DistributionId)].Products;
                        _operations = _distributions[_distributions.FindIndex(d => d.DistributionId == _jobObservation.DistributionId)].Operations;

                        _jobObservation.OperationId = int.Parse(PatOperationId);

                        //operator User
                        _operators = await UsersService.GetUsersByType(4, true, false);
                        //operator User
                        foreach (var operatorUser in _operators)
                        {
                            if (operatorUser.AreaId == _jobObservation.AreaId && operatorUser.SuperiorId == _jobObservation.SupervisorId)
                            {
                                operatorUsers.Add(operatorUser);
                            }
                        }

                        _jobObservation.OperatorId = int.Parse(PatOperatorId);
                    }
                    else
                    {
                        _jobObservation.PlantId = (int)user.PlantId;

                        _jobObservation.AreaId = (int)user.AreaId;

                        _areas = await AreaServices.GetAreas((int)user.PlantId);
                        _jobObservation.SupervisorId = user.UserId;
                        _jobObservation.Supervisor = await UsersService.GetUser(user.UserId);

                        _distributions = await DistributionService.GetDistributionsWithCollections(_jobObservation.PlantId, _jobObservation.AreaId);


                        //operator User
                        _operators = await UsersService.GetUsersByType(4, true, false);
                        foreach (var operatorUser in _operators)
                        {
                            if (user != null && operatorUser.AreaId == user.AreaId && operatorUser.SuperiorId == user.UserId)
                            {
                                operatorUsers.Add(operatorUser);
                            }
                        }
                    }

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

            StateHasChanged();

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
            _jobObservation.OperatorId = 0;
            _jobObservation.SupervisorId = 0;
            _assychart = null;

            _areas = await AreaServices.GetAreas(_jobObservation.PlantId);
        }

        private async void ShowDistributions()
        {
            _jobObservation.SupervisorId = 0;
            _supervisors.Clear();
            _assychart = null;
            if (user.UserType == 1)
            {
                foreach (User sv in _allSupervisors)
                {
                    if (sv.PlantId == _jobObservation.PlantId && sv.AreaId == _jobObservation.AreaId)
                    {
                        _supervisors.Add(sv);
                    }
                }

            }
            else if (user.UserType == 2)
            {
                foreach (User sv in _allSupervisors)
                {
                    if (sv.PlantId == _jobObservation.PlantId && sv.AreaId == _jobObservation.AreaId && sv.SuperiorId == user.UserId)
                    {
                        _supervisors.Add(sv);
                    }
                }
            }

            _jobObservation.OperatorId = 0;
            _jobObservation.DistributionId = 0;
            _jobObservation.OperationId = 0;
            _distributions = await DistributionService.GetDistributionsWithCollections(_jobObservation.PlantId, _jobObservation.AreaId);
            StateHasChanged();
        }

        private void ShowOperators()
        {
            if (_jobObservation.DistributionId != 0 && _jobObservation.OperationId != 0)
                ShowPastJobObservations();
            operatorUsers = new();
            _jobObservation.OperatorId = 0;
            //operator User
            foreach (var operatorUser in _operators)
            {
                if (operatorUser.AreaId == _jobObservation.AreaId && operatorUser.SuperiorId == _jobObservation.SupervisorId)
                {
                    operatorUsers.Add(operatorUser);
                }
            }
            StateHasChanged();
        }

        private async void ShowOperations()
        {
            _assychart = null;

            _products = _distributions[_distributions.FindIndex(d => d.DistributionId == _jobObservation.DistributionId)].Products;
            _jobObservation.OperationId = 0;
            _operations = _distributions[_distributions.FindIndex(d => d.DistributionId == _jobObservation.DistributionId)].Operations;
            _assychart = await AssychartsServices.GetAssyChartJobObservation(_jobObservation.PlantId, _jobObservation.AreaId, _jobObservation.DistributionId);
            await Task.Delay(150);

            distribution = await DistributionService.GetDistributionById(_jobObservation.PlantId, _jobObservation.AreaId, _jobObservation.DistributionId);
            StateHasChanged();
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
                    if (job.SupervisorId == _jobObservation.SupervisorId && Convert.ToDateTime(job.StartDate?.ToShortDateString()).Date <= Convert.ToDateTime(_jobObservation.StartDate?.ToShortDateString()).Date
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

            listFilter = _assychart.RoutesProductsAssyChart.Where(r => r.Code.ToLower().Contains(operation.Code.ToLower(), StringComparison.OrdinalIgnoreCase)).ToList();
            FilterOperation = true;


            StateHasChanged();
        }

        private async Task CreateNewJobObservation()
        {
            if (_jobObservation.Option == 3 && _jobObservation.Anomaly.IsNullOrEmpty())
            {
                Snackbar.Clear();
                Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                Snackbar.Add($"Write down the anomaly first", Severity.Error);
                return;
            }



            if (CultureInfo.CurrentCulture.Name == "en-US")
            {
                var formatedStartDate = _jobObservation.StartDate;
                var formatedEndDate = _jobObservation.EndDate;

                var EnglishStartDate = formatedStartDate?.Month.ToString() + "/" + formatedStartDate?.Day.ToString() + "/" + formatedStartDate?.Year.ToString();
                var EnglishEndDate = formatedEndDate?.Month.ToString() + "/" + formatedEndDate?.Day.ToString() + "/" + formatedEndDate?.Year.ToString();
                _jobObservation.StartDate = DateTime.ParseExact(EnglishStartDate, "M/d/yyyy", CultureInfo.InvariantCulture);
                _jobObservation.EndDate = DateTime.ParseExact(EnglishEndDate, "M/d/yyyy", CultureInfo.InvariantCulture);


                hour1 = _jobObservation.StartDate?.ToShortDateString() + $" {startHour}";
                hour2 = _jobObservation.EndDate?.ToShortDateString() + $" {endHour}";

                if (DateTime.TryParseExact(hour1, $"M/d/yyyy HH:mm:ss", null, DateTimeStyles.None, out newDate1))
                {
                    Console.WriteLine(newDate1);
                }
                else
                    Console.WriteLine("Unable to parse '{0}'", hour1);


                if (DateTime.TryParseExact(hour2, $"M/d/yyyy HH:mm:ss", null, DateTimeStyles.None, out newDate2))
                {
                    Console.WriteLine(newDate2);
                }
                else
                    Console.WriteLine("Unable to parse '{0}'", hour2);

                _jobObservation.Models = models[0] + "|" + models[1] + "|" + models[2] + "|" + models[3] + "|" + models[4];
                _jobObservation.Cicles = cycles[0] + "|" + cycles[1] + "|" + cycles[2] + "|" + cycles[3] + "|" + cycles[4];
                _jobObservation.HOEStandardTimes = HoeTimes[0] + "|" + HoeTimes[1] + "|" + HoeTimes[2] + "|" + HoeTimes[3] + "|" + HoeTimes[4];
                _jobObservation.StartDate = newDate1;
                _jobObservation.EndDate = newDate2;
                //Eventual
                _jobObservation.Type = 2;
                _jobObservation.PlannedStartDate = newDate1;
                _jobObservation.PlannedEndDate = newDate2;


                _jobObservation.Status = 1;

                var result = await JobObservationService.CreateJobObservation(_jobObservation);
                if (result != null)
                {
                    Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                    Snackbar.Add($"Job Observation Created", Severity.Info);

                    if (_tempLup.Count > 0)
                    {
                        _jobObservations = await JobObservationService.GetAllJobObservations();
                        foreach (var temp in _tempLup)
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
            else
            {
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
                _jobObservation.Cicles = cycles[0] + "|" + cycles[1] + "|" + cycles[2] + "|" + cycles[3] + "|" + cycles[4];
                _jobObservation.HOEStandardTimes = HoeTimes[0] + "|" + HoeTimes[1] + "|" + HoeTimes[2] + "|" + HoeTimes[3] + "|" + HoeTimes[4];
                _jobObservation.StartDate = newDate1;
                _jobObservation.EndDate = newDate2;

                //Eventual
                _jobObservation.Type = 2;
                _jobObservation.PlannedStartDate = newDate1;
                _jobObservation.PlannedEndDate = newDate2;


                _jobObservation.Status = 1;

                var result = await JobObservationService.CreateJobObservation(_jobObservation);
                if (result != null)
                {
                    Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                    Snackbar.Add($"Job Observation Created", Severity.Info);

                    if (_tempLup.Count > 0)
                    {
                        _jobObservations = await JobObservationService.GetAllJobObservations();
                        foreach (var temp in _tempLup)
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
        }

        void CancelCreateJobObservation()
        {
            NavigationManager.NavigateTo("/jobobservation");
        }



        //timer
        private void OnTimedEvent(Object source, ElapsedEventArgs e)
        {
            TimeSpan hundreths;
            int centiseconds = 0;
            if (TimeSpan.TryParseExact(elapsedTime, "hh\\:mm\\:ss\\.fff", CultureInfo.InvariantCulture, out hundreths))
            {
                centiseconds = (int)hundreths.TotalMilliseconds / 10;
            }
            else
            {
                Console.WriteLine("Wrong timestamp format.");
            }
            switch (opt)
            {
                case 1:
                    cycles[0] = centiseconds.ToString(); break;
                case 2:
                    cycles[1] = centiseconds.ToString(); break;
                case 3:
                    cycles[2] = centiseconds.ToString(); break;
                case 4:
                    cycles[3] = centiseconds.ToString(); break;
                case 5:
                    cycles[4] = centiseconds.ToString(); break;
            }
            DateTime currentTime = e.SignalTime;
            elapsedTime = $"{currentTime.Subtract(startTime)}".Substring(0,12);
            StateHasChanged();
        }

        void StartTimer(int option)
        {
            startTime = DateTime.Now;
            timer = new System.Timers.Timer(1);
            timer.Elapsed += OnTimedEvent;
            timer.AutoReset = true;
            timer.Enabled = true;
            switch (option)
            {
                case 1: isRunning = true; break;
                case 2: isRunning2 = true; break;
                case 3: isRunning3 = true; break;
                case 4: isRunning4 = true; break;
                case 5: isRunning5 = true; break;
            }
        }

        void StopTimer()
        {
            TimeSpan hundreths;
            int centiseconds = 0;
            if (TimeSpan.TryParseExact(elapsedTime, "hh\\:mm\\:ss\\.fff", CultureInfo.InvariantCulture, out hundreths))
            {
                centiseconds = (int)hundreths.TotalMilliseconds / 10;
            }
            else
            {
                Console.WriteLine("Wrong timestamp format.");
            }
            switch (opt)
            {
                case 1:
                    cycles[0] = centiseconds.ToString(); isRunning = false; break;
                case 2:
                    cycles[1] = centiseconds.ToString(); isRunning2 = false; break;
                case 3:
                    cycles[2] = centiseconds.ToString(); isRunning3 = false; break;
                case 4:
                    cycles[3] = centiseconds.ToString(); isRunning4 = false; break;
                case 5:
                    cycles[4] = centiseconds.ToString(); isRunning5 = false; break;
            }

            Console.WriteLine($"Elapsed Time: {elapsedTime}");
            timer.Enabled = false;
            elapsedTime = DEFAULT_TIME;
        }

        void OnTimerChanged(int option)
        {
            if(option == 1 && HoeTimes[0] == 0) {
                Snackbar.Clear();
                Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                Snackbar.Add($"First enter the Hoe Standard Time 1", Severity.Warning);
                return;
            }
            else if(option == 2 && HoeTimes[1] == 0)
            {
                Snackbar.Clear();
                Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                Snackbar.Add($"First enter the Hoe Standard Time 2", Severity.Warning);
                return;
            }
            else if (option == 3 && HoeTimes[2] == 0)
            {
                Snackbar.Clear();
                Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                Snackbar.Add($"First enter the Hoe Standard Time 3", Severity.Warning);
                return;
            }
            else if (option == 4 && HoeTimes[3] == 0)
            {
                Snackbar.Clear();
                Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                Snackbar.Add($"First enter the Hoe Standard Time 4", Severity.Warning);
                return;
            }
            else if (option == 5 && HoeTimes[4] == 0)
            {
                Snackbar.Clear();
                Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                Snackbar.Add($"First enter the Hoe Standard Time 5", Severity.Warning);
                return;
            }
            opt = option;

            if (opt == 1 && !isRunning || opt == 2 && !isRunning2 || opt == 3 && !isRunning3 || opt == 4 && !isRunning4 || opt == 5 && !isRunning5)
            {
                StopTimer();
                switch (opt)
                {
                    case 1:
                        isRunning2 = false;
                        isRunning3 = false;
                        isRunning4 = false;
                        isRunning5 = false;
                        break;
                    case 2:
                        isRunning = false;
                        isRunning3 = false;
                        isRunning4 = false;
                        isRunning5 = false;
                        break;
                    case 3:
                        isRunning = false;
                        isRunning2 = false;
                        isRunning4 = false;
                        isRunning5 = false;
                        break;
                    case 4:
                        isRunning = false;
                        isRunning2 = false;
                        isRunning3 = false;
                        isRunning5 = false;
                        break;
                    case 5:
                        isRunning = false;
                        isRunning2 = false;
                        isRunning3 = false;
                        isRunning4 = false;
                        break;
                }

                StartTimer(opt);
            }
            else
            {

                StopTimer();
                for(int i=0; i<HoeTimes.Length; i++)
                {
                    if (cycles[i] != "" && HoeTimes[i] != 0 && int.Parse(cycles[i]) > HoeTimes[i])
                    {
                        if(areaD == "" || areaD == null)
                            areaD = $"Cycle time {i+1} took longer than standard time";
                        else
                        {
                            if(areaD.Contains($"Cycle time {i + 1} took longer than standard time"))
                            {
                                continue;
                            }
                            areaD = areaD + $", Cycle time {i + 1} took longer than standard time";
                        }
                    }
                }    
            }
        }

        void Option1() => opt = 1;
        void Option2() => opt = 2;
        void Option3() => opt = 3;
        void Option4() => opt = 4;
        void Option5() => opt = 5;


        //Lup
        void Closed(MudChip chip)
        {
            // react to chip closed
        }
        public void AddTempLup(int pillar)
        {
            switch (pillar)
            {
                case 1:
                    if (areaS != null && areaS.Length > 0)
                    {
                        lup.Oportunity = areaS;
                        areaS = "";
                    }
                    else
                    {
                        Snackbar.Clear();
                        Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                        Snackbar.Add($"Error S Area is empty", Severity.Error);
                        return;
                    }
                    break;
                case 2:
                    if (areaQ != null && areaQ.Length > 0)
                    {
                        lup.Oportunity = areaQ;
                        areaQ = "";
                    }
                    else
                    {
                        Snackbar.Clear();
                        Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                        Snackbar.Add($"Error Q Area is empty", Severity.Error);
                        return;
                    }
                    break;
                case 3:
                    if (areaD != null && areaD.Length > 0)
                    {
                        lup.Oportunity = areaD;
                        areaD = "";
                    }
                    else
                    {
                        Snackbar.Clear();
                        Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                        Snackbar.Add($"Error D Area is empty", Severity.Error);
                        return;
                    }
                    break;
                case 4:
                    if (areaC != null && areaC.Length > 0)
                    {
                        lup.Oportunity = areaC;
                        areaC = "";
                    }
                    else
                    {
                        Snackbar.Clear();
                        Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                        Snackbar.Add($"Error C Area is empty", Severity.Error);
                        return;
                    }
                    break;
                case 5:
                    if (areaOther != null && areaOther.Length > 0)
                    {
                        lup.Oportunity = areaOther;
                        areaOther = "";
                    }
                    else
                    {
                        Snackbar.Clear();
                        Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                        Snackbar.Add($"Error Others Area is empty", Severity.Error);
                        return;
                    }
                    break;

            }

            if(_jobObservation.SupervisorId == 0)
            {
                Snackbar.Clear();
                Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                Snackbar.Add($"First select a Supervisor", Severity.Error);
                return;
            }

            foreach(User supervisor in _supervisors)
            {
                if (_jobObservation.SupervisorId == supervisor.UserId)
                {
                    lup.Observer = supervisor.Name;
                    
                }
            }
            lup.JobObservationId = 0;
            lup.Pillar = pillar;
            lup.Status = 1;
            lup.CreatedDate = DateTime.Now;
            lup.IsActive = true;

            _tempLup.Add(lup);
            lup = new();
            
            Snackbar.Clear();
            Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
            Snackbar.Add($"Lup item added", Severity.Info);



        }

        public void DeleteLup(Lup lup)
        {
            switch (lup.Pillar) {
                case 1: areaS = ""; break;
                case 2: areaQ = ""; break;
                case 3: areaD = ""; break;
                case 4: areaC = ""; break;
                case 5: areaOther = ""; break; 
            }
            _tempLup.Remove(lup);
        }

        //Past Job observation
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
                Snackbar.Add(Localizer["selectADistributionAndAnOperation"], Severity.Warning);
                return;
            }
            if (_jobObservation.SupervisorId == 0)
            {
                Snackbar.Clear();
                Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                Snackbar.Add(Localizer["firstSelectASupervisor"], Severity.Warning);
                return;
            }

            visiblePast = true;
        }

        void CloseLup() => visibleLup = false;
        void CloseOverdue() => visiblePast = false;

        void EditLup(int lupId)
        {
            NavigationManager.NavigateTo($"lup/updatelup/{lupId}");
        }

        void GoToJobObservation(int jobObservationId)
        {
            NavigationManager.NavigateTo($"/");
            NavigationManager.NavigateTo($"jobobservation/updatejobobservation/{jobObservationId}");
        }













        //In progress
        private async Task SaveProgressJobObservation()
        {
            if (_jobObservation.DistributionId == new int())
            {
                Snackbar.Clear();
                Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                Snackbar.Add($"First select a distribution!", Severity.Error);
                visibleSign = false;
                return;
            }
            if (_jobObservation.OperationId == new int())
            {
                Snackbar.Clear();
                Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                Snackbar.Add($"First select an operation!", Severity.Error);
                visibleSign = false;
                return;
            }

            if (_jobObservation.OperatorId == new int())
            {
                Snackbar.Clear();
                Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                Snackbar.Add($"First select an operator!", Severity.Error);
                visibleSign = false;
                return;
            }
            if (_jobObservation.Option == 3 && _jobObservation.Anomaly.IsNullOrEmpty())
            {
                Snackbar.Clear();
                Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                Snackbar.Add($"Write down the anomaly first", Severity.Error);
                return;
            }

            startHour = DateTime.Now.TimeOfDay;


            if (CultureInfo.CurrentCulture.Name == "en-US")
            {
                var formatedStartDate = _jobObservation.StartDate;
                var formatedEndDate = _jobObservation.EndDate;

                var EnglishStartDate = formatedStartDate?.Month.ToString() + "/" + formatedStartDate?.Day.ToString() + "/" + formatedStartDate?.Year.ToString();
                var EnglishEndDate = formatedEndDate?.Month.ToString() + "/" + formatedEndDate?.Day.ToString() + "/" + formatedEndDate?.Year.ToString();
                _jobObservation.StartDate = DateTime.ParseExact(EnglishStartDate, "M/d/yyyy", CultureInfo.InvariantCulture);
                _jobObservation.EndDate = DateTime.ParseExact(EnglishEndDate, "M/d/yyyy", CultureInfo.InvariantCulture);



                hour1 = _jobObservation.StartDate?.ToShortDateString() + $" {startHour?.ToString("hh\\:mm\\:ss")}";
                hour2 = _jobObservation.EndDate?.ToShortDateString() + $" {endHour}";

                if (DateTime.TryParseExact(hour1, $"M/d/yyyy HH:mm:ss", null, DateTimeStyles.None, out newDate1))
                {
                    Console.WriteLine(newDate1);
                }
                else
                {
                    Snackbar.Clear();
                    Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                    Snackbar.Add($"Error in Date Start", Severity.Error);
                    Console.WriteLine("Unable to parse '{0}'", hour1);
                }


                if (DateTime.TryParseExact(hour2, $"M/d/yyyy HH:mm:ss", null, DateTimeStyles.None, out newDate2))
                {
                    Console.WriteLine(newDate2);
                }
                else
                {
                    Snackbar.Clear();
                    Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                    Snackbar.Add($"Error in Date End", Severity.Error);
                    Console.WriteLine("Unable to parse '{0}'", hour2);
                }
                _jobObservation.Models = models[0] + "|" + models[1] + "|" + models[2] + "|" + models[3] + "|" + models[4];
                _jobObservation.Cicles = cycles[0] + "|" + cycles[1] + "|" + cycles[2] + "|" + cycles[3] + "|" + cycles[4];
                _jobObservation.HOEStandardTimes = HoeTimes[0] + "|" + HoeTimes[1] + "|" + HoeTimes[2] + "|" + HoeTimes[3] + "|" + HoeTimes[4];
                _jobObservation.StartDate = newDate1;
                _jobObservation.EndDate = newDate2;
                //Eventual
                _jobObservation.Type = 2;
                _jobObservation.PlannedStartDate = newDate1;
                _jobObservation.PlannedEndDate = newDate2;
                _jobObservation.Status = 2;

                if (_jobObservation.Justification == "")
                {
                    _jobObservation.Justification = null;
                }

                var result = await JobObservationService.CreateJobObservation(_jobObservation);
                if (result != null)
                {
                    Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                    Snackbar.Add($"Job Observation Created", Severity.Info);

                    if (_tempLup.Count > 0)
                    {
                        _jobObservations = await JobObservationService.GetAllJobObservations();
                        foreach (var temp in _tempLup)
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
            else
            {

                hour1 = _jobObservation.StartDate?.ToShortDateString() + $" {startHour?.ToString("hh\\:mm\\:ss")}";
                hour2 = _jobObservation.EndDate?.ToShortDateString() + $" {endHour}";

                if (DateTime.TryParseExact(hour1, $"d/M/yyyy HH:mm:ss", null, DateTimeStyles.None, out newDate1))
                {
                    Console.WriteLine(newDate1);
                }
                else
                {
                    Snackbar.Clear();
                    Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                    Snackbar.Add($"Error in Date Start", Severity.Error);
                    Console.WriteLine("Unable to parse '{0}'", hour1);
                }


                if (DateTime.TryParseExact(hour2, $"d/M/yyyy HH:mm:ss", null, DateTimeStyles.None, out newDate2))
                {
                    Console.WriteLine(newDate2);
                }
                else
                {
                    Snackbar.Clear();
                    Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                    Snackbar.Add($"Error in Date End", Severity.Error);
                    Console.WriteLine("Unable to parse '{0}'", hour2);
                }
                _jobObservation.Models = models[0] + "|" + models[1] + "|" + models[2] + "|" + models[3] + "|" + models[4];
                _jobObservation.Cicles = cycles[0] + "|" + cycles[1] + "|" + cycles[2] + "|" + cycles[3] + "|" + cycles[4];
                _jobObservation.HOEStandardTimes = HoeTimes[0] + "|" + HoeTimes[1] + "|" + HoeTimes[2] + "|" + HoeTimes[3] + "|" + HoeTimes[4];
                _jobObservation.StartDate = newDate1;
                _jobObservation.EndDate = newDate2;
                //Eventual
                _jobObservation.Type = 2;
                _jobObservation.PlannedStartDate = newDate1;
                _jobObservation.PlannedEndDate = newDate2;
                _jobObservation.Status = 2;

                if (_jobObservation.Justification == "")
                {
                    _jobObservation.Justification = null;
                }

                var result = await JobObservationService.CreateJobObservation(_jobObservation);
                if (result != null)
                {
                    Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                    Snackbar.Add($"Job Observation Created", Severity.Info);

                    if (_tempLup.Count > 0)
                    {
                        _jobObservations = await JobObservationService.GetAllJobObservations();
                        foreach (var temp in _tempLup)
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
        }





        //Under Review Job observation
        public async void UnderReviewJobObservation()
        {
            if (_jobObservation.DistributionId == new int())
            {
                Snackbar.Clear();
                Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                Snackbar.Add($"First select a distribution!", Severity.Error);
                visibleSign = false;
                return;
            }
            if (_jobObservation.OperationId == new int())
            {
                Snackbar.Clear();
                Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                Snackbar.Add($"First select an operation!", Severity.Error);
                visibleSign = false;
                return;
            }

            if (_jobObservation.OperatorId == new int())
            {
                Snackbar.Clear();
                Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                Snackbar.Add($"First select an operator!", Severity.Error);
                visibleSign = false;
                return;
            }

            if (_jobObservation.Option == 3 && _jobObservation.Anomaly.IsNullOrEmpty())
            {
                Snackbar.Clear();
                Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                Snackbar.Add($"Write down the anomaly first", Severity.Error);
                return;
            }
            if (_jobObservation.OperatorSignature == null || _jobObservation.OperatorSignature == "")
            {
                Snackbar.Clear();
                Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                Snackbar.Add($"Operator's Signature is missing!", Severity.Error);
                visibleSign = false;
                return;
            }

            User operatorUser = await UsersService.GetUser(_jobObservation.OperatorId);

            if (_jobObservation.OperatorSignature != operatorUser.Payroll.ToString())
            {
                Snackbar.Clear();
                Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                Snackbar.Add($"Operator Signature doesn't match", Severity.Error);
                return;
            }


            if (CultureInfo.CurrentCulture.Name == "en-US")
            {
                var formatedStartDate = _jobObservation.StartDate;
                var formatedEndDate = _jobObservation.EndDate;

                var EnglishStartDate = formatedStartDate?.Month.ToString() + "/" + formatedStartDate?.Day.ToString() + "/" + formatedStartDate?.Year.ToString();
                var EnglishEndDate = formatedEndDate?.Month.ToString() + "/" + formatedEndDate?.Day.ToString() + "/" + formatedEndDate?.Year.ToString();
                _jobObservation.StartDate = DateTime.ParseExact(EnglishStartDate, "M/d/yyyy", CultureInfo.InvariantCulture);
                _jobObservation.EndDate = DateTime.ParseExact(EnglishEndDate, "M/d/yyyy", CultureInfo.InvariantCulture);


                hour1 = _jobObservation.StartDate?.ToShortDateString() + $" {startHour}";
                hour2 = _jobObservation.EndDate?.ToShortDateString() + $" {endHour}";

                if (DateTime.TryParseExact(hour1, $"M/d/yyyy HH:mm:ss", null, DateTimeStyles.None, out newDate1))
                {
                    Console.WriteLine(newDate1);
                }
                else
                {
                    Snackbar.Clear();
                    Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                    Snackbar.Add($"Error in Date Start", Severity.Error);
                    Console.WriteLine("Unable to parse '{0}'", hour1);
                }


                if (DateTime.TryParseExact(hour2, $"M/d/yyyy HH:mm:ss", null, DateTimeStyles.None, out newDate2))
                {
                    Console.WriteLine(newDate2);
                }
                else
                {
                    Snackbar.Clear();
                    Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                    Snackbar.Add($"Error in Date End", Severity.Error);
                    Console.WriteLine("Unable to parse '{0}'", hour2);
                }

                _jobObservation.Models = models[0] + "|" + models[1] + "|" + models[2] + "|" + models[3] + "|" + models[4];
                _jobObservation.Cicles = cycles[0] + "|" + cycles[1] + "|" + cycles[2] + "|" + cycles[3] + "|" + cycles[4];
                _jobObservation.HOEStandardTimes = HoeTimes[0] + "|" + HoeTimes[1] + "|" + HoeTimes[2] + "|" + HoeTimes[3] + "|" + HoeTimes[4];
                _jobObservation.StartDate = newDate1;
                _jobObservation.EndDate = newDate2;
                //Eventual
                _jobObservation.Type = 2;
                _jobObservation.PlannedStartDate = newDate1;
                _jobObservation.PlannedEndDate = newDate2;

                _jobObservation.Status = 4;

                var result = await JobObservationService.CreateJobObservation(_jobObservation);
                if (result != null)
                {
                    Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                    Snackbar.Add($"Job Observation Created", Severity.Info);

                    if (_tempLup.Count > 0)
                    {
                        _jobObservations = await JobObservationService.GetAllJobObservations();
                        foreach (var temp in _tempLup)
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
            else
            {
                hour1 = _jobObservation.StartDate?.ToShortDateString() + $" {startHour}";
                hour2 = _jobObservation.EndDate?.ToShortDateString() + $" {endHour}";

                if (DateTime.TryParseExact(hour1, $"d/M/yyyy HH:mm:ss", null, DateTimeStyles.None, out newDate1))
                {
                    Console.WriteLine(newDate1);
                }
                else
                {
                    Snackbar.Clear();
                    Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                    Snackbar.Add($"Error in Date Start", Severity.Error);
                    Console.WriteLine("Unable to parse '{0}'", hour1);
                }


                if (DateTime.TryParseExact(hour2, $"d/M/yyyy HH:mm:ss", null, DateTimeStyles.None, out newDate2))
                {
                    Console.WriteLine(newDate2);
                }
                else
                {
                    Snackbar.Clear();
                    Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                    Snackbar.Add($"Error in Date End", Severity.Error);
                    Console.WriteLine("Unable to parse '{0}'", hour2);
                }

                _jobObservation.Models = models[0] + "|" + models[1] + "|" + models[2] + "|" + models[3] + "|" + models[4];
                _jobObservation.Cicles = cycles[0] + "|" + cycles[1] + "|" + cycles[2] + "|" + cycles[3] + "|" + cycles[4];
                _jobObservation.HOEStandardTimes = HoeTimes[0] + "|" + HoeTimes[1] + "|" + HoeTimes[2] + "|" + HoeTimes[3] + "|" + HoeTimes[4];
                _jobObservation.StartDate = newDate1;
                _jobObservation.EndDate = newDate2;

                _jobObservation.FinishedDate = DateTime.Now;
                Console.WriteLine(_jobObservation.FinishedDate);

                _jobObservation.Status = 4;

                var result = await JobObservationService.CreateJobObservation(_jobObservation);
                if (result != null)
                {
                    Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                    Snackbar.Add($"Job Observation Created", Severity.Info);

                    if (_tempLup.Count > 0)
                    {
                        _jobObservations = await JobObservationService.GetAllJobObservations();
                        foreach (var temp in _tempLup)
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

        }


        //Reject Job observation
        async void Reject()
        {

            if(_jobObservation.DistributionId == new int())
            {
                Snackbar.Clear();
                Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                Snackbar.Add($"First select a distribution!", Severity.Error);
                visibleSign = false;
                return;
            }
            if (_jobObservation.OperationId == new int())
            {
                Snackbar.Clear();
                Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                Snackbar.Add($"First select an operation!", Severity.Error);
                visibleSign = false;
                return;
            }

            if (_jobObservation.OperatorId == new int())
            {
                Snackbar.Clear();
                Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                Snackbar.Add($"First select an operator!", Severity.Error);
                visibleSign = false;
                return;
            }

            if (_jobObservation.SsvCommentary == null || _jobObservation.SsvCommentary == "")
            {
                Snackbar.Clear();
                Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                Snackbar.Add($"You need to add a commentary to reject the job observation", Severity.Error);
                visibleSign = false;
                return;
            }
            if (_jobObservation.OperatorSignature == null || _jobObservation.OperatorSignature == "")
            {
                Snackbar.Clear();
                Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                Snackbar.Add($"Operator's Signature is missing!", Severity.Error);
                visibleSign = false;
                return;
            }

            User operatorUser = await UsersService.GetUser(_jobObservation.OperatorId);

            if (_jobObservation.OperatorSignature != operatorUser.Payroll.ToString())
            {
                Snackbar.Clear();
                Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                Snackbar.Add($"Operator Signature doesn't match", Severity.Error);
                return;
            }
            if (_jobObservation.Option == 3 && _jobObservation.Anomaly.IsNullOrEmpty())
            {
                Snackbar.Clear();
                Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                Snackbar.Add($"Write down the anomaly first", Severity.Error);
                return;
            }

            if (CultureInfo.CurrentCulture.Name == "en-US")
            {
                var formatedStartDate = _jobObservation.StartDate;
                var formatedEndDate = _jobObservation.EndDate;

                var EnglishStartDate = formatedStartDate?.Month.ToString() + "/" + formatedStartDate?.Day.ToString() + "/" + formatedStartDate?.Year.ToString();
                var EnglishEndDate = formatedEndDate?.Month.ToString() + "/" + formatedEndDate?.Day.ToString() + "/" + formatedEndDate?.Year.ToString();
                _jobObservation.StartDate = DateTime.ParseExact(EnglishStartDate, "M/d/yyyy", CultureInfo.InvariantCulture);
                _jobObservation.EndDate = DateTime.ParseExact(EnglishEndDate, "M/d/yyyy", CultureInfo.InvariantCulture);

                hour1 = _jobObservation.StartDate?.ToShortDateString() + $" {startHour}";
                hour2 = _jobObservation.EndDate?.ToShortDateString() + $" {endHour}";

                if (DateTime.TryParseExact(hour1, $"M/d/yyyy HH:mm:ss", null, DateTimeStyles.None, out newDate1))
                {
                    Console.WriteLine(newDate1);
                }
                else
                {
                    Snackbar.Clear();
                    Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                    Snackbar.Add($"Error in Date Start", Severity.Error);
                    Console.WriteLine("Unable to parse '{0}'", hour1);
                }


                if (DateTime.TryParseExact(hour2, $"M/d/yyyy HH:mm:ss", null, DateTimeStyles.None, out newDate2))
                {
                    Console.WriteLine(newDate2);
                }
                else
                {
                    Snackbar.Clear();
                    Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                    Snackbar.Add($"Error in Date End", Severity.Error);
                    Console.WriteLine("Unable to parse '{0}'", hour2);
                }

                _jobObservation.Models = models[0] + "|" + models[1] + "|" + models[2] + "|" + models[3] + "|" + models[4];
                _jobObservation.Cicles = cycles[0] + "|" + cycles[1] + "|" + cycles[2] + "|" + cycles[3] + "|" + cycles[4];
                _jobObservation.HOEStandardTimes = HoeTimes[0] + "|" + HoeTimes[1] + "|" + HoeTimes[2] + "|" + HoeTimes[3] + "|" + HoeTimes[4];
                _jobObservation.StartDate = newDate1;
                _jobObservation.EndDate = newDate2;
                //Eventual
                _jobObservation.Type = 2;
                _jobObservation.PlannedStartDate = newDate1;
                _jobObservation.PlannedEndDate = newDate2; 

                _jobObservation.Status = 5;
                var result = await JobObservationService.CreateJobObservation(_jobObservation);
                if (result != null)
                {
                    Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                    Snackbar.Add($"Job Observation Created", Severity.Info);

                    if (_tempLup.Count > 0)
                    {
                        _jobObservations = await JobObservationService.GetAllJobObservations();
                        foreach (var temp in _tempLup)
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
            else
            {
                hour1 = _jobObservation.StartDate?.ToShortDateString() + $" {startHour}";
                hour2 = _jobObservation.EndDate?.ToShortDateString() + $" {endHour}";

                if (DateTime.TryParseExact(hour1, $"d/M/yyyy HH:mm:ss", null, DateTimeStyles.None, out newDate1))
                {
                    Console.WriteLine(newDate1);
                }
                else
                {
                    Snackbar.Clear();
                    Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                    Snackbar.Add($"Error in Date Start", Severity.Error);
                    Console.WriteLine("Unable to parse '{0}'", hour1);
                }


                if (DateTime.TryParseExact(hour2, $"d/M/yyyy HH:mm:ss", null, DateTimeStyles.None, out newDate2))
                {
                    Console.WriteLine(newDate2);
                }
                else
                {
                    Snackbar.Clear();
                    Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                    Snackbar.Add($"Error in Date End", Severity.Error);
                    Console.WriteLine("Unable to parse '{0}'", hour2);
                }

                _jobObservation.Models = models[0] + "|" + models[1] + "|" + models[2] + "|" + models[3] + "|" + models[4];
                _jobObservation.Cicles = cycles[0] + "|" + cycles[1] + "|" + cycles[2] + "|" + cycles[3] + "|" + cycles[4];
                _jobObservation.HOEStandardTimes = HoeTimes[0] + "|" + HoeTimes[1] + "|" + HoeTimes[2] + "|" + HoeTimes[3] + "|" + HoeTimes[4];
                _jobObservation.StartDate = newDate1;
                _jobObservation.EndDate = newDate2;
                //Eventual
                _jobObservation.Type = 2;
                _jobObservation.PlannedStartDate = newDate1;
                _jobObservation.PlannedEndDate = newDate2;

                _jobObservation.Status = 5;

                _jobObservation.FinishedDate = DateTime.Now;

                var result = await JobObservationService.CreateJobObservation(_jobObservation);
                if (result != null)
                {
                    Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                    Snackbar.Add($"Job Observation Created", Severity.Info);

                    if (_tempLup.Count > 0)
                    {
                        _jobObservations = await JobObservationService.GetAllJobObservations();
                        foreach (var temp in _tempLup)
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
        }





        //Finished Job observation
        private bool visibleSign = false;
        private void OpenSignComment()
        {
            visibleSign = true;
        }
        void CloseSign() => visibleSign = false;
        private DialogOptions dialogSignOptions = new() { CloseOnEscapeKey = true, MaxWidth = MaxWidth.ExtraSmall, FullWidth = true };

        public async Task SignDate()
        {

            if (_jobObservation.DistributionId == new int())
            {
                Snackbar.Clear();
                Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                Snackbar.Add($"First select a distribution!", Severity.Error);
                visibleSign = false;
                return;
            }
            if (_jobObservation.OperationId == new int())
            {
                Snackbar.Clear();
                Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                Snackbar.Add($"First select an operation!", Severity.Error);
                visibleSign = false;
                return;
            }
            if (_jobObservation.Option == 3 && _jobObservation.Anomaly.IsNullOrEmpty())
            {
                Snackbar.Clear();
                Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                Snackbar.Add($"Write down the anomaly first", Severity.Error);
                return;
            }

            if (_jobObservation.OperatorId == new int())
            {
                Snackbar.Clear();
                Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                Snackbar.Add($"First select an operator!", Severity.Error);
                visibleSign = false;
                return;
            }
            if (_jobObservation.OperatorSignature == null || _jobObservation.OperatorSignature == "")
            {
                Snackbar.Clear();
                Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                Snackbar.Add($"Operator's Signature is missing!", Severity.Error);
                visibleSign = false;
                return;
            }

            User operatorUser = await UsersService.GetUser(_jobObservation.OperatorId);

            if (_jobObservation.OperatorSignature != operatorUser.Payroll.ToString())
            {
                Snackbar.Clear();
                Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                Snackbar.Add($"Operator Signature doesn't match", Severity.Error);
                return;
            }

            endHour = DateTime.Now.TimeOfDay;

            if (CultureInfo.CurrentCulture.Name == "en-US")
            {
                var formatedStartDate = _jobObservation.StartDate;
                var formatedEndDate = _jobObservation.EndDate;

                var EnglishStartDate = formatedStartDate?.Month.ToString() + "/" + formatedStartDate?.Day.ToString() + "/" + formatedStartDate?.Year.ToString();
                var EnglishEndDate = formatedEndDate?.Month.ToString() + "/" + formatedEndDate?.Day.ToString() + "/" + formatedEndDate?.Year.ToString();
                _jobObservation.StartDate = DateTime.ParseExact(EnglishStartDate, "M/d/yyyy", CultureInfo.InvariantCulture);
                _jobObservation.EndDate = DateTime.ParseExact(EnglishEndDate, "M/d/yyyy", CultureInfo.InvariantCulture);

                hour1 = _jobObservation.StartDate?.ToShortDateString() + $" {startHour}";
                hour2 = _jobObservation.EndDate?.ToShortDateString() + $" {endHour?.ToString("hh\\:mm\\:ss")}";

                if (DateTime.TryParseExact(hour1, $"M/d/yyyy HH:mm:ss", null, DateTimeStyles.None, out newDate1))
                {
                    Console.WriteLine(newDate1);
                }
                else
                {
                    Snackbar.Clear();
                    Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                    Snackbar.Add($"Error in Date Start", Severity.Error);
                    Console.WriteLine("Unable to parse '{0}'", hour1);
                }


                if (DateTime.TryParseExact(hour2, $"M/d/yyyy HH:mm:ss", null, DateTimeStyles.None, out newDate2))
                {
                    Console.WriteLine(newDate2);
                }
                else
                {
                    Snackbar.Clear();
                    Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                    Snackbar.Add($"Error in Date End", Severity.Error);
                    Console.WriteLine("Unable to parse '{0}'", hour2);
                }

                _jobObservation.Models = models[0] + "|" + models[1] + "|" + models[2] + "|" + models[3] + "|" + models[4];
                _jobObservation.Cicles = cycles[0] + "|" + cycles[1] + "|" + cycles[2] + "|" + cycles[3] + "|" + cycles[4];
                _jobObservation.HOEStandardTimes = HoeTimes[0] + "|" + HoeTimes[1] + "|" + HoeTimes[2] + "|" + HoeTimes[3] + "|" + HoeTimes[4];
                _jobObservation.StartDate = newDate1;
                _jobObservation.EndDate = newDate2;
                _jobObservation.SsvSignature = "Signed";

                //Eventual
                _jobObservation.Type = 2;
                _jobObservation.PlannedStartDate = newDate1;
                _jobObservation.PlannedEndDate = newDate2;

                _jobObservation.Status = 6;
                _jobObservation.FinishedDate = DateTime.Now;

                var result = await JobObservationService.CreateJobObservation(_jobObservation);
                if (result != null)
                {
                    Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                    Snackbar.Add($"Job Observation Created", Severity.Info);

                    if (_tempLup.Count > 0)
                    {
                        _jobObservations = await JobObservationService.GetAllJobObservations();
                        foreach (var temp in _tempLup)
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
            else
            {
                hour1 = _jobObservation.StartDate?.ToShortDateString() + $" {startHour}";
                hour2 = _jobObservation.EndDate?.ToShortDateString() + $" {endHour?.ToString("hh\\:mm\\:ss")}";

                if (DateTime.TryParseExact(hour1, $"d/M/yyyy HH:mm:ss", null, DateTimeStyles.None, out newDate1))
                {
                    Console.WriteLine(newDate1);
                }
                else
                {
                    Snackbar.Clear();
                    Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                    Snackbar.Add($"Error in Date Start", Severity.Error);
                    Console.WriteLine("Unable to parse '{0}'", hour1);
                }


                if (DateTime.TryParseExact(hour2, $"d/M/yyyy HH:mm:ss", null, DateTimeStyles.None, out newDate2))
                {
                    Console.WriteLine(newDate2);
                }
                else
                {
                    Snackbar.Clear();
                    Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                    Snackbar.Add($"Error in Date End", Severity.Error);
                    Console.WriteLine("Unable to parse '{0}'", hour2);
                }

                _jobObservation.Models = models[0] + "|" + models[1] + "|" + models[2] + "|" + models[3] + "|" + models[4];
                _jobObservation.Cicles = cycles[0] + "|" + cycles[1] + "|" + cycles[2] + "|" + cycles[3] + "|" + cycles[4];
                _jobObservation.HOEStandardTimes = HoeTimes[0] + "|" + HoeTimes[1] + "|" + HoeTimes[2] + "|" + HoeTimes[3] + "|" + HoeTimes[4];
                _jobObservation.StartDate = newDate1;
                _jobObservation.EndDate = newDate2;
                _jobObservation.SsvSignature = "Signed";
                _jobObservation.Status = 6;
                //Eventual
                _jobObservation.Type = 2;
                _jobObservation.PlannedStartDate = newDate1;
                _jobObservation.PlannedEndDate = newDate2;

                var result = await JobObservationService.CreateJobObservation(_jobObservation);
                if (result != null)
                {
                    Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                    Snackbar.Add($"Job Observation Created", Severity.Info);

                    if (_tempLup.Count > 0)
                    {
                        _jobObservations = await JobObservationService.GetAllJobObservations();
                        foreach (var temp in _tempLup)
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
        }

        private DialogOptions dialogOptions = new() { CloseOnEscapeKey = true, MaxWidth = MaxWidth.Large, FullWidth = true };

        //Files Path
        private CDMS_CCP_Archives? CcpFilesInFolder;
        private CDMS_HOE_Archives? HoeFilesInFolder;
        private CDMS_GOS_Archives? GosFilesInFolder;
        private CDMS_CCP_Archives? AuxCcpFilesInFolder;
        private CDMS_HOE_Archives? AuxHoeFilesInFolder;
        private CDMS_GOS_Archives? AuxGosFilesInFolder;
        //Error Display Rutes Select ONLY
        private bool folderCCPError = false;
        private bool folderHOEError = false;
        private bool folderGOSError = false;
        //Display Files Errors
        private bool folderErrorGOS = false;
        private bool folderErrorCCP = false;
        private bool folderErrorHOE = false;
        private string HOErute = "";
        private string CCPrute = "";
        private string GOSrute = "";
        //CommonDirection
        private bool folderErrorGOSCD = false;
        private bool folderErrorCCPCD = false;
        private bool folderErrorHOECD = false;
        private string HOEruteCD = "";
        private string CCPruteCD = "";
        private string GOSruteCD = "";
        //CommonDirection Files
        private CDMS_CCP_Archives? CcpFilesInFolderCD;
        private CDMS_HOE_Archives? HoeFilesInFolderCD;
        private CDMS_GOS_Archives? GosFilesInFolderCD;
        private CDMS_CCP_Archives? AuxCcpFilesInFolderCD;
        private CDMS_HOE_Archives? AuxHoeFilesInFolderCD;
        private CDMS_GOS_Archives? AuxGosFilesInFolderCD;



        private int plantId = 0;
        private bool if_pick_Plant = false;
        private int areaId = 0;
        private bool if_pick_Area = false;
        private int distributionId = 0;
        private bool if_pick_Distribution = false;
        private int productId = 0;
        public int idFilter;

        MudTabs FilesViewer;
        MudTabPanel HOE;
        MudTabPanel HOECD;
        MudTabPanel CCP;
        MudTabPanel CCPCD;
        MudTabPanel GOS;
        MudTabPanel GOSCD;

        public bool CodePathModalDisplay { get; set; } = false;
        private string searchCodeString = "";
        bool ShowLoading = true;
        private IList<string> _sourceMsgLoading = new List<string>();
        private IList<Color> _Colors = new List<Color>() { Color.Default, Color.Primary, Color.Secondary, Color.Success, Color.Info, Color.Default, Color.Primary, Color.Secondary, Color.Success, Color.Info };

        SOSCodePath CodePathDialogDisplay { get; set; }

        private List<SOSCodePath> listFilter = new();
        bool FilterOperation = false;

        private async void CloseModalFiles()
        {
            CodePathModalDisplay = false;

            StateHasChanged();

        }
        private async Task<AsyncVoidMethodBuilder> OpenDialogCodePath(SOSCodePath itemselected, MudTabPanel panelSelect)
        {
            searchCodeString = itemselected.Code;
            ShowLoading = true;
            CodePathModalDisplay = true;
            HoeFilesInFolder = new CDMS_HOE_Archives();
            StateHasChanged();

            try
            {
                CodePathDialogDisplay = itemselected;

                HOErute = itemselected.HOE;
                if (itemselected.HOE != "")
                {
                    Console.WriteLine($"hoe {itemselected.HOE}");
                    HoeFilesInFolder = await CDMSServices.GetFilesHOE(itemselected.HOE);
                    if (HoeFilesInFolder == null)
                        folderErrorHOE = true;
                    else
                    {
                        AuxHoeFilesInFolder = ObjectCloner.ObjectCloner.DeepClone(HoeFilesInFolder);

                        folderErrorHOE = false;
                    }
                }

                folderErrorGOS = true;
                GOSrute = itemselected.GOS;

                if (itemselected.GOS != "")
                {

                    Console.WriteLine($"gos {GOSrute}");


                    GosFilesInFolder = await CDMSServices.GetFilesGOS(GOSrute);
                    if (GosFilesInFolder == null)
                    {
                        folderErrorGOS = true;
                    }
                    else
                    {
                        folderErrorGOS = false;
                        AuxGosFilesInFolder = ObjectCloner.ObjectCloner.DeepClone(GosFilesInFolder);
                    }

                }

                folderErrorCCP = true;
                CCPrute = itemselected.CCP;
                if (itemselected.CCP != "")
                {

                    Console.WriteLine($"CCP {CCPrute}");

                    CcpFilesInFolder = new CDMS_CCP_Archives();
                    CcpFilesInFolder = await CDMSServices.GetFilesCCP(CCPrute);
                    if (CcpFilesInFolder == null)
                        folderErrorCCP = true;
                    else
                    {
                        AuxCcpFilesInFolder = ObjectCloner.ObjectCloner.DeepClone(CcpFilesInFolder);
                        folderErrorCCP = false;

                    }
                }


                //Common Directions
                folderErrorGOSCD = true;
                if (itemselected.CommonDirectionGOS != "")
                {

                    GOSruteCD = itemselected.CommonDirectionGOS;

                    Console.WriteLine($"gos cd {GOSruteCD}");

                    GosFilesInFolderCD = new CDMS_GOS_Archives();

                    GosFilesInFolderCD = await CDMSServices.GetFilesGOS(GOSruteCD);
                    if (GosFilesInFolderCD == null)
                    {
                        folderErrorGOSCD = true;
                    }
                    else
                    {
                        folderErrorGOSCD = false;
                        AuxGosFilesInFolderCD = ObjectCloner.ObjectCloner.DeepClone(GosFilesInFolderCD);
                    }

                }

                folderErrorCCPCD = true;
                if (itemselected.CommonDirectionCCP != "")
                {
                    CCPruteCD = itemselected.CommonDirectionCCP;
                    Console.WriteLine($"Ccp cd {CCPruteCD}");

                    CcpFilesInFolderCD = new CDMS_CCP_Archives();
                    CcpFilesInFolderCD = await CDMSServices.GetFilesCCP(CCPruteCD);
                    if (CcpFilesInFolderCD == null)
                        folderErrorCCPCD = true;
                    else
                    {
                        folderErrorCCPCD = false;
                        AuxCcpFilesInFolderCD = ObjectCloner.ObjectCloner.DeepClone(CcpFilesInFolderCD);
                    }

                }

                if (itemselected.CommonDirectionHOE != "")
                {


                    folderErrorHOECD = true;

                    HOEruteCD = itemselected.CommonDirectionHOE;
                    Console.WriteLine($"hoe cd {HOEruteCD}");
                    HoeFilesInFolderCD = new CDMS_HOE_Archives();
                    HoeFilesInFolderCD = await CDMSServices.GetFilesHOE(HOEruteCD);
                    if (HoeFilesInFolderCD == null)
                        folderErrorHOECD = true;
                    else
                    {
                        AuxHoeFilesInFolderCD = ObjectCloner.ObjectCloner.DeepClone(HoeFilesInFolderCD);

                        folderErrorHOECD = false;
                    }
                }

                //EndCommon Directions




            }
            catch (Exception ex)
            {
                Console.WriteLine($"OpenDialogCodePath Error: {ex.Message}");
            }
            finally
            {
                await SearchFunction();
                ShowLoading = false;
                StateHasChanged();
            }

            return new AsyncVoidMethodBuilder();
        }
        private async Task<AsyncVoidMethodBuilder> SearchFunction()
        {
            Console.WriteLine($"SearchFunction - Start {DateTime.Now}");

            if (CodePathDialogDisplay != null)
            {
                try
                {
                    ShowLoading = true;
                    Console.WriteLine($"State Start {ShowLoading}");
                    StateHasChanged();

                    if (string.IsNullOrEmpty(searchCodeString))
                    {
                        if (CodePathDialogDisplay.HOE != "")
                            HoeFilesInFolder = ObjectCloner.ObjectCloner.DeepClone(AuxHoeFilesInFolder);

                        if (CodePathDialogDisplay.GOS != "")
                            GosFilesInFolder = ObjectCloner.ObjectCloner.DeepClone(AuxGosFilesInFolder);

                        if (CodePathDialogDisplay.CCP != "")
                            CcpFilesInFolder = ObjectCloner.ObjectCloner.DeepClone(AuxCcpFilesInFolder);

                        if (CodePathDialogDisplay.CommonDirectionHOE != "")
                            HoeFilesInFolderCD = ObjectCloner.ObjectCloner.DeepClone(AuxHoeFilesInFolderCD);

                        if (CodePathDialogDisplay.CommonDirectionGOS != "")
                            GosFilesInFolderCD = ObjectCloner.ObjectCloner.DeepClone(AuxGosFilesInFolderCD);

                        if (CodePathDialogDisplay.CommonDirectionCCP != "")
                            CcpFilesInFolderCD = ObjectCloner.ObjectCloner.DeepClone(AuxCcpFilesInFolderCD);
                    }
                    else
                    {
                        if (CodePathDialogDisplay.HOE != "")
                        {
                            HoeFilesInFolder = ObjectCloner.ObjectCloner.DeepClone(AuxHoeFilesInFolder);
                            HoeFilesInFolder.operation = HoeFilesInFolder.operation.Where(x => x.Nombre.ToLower().Contains(searchCodeString.ToLower(), StringComparison.InvariantCultureIgnoreCase)).ToList();
                        }

                        if (CodePathDialogDisplay.GOS != "")
                        {
                            GosFilesInFolder = ObjectCloner.ObjectCloner.DeepClone(AuxGosFilesInFolder);
                            GosFilesInFolder.operation = AuxGosFilesInFolder.operation.Where(x => x.Nombre.ToLower().Contains(searchCodeString.ToLower(), StringComparison.InvariantCultureIgnoreCase)).ToList();
                        }

                        if (CodePathDialogDisplay.CCP != "")
                        {
                            CcpFilesInFolder = ObjectCloner.ObjectCloner.DeepClone(AuxCcpFilesInFolder);
                            CcpFilesInFolder.operation = CcpFilesInFolder.operation.Where(x => x.Nombre.ToLower().Contains(searchCodeString.ToLower(), StringComparison.InvariantCultureIgnoreCase)).ToList();
                        }

                        if (CodePathDialogDisplay.CommonDirectionHOE != "")
                        {
                            HoeFilesInFolderCD = ObjectCloner.ObjectCloner.DeepClone(AuxHoeFilesInFolderCD);
                            HoeFilesInFolderCD.operation = HoeFilesInFolderCD.operation.Where(x => x.Nombre.ToLower().Contains(searchCodeString.ToLower(), StringComparison.InvariantCultureIgnoreCase)).ToList();
                        }

                        if (CodePathDialogDisplay.CommonDirectionGOS != "")
                        {
                            GosFilesInFolderCD = ObjectCloner.ObjectCloner.DeepClone(AuxGosFilesInFolderCD);
                            GosFilesInFolderCD.operation = GosFilesInFolderCD.operation.Where(x => x.Nombre.ToLower().Contains(searchCodeString.ToLower(), StringComparison.InvariantCultureIgnoreCase)).ToList();
                        }

                        if (CodePathDialogDisplay.CommonDirectionCCP != "")
                        {
                            CcpFilesInFolderCD = ObjectCloner.ObjectCloner.DeepClone(AuxCcpFilesInFolderCD);
                            CcpFilesInFolderCD.operation = CcpFilesInFolderCD.operation.Where(x => x.Nombre.ToLower().Contains(searchCodeString.ToLower(), StringComparison.InvariantCultureIgnoreCase)).ToList();
                        }

                    }



                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error Filter: {ex.Message}");
                }
                finally
                {
                    ShowLoading = false;
                    StateHasChanged();
                }
            }
            else
            {
                Console.WriteLine($"Error Filter es nullo");
            }

            Console.WriteLine($"SearchFunction - End {DateTime.Now}");
            Console.WriteLine($"State End {ShowLoading}");
            //// if text is null or empty, show complete list
            //if (string.IsNullOrEmpty(searchString))
            //    return GosFilesInFolder.operation;

            //return GosFilesInFolder.operation.Where(x => x.Nombre.ToLower().Contains(searchString.ToLower(), StringComparison.InvariantCultureIgnoreCase)).ToList();
            return new AsyncVoidMethodBuilder();
        }


        private async Task DownloadFileFromURL(string urlroute, string namefile)
        {
            var fileName = namefile;
            var fileURL = urlroute;
            await JS.InvokeVoidAsync("triggerFileDownload", fileName, fileURL);
        }

        private async Task DownloadFileFromURL_HOE(string urlroute, string namefile)
        {
            var fileName = namefile;
            var fileURL = urlroute;
            await JS.InvokeVoidAsync("triggerFileDownload", fileName, fileURL);
        }
        private async Task DownloadFileFromURL_CCP(string urlroute, string namefile)
        {

            CDMS_DownloadFile DownloadLink = await CDMSServices.GetDownloadLinkCCP(urlroute);

            if (DownloadLink is not null)
            {
                var fileName = namefile;
                var fileURL = DownloadLink?.operation.URL;

                Console.WriteLine($"NamekEY: {DownloadLink?.operation.NameDocKey}");

                try
                {
                    var result = await JS.InvokeAsync<string>("triggerFileDownloadAndWaitForConfirmation", fileName, fileURL);
                    if (result == "File downloaded successfully")
                    {
                        var DeleteTemp = await CDMSServices.DeleteFileTempCCP(DownloadLink?.operation.NameDocKey);
                        if (DeleteTemp is not null)
                        {
                            Console.WriteLine($"Download GOS - fileDownlaod Succes");
                        }
                    }

                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error In Download Gos File: {ex.Message} ");
                }
            }
        }
        private async Task DownloadFileFromURL_GOS(string urlroute, string namefile)
        {
            CDMS_DownloadFile DownloadLink = await CDMSServices.GetDownloadLinkGOS(urlroute);

            if (DownloadLink is not null)
            {
                var fileName = namefile;
                var fileURL = DownloadLink?.operation.URL;

                Console.WriteLine($"NamekEY: {DownloadLink?.operation.NameDocKey}");

                try
                {
                    var result = await JS.InvokeAsync<string>("triggerFileDownloadAndWaitForConfirmation", fileName, fileURL);
                    if (result == "File downloaded successfully")
                    {
                        var DeleteTemp = await CDMSServices.DeleteFileTempGOS(DownloadLink?.operation.NameDocKey);
                        if (DeleteTemp is not null)
                        {
                            Console.WriteLine($"Download GOS - fileDownlaod Succes");
                        }
                    }

                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error In Download Gos File: {ex.Message} ");
                }
            }

        }

    /////
    }
}
