using BlazorCameraStreamer;
using Blazorise.Extensions;
using DocumentFormat.OpenXml.Packaging;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
using Microsoft.JSInterop;
using MudBlazor;
using SupervisorMobility.Client.Data.Entities;
using SupervisorMobility.Client.Data.Entities.TreeStruct;
using System.Diagnostics;
using System.Globalization;
using System.Net;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Security;
using System.Timers;

namespace SupervisorMobility.Client.Pages.Inicio.JobObservationPage
{
    public partial class UpdateJobObservation
    {

        [Parameter]
        public int JobObservationId { get; set; }

        public JobObservation _jobObservation { get; set; } = new();
        public Lup lup { get; set; } = new();
        public JobObservation _lupJobObservations { get; set; } = new();

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
        private AssyChart _assychart { get; set; } = new AssyChart();
        private string messageErrorFolders;
        private bool searchAssychart = false;


        public int plantId;
        public int areaId;
        public int distributionId;
        public int operationId;

        public string areaS;
        public string areaQ;
        public string areaD;
        public string areaC;
        public string areaOther;

        int[] models = new int[5];
        string[] cycles = new string[5];
        double[] HoeTimes = new double[5];
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


        //Lup Modal
        private bool visibleLup = false;
        private bool visiblePast = false;
        private int lupId;

        private DialogOptions dialogLup = new() { CloseOnEscapeKey = true, MaxWidth = MaxWidth.Large, FullWidth = true };
        private DialogOptions dialogPastJobObservations = new() { CloseOnEscapeKey = true, MaxWidth = MaxWidth.Large, FullWidth = true };

        //Past job observation
        public List<JobObservation> pastJobs = new();
        public List<JobObservation> pastjobObservations = new();
        public List<Lup> pastLup = new();
        public JobObservation pastJob = new();

        // Breadcrumb links
        private List<BreadcrumbItem> _links;

        //User
        private string json = string.Empty;
        public User user = new();
        public bool logged = false;

        //Operator user
        public List<User> _operators = new();
        public List<User> operatorUsers = new();

        public string route = string.Empty;
        public bool sign = false;
        void Closed(MudChip chip)
        {
            // react to chip closed
        }

        //Change date
        DateTime? plannedStartDate = new();
        DateTime? plannedEndDate = new();

        private List<SOSCodePath> listFilter = new();
        bool FilterOperation = false;

        [Inject] public AuthenticationStateProvider AuthenticationStateProvider { get; set; }

        public string[] questions = new string[5];

        public double taktTime { get; set; }
        public int kpiID = 0;
        public int auxErgonomicsLevel = 0;

        //Checklist Categories and questions
        public List<ChecklistCategory> _checklistCategoriesAndQuestions { get; set; } = new();
        private Dictionary<int, string> questionResponses = new Dictionary<int, string>();
        public List<ChecklistAnswer> _checklistAnswers { get; set; } = new();
        private Dictionary<int, ChecklistAnswer> questionAnswers = new Dictionary<int, ChecklistAnswer>();

        protected async override Task OnInitializedAsync()
        {
            _links = new List<BreadcrumbItem>
            {
                new BreadcrumbItem(text: Localizer["home"], href: "/"),
                new BreadcrumbItem(text: Localizer["jobObservations"], href: "/jobobservation"),
                new BreadcrumbItem(text: Localizer["update"] + " " + Localizer["jobObservation"], href: "", disabled: true)
            };


            logged = await HasPropertyAsync();
            if (!logged)
            {
                Snackbar.Clear();
                Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                Snackbar.Add($"Error You have to log in", Severity.Error);
                NavigationManager.NavigateTo($"/");
            }
            else
            {
                _checklistCategoriesAndQuestions = await ChecklistService.GetChecklistCategories(true);
                foreach (var category in _checklistCategoriesAndQuestions)
                {
                    foreach (var question in category.ChecklistQuestions)
                    {
                        questionResponses[question.QuestionID] = null;
                    }
                }
                _jobObservation.Supervisor = new();
                //glosary
                glosary = await GlosaryService.GetGlosary();
                _glosaryInfo = glosary.ToDictionary(x => x.Name, x => x);

                _jobObservation = await JobObservationService.GetJobObservationById(JobObservationId, true, true, true,false,true);
                //Por ahora te lo clono
                _lupJobObservations = ObjectCloner.ObjectCloner.DeepClone(_jobObservation);

                //Change date

                startHour = _jobObservation.StartDate?.TimeOfDay;
                endHour = _jobObservation.EndDate?.TimeOfDay;


                if (_jobObservation.PlannedStartDate != null)
                {
                    plannedStartDate = _jobObservation.PlannedStartDate;
                    plannedEndDate = _jobObservation.PlannedEndDate;
                }
                else
                {
                    plannedStartDate = _jobObservation.StartDate;
                    plannedEndDate = _jobObservation.EndDate;
                }

                if (CultureInfo.CurrentCulture.Name == "en-US")
                {
                    if (_jobObservation.HOEStandardTimes != null)
                        _jobObservation.HOEStandardTimes = _jobObservation.HOEStandardTimes.Replace(",", ".");
                    if (_jobObservation.Cycles != null)
                        _jobObservation.Cycles = _jobObservation.Cycles.Replace(",", ".");
                }
                else
                {
                    if (_jobObservation.HOEStandardTimes != null)
                        _jobObservation.HOEStandardTimes = _jobObservation.HOEStandardTimes.Replace(".", ",");
                    if (_jobObservation.Cycles != null)
                        _jobObservation.Cycles = _jobObservation.Cycles.Replace(".", ",");
                }

                _plants = await PlantServices.GetPlants();
                //_products = await ProductService.GetProducts();
                _areas = await AreaServices.GetAreas(_jobObservation.PlantId);
                _distributions = await DistributionService.GetDistributionsWithCollections(_jobObservation.PlantId, _jobObservation.AreaId);

                _products = _distributions[_distributions.FindIndex(d => d.DistributionId == _jobObservation.DistributionId)].Products;
                _operations = _distributions[_distributions.FindIndex(d => d.DistributionId == _jobObservation.DistributionId)].Operations;

                if (_jobObservation.KpiId != null)
                {
                    kpiID = (int)_jobObservation.KpiId;
                }

                if (_jobObservation.TaktTime == null)
                {
                    taktTime = 0.0;
                }
                else
                {
                    taktTime = double.Parse(_jobObservation.TaktTime, CultureInfo.InvariantCulture);
                }

                if (_jobObservation.Questions != null)
                {
                    var quets = _jobObservation.Questions.Split('|');
                    for (int i = 0; i < 5; i++)
                    {
                        questions[i] = quets[i];
                    }
                }
                else
                {
                    for (int i = 0; i < 5; i++)
                    {
                        questions[i] = null;
                    }
                }


                if (_jobObservation.HOEStandardTimes != null && _jobObservation.HOEStandardTimes != "||||")
                {
                    var HOEtime = _jobObservation.HOEStandardTimes.Replace(',', '.').Split('|');
                    for (int i = 0; i < 5; i++)
                    {
                        HoeTimes[i] = double.Parse(HOEtime[i], CultureInfo.InvariantCulture);
                    }
                }

                else
                {
                    for (int i = 0; i < 5; i++)
                    {
                        HoeTimes[i] = 0.0;
                    }
                }
                if (_jobObservation.Models != null)
                {
                    var prod = _jobObservation.Models.Split('|');
                    for (int i = 0; i < 5; i++)
                    {
                        models[i] = Int32.Parse(prod[i]);
                    }

                }
                else
                {
                    for (int i = 0; i < 5; i++)
                    {
                        models[i] = 0;
                    }
                }

                if (_jobObservation.Cycles != null)
                {
                    cycles = _jobObservation.Cycles.Replace(',', '.').Split('|');

                }
                else
                {
                    for (int i = 0; i < 5; i++)
                    {
                        cycles[i] = "";
                    }
                }

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
                        }
                    }
                }
                StateHasChanged();
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
                StateHasChanged();
                await GetUserAsync();

                if (user != null)
                {
                    pastJobs = await JobObservationService.GetAllJobObservations();

                    foreach (var job in pastJobs)
                    {
                        if (job.SupervisorId == _jobObservation.SupervisorId && Convert.ToDateTime(job.StartDate?.ToShortDateString()).Date < Convert.ToDateTime(_jobObservation.StartDate?.ToShortDateString()).Date
                            && job.DistributionId == _jobObservation.DistributionId && job.OperationId == _jobObservation.OperationId)
                        {

                            pastjobObservations.Add(job);

                            pastJob = await JobObservationService.GetJobObservationById(JobObservationId, true, true, true, false, false);
                            foreach (var lups in pastJob.Lup)
                            {
                                pastLup.Add(lups);
                            }
                        }

                    }

                }
                pastjobObservations = pastjobObservations.OrderBy(x => x.StartDate).ToList();


                if (_jobObservation.PlantId != 0)
                {
                    if (_jobObservation.AreaId != 0)
                    {
                        if (_jobObservation.DistributionId != 0)
                        {

                            try
                            {
                                _assychart = await AssychartServices.GetAssyChartJobObservation(_jobObservation.PlantId, _jobObservation.AreaId, _jobObservation.DistributionId);

                                if (_assychart == null)
                                {
                                    messageErrorFolders = Localizer["theFoldersWithTheInformationWereNotLocated"];
                                }
                                else
                                {
                                    if (_assychart.ErgonomicsLevel != null)
                                    {
                                        auxErgonomicsLevel = (int)_assychart.ErgonomicsLevel;
                                    }

                                    searchAssychart = true;
                                }
                            }
                            catch (Exception ex)
                            {
                                messageErrorFolders = "The folders with the information provided were not located.";
                            }

                        }
                        else
                        {
                            messageErrorFolders = "Job Observation does not contain a valid distribution";
                            Console.WriteLine("missing plant");
                        }
                    }
                    else
                    {
                        messageErrorFolders = "Job Observation does not contain a valid area";
                        Console.WriteLine("missing plant");
                    }
                }
                else
                {
                    messageErrorFolders = "Job Observation does not contain a valid plant";
                    Console.WriteLine("missing plant");
                }
            }

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

            if (searchAssychart)
            {
                listFilter = _assychart.RoutesProductsAssyChart.Where(r => r.Code.ToLower().Contains(_jobObservation.Operation.Code.ToLower(), StringComparison.OrdinalIgnoreCase)).ToList();
                FilterOperation = true;
            }
            StateHasChanged();
        }

        private async Task GetUserAsync()
        {
            if (!await TryGetAsync())
            {
                user = new();
            }
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

        void history()
        {
            NavigationManager.NavigateTo($"jobobservation/history/{JobObservationId}");
        }



        //Change date modal and function
        private bool visible = false;
        private void OpenCommentDialog()
        {
            visible = true;
        }
        void Close() => visible = false;
        private DialogOptions dialogCommentOptions = new() { CloseOnEscapeKey = true, MaxWidth = MaxWidth.Small, FullWidth = true };

        public async Task ChangeDate()
        {

            if (_jobObservation.Justification == null || _jobObservation.Justification == "")
            {
                Snackbar.Clear();
                Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                Snackbar.Add($"You need to add a comment", Severity.Error);
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

                _jobObservation.StartDate = newDate1;
                _jobObservation.EndDate = newDate2;

                _jobObservation.PlannedStartDate = newDate1;
                _jobObservation.PlannedEndDate = newDate2;

                Console.WriteLine(_jobObservation.StartDate);
                Console.WriteLine(_jobObservation.EndDate);


                if (plannedStartDate == _jobObservation.StartDate && plannedEndDate == _jobObservation.EndDate)
                {
                    Snackbar.Clear();
                    Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                    Snackbar.Add($"You need to change the date first", Severity.Error);
                    return;
                }

                if (_jobObservation.Status == 3)
                {
                    _jobObservation.Status = 1;
                }

                var result = await JobObservationService.UpdateJobObservation(_jobObservation, user.ObjectId);

                if (result)
                {

                    Snackbar.Clear();
                    Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                    Snackbar.Add($"Date Changed in Job Observation {_jobObservation.JobObservationId}", Severity.Info);
                    NavigationManager.NavigateTo("/jobobservation");
                }
                else
                    await JSRuntime.InvokeVoidAsync("alert", "Update failed!"); // Alert
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

                _jobObservation.StartDate = newDate1;
                _jobObservation.EndDate = newDate2;

                _jobObservation.PlannedStartDate = newDate1;
                _jobObservation.PlannedEndDate = newDate2;

                Console.WriteLine(_jobObservation.StartDate);
                Console.WriteLine(_jobObservation.EndDate);


                if (plannedStartDate == _jobObservation.StartDate && plannedEndDate == _jobObservation.EndDate)
                {
                    Snackbar.Clear();
                    Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                    Snackbar.Add($"You need to change the date first", Severity.Error);
                    return;
                }

                if (_jobObservation.Status == 3)
                {
                    _jobObservation.Status = 1;
                }

                var result = await JobObservationService.UpdateJobObservation(_jobObservation, user.ObjectId);

                if (result)
                {

                    Snackbar.Clear();
                    Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                    Snackbar.Add($"Date Changed in Job Observation {_jobObservation.JobObservationId}", Severity.Info);
                    NavigationManager.NavigateTo("/jobobservation");
                }
                else
                    await JSRuntime.InvokeVoidAsync("alert", "Update failed!"); // Alert
            }
        }


        //In progress
        private async Task SaveProgressJobObservation()
        {
            if (_jobObservation.Option == 3 && _jobObservation.Anomaly.IsNullOrEmpty())
            {
                Snackbar.Clear();
                Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                Snackbar.Add($"Write down the anomaly first", Severity.Error);
                return;
            }

            startHour = DateTime.Now.TimeOfDay;
            _jobObservation.KpiId = kpiID;
            _jobObservation.Models = models[0] + "|" + models[1] + "|" + models[2] + "|" + models[3] + "|" + models[4];
            _jobObservation.Cycles = cycles[0] + "|" + cycles[1] + "|" + cycles[2] + "|" + cycles[3] + "|" + cycles[4];
            _jobObservation.HOEStandardTimes = HoeTimes[0] + "|" + HoeTimes[1] + "|" + HoeTimes[2] + "|" + HoeTimes[3] + "|" + HoeTimes[4];
            _jobObservation.Questions = questions[0] + "|" + questions[1] + "|" + questions[2] + "|" + questions[3] + "|" + questions[4];
            _jobObservation.TaktTime = taktTime.ToString();

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

                if (_jobObservation.HOEStandardTimes != null)
                    _jobObservation.HOEStandardTimes = _jobObservation.HOEStandardTimes.Replace(",", ".");
                if (_jobObservation.Cycles != null)
                    _jobObservation.Cycles = _jobObservation.Cycles.Replace(",", ".");

                _jobObservation.StartDate = newDate1;
                _jobObservation.EndDate = newDate2;
                _jobObservation.Status = 2;

                if (_jobObservation.Justification == "")
                {
                    _jobObservation.Justification = null;
                }

                var result = await JobObservationService.UpdateJobObservation(_jobObservation, user.ObjectId);

                if (result)
                {

                    Snackbar.Clear();
                    Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                    Snackbar.Add($"Job Observation {_jobObservation.JobObservationId} Updated", Severity.Info);
                    NavigationManager.NavigateTo("/jobobservation");
                }
                else
                    await JSRuntime.InvokeVoidAsync("alert", "Update failed!"); // Alert
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

                if (_jobObservation.HOEStandardTimes != null)
                    _jobObservation.HOEStandardTimes = _jobObservation.HOEStandardTimes.Replace(",", ".");
                if (_jobObservation.Cycles != null)
                    _jobObservation.Cycles = _jobObservation.Cycles.Replace(",", ".");
                _jobObservation.StartDate = newDate1;
                _jobObservation.EndDate = newDate2;
                _jobObservation.Status = 2;

                if (_jobObservation.Justification == "")
                {
                    _jobObservation.Justification = null;
                }

                var result = await JobObservationService.UpdateJobObservation(_jobObservation, user.ObjectId);

                if (result)
                {

                    Snackbar.Clear();
                    Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                    Snackbar.Add($"Job Observation {_jobObservation.JobObservationId} Updated", Severity.Info);
                    NavigationManager.NavigateTo("/jobobservation");
                }
                else
                    await JSRuntime.InvokeVoidAsync("alert", "Update failed!"); // Alert
            }
        }


        //Cancel
        void CancelUpdateJobObservation()
        {
            NavigationManager.NavigateTo("/jobobservation");
        }


        //Under Review Job observation
        public async void UnderReviewJobObservation()
        {
            if (_jobObservation.OperatorSignature == null || _jobObservation.OperatorSignature == "")
            {
                Snackbar.Clear();
                Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                Snackbar.Add($"Operator's Signature is missing!", Severity.Error);
                visibleSign = false;
                return;
            }

            if (_jobObservation.OperatorSignature != _jobObservation.Operator.Payroll.ToString())
            {
                Snackbar.Clear();
                Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                Snackbar.Add($"Operator Signature doesn't match", Severity.Error);
                return;
            }

            _jobObservation.KpiId = kpiID;
            _jobObservation.Models = models[0] + "|" + models[1] + "|" + models[2] + "|" + models[3] + "|" + models[4];
            _jobObservation.Cycles = cycles[0] + "|" + cycles[1] + "|" + cycles[2] + "|" + cycles[3] + "|" + cycles[4];
            _jobObservation.HOEStandardTimes = HoeTimes[0] + "|" + HoeTimes[1] + "|" + HoeTimes[2] + "|" + HoeTimes[3] + "|" + HoeTimes[4];
            _jobObservation.Questions = questions[0] + "|" + questions[1] + "|" + questions[2] + "|" + questions[3] + "|" + questions[4];
            _jobObservation.TaktTime = taktTime.ToString();

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
                if (_jobObservation.OperatorSignature != _jobObservation.Operator.Payroll.ToString())
                {
                    Snackbar.Clear();
                    Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                    Snackbar.Add($"Error in Operator Signature", Severity.Error);
                    return;
                }

                if (_jobObservation.HOEStandardTimes != null)
                    _jobObservation.HOEStandardTimes = _jobObservation.HOEStandardTimes.Replace(",", ".");
                if (_jobObservation.Cycles != null)
                    _jobObservation.Cycles = _jobObservation.Cycles.Replace(",", ".");
                _jobObservation.StartDate = newDate1;
                _jobObservation.EndDate = newDate2;

                _jobObservation.Status = 4;


                var result = await JobObservationService.UpdateJobObservation(_jobObservation, user.ObjectId);


                if (result)
                {

                    Snackbar.Clear();
                    Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                    Snackbar.Add($"Job Observation {_jobObservation.JobObservationId} Under Review", Severity.Info);
                    NavigationManager.NavigateTo("/jobobservation");
                }
                else
                    await JSRuntime.InvokeVoidAsync("alert", "Update failed!"); // Alert
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
                if (_jobObservation.OperatorSignature != _jobObservation.Operator.Payroll.ToString())
                {
                    Snackbar.Clear();
                    Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                    Snackbar.Add($"Error in Operator Signature", Severity.Error);
                    return;
                }


                if (_jobObservation.HOEStandardTimes != null)
                    _jobObservation.HOEStandardTimes = _jobObservation.HOEStandardTimes.Replace(",", ".");
                if (_jobObservation.Cycles != null)
                    _jobObservation.Cycles = _jobObservation.Cycles.Replace(",", ".");
                _jobObservation.StartDate = newDate1;
                _jobObservation.EndDate = newDate2;

                _jobObservation.Status = 4;


                var result = await JobObservationService.UpdateJobObservation(_jobObservation, user.ObjectId);


                if (result)
                {

                    Snackbar.Clear();
                    Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                    Snackbar.Add($"Job Observation {_jobObservation.JobObservationId} Under Review", Severity.Info);
                    NavigationManager.NavigateTo("/jobobservation");
                }
                else
                    await JSRuntime.InvokeVoidAsync("alert", "Update failed!"); // Alert
            }

        }


        //Reject Job observation
        async void Reject()
        {

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

            if (_jobObservation.OperatorSignature != _jobObservation.Operator.Payroll.ToString())
            {
                Snackbar.Clear();
                Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                Snackbar.Add($"Operator Signature doesn't match", Severity.Error);
                return;
            }

            _jobObservation.KpiId = kpiID;
            _jobObservation.Models = models[0] + "|" + models[1] + "|" + models[2] + "|" + models[3] + "|" + models[4];
            _jobObservation.Cycles = cycles[0] + "|" + cycles[1] + "|" + cycles[2] + "|" + cycles[3] + "|" + cycles[4];
            _jobObservation.HOEStandardTimes = HoeTimes[0] + "|" + HoeTimes[1] + "|" + HoeTimes[2] + "|" + HoeTimes[3] + "|" + HoeTimes[4];
            _jobObservation.Questions = questions[0] + "|" + questions[1] + "|" + questions[2] + "|" + questions[3] + "|" + questions[4];
            _jobObservation.TaktTime = taktTime.ToString();

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

                if (_jobObservation.HOEStandardTimes != null)
                    _jobObservation.HOEStandardTimes = _jobObservation.HOEStandardTimes.Replace(",", ".");
                if (_jobObservation.Cycles != null)
                    _jobObservation.Cycles = _jobObservation.Cycles.Replace(",", ".");
                _jobObservation.StartDate = newDate1;
                _jobObservation.EndDate = newDate2;

                _jobObservation.Status = 5;

                _jobObservation.FinishedDate = DateTime.Now;

                var result = await JobObservationService.UpdateJobObservation(_jobObservation, user.ObjectId);

                if (result)
                {

                    Snackbar.Clear();
                    Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                    Snackbar.Add($"Job Observation {_jobObservation.JobObservationId} Rejected", Severity.Info);
                    NavigationManager.NavigateTo("/jobobservation");
                }
                else
                    await JSRuntime.InvokeVoidAsync("alert", "Update failed!"); // Alert
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

                if (_jobObservation.HOEStandardTimes != null)
                    _jobObservation.HOEStandardTimes = _jobObservation.HOEStandardTimes.Replace(",", ".");
                if (_jobObservation.Cycles != null)
                    _jobObservation.Cycles = _jobObservation.Cycles.Replace(",", ".");
                _jobObservation.StartDate = newDate1;
                _jobObservation.EndDate = newDate2;

                _jobObservation.Status = 5;

                _jobObservation.FinishedDate = DateTime.Now;

                var result = await JobObservationService.UpdateJobObservation(_jobObservation, user.ObjectId);

                if (result)
                {

                    Snackbar.Clear();
                    Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                    Snackbar.Add($"Job Observation {_jobObservation.JobObservationId} Rejected", Severity.Info);
                    NavigationManager.NavigateTo("/jobobservation");
                }
                else
                    await JSRuntime.InvokeVoidAsync("alert", "Update failed!"); // Alert
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

            if (_jobObservation.OperatorSignature == null || _jobObservation.OperatorSignature == "")
            {
                Snackbar.Clear();
                Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                Snackbar.Add($"Operator's Signature is missing!", Severity.Error);
                visibleSign = false;
                return;
            }

            if (_jobObservation.OperatorSignature != _jobObservation.Operator.Payroll.ToString())
            {
                Snackbar.Clear();
                Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                Snackbar.Add($"Operator's Signature doesn't match", Severity.Error);
                return;
            }

            _jobObservation.KpiId = kpiID;
            endHour = DateTime.Now.TimeOfDay;
            _jobObservation.Models = models[0] + "|" + models[1] + "|" + models[2] + "|" + models[3] + "|" + models[4];
            _jobObservation.Cycles = cycles[0] + "|" + cycles[1] + "|" + cycles[2] + "|" + cycles[3] + "|" + cycles[4];
            _jobObservation.HOEStandardTimes = HoeTimes[0] + "|" + HoeTimes[1] + "|" + HoeTimes[2] + "|" + HoeTimes[3] + "|" + HoeTimes[4];
            _jobObservation.Questions = questions[0] + "|" + questions[1] + "|" + questions[2] + "|" + questions[3] + "|" + questions[4];
            _jobObservation.TaktTime = taktTime.ToString();

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


                if (_jobObservation.HOEStandardTimes != null)
                    _jobObservation.HOEStandardTimes = _jobObservation.HOEStandardTimes.Replace(",", ".");
                if (_jobObservation.Cycles != null)
                    _jobObservation.Cycles = _jobObservation.Cycles.Replace(",", ".");
                _jobObservation.StartDate = newDate1;
                _jobObservation.EndDate = newDate2;
                _jobObservation.SsvSignature = "Signed";
                _jobObservation.Status = 6;
                _jobObservation.FinishedDate = DateTime.Now;

                var result = await JobObservationService.UpdateJobObservation(_jobObservation, user.ObjectId);

                if (result)
                {

                    Snackbar.Clear();
                    Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                    Snackbar.Add($"Job Observation {_jobObservation.JobObservationId} Finished", Severity.Info);
                    NavigationManager.NavigateTo("/jobobservation");
                }
                else
                    await JSRuntime.InvokeVoidAsync("alert", "Update failed!"); // Alert
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

                if (_jobObservation.HOEStandardTimes != null)
                    _jobObservation.HOEStandardTimes = _jobObservation.HOEStandardTimes.Replace(",", ".");
                if (_jobObservation.Cycles != null)
                    _jobObservation.Cycles = _jobObservation.Cycles.Replace(",", ".");
                _jobObservation.StartDate = newDate1;
                _jobObservation.EndDate = newDate2;
                _jobObservation.SsvSignature = "Signed";
                _jobObservation.Status = 6;
                var result = await JobObservationService.UpdateJobObservation(_jobObservation, user.ObjectId);

                if (result)
                {

                    Snackbar.Clear();
                    Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                    Snackbar.Add($"Job Observation {_jobObservation.JobObservationId} Finished", Severity.Info);
                    NavigationManager.NavigateTo("/jobobservation");
                }
                else
                    await JSRuntime.InvokeVoidAsync("alert", "Update failed!"); // Alert
            }
        }

        public async Task AddReleasedFeedback()
        {


            if (_jobObservation.ReleasedFeedback == null || _jobObservation.ReleasedFeedback == "")
            {
                Snackbar.Clear();
                Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                Snackbar.Add($"Feedback is missing!", Severity.Warning);
                visibleSign = false;
                return;
            }

            var result = await JobObservationService.UpdateJobObservation(_jobObservation, user.ObjectId);

            if (result)
            {

                Snackbar.Clear();
                Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                Snackbar.Add($"Job Observation {_jobObservation.JobObservationId} With Released Feedback", Severity.Info);
                NavigationManager.NavigateTo("/jobobservation");
            }
            else
                await JSRuntime.InvokeVoidAsync("alert", "Update failed!"); // Alert
        }

        //Job observations
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


        //Lup
        public async void AddLup(int pillar)
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

            lup.Observer = _jobObservation.Supervisor.Name;
            lup.JobObservationId = _jobObservation.JobObservationId;
            lup.Pillar = pillar;
            lup.Status = 1;
            lup.CreatedDate = DateTime.Now;
            lup.IsActive = true;


            var result = await LupService.CreateLup(lup);
            if (result != null)
            {
                _lupJobObservations = await JobObservationService.GetJobObservationById(JobObservationId, true, true, true, false, false);

                await GetUserAsync();

                pastjobObservations = new();
                pastLup = new();
                if (user != null)
                {
                    pastJobs = await JobObservationService.GetAllJobObservations();

                    foreach (var job in pastJobs)
                    {
                        if (job.SupervisorId == _jobObservation.SupervisorId && Convert.ToDateTime(job.StartDate?.ToShortDateString()).Date < Convert.ToDateTime(_jobObservation.StartDate?.ToShortDateString()).Date
                            && job.DistributionId == _jobObservation.DistributionId && job.OperationId == _jobObservation.OperationId)
                        {

                            pastjobObservations.Add(job);

                            pastJob = await JobObservationService.GetJobObservationById(JobObservationId, true, true, true, false, false);
                            foreach (var lups in pastJob.Lup)
                            {
                                pastLup.Add(lups);
                            }
                        }

                    }

                }
                pastjobObservations = pastjobObservations.OrderBy(x => x.StartDate).ToList();
                StateHasChanged();

                Snackbar.Clear();
                Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                Snackbar.Add($"Lup Created", Severity.Info);
            }
            else
            {
                Snackbar.Clear();
                Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                Snackbar.Add($"Error in Lup", Severity.Error);
            }
        }


        //Past Job Observations Modal
        private void OpenDialogPastJobObservations()
        {
            visiblePast = true;
        }
        void CloseOverdue() => visiblePast = false;


        //Lup Modal
        private void OpenDialogLup(int id)
        {
            lupId = id;
            visibleLup = true;
        }

        void CloseLup() => visibleLup = false;
        void EditLup(int lupId)
        {
            NavigationManager.NavigateTo($"lup/updatelup/{lupId}");
        }

        async Task DeleteLup(int lupId)
        {
            await LupService.DeleteLup(lupId);

            _lupJobObservations = await JobObservationService.GetJobObservationById(JobObservationId, true, true, true, false, false);

            await GetUserAsync();

            pastjobObservations = new();
            pastLup = new();
            if (user != null)
            {
                pastJobs = await JobObservationService.GetAllJobObservations();

                foreach (var job in pastJobs)
                {
                    if (job.SupervisorId == _jobObservation.SupervisorId && Convert.ToDateTime(job.StartDate?.ToShortDateString()).Date < Convert.ToDateTime(_jobObservation.StartDate?.ToShortDateString()).Date
                        && job.DistributionId == _jobObservation.DistributionId && job.OperationId == _jobObservation.OperationId)
                    {

                        pastjobObservations.Add(job);

                        pastJob = await JobObservationService.GetJobObservationById(JobObservationId, true, true, true, false, false);
                        foreach (var lups in pastJob.Lup)
                        {
                            pastLup.Add(lups);
                        }
                    }

                }

            }
            pastjobObservations = pastjobObservations.OrderBy(x => x.StartDate).ToList();


            Snackbar.Clear();
            Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
            Snackbar.Add($"Lup Deleted", Severity.Info);
            visibleDelete = false;

            StateHasChanged();
        }

        void GoToJobObservation(int jobObservationId)
        {
            NavigationManager.NavigateTo($"/");
            NavigationManager.NavigateTo($"jobobservation/updatejobobservation/{jobObservationId}");
        }

        //HOE, CCP, GOS
        private bool CcpDialog = false;
        private bool HoeDialog = false;
        private bool GosDialog = false;
        private bool folderError = false;


        private CDMS_CCP_Archives CcpFilesInFolder = new CDMS_CCP_Archives();
        private CDMS_HOE_Archives HoeFilesInFolder = new CDMS_HOE_Archives();
        private CDMS_GOS_Archives GosFilesInFolder = new CDMS_GOS_Archives();

        private string HOErute = "";
        private string CCPrute = "";
        private string GOSrute = "";


        private DialogOptions dialogOptions = new() { CloseOnEscapeKey = true, MaxWidth = MaxWidth.Large, FullWidth = true };

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

        public void ShowDateMessage()
        {
            Snackbar.Clear();
            Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
            Snackbar.Add($"To change the date, press the button Change Date", Severity.Info);
        }

        public void ShowHourMessage()
        {
            Snackbar.Clear();
            Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
            Snackbar.Add($"To change the hour, press the button Change Date", Severity.Info);
        }


        //Delete lup
        private bool visibleDelete = false;
        public int deleteLupId = 0;
        private void OpenDeleteDialog(int deleteId)
        {
            deleteLupId = deleteId;
            visibleDelete = true;
        }
        void CloseDeleteModal() => visibleDelete = false;
        private DialogOptions dialogDeleteOptions = new() { CloseOnEscapeKey = true, MaxWidth = MaxWidth.ExtraSmall, FullWidth = true, Position = DialogPosition.TopCenter };


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



        private bool if_pick_Plant = false;
        private bool if_pick_Area = false;
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

        private async Task UploadEvidence()
        {
            if (capturedImages.Count > 0)
            {
                foreach (var imageData in capturedImages)
                {
                    if (!string.IsNullOrEmpty(imageData))
                    {
                        // Elimina la cabecera si está presente
                        var base64Data = imageData.Replace("data:image/png;base64,", "");

                        if (IsValidBase64String(base64Data))
                        {
                            // Convierte base64Data en bytes
                            var imageBytes = Convert.FromBase64String(base64Data);

                            using var content = new MultipartFormDataContent();
                            var imageStream = new MemoryStream(imageBytes);
                            var fileContent = new StreamContent(imageStream);
                            fileContent.Headers.ContentType = new MediaTypeHeaderValue("image/png");

                            content.Add(
                                content: fileContent,
                                name: "\"file\"",
                                fileName: "evidence.png");

                            // Llama a tu servicio de carga de archivos aquí
                            var result = await FilesServices.UploadEvidences(content, lupPhotosId);

                            if (result is not null)
                            {
                                Snackbar.Configuration.MaxDisplayedSnackbars = 10;
                                Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                                Snackbar.Add("Image Added to Lup", Severity.Info);
                                StateHasChanged();
                            }
                            else
                            {
                                Snackbar.Clear();
                                Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                                Snackbar.Add("Failed to upload Image to Lup", Severity.Error);
                            }
                        }
                        else
                        {
                            Snackbar.Clear();
                            Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                            Snackbar.Add("Invalid image data", Severity.Error);
                        }
                    }
                    else
                    {
                        Snackbar.Clear();
                        Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                        Snackbar.Add("No image data to upload", Severity.Warning);
                    }
                }

                // Limpia la lista de imágenes capturadas después de cargarlas
                capturedImages.Clear();
            }
            else
            {
                Snackbar.Clear();
                Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                Snackbar.Add("No images to upload", Severity.Warning);
            }
        }

        private void RemoveImage(int index)
        {
            if (index >= 0 && index < capturedImages.Count)
            {
                capturedImages.RemoveAt(index);
            }
        }

        //Questions and answers
        private void AddLupOpportunity(int question)
        {
            Snackbar.Configuration.MaxDisplayedSnackbars = 4;
            switch (question)
            {
                case 1:
                    areaQ = "No respeta pasos principales y puntos críticos";
                    Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                    Snackbar.Add("LUP added in Quality Pillar SECTION 3", Severity.Warning);
                    break;
                case 2:
                    areaQ = "El empaque, herramientas, manipuladores no están en buenas condiciones y hay riesgos de calidad";
                    Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                    Snackbar.Add("LUP added in Quality Pillar SECTION 3", Severity.Warning);
                    break;
                case 3:
                    areaS = "No respeta el cumplimiento a los estados de referencia, identificación de sustancias ni disposición de residuos";
                    Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                    Snackbar.Add("LUP added in Safety Pillar SECTION 3", Severity.Warning);
                    break;
                case 4:
                    areaQ = "El operador no es capaz de nombrar paso principales, puntos críticos ni razón";
                    Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                    Snackbar.Add("LUP added in Quality Pillar SECTION 3", Severity.Warning);
                    break;



            }
            StateHasChanged();
        }
     

        private void AddLupOpportunity(int pillarId, string notGood)
        {

            Snackbar.Configuration.MaxDisplayedSnackbars = 5;
            Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
            switch (pillarId)
            {
                case 1:
                    areaS = notGood;
                    Snackbar.Add("LUP added in Safety & Environment Pillar SECTION 3", Severity.Warning);
                    break;
                case 2:
                    areaQ = notGood;
                    Snackbar.Add("LUP added in Quality Pillar SECTION 3", Severity.Warning);
                    break;
                case 3:
                    areaD = notGood;
                    Snackbar.Add("LUP added in Delivery Pillar SECTION 3", Severity.Warning);
                    break;
                case 4:
                    areaC = notGood;
                    Snackbar.Add("LUP added in Cost Pillar SECTION 3", Severity.Warning);
                    break;
                case 5:
                    areaOther = notGood;
                    Snackbar.Add("LUP added in Other Pillar SECTION 3", Severity.Warning);
                    break;

            }


            foreach (var kvp in questionResponses)
            {
                int questionId = kvp.Key;
                string answer = kvp.Value;
                Console.WriteLine($"QuestionID: {questionId}, Respuesta: {answer}");

            }

            StateHasChanged();
        }

        private void PruebaChecklist()
        {

            foreach (var kvp in questionResponses)
            {
                int questionId = kvp.Key;
                string answer = kvp.Value;
                var notGood = "";
                Console.WriteLine($"QuestionID: {questionId}, Respuesta: {answer}");

                foreach (var category in _checklistCategoriesAndQuestions)
                {
                    foreach (var question in category.ChecklistQuestions)
                    {
                        if (question.QuestionID == questionId)
                        {
                            notGood = question.Prompt;
                        }
                    }
                }

                ChecklistAnswer Answer = new ChecklistAnswer
                {
                    QuestionID = questionId,
                    Answer = answer,
                    Prompt = notGood,

                };

                questionAnswers[questionId] = Answer;
            }

            foreach (var kvp in questionAnswers)
            {
                int questionId = kvp.Key;
                string answer = kvp.Value.Answer;
                string prompt = kvp.Value.Prompt;

                Console.WriteLine($"QuestionID: {questionId}, Respuesta: {answer}, Prompt: {prompt}");
            }
        }

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




    }
}
