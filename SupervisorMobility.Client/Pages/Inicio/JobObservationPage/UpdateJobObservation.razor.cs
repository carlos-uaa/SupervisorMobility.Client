using BlazorCameraStreamer;
using Blazorise.Extensions;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using MudBlazor;
using SupervisorMobility.Client.Data.Entities.TreeStruct;
using System.Globalization;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
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
        public List<JobCategoryStructure> _checklistCategoriesAndQuestions { get; set; } = new();
        public List<ChecklistAnswer> _checklistAnswers { get; set; } = new();
        private Dictionary<int, ChecklistAnswer> questionAnswers = new Dictionary<int, ChecklistAnswer>();
        private Dictionary<int, List<int>> questionDelete = new Dictionary<int, List<int>>();

        Dictionary<int, string> imageUrls = new Dictionary<int, string>();


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
                _jobObservation = await JobObservationService.GetJobObservationById(JobObservationId, true, true, true, false, true);


                _checklistCategoriesAndQuestions = await ChecklistService.GetChecklistCategories(true);
                foreach (var category in _checklistCategoriesAndQuestions)
                {
                    foreach (var question in category.ChecklistQuestions)
                    {
                        if (!_jobObservation.ChecklistAnswers.Any(cka => cka.QuestionID == question.QuestionID))
                        {
                            ChecklistAnswer newChAnswer = new();
                            newChAnswer.JobObservationId = _jobObservation.JobObservationId;
                            newChAnswer.QuestionID = question.QuestionID;
                            newChAnswer.Prompt = question.Prompt;
                            questionAnswers.Add(question.QuestionID, newChAnswer);
                        }
                        else
                        {
                            var item = _jobObservation.ChecklistAnswers.ToList().Find(cka => cka.QuestionID == question.QuestionID);
                            if (item.Evidences.Count > 0)
                            {
                                foreach (var evidence in item.Evidences)
                                {
                                    var imageUrl = await FilesServices.ShowImageEvidence(evidence.FileUploadId);
                                    imageUrls[evidence.FileUploadId] = imageUrl;

                                }
                            }
                        }
                    }
                }

                _jobObservation.Supervisor = new();
                //glosary
                glosary = await GlosaryService.GetGlosary();
                _glosaryInfo = glosary.ToDictionary(x => x.Name, x => x);

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
                if (_jobObservation.ModelsSpecification != null)
                {
                    var prod = _jobObservation.ModelsSpecification.Split('|');
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
                                messageErrorFolders = Localizer["theFoldersWithTheInformationWereNotLocated"];
                            }

                        }
                        else
                        {
                            messageErrorFolders = Localizer["jobObservationDoesNotContainAValidDistribution"];
                            Console.WriteLine("missing plant");
                        }
                    }
                    else
                    {
                        messageErrorFolders = Localizer["jobObservationDoesNotContainAValidArea"];

                        Console.WriteLine("missing plant");
                    }
                }
                else
                {
                    messageErrorFolders = Localizer["jobObservationDoesNotContainAValidPlant"];

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
                Snackbar.Add(Localizer["AddComment"], Severity.Error);
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
                    Snackbar.Add(Localizer["DateError"], Severity.Error);
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
                    Snackbar.Add(Localizer["YouNeedChangeDate"], Severity.Error);
                    return;
                }

                if (_jobObservation.Status == 3)
                {
                    _jobObservation.Status = 1;
                }
                _ = await GenerateChecklistAnswers();

                var result = await JobObservationService.UpdateJobObservation(_jobObservation, user.ObjectId);



                if (result)
                {

                    Snackbar.Clear();
                    Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                    Snackbar.Add(Localizer["DateChangeInJob"] + $" {_jobObservation.JobObservationId}", Severity.Info);
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
                    Snackbar.Add(Localizer["DateStartError"], Severity.Error);
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
                    Snackbar.Add(Localizer["DateEndError"], Severity.Error);
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
                    Snackbar.Add(Localizer["YouNeedChangeDate"], Severity.Error);
                    return;
                }

                if (_jobObservation.Status == 3)
                {
                    _jobObservation.Status = 1;
                }
                _ = await GenerateChecklistAnswers();

                var result = await JobObservationService.UpdateJobObservation(_jobObservation, user.ObjectId);


                if (result)
                {

                    Snackbar.Clear();
                    Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                    Snackbar.Add(Localizer["DateChangeInJob"] +  $" {_jobObservation.JobObservationId}", Severity.Info);
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
                Snackbar.Add(Localizer["AnomalyFirst"], Severity.Error);
                return;
            }

            startHour = DateTime.Now.TimeOfDay;
            _jobObservation.KpiId = kpiID;
            _jobObservation.ModelsSpecification = models[0] + "|" + models[1] + "|" + models[2] + "|" + models[3] + "|" + models[4];
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
                    Snackbar.Add(Localizer["DateStartError"], Severity.Error);
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
                    Snackbar.Add(Localizer["DateEndError"], Severity.Error);
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

                _ = await GenerateChecklistAnswers();

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
                    Snackbar.Add(Localizer["DateStartError"], Severity.Error);
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
                    Snackbar.Add(Localizer["DateEndError"], Severity.Error);
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
                _ = await GenerateChecklistAnswers();

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
                Snackbar.Add(Localizer["operatorsignaturemiss"] + $"!", Severity.Error);
                visibleSign = false;
                return;
            }

            if (_jobObservation.OperatorSignature != _jobObservation.Operator.Payroll.ToString())
            {
                Snackbar.Clear();
                Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                Snackbar.Add(Localizer["operatorsignaturenotmarch"], Severity.Error);
                return;
            }

            _jobObservation.KpiId = kpiID;
            _jobObservation.ModelsSpecification = models[0] + "|" + models[1] + "|" + models[2] + "|" + models[3] + "|" + models[4];
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
                    Snackbar.Add(Localizer["DateStartError"], Severity.Error);
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
                    Snackbar.Add(Localizer["DateEndError"], Severity.Error);
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

                _ = await GenerateChecklistAnswers();

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
                    Snackbar.Add(Localizer["DateEndError"], Severity.Error);
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

                _ = await GenerateChecklistAnswers();

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
            _jobObservation.ModelsSpecification = models[0] + "|" + models[1] + "|" + models[2] + "|" + models[3] + "|" + models[4];
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
                    Snackbar.Add(Localizer["DateEndError"], Severity.Error);
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

                _ = await GenerateChecklistAnswers();

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
                    Snackbar.Add(Localizer["DateEndError"], Severity.Error);
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

                _ = await GenerateChecklistAnswers();

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
            _jobObservation.ModelsSpecification = models[0] + "|" + models[1] + "|" + models[2] + "|" + models[3] + "|" + models[4];
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
                    Snackbar.Add(Localizer["DateEndError"], Severity.Error);
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

                _ = await GenerateChecklistAnswers();

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
                    Snackbar.Add(Localizer["DateStartError"], Severity.Error);
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
                    Snackbar.Add(Localizer["DateEndError"], Severity.Error);
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

                _ = await GenerateChecklistAnswers();

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
                Snackbar.Add(Localizer["Feedbackmissing"], Severity.Warning);
                visibleSign = false;
                return;
            }
            _ = await GenerateChecklistAnswers();

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
                Snackbar.Add(Localizer["firstTakeTime"], Severity.Warning);
                return;
            }


            if (option == 1 && HoeTimes[0] == 0.0)
            {
                Snackbar.Clear();
                Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                Snackbar.Add(Localizer["firstTakeTime1"], Severity.Warning);
                return;
            }
            else if (option == 2 && HoeTimes[1] == 0.0)
            {

                Snackbar.Clear();
                Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                Snackbar.Add(Localizer["firstTakeTime2"], Severity.Warning);
                return;
            }
            else if (option == 3 && HoeTimes[2] == 0.0)
            {
                Snackbar.Clear();
                Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                Snackbar.Add(Localizer["firstTakeTime3"], Severity.Warning);
                return;
            }
            else if (option == 4 && HoeTimes[3] == 0.0)
            {
                Snackbar.Clear();
                Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                Snackbar.Add(Localizer["firstTakeTime4"], Severity.Warning);
                return;
            }
            else if (option == 5 && HoeTimes[4] == 0.0)
            {
                Snackbar.Clear();
                Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                Snackbar.Add(Localizer["firstTakeTime5"], Severity.Warning);
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
                            Snackbar.Add(Localizer["timeNG..."], Severity.Warning);
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
                        Snackbar.Add(Localizer["ErrorSArea"], Severity.Error);
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
                        Snackbar.Add(Localizer["ErrorQArea"], Severity.Error);
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
                        Snackbar.Add(Localizer["ErrorDArea"], Severity.Error);
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
                        Snackbar.Add(Localizer["ErrorCArea"], Severity.Error);
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
                        Snackbar.Add(Localizer["ErrorOtherArea"], Severity.Error);
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
            Snackbar.Add(Localizer["btnChangeDate"], Severity.Info);
        }

        public void ShowHourMessage()
        {
            Snackbar.Clear();
            Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
            Snackbar.Add(Localizer["btnChangeHour"], Severity.Info);
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
                                Snackbar.Add(Localizer["imgAddLup"], Severity.Info);
                                StateHasChanged();
                            }
                            else
                            {
                                Snackbar.Clear();
                                Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                                Snackbar.Add(Localizer["imgErrorLup"], Severity.Error);
                            }
                        }
                        else
                        {
                            Snackbar.Clear();
                            Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                            Snackbar.Add(Localizer["InvalidImgData"], Severity.Error);
                        }
                    }
                    else
                    {
                        Snackbar.Clear();
                        Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                        Snackbar.Add(Localizer["NoImgData"], Severity.Warning);
                    }
                }

                // Limpia la lista de imágenes capturadas después de cargarlas
                capturedImages.Clear();
            }
            else
            {
                Snackbar.Clear();
                Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                Snackbar.Add(Localizer["NoImgsUpload"], Severity.Warning);
            }
        }

        private void RemoveImage(int index)
        {
            if (index >= 0 && index < capturedImages.Count)
            {
                capturedImages.RemoveAt(index);
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

        private void RemoveEvidenceAnswer(ChecklistAnswer item, int index)
        {
            if (index >= 0 && index < item.Evidences.ToList().Count)
            {
                var elementRemove = item.Evidences.ToList()[index];

                if (questionDelete.ContainsKey(item.QuestionID))
                {
                    List<int> listaExistente = questionDelete[item.QuestionID];

                    // Agrega el nuevo número a la lista
                    listaExistente.Add(elementRemove.FileUploadId);
                }
                else
                {
                    List<int> newList = new List<int>();
                    newList.Add(elementRemove.FileUploadId);
                    questionDelete.Add(item.QuestionID, newList);
                }

                item.Evidences.Remove(elementRemove);
                item.Edited = true;
            }
            base.StateHasChanged();
        }

        

        //Questions and answers

        private void AddLupOpportunity(int pillarId, string notGood, ChecklistAnswer item)
        {

            Snackbar.Configuration.MaxDisplayedSnackbars = 5;
            Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
            switch (pillarId)
            {
                case 1:
                    areaS = notGood;
                    Snackbar.Add(Localizer["LupAddSE"], Severity.Warning);
                    break;
                case 2:
                    areaQ = notGood;
                    Snackbar.Add(Localizer["LupAddQP"], Severity.Warning);
                    break;
                case 3:
                    areaD = notGood;
                    Snackbar.Add(Localizer["LUPAddDP"], Severity.Warning);
                    break;
                case 4:
                    areaC = notGood;
                    Snackbar.Add(Localizer["LUPAddCP"], Severity.Warning);
                    break;
                case 5:
                    areaOther = notGood;
                    Snackbar.Add(Localizer["LUPAddOP"], Severity.Warning);
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

        private async Task<AsyncVoidMethodBuilder> GenerateChecklistAnswers()
        {

            if (_jobObservation.ChecklistAnswers.Count > 0)
            {
                foreach ((ChecklistAnswer question, int index) in _jobObservation.ChecklistAnswers.Select((question, index) => (question, index)))
                {
                    if (question.Edited)
                    {
                        using var content = new MultipartFormDataContent();

                        for (int i = 0; i < question.capturedImagesFiles.Count; i++)
                        {
                            question.NewFilesStreams.ElementAt(i).Position = 0;

                            var fileContent = new StreamContent(question.NewFilesStreams.ElementAt(i));
                            fileContent.Headers.ContentType = new MediaTypeHeaderValue(question.capturedImagesFiles.ElementAt(i).ContentType);

                            content.Add(
                                content: fileContent,
                                name: "Files",
                                fileName: question.capturedImagesFiles.ElementAt(i).Name
                            );
                        }

                        if (question.capturedImages.Count > 0)
                            foreach (var imageData in question.capturedImages)
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


                        if (questionDelete.ContainsKey(question.QuestionID))
                        {

                            var result0 = await ChecklistAnswerServices.RemoveEvidencesChecklistAnswer(question.AnswerId, questionDelete[question.QuestionID]);

                            if (result0 != null)
                            {
                                Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                                Snackbar.Add($"Answer Update Succesfull", Severity.Info);
                            }
                            else
                            {
                                Snackbar.Clear();
                                Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                                Snackbar.Add($"Error in Answer Update", Severity.Error);
                            }
                        }

                       

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


            if (questionAnswers.Count > 0)
            {
                foreach (var question in questionAnswers)
                {
                    if (question.Value.Answer != "" || question.Value.Edited)
                    {
                        using var content = new MultipartFormDataContent();
                        for (int i = 0; i < question.Value.capturedImagesFiles.Count; i++)
                        {
                            question.Value.NewFilesStreams.ElementAt(i).Position = 0;

                            var fileContent = new StreamContent(question.Value.NewFilesStreams.ElementAt(i));
                            fileContent.Headers.ContentType = new MediaTypeHeaderValue(question.Value.capturedImagesFiles.ElementAt(i).ContentType);

                            content.Add(
                                content: fileContent,
                                name: "Files",
                                fileName: question.Value.capturedImagesFiles.ElementAt(i).Name
                            );
                        }


                        foreach (var imageData in question.Value.capturedImages)
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
                                    Snackbar.Add("Invalid image data, Answer", Severity.Error);
                                }
                            }
                            else
                            {
                                Snackbar.Clear();
                                Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                                Snackbar.Add("No image data to upload, Answer", Severity.Warning);
                            }
                        }

                        question.Value.JobObservationId = _jobObservation.JobObservationId;



                        ChecklistAnswerDto DtoAnswer = _mapper.Map<ChecklistAnswerDto>(question.Value);

                        content.Add(content: new StringContent(DtoAnswer.AnswerId.ToString()), name: "checklistAnswer.AnswerId");
                        content.Add(content: new StringContent(DtoAnswer.JobObservationId.ToString()), name: "checklistAnswer.JobObservationId");
                        content.Add(content: new StringContent(DtoAnswer.QuestionID.ToString()), name: "checklistAnswer.QuestionID");
                        content.Add(content: new StringContent(DtoAnswer.Prompt.ToString()), name: "checklistAnswer.Prompt");
                        content.Add(content: new StringContent(DtoAnswer.Answer.ToString()), name: "checklistAnswer.Answer");
                        if (!DtoAnswer.CommentarySV.IsNullOrEmpty())
                            content.Add(content: new StringContent(DtoAnswer.CommentarySV?.ToString()), name: "checklistAnswer.CommentarySV");
                        if (!DtoAnswer.CommentarySSV.IsNullOrEmpty())
                            content.Add(content: new StringContent(DtoAnswer.CommentarySSV?.ToString()), name: "checklistAnswer.CommentarySSV");


                        var result2 = await ChecklistAnswerServices.CreateChecklistAnswer(content);

                        if (result2 != null)
                        {
                            Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                            Snackbar.Add($"Job observation Question item Created", Severity.Info);
                        }
                        else
                        {
                            Snackbar.Clear();
                            Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                            Snackbar.Add($"Error in Question", Severity.Error);
                        }
                    }
                }
            }



            return new AsyncVoidMethodBuilder();
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

        private async Task DownloadFile(int fileId, string filename)
        {
            await FilesServices.DownloadFileEvidence(fileId, filename);
        }

        //Show Photo
        private DialogOptions dialogPhotoOptions = new() { CloseOnEscapeKey = true, MaxWidth = MaxWidth.Medium, FullWidth = true, CloseButton = true };

        private bool visiblePhoto = false;

        private int photoIndex = 0;

        private void OpenPhotoDialog(int index, ChecklistAnswer item)
        {
            SelectedAnswer = item;
            photoIndex = index;
            visiblePhoto = true;
        }
    }//end class

}//end namespace
