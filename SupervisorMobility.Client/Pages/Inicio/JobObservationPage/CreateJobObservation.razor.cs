using BlazorCameraStreamer;
using Blazorise.Extensions;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using MudBlazor;
using SupervisorMobility.Client.Data.Entities;
using SupervisorMobility.Client.Data.Entities.TreeStruct;
using SupervisorMobility.Client.Pages.Configuration.PlantPage;
using System;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Timers;
using Microsoft.AspNetCore.Components.Authorization;
using System.Net.Http.Headers;
using SupervisorMobility.Client.Services.AssyChartService;
using SupervisorMobility.Client.Services.BreadcrumsService;

namespace SupervisorMobility.Client.Pages.Inicio.JobObservationPage
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

        public List<string> area_ListS = new List<string>();
        public List<string> area_ListQ = new List<string>();
        public List<string> area_ListD = new List<string>();
        public List<string> area_ListC = new List<string>();
        public List<string> area_ListOther = new List<string>();
        public JobObservation _jobObservation { get; set; } = new();

        public string areaS;
        public string areaQ;
        public string areaD;
        public string areaC;
        public string areaOther;

        int[] models = new int[5];
        string[] cycles = new string[5] { "", "", "", "", "" };
        double[] HoeTimes = new double[5] { 0.0, 0.0, 0.0, 0.0, 0.0 };

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

        string cycle1Color = "";
        string cycle2Color = "";
        string cycle3Color = "";
        string cycle4Color = "";
        string cycle5Color = "";

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

        public Distribution distribution = new Distribution();
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

        public string[] questions = new string[5];
        public double taktTime { get; set; }
        public int kpiID = 0;
        public int auxErgonomicsLevel = 0;

      
        //Checklist Categories and questions
        public List<JobCategoryStructure> _checklistCategoriesAndQuestions { get; set; } = new();
        private Dictionary<int, string> questionResponses = new Dictionary<int, string>();
        private Dictionary<int, ChecklistAnswer> questionAnswers = new Dictionary<int, ChecklistAnswer>();

        public List<ChecklistAnswer> _checklistAnswers { get; set; } = new();

        string currentLanguage = "es-ES";
        protected async override Task OnInitializedAsync()
        {

            try
            {
                currentLanguage = await JS.InvokeAsync<string>("localStorage.getItem", "i18nextLng");
                Console.WriteLine($" Current:'{currentLanguage}'");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception Load Language: {ex.Message}");
            }
         

            _links = new List<BreadcrumbItem>
            {
                new BreadcrumbItem(text: Localizer["home"], href: "/"),
                new BreadcrumbItem(text: Localizer["jobObservations"], href: "/jobobservation"),
                new BreadcrumbItem(text: Localizer["create"] + " " +  Localizer["jobObservation"], href: "", disabled: true)
            };
            BreadcrumbService.UpdateBreadcrumbs(_links);
            _jobObservation.Supervisor = new();


            date = date.Replace("-", "/");

            _jobObservation.IsActive = true;
            _jobObservation.StartDate = DateTime.ParseExact(date, "d/M/yyyy", null);
            _jobObservation.EndDate = DateTime.ParseExact(date, "d/M/yyyy", null);
            _jobObservation.Option = 1;

            _plants = await PlantServices.GetPlants();
            _plants = _plants.OrderBy(p => p.Description).ToList();
            _checklistCategoriesAndQuestions = await JobStructureCategoriesService.GetChecklistCategories(true);

            foreach (var category in _checklistCategoriesAndQuestions)
            {
                foreach (var question in category.ChecklistQuestions)
                {
                    ChecklistAnswer newChAnswer = new();
                    newChAnswer.JobObservationId = _jobObservation.JobObservationId;
                    newChAnswer.QuestionID = question.QuestionID;
                    newChAnswer.Prompt = question.Prompt;
                    questionAnswers.Add(question.QuestionID, newChAnswer);
                }
            }

            await GetUserAsync();
            StateHasChanged();

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


                if (user.UserType == 1)
                {
                    if (PatPlantId != null)
                    {

                        _jobObservation.PlantId = int.Parse(PatPlantId);

                        _jobObservation.AreaId = int.Parse(PatAreaId);

                        _areas = await AreaServices.GetAreas(_jobObservation.PlantId);
                        _areas = _areas.OrderBy(a => a.Description).ToList();

                        _jobObservation.SupervisorId = int.Parse(PatSupervisorId);

                        _jobObservation.Supervisor = await UsersService.GetUser(_jobObservation.SupervisorId);

                        _distributions = await DistributionService.GetDistributionsWithCollections(_jobObservation.PlantId, _jobObservation.AreaId);
                        _distributions = _distributions.OrderBy(d => d.Description).ToList();

                        _jobObservation.DistributionId = int.Parse(PatDistributionId);

                        _products = _distributions[_distributions.FindIndex(d => d.DistributionId == _jobObservation.DistributionId)].Products;
                        _products = _products.OrderBy(p => p.Description).ToList();

                        _operations = _distributions[_distributions.FindIndex(d => d.DistributionId == _jobObservation.DistributionId)].Operations;
                        _operations = _operations.OrderBy(o => o.Description).ToList();

                        _jobObservation.OperationId = int.Parse(PatOperationId);


                        _supervisors = await UsersService.GetUsersByUserTypeInPlantAndArea(_jobObservation.PlantId, _jobObservation.AreaId, 3, false, false);
                        _supervisors = _supervisors.OrderBy(s => s.Name).ToList();

                        _operators = await UsersService.GetSubordinates(_jobObservation.SupervisorId, false);
                        _operators = _operators.OrderBy(o => o.Name).ToList();

                        _jobObservation.OperatorId = int.Parse(PatOperatorId);
                    }
                    else
                    {
                        _jobObservation.PlantId = 0;
                        _jobObservation.AreaId = 0;
                        _jobObservation.SupervisorId = 0;
                        _allSupervisors = await UsersService.GetUsersByType(3, true, false);
                        _allSupervisors = _allSupervisors.OrderBy(s => s.Name).ToList();


                    }

                }
                else if (user.UserType == 2)
                {
                    _jobObservation.PlantId = (int)user.PlantId;
                    _jobObservation.AreaId = 0;
                    _jobObservation.SupervisorId = 0;
                    //_allSupervisors = await UsersService.GetUsersByType(3);
                    //_operators = await UsersService.GetUsersByType(4);


                }
                else
                {

                    if (PatPlantId != null)
                    {
                        _jobObservation.PlantId = int.Parse(PatPlantId);

                        _jobObservation.AreaId = int.Parse(PatAreaId);

                        _areas = await AreaServices.GetAreas(_jobObservation.PlantId);
                        _areas = _areas.OrderBy(a => a.Description).ToList();

                        _jobObservation.SupervisorId = int.Parse(PatSupervisorId);

                        _jobObservation.Supervisor = await UsersService.GetUser(_jobObservation.SupervisorId);

                        _distributions = await DistributionService.GetDistributionsWithCollections(_jobObservation.PlantId, _jobObservation.AreaId);
                        _distributions = _distributions.OrderBy(d => d.Description).ToList();

                        _jobObservation.DistributionId = int.Parse(PatDistributionId);

                        _products = _distributions[_distributions.FindIndex(d => d.DistributionId == _jobObservation.DistributionId)].Products;
                        _products = _products.OrderBy(p => p.Description).ToList();

                        _operations = _distributions[_distributions.FindIndex(d => d.DistributionId == _jobObservation.DistributionId)].Operations;
                        _operations = _operations.OrderBy(o => o.Description).ToList();

                        _jobObservation.OperationId = int.Parse(PatOperationId);

                        //operator User
                        _operators = await UsersService.GetSubordinates(_jobObservation.SupervisorId, false);
                        _operators = _operators.OrderBy(o => o.Name).ToList();
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
                        _areas = _areas.OrderBy(a => a.Description).ToList();


                        _jobObservation.SupervisorId = user.UserId;
                        _jobObservation.Supervisor = await UsersService.GetUser(user.UserId);

                        _distributions = await DistributionService.GetDistributionsWithCollections(_jobObservation.PlantId, _jobObservation.AreaId);
                        _distributions = _distributions.OrderBy(d => d.Description).ToList();


                        //operator User
                        //_operators = await UsersService.GetUsersByType(4, true, false);


                        _operators = await UsersService.GetSubordinates(_jobObservation.SupervisorId, false);
                        _operators = _operators.OrderBy(o => o.Name).ToList();
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

            StateHasChanged();


            try
            {
                CCPFolders = await CDMSServices.GetFoldersCCP();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error Get CCP Folder From CCP");
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.Message);
            }

            if (CCPFolders != null)
            {
                folderCCPError = false;
                rootNodeCCP = TreeServices.ConstruirArbolCCP(CCPFolders.operation);
            }
            else
            {
                folderCCPError = true;
            }

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
            _areas = _areas.OrderBy(a => a.Description).ToList();

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
                _supervisors = new();
                _supervisors = await UsersService.GetSubordinates(user.UserId, false);
                _supervisors = _supervisors.OrderBy(s => s.Name).ToList();

            }

            _jobObservation.OperatorId = 0;
            _jobObservation.DistributionId = 0;
            _jobObservation.OperationId = 0;
            _distributions = await DistributionService.GetDistributionsWithCollections(_jobObservation.PlantId, _jobObservation.AreaId);
            _distributions = _distributions.OrderBy(d => d.Description).ToList();

            StateHasChanged();
        }

        private async void ShowOperators()
        {
            if (_jobObservation.DistributionId != 0 && _jobObservation.OperationId != 0)
                ShowPastJobObservations();

            if (user.UserType == 1 || user.UserType == 2)
            {
                _operators = await UsersService.GetSubordinates(_jobObservation.SupervisorId, false);
                _operators = _operators.OrderBy(o => o.Name).ToList();
            }
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
            _products = _products.OrderBy(p => p.Description).ToList();

            _jobObservation.OperationId = 0;
            _operations = _distributions[_distributions.FindIndex(d => d.DistributionId == _jobObservation.DistributionId)].Operations;
            _operations = _operations.OrderBy(o => o.Description).ToList();

            _assychart = await AssychartsServices.GetAssyChartJobObservation(_jobObservation.PlantId, _jobObservation.AreaId, _jobObservation.DistributionId);
            if (_assychart != null && _assychart.ErgonomicsLevel != null)
            {
                auxErgonomicsLevel = (int)_assychart.ErgonomicsLevel;
            }

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

                        pastJob = await JobObservationService.GetJobObservationById(job.JobObservationId, true, true, true, false, false);
                        foreach (var lups in pastJob.Lup)
                        {
                            pastLup.Add(lups);
                        }
                    }

                }


            }
            pastjobObservations = pastjobObservations.OrderBy(x => x.StartDate).ToList();

            if (_assychart != null && _assychart.RoutesProductsAssyChart?.Count > 0)
            {
                listFilter = _assychart.RoutesProductsAssyChart.Where(r => r.Code.ToLower().Contains(operation.Code.ToLower(), StringComparison.OrdinalIgnoreCase)).ToList();
                FilterOperation = true;
            }


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


            _jobObservation.ModelsSpecification = models[0] + "|" + models[1] + "|" + models[2] + "|" + models[3] + "|" + models[4];
            _jobObservation.Cycles = cycles[0] + "|" + cycles[1] + "|" + cycles[2] + "|" + cycles[3] + "|" + cycles[4];
            _jobObservation.HOEStandardTimes = HoeTimes[0] + "|" + HoeTimes[1] + "|" + HoeTimes[2] + "|" + HoeTimes[3] + "|" + HoeTimes[4];
            _jobObservation.Questions = questions[0] + "|" + questions[1] + "|" + questions[2] + "|" + questions[3] + "|" + questions[4];
            _jobObservation.TaktTime = taktTime.ToString();
            _jobObservation.KpiId = kpiID;

            if (_jobObservation.HOEStandardTimes != null)
                _jobObservation.HOEStandardTimes = _jobObservation.HOEStandardTimes.Replace(",", ".");
            if (_jobObservation.Cycles != null)
                _jobObservation.Cycles = _jobObservation.Cycles.Replace(",", ".");

            //Eventual
            _jobObservation.Type = 2;
            _jobObservation.Status = 1;

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

                _jobObservation.PlannedStartDate = newDate1;
                _jobObservation.PlannedEndDate = newDate2;
                _jobObservation.StartDate = newDate1;
                _jobObservation.EndDate = newDate2;
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

                _jobObservation.PlannedStartDate = newDate1;
                _jobObservation.PlannedEndDate = newDate2;
                _jobObservation.StartDate = newDate1;
                _jobObservation.EndDate = newDate2;
            }


            _jobObservation.Lup = _tempLup;
            foreach (var question in questionAnswers)
            {
                if (question.Value.Answer != "" || question.Value.Edited)
                {
                    _jobObservation.ChecklistAnswers?.Add(question.Value);
                }
            }

            var result = await JobObservationService.CreateJobObservationWithLup(_jobObservation);

            //var result = await JobObservationService.CreateJobObservation(_jobObservation);
            if (result != null)
            {
                Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                Snackbar.Add($"Job Observation Created", Severity.Info);

                _jobObservation = result;
                _ = await GenerateChecklistAnswers();


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

            TimeSpan hundreths;
            double centiseconds = 0.0;
            if (TimeSpan.TryParseExact(elapsedTime, "hh\\:mm\\:ss\\.fff", CultureInfo.InvariantCulture, out hundreths))
            {
                centiseconds = hundreths.TotalMilliseconds / 60000.0;
            }
            else
            {
                Console.WriteLine("Wrong timestamp format.");
            }
            switch (opt)
            {
                case 1:
                    cycles[0] = string.Format("{0:0.000}", centiseconds); break;
                case 2:
                    cycles[1] = string.Format("{0:0.000}", centiseconds); break;
                case 3:
                    cycles[2] = string.Format("{0:0.000}", centiseconds); break;
                case 4:
                    cycles[3] = string.Format("{0:0.000}", centiseconds); break;
                case 5:
                    cycles[4] = string.Format("{0:0.000}", centiseconds); break;
            }

            DateTime currentTime = e.SignalTime;
            elapsedTime = $"{currentTime.Subtract(startTime)}".Substring(0, 12);
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
                case 1: isRunning = true; cycle1Color = ""; break;
                case 2: isRunning2 = true; cycle2Color = ""; break;
                case 3: isRunning3 = true; cycle3Color = ""; break;
                case 4: isRunning4 = true; cycle4Color = ""; break;
                case 5: isRunning5 = true; cycle5Color = ""; break;
            }

        }
        void StopTimer()
        {
            TimeSpan hundreths;
            double centiseconds = 0.0;
            if (TimeSpan.TryParseExact(elapsedTime, "hh\\:mm\\:ss\\.fff", CultureInfo.InvariantCulture, out hundreths))
            {
                centiseconds = hundreths.TotalMilliseconds / 60000.0;
            }
            else
            {
                Console.WriteLine("Wrong timestamp format.");
            }
            switch (opt)
            {
                case 1:
                    cycles[0] = string.Format("{0:0.000}", centiseconds); isRunning = false; break;
                case 2:
                    cycles[1] = string.Format("{0:0.000}", centiseconds); isRunning2 = false; break;
                case 3:
                    cycles[2] = string.Format("{0:0.000}", centiseconds); isRunning3 = false; break;
                case 4:
                    cycles[3] = string.Format("{0:0.000}", centiseconds); isRunning4 = false; break;
                case 5:
                    cycles[4] = string.Format("{0:0.000}", centiseconds); isRunning5 = false; break;
            }

            isRunning = false;
            Console.WriteLine($"Elapsed Time: {elapsedTime}");
            timer.Enabled = false;
            elapsedTime = DEFAULT_TIME;


        }

        void OnTimerChanged(int option)
        {
            if (taktTime == 0.0)
            {
                Snackbar.Clear();
                Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                Snackbar.Add($"First enter the Takt Time", Severity.Warning);
                return;
            }
            if (option == 1 && HoeTimes[0] == 0.0)
            {
                Snackbar.Clear();
                Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                Snackbar.Add($"First enter the Hoe Standard Time 1", Severity.Warning);
                return;
            }
            else if (option == 2 && HoeTimes[1] == 0.0)
            {

                Snackbar.Clear();
                Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                Snackbar.Add($"First enter the Hoe Standard Time 2", Severity.Warning);
                return;
            }
            else if (option == 3 && HoeTimes[2] == 0.0)
            {
                Snackbar.Clear();
                Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                Snackbar.Add($"First enter the Hoe Standard Time 3", Severity.Warning);
                return;
            }
            else if (option == 4 && HoeTimes[3] == 0.0)
            {
                Snackbar.Clear();
                Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                Snackbar.Add($"First enter the Hoe Standard Time 4", Severity.Warning);
                return;
            }
            else if (option == 5 && HoeTimes[4] == 0.0)
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


                for (int i = 0; i < HoeTimes.Length; i++)
                {
                    if (double.TryParse(cycles[i], out double cycleValue2) && HoeTimes[i] != 0.0)
                    {
                        double lowerBound = HoeTimes[i] * 0.95; // Valor mínimo permitido (95% de HoeTimes)
                        double upperBound = HoeTimes[i] * 1.05; // Valor máximo permitido (105% de HoeTimes)

                        if (cycleValue2 >= lowerBound && cycleValue2 <= upperBound)
                        {
                            switch (i)
                            {
                                case 0: cycle1Color = "green"; break;
                                case 1: cycle2Color = "green"; break;
                                case 2: cycle3Color = "green"; break;
                                case 3: cycle4Color = "green"; break;
                                case 4: cycle5Color = "green"; break;
                            }
                        }
                        else if (cycleValue2 < HoeTimes[i])
                        {
                            switch (i)
                            {
                                case 0: cycle1Color = "yellow"; break;
                                case 1: cycle2Color = "yellow"; break;
                                case 2: cycle3Color = "yellow"; break;
                                case 3: cycle4Color = "yellow"; break;
                                case 4: cycle5Color = "yellow"; break;
                            }
                        }
                        else
                        {
                            switch (i)
                            {
                                case 0: cycle1Color = "red"; break;
                                case 1: cycle2Color = "red"; break;
                                case 2: cycle3Color = "red"; break;
                                case 3: cycle4Color = "red"; break;
                                case 4: cycle5Color = "red"; break;
                            }
                            if (areaD == "" || areaD == null)
                                areaD = $"Cycle time {i + 1} ({cycleValue2}) took longer than standard time ({HoeTimes[i]})";
                            else
                            {
                                if (areaD.Contains($"Cycle time {i + 1}"))
                                {
                                    continue;
                                }
                                areaD = areaD + $", Cycle time {i + 1} ({cycleValue2}) took longer than standard time ({HoeTimes[i]})";
                            }
                        }
                        if (cycleValue2 > taktTime)
                        {
                            switch (i)
                            {
                                case 0: cycle1Color = "red"; break;
                                case 1: cycle2Color = "red"; break;
                                case 2: cycle3Color = "red"; break;
                                case 3: cycle4Color = "red"; break;
                                case 4: cycle5Color = "red"; break;
                            }
                            Snackbar.Clear();
                            Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                            Snackbar.Add($"Time is NG, LUP added to Delivery Pillar", Severity.Warning);
                            areaD = $"Cycle time {i + 1} ({cycleValue2}) took longer than Takt time ({taktTime})";
                        }
                    }
                }


            }
            StateHasChanged();
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
            if (_jobObservation.SupervisorId == 0)
            {
                Snackbar.Clear();
                Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                Snackbar.Add($"First select a Supervisor", Severity.Error);
                return;
            }

            List<Lup> lupsToAdd = new List<Lup>();

            switch (pillar)
            {
                case 1:
                    if (area_ListS != null && area_ListS.Count > 0)
                    {
                        foreach(string str in area_ListS)
                        {
                            Lup newLup = new Lup();
                            newLup.Oportunity = ObjectCloner.ObjectCloner.DeepClone(str);
                            lupsToAdd?.Add(newLup);  
                        }
                        area_ListS.Clear();
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
                    if (area_ListQ != null && area_ListQ.Count > 0)
                    {
                        foreach (string str in area_ListQ)
                        {
                            Lup newLup = new Lup();
                            newLup.Oportunity = ObjectCloner.ObjectCloner.DeepClone(str);
                            lupsToAdd?.Add(newLup);
                        }
                        area_ListQ.Clear();
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
                    if (area_ListD != null && area_ListD.Count > 0)
                    {
                        foreach (string str in area_ListD)
                        {
                            Lup newLup = new Lup();
                            newLup.Oportunity = ObjectCloner.ObjectCloner.DeepClone(str);
                            lupsToAdd?.Add(newLup);
                        }
                        area_ListD.Clear();
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
                    if (area_ListC != null && area_ListC.Count > 0)
                    {
                        foreach (string str in area_ListC)
                        {
                            Lup newLup = new Lup();
                            newLup.Oportunity = ObjectCloner.ObjectCloner.DeepClone(str);
                            lupsToAdd?.Add(newLup);
                        }
                        area_ListC.Clear();
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
                    if (area_ListOther != null && area_ListOther.Count > 0)
                    {
                        foreach (string str in area_ListOther)
                        {
                            Lup newLup = new Lup();
                            newLup.Oportunity = ObjectCloner.ObjectCloner.DeepClone(str);
                            lupsToAdd?.Add(newLup);
                        }
                        area_ListOther.Clear();
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


            User svAux = _supervisors?.Find(u => _jobObservation.SupervisorId == u.UserId);

            foreach(Lup LupItem in lupsToAdd)
            {
                LupItem.Observer = svAux.Name;

                LupItem.JobObservationId = 0;
                LupItem.Pillar = pillar;
                LupItem.Status = 1;
                LupItem.CreatedDate = DateTime.Now;
                LupItem.IsActive = true;

                _tempLup.Add(LupItem);
            }
                lup = new();

            Snackbar.Clear();
            Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
            Snackbar.Add($"Lup item added", Severity.Info);



        }

        public void DeleteLup(Lup lup)
        {
            switch (lup.Pillar)
            {
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


            _jobObservation.ModelsSpecification = models[0] + "|" + models[1] + "|" + models[2] + "|" + models[3] + "|" + models[4];
            _jobObservation.Cycles = cycles[0] + "|" + cycles[1] + "|" + cycles[2] + "|" + cycles[3] + "|" + cycles[4];
            _jobObservation.HOEStandardTimes = HoeTimes[0] + "|" + HoeTimes[1] + "|" + HoeTimes[2] + "|" + HoeTimes[3] + "|" + HoeTimes[4];
            _jobObservation.Questions = questions[0] + "|" + questions[1] + "|" + questions[2] + "|" + questions[3] + "|" + questions[4];
            _jobObservation.TaktTime = taktTime.ToString();
            _jobObservation.KpiId = kpiID;
            if (_jobObservation.HOEStandardTimes != null)
                _jobObservation.HOEStandardTimes = _jobObservation.HOEStandardTimes.Replace(",", ".");
            if (_jobObservation.Cycles != null)
                _jobObservation.Cycles = _jobObservation.Cycles.Replace(",", ".");
            //Eventual
            _jobObservation.Type = 2;
            _jobObservation.Status = 2;

            if (_jobObservation.Justification == "")
            {
                _jobObservation.Justification = null;
            }


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
                _jobObservation.PlannedStartDate = newDate1;
                _jobObservation.PlannedEndDate = newDate2;
                _jobObservation.StartDate = newDate1;
                _jobObservation.EndDate = newDate2;
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
                _jobObservation.PlannedStartDate = newDate1;
                _jobObservation.PlannedEndDate = newDate2;
                _jobObservation.StartDate = newDate1;
                _jobObservation.EndDate = newDate2;

            }

            _jobObservation.Lup = _tempLup;
            foreach (var question in questionAnswers)
            {
                if (question.Value.Answer != "" || question.Value.Edited)
                {
                    _jobObservation.ChecklistAnswers?.Add(question.Value);
                }
            }

            var result = await JobObservationService.CreateJobObservationWithLup(_jobObservation);
            //var result = await JobObservationService.CreateJobObservation(_jobObservation);
            if (result != null)
            {
                Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                Snackbar.Add($"Job Observation Created", Severity.Info);
                _jobObservation = result;
                _ = await GenerateChecklistAnswers();

                NavigationManager.NavigateTo("/jobobservation");
            }
            else
                await JSRuntime.InvokeVoidAsync("alert", "Error en los datos!"); // Alert
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



            _jobObservation.ModelsSpecification = models[0] + "|" + models[1] + "|" + models[2] + "|" + models[3] + "|" + models[4];
            _jobObservation.Cycles = cycles[0] + "|" + cycles[1] + "|" + cycles[2] + "|" + cycles[3] + "|" + cycles[4];
            _jobObservation.HOEStandardTimes = HoeTimes[0] + "|" + HoeTimes[1] + "|" + HoeTimes[2] + "|" + HoeTimes[3] + "|" + HoeTimes[4];
            _jobObservation.Questions = questions[0] + "|" + questions[1] + "|" + questions[2] + "|" + questions[3] + "|" + questions[4];
            _jobObservation.TaktTime = taktTime.ToString();
            _jobObservation.KpiId = kpiID;
            if (_jobObservation.HOEStandardTimes != null)
                _jobObservation.HOEStandardTimes = _jobObservation.HOEStandardTimes.Replace(",", ".");
            if (_jobObservation.Cycles != null)
                _jobObservation.Cycles = _jobObservation.Cycles.Replace(",", ".");

            //Eventual
            _jobObservation.Type = 2;
            _jobObservation.Status = 4;

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
                _jobObservation.PlannedStartDate = newDate1;
                _jobObservation.PlannedEndDate = newDate2;
                _jobObservation.StartDate = newDate1;
                _jobObservation.EndDate = newDate2;

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
                _jobObservation.PlannedStartDate = newDate1;
                _jobObservation.PlannedEndDate = newDate2;
                _jobObservation.StartDate = newDate1;
                _jobObservation.EndDate = newDate2;

            }

            _jobObservation.Lup = _tempLup;
            foreach (var question in questionAnswers)
            {
                if (question.Value.Answer != "" || question.Value.Edited)
                {
                    _jobObservation.ChecklistAnswers?.Add(question.Value);
                }
            }

            var result = await JobObservationService.CreateJobObservationWithLup(_jobObservation);
            //var result = await JobObservationService.CreateJobObservation(_jobObservation);
            if (result != null)
            {
                Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                Snackbar.Add($"Job Observation Created", Severity.Info);

                _jobObservation = result;
                _ = await GenerateChecklistAnswers();

                NavigationManager.NavigateTo("/jobobservation");
            }
            else
                await JSRuntime.InvokeVoidAsync("alert", "Error en los datos!"); // Alert
        }


        //Reject Job observation
        async void Reject()
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



            _jobObservation.ModelsSpecification = models[0] + "|" + models[1] + "|" + models[2] + "|" + models[3] + "|" + models[4];
            _jobObservation.Cycles = cycles[0] + "|" + cycles[1] + "|" + cycles[2] + "|" + cycles[3] + "|" + cycles[4];
            _jobObservation.HOEStandardTimes = HoeTimes[0] + "|" + HoeTimes[1] + "|" + HoeTimes[2] + "|" + HoeTimes[3] + "|" + HoeTimes[4];
            _jobObservation.Questions = questions[0] + "|" + questions[1] + "|" + questions[2] + "|" + questions[3] + "|" + questions[4];
            _jobObservation.TaktTime = taktTime.ToString();
            _jobObservation.KpiId = kpiID;
            if (_jobObservation.HOEStandardTimes != null)
                _jobObservation.HOEStandardTimes = _jobObservation.HOEStandardTimes.Replace(",", ".");
            if (_jobObservation.Cycles != null)
                _jobObservation.Cycles = _jobObservation.Cycles.Replace(",", ".");
            //Eventual
            _jobObservation.Type = 2;

            _jobObservation.Status = 5;

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
                _jobObservation.PlannedStartDate = newDate1;
                _jobObservation.PlannedEndDate = newDate2;
                _jobObservation.StartDate = newDate1;
                _jobObservation.EndDate = newDate2;

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
                _jobObservation.PlannedStartDate = newDate1;
                _jobObservation.PlannedEndDate = newDate2;
                _jobObservation.StartDate = newDate1;
                _jobObservation.EndDate = newDate2;

            }

            _jobObservation.Lup = _tempLup;
            foreach (var question in questionAnswers)
            {
                if (question.Value.Answer != "" || question.Value.Edited)
                {
                    _jobObservation.ChecklistAnswers?.Add(question.Value);
                }
            }

            var result = await JobObservationService.CreateJobObservationWithLup(_jobObservation);
            //var result = await JobObservationService.CreateJobObservation(_jobObservation);
            if (result != null)
            {
                Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                Snackbar.Add($"Job Observation Created", Severity.Info);

                _jobObservation = result;
                _ = await GenerateChecklistAnswers();
                NavigationManager.NavigateTo("/jobobservation");
            }
            else
                await JSRuntime.InvokeVoidAsync("alert", "Error en los datos!"); // Alert
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



            _jobObservation.ModelsSpecification = models[0] + "|" + models[1] + "|" + models[2] + "|" + models[3] + "|" + models[4];
            _jobObservation.Cycles = cycles[0] + "|" + cycles[1] + "|" + cycles[2] + "|" + cycles[3] + "|" + cycles[4];
            _jobObservation.HOEStandardTimes = HoeTimes[0] + "|" + HoeTimes[1] + "|" + HoeTimes[2] + "|" + HoeTimes[3] + "|" + HoeTimes[4];
            _jobObservation.Questions = questions[0] + "|" + questions[1] + "|" + questions[2] + "|" + questions[3] + "|" + questions[4];
            _jobObservation.TaktTime = taktTime.ToString();
            _jobObservation.KpiId = kpiID;
            if (_jobObservation.HOEStandardTimes != null)
                _jobObservation.HOEStandardTimes = _jobObservation.HOEStandardTimes.Replace(",", ".");
            if (_jobObservation.Cycles != null)
                _jobObservation.Cycles = _jobObservation.Cycles.Replace(",", ".");
            _jobObservation.SsvSignature = "Signed";

            //Eventual
            _jobObservation.Type = 2;

            _jobObservation.Status = 6;
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


                _jobObservation.StartDate = newDate1;
                _jobObservation.EndDate = newDate2;
                _jobObservation.PlannedStartDate = newDate1;
                _jobObservation.PlannedEndDate = newDate2;
                _jobObservation.FinishedDate = DateTime.Now;

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

                _jobObservation.StartDate = newDate1;
                _jobObservation.EndDate = newDate2;
                _jobObservation.PlannedStartDate = newDate1;
                _jobObservation.PlannedEndDate = newDate2;
                _jobObservation.FinishedDate = DateTime.Now;

            }

            _jobObservation.Lup = _tempLup;
            foreach (var question in questionAnswers)
            {
                if (question.Value.Answer != "" || question.Value.Edited)
                {
                    _jobObservation.ChecklistAnswers?.Add(question.Value);
                }
            }

            var result = await JobObservationService.CreateJobObservationWithLup(_jobObservation);
            //var result = await JobObservationService.CreateJobObservation(_jobObservation);
            if (result != null)
            {
                Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                Snackbar.Add($"Job Observation Created", Severity.Info);
                _jobObservation = result;
                _ = await GenerateChecklistAnswers();
                NavigationManager.NavigateTo("/jobobservation");
            }
            else
                await JSRuntime.InvokeVoidAsync("alert", "Error en los datos!"); // Alert
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

                    nodoEncontrado = TreeServices.FindNodeByPath(rootNodeCCP, CCPrute);

                    if (nodoEncontrado != null)
                    {
                        // El nodo fue encontrado, puedes trabajar con él aquí
                        // Por ejemplo, imprimir su nombre
                        Console.WriteLine("Nombre del nodo encontrado: " + nodoEncontrado.Nombre);
                    }
                    else
                    {
                        // El nodo no fue encontrado
                        Console.WriteLine("La ruta no se encontró en el árbol.");
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
        TreeItemData nodoEncontrado { get; set; }
        CDMS_CCP_Directory CCPFolders { get; set; } = new CDMS_CCP_Directory();

        TreeItemData rootNodeCCP { get; set; } = new TreeItemData();
        TreeItemData SelectedNodeCCP { get; set; }
        private async Task<AsyncVoidMethodBuilder> CCPFolderByDirectory(string CCPrute)
        {

            try
            {
                ShowLoading = true;

                if (CCPrute != "")
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



            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error CCPFolderByDirectory: {ex.Message}");
            }
            finally
            {
                ShowLoading = false;
                StateHasChanged();
            }

            return new AsyncVoidMethodBuilder();

        }

        /////
        ///
             //Guide Modal
        MudTabs guideTabs;

        private bool visibleGuide = false;
        private int selectedPillar = 0;
        private void OpenGuideDialog(int pillarID)
        {
            selectedPillar = pillarID;
            visibleGuide = true;

        }
        void CloseGuideModal() => visibleGuide = false;
        private DialogOptions dialogGuideOptions = new() { CloseOnEscapeKey = true, MaxWidth = MaxWidth.Medium, FullWidth = true, Position = DialogPosition.TopCenter };

        //Questions and answers

        private void RemoveFromList(int pilarId, int indexRemove)
        {
            switch (pilarId)
            {
                case 1:
                    area_ListS?.RemoveAt(indexRemove);
                    Snackbar.Add("LUP remove in Safety & Environment Pillar SECTION 3", Severity.Warning);
                    break;
                case 2:
                    area_ListQ?.RemoveAt(indexRemove);
                    Snackbar.Add("LUP remove in Quality Pillar SECTION 3", Severity.Warning);
                    break;
                case 3:
                    area_ListD?.RemoveAt(indexRemove);
                    Snackbar.Add("LUP remove in Delivery Pillar SECTION 3", Severity.Warning);
                    break;
                case 4:
                    area_ListC?.RemoveAt(indexRemove);
                    Snackbar.Add("LUP remove in Cost Pillar SECTION 3", Severity.Warning);
                    break;
                case 5:
                    area_ListOther?.RemoveAt(indexRemove);
                    Snackbar.Add("LUP remove in Other Pillar SECTION 3", Severity.Warning);
                    break;
            }

            base.StateHasChanged();
        }
        private void AddLupOpportunity(int pillarId, string notGood, ChecklistAnswer item, int secction, int order)
        {

            Snackbar.Configuration.MaxDisplayedSnackbars = 5;
            Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
            switch (pillarId)
            {
                case 1:
                    areaS = notGood;
                    area_ListS?.Add($"{secction}.{order}- " + notGood);
                    Snackbar.Add("LUP added in Safety & Environment Pillar SECTION 3", Severity.Warning);
                    break;
                case 2:
                    areaQ = notGood;
                    area_ListQ?.Add($"{secction}.{order}- "+notGood);
                    Snackbar.Add("LUP added in Quality Pillar SECTION 3", Severity.Warning);
                    break;
                case 3:
                    areaD = notGood;
                    area_ListD?.Add($"{secction}.{order}- "+notGood);
                    Snackbar.Add("LUP added in Delivery Pillar SECTION 3", Severity.Warning);
                    break;
                case 4:
                    areaC = notGood;
                    area_ListC?.Add($"{secction}.{order}- " +notGood);
                    Snackbar.Add("LUP added in Cost Pillar SECTION 3", Severity.Warning);
                    break;
                case 5:
                    areaOther = notGood;
                    area_ListOther?.Add($"{secction}.{order}- "+notGood);
                    Snackbar.Add("LUP added in Other Pillar SECTION 3", Severity.Warning);
                    break;

            }

            item.Show = true;
            item.Edited = true;

            //foreach (var kvp in questionResponses)
            //{
            //    int questionId = kvp.Key;
            //    string answer = kvp.Value;
            //    Console.WriteLine($"QuestionID: {questionId}, Respuesta: {answer}");

            //}

            StateHasChanged();
            base.StateHasChanged();
        }

        private async Task UploadFiles(InputFileChangeEventArgs e, ChecklistAnswer item)
        {
            if (e.File.ContentType.StartsWith("image/"))
            {
                item.Edited = true;
                using (Stream mediaStream = e.File.OpenReadStream(e.File.Size))
                {
                    MemoryStream ms = new();
                    await mediaStream.CopyToAsync(ms);
                    string MediaUri = $"data:{e.File.ContentType};base64,{Convert.ToBase64String(ms.ToArray())}";

                    item.MediaUris.Add(MediaUri);
                    item.capturedImagesFiles?.Add(e.File);
                    item.NewFilesStreams.Add(ms);
                }
            }

        }

        //camara for atachment answer
        bool visibleDialogAnswerCamera = false;
        ChecklistAnswer SelectedAnswer { get; set; }
        private void OpenCameraAnswerDialog(ChecklistAnswer item)
        {
            SelectedAnswer = item;
            visibleDialogAnswerCamera = true;
        }

        //Camera
        private DialogOptions dialogCameraOptions = new() { CloseOnEscapeKey = true, MaxWidth = MaxWidth.Medium, FullWidth = true, CloseButton = true, DisableBackdropClick = true };

        private List<string> capturedImages = new List<string>();

        public int lupPhotosId = 0;
        public string oportunity = "";
        public int photosPillar = 0;

        private bool visibleCamera = false;
        public bool accessCamera = false;
        private void OpenCameraDialog(int lupId, string lupOportunity, int pillar)
        {
            photosPillar = pillar;
            oportunity = lupOportunity;
            lupPhotosId = lupId;

            visibleCamera = true;

        }
        void Close2() => visibleCamera = false;
        private CameraStreamer CameraStreamerReference;

        private string? cameraId = null;

        private int frameCount;

        private string imageData;


        private async void OnRenderedHandler()
        {

            frameCount = 0;
            if (await CameraStreamerReference.GetCameraAccessAsync())
            {
                await CameraStreamerReference.ReloadAsync();

            }
        }

        private async void Start()
        {
            await CameraStreamerReference.StartAsync();
        }

        private async void Stop()
        {
            await CameraStreamerReference.StopAsync();
        }

        private void OnFrameHandler(string _)
        {
            ++frameCount;
        }

        private async void GetCurrentFrame()
        {
            imageData = await CameraStreamerReference.GetCurrentFrameAsync();

            if (!string.IsNullOrEmpty(imageData))
            {
                capturedImages.Add(imageData);
            }
            visibleCamera = false;
            StateHasChanged();
            Stop();
        }

        private async void GetCurrentFrameAnswer()
        {
            imageData = await CameraStreamerReference.GetCurrentFrameAsync();

            if (!string.IsNullOrEmpty(imageData))
            {
                SelectedAnswer.capturedImages.Add(imageData);
            }
            visibleDialogAnswerCamera = false;
            SelectedAnswer.Edited = true;

            StateHasChanged();
            Stop();
        }
        private bool IsValidBase64String(string base64String)
        {
            if (string.IsNullOrEmpty(base64String))
            {
                return false;
            }

            try
            {
                Convert.FromBase64String(base64String);
                return true;
            }
            catch (FormatException)
            {
                return false;
            }
        }

        private void RemoveImageAnswer(ChecklistAnswer item, int index)
        {
            if (index >= 0 && index < item.capturedImages.Count)
            {
                item.capturedImages.RemoveAt(index);
            }
            base.StateHasChanged();
        }

        private void RemoveImageFileAnswer(ChecklistAnswer item, int index)
        {
            if (index >= 0 && index < item.capturedImagesFiles.Count)
            {
                item.capturedImagesFiles.RemoveAt(index);
                item.MediaUris.RemoveAt(index);
            }
            base.StateHasChanged();
        }

        private async Task<AsyncVoidMethodBuilder> GenerateChecklistAnswers()
        {
            if (_jobObservation.ChecklistAnswers.Count > 0)
            {
                foreach ((ChecklistAnswer question, int index) in _jobObservation.ChecklistAnswers.Select((question, index) => (question, index)))
                {
                    ChecklistAnswer answer = questionAnswers[question.QuestionID];

                    if (answer.Edited)
                    {
                        using var content = new MultipartFormDataContent();

                        for (int i = 0; i < answer.capturedImagesFiles.Count; i++)
                        {
                            answer.NewFilesStreams.ElementAt(i).Position = 0;

                            var fileContent = new StreamContent(answer.NewFilesStreams.ElementAt(i));
                            fileContent.Headers.ContentType = new MediaTypeHeaderValue(answer.capturedImagesFiles.ElementAt(i).ContentType);

                            content.Add(
                                content: fileContent,
                                name: "Files",
                                fileName: answer.capturedImagesFiles.ElementAt(i).Name
                            );
                        }

                        if (answer.capturedImages.Count > 0)
                            foreach (var imageData in answer.capturedImages)
                            {
                                if (!string.IsNullOrEmpty(imageData))
                                {
                                    // Elimina la cabecera si está presente
                                    var base64Data = imageData.Replace("data:image/png;base64,", "");

                                    if (IsValidBase64String(base64Data))
                                    {
                                        // Convierte base64Data en bytes
                                        var imageBytes = Convert.FromBase64String(base64Data);

                                        var imageStream = new MemoryStream(imageBytes);
                                        imageStream.Position = 0;
                                        var fileContent = new StreamContent(imageStream);
                                        fileContent.Headers.ContentType = new MediaTypeHeaderValue("image/png");

                                        content.Add(
                                            content: fileContent,
                                            name: "Files",
                                            fileName: "CameraEvidence.png");

                                    }
                                    else
                                    {
                                        Snackbar.Clear();
                                        Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                                        Snackbar.Add("Invalid image data, Update Evidences", Severity.Error);
                                    }
                                }
                                else
                                {
                                    Snackbar.Clear();
                                    Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                                    Snackbar.Add("No image data to upload, Update Evidences", Severity.Warning);
                                }
                            }

                        if (question.JobObservationId != 0)
                        {
                            question.JobObservationId = _jobObservation.JobObservationId;
                        }


                        content.Add(content: new StringContent(question.AnswerId.ToString()), name: "checklistAnswer.AnswerId");
                        content.Add(content: new StringContent(question.JobObservationId.ToString()), name: "checklistAnswer.JobObservationId");
                        content.Add(content: new StringContent(question.QuestionID.ToString()), name: "checklistAnswer.QuestionID");
                        content.Add(content: new StringContent(question.Prompt.ToString()), name: "checklistAnswer.Prompt");
                        content.Add(content: new StringContent(question.Answer.ToString()), name: "checklistAnswer.Answer");
                        if (!question.CommentarySV.IsNullOrEmpty())
                            content.Add(content: new StringContent(question.CommentarySV?.ToString()), name: "checklistAnswer.CommentarySV");
                        if (!question.CommentarySSV.IsNullOrEmpty())
                            content.Add(content: new StringContent(question.CommentarySSV?.ToString()), name: "checklistAnswer.CommentarySSV");



                        var result1 = await ChecklistAnswerServices.CreateEvidencesChecklistAnswer(content);

                        if (result1 != null)
                        {
                            Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                            Snackbar.Add($"Job observation Question item Update Evidences", Severity.Info);
                        }
                        else
                        {
                            Snackbar.Clear();
                            Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                            Snackbar.Add($"Error in Question Update Evidences", Severity.Error);
                        }
                    }
                }
            }





            return new AsyncVoidMethodBuilder();
        }

    }
}
