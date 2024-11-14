using BlazorCameraStreamer;
using Blazorise.Extensions;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using MudBlazor;
using SupervisorMobility.Client.Data.Entities.TreeStruct;
using SupervisorMobility.Client.Pages.Inicio.JobObservationPage.Modals;
using System.Globalization;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Timers;
using static SupervisorMobility.Client.Pages.Inicio.JobObservationPage.CreateJobObservationNew;


namespace SupervisorMobility.Client.Pages.Inicio.JobObservationPage
{
    public partial class UpdateJobObservation
    {

        [Parameter]
        public int JobObservationId { get; set; }
        [Inject]
        private IBreadcrumbService BreadcrumbService { get; set; }
        public JobObservation _jobObservation { get; set; } = new();
        public Lup lup { get; set; } = new();
        public JobObservation _lupJobObservations { get; set; } = new();

        public string hour1 { get; set; }
        public string hour2 { get; set; }

        DateTime newDate1;
        DateTime newDate2;

        TimeSpan? endHour { get; set; }
        TimeSpan? startHour { get; set; }

        List<Plant> _plants { get; set; } = new();
        List<Area> _areas = new();
        List<Distribution> _distributions = new();
        List<Operation> _operations = new();
        List<Product> _products { get; set; } = new();
        List<User> _supervisors { get; set; } = new();

        Dictionary<Lup, List<string>> _tempLup = new();

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
        private int lupId;

        private DialogOptions dialogPastJobObservations = new() { CloseOnEscapeKey = true, MaxWidth = MaxWidth.Large, FullWidth = true };

        //Past job observation
        public List<JobObservation> pastJobs = new();
        public List<JobObservation> pastjobObservations = new();
        public List<Lup> pastLup = new();
        public JobObservation pastJob = new();
        public Operation operation = new();
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
        public double hoeStandardTime { get; set; }
        public int kpiID = 0;
        public int auxErgonomicsLevel = 0;

        //Checklist Categories and questions
        public List<JobCategoryStructure> _checklistCategoriesAndQuestions { get; set; } = new();
        public List<ChecklistAnswer> _checklistAnswers { get; set; } = new();
        private Dictionary<int, ChecklistAnswer> questionAnswers = new Dictionary<int, ChecklistAnswer>();
        private Dictionary<int, List<int>> questionDelete = new Dictionary<int, List<int>>();

        Dictionary<int, string> imageUrls = new Dictionary<int, string>();
        public int jobProductId = 0;
        bool showLoading = true;
        List<Operation> _filteredOperations = new();
        string[] ids { get; set; }
        string currentLanguage = "es-ES";
        private string currentImage = "";
        bool operatorSignatureSigned = false;

        bool NoData { get; set; } = false;
        //Lup list
        public class LupOpportunity
        {
            public int QuestionID { get; set; }
            public string Opportunity { get; set; }
        }

        //Lup list
        public List<LupOpportunity> area_ListS = new List<LupOpportunity>();
        public List<LupOpportunity> area_ListQ = new List<LupOpportunity>();
        public List<LupOpportunity> area_ListD = new List<LupOpportunity>();
        public List<LupOpportunity> area_ListC = new List<LupOpportunity>();
        public List<LupOpportunity> area_ListOther = new List<LupOpportunity>();

        List<Lup> lupInsidences = new();

        public bool flag = false;
        public Lup? selectedLup = null;


        public Dictionary<string, double[]> OperationTimes = new Dictionary<string, double[]>
        {
            { "CycleTime", new double[5] },
            { "WaitingTime", new double[5] }
        };
        private int?[] StepsNumber = new int?[5];
        private int?[] DoubleManagment = new int?[5];
        private int?[] Waiting = new int?[5];
        private string?[] CycleTimes = new string?[5] { "", "", "", "", "" };
        private string?[] WaitingTimes = new string?[5] { "", "", "", "", "" };
        public string[] productSpecification = new string[5];


        List<List<string>> _specifications { get; set; } = new List<List<string>>
        {
            new List<string>(), // Product 1
            new List<string>(), // Product 2
            new List<string>(), // Product 3
            new List<string>(), // Product 4
            new List<string>()  // Product 5
        };

        public int[] jobProductIds = new int[5];

        ProductAndStandardTime[] _productAndSpecification =
            new ProductAndStandardTime[5].Select(x => new ProductAndStandardTime()).ToArray();

        private bool isWaitingTimeActive = false;

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
                new BreadcrumbItem(text: Localizer["update"] + " " + Localizer["jobObservation"], href: "", disabled: true)
            };

            BreadcrumbService.UpdateBreadcrumbs(_links);
            await GetUserAsync();
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
                _jobObservation.Supervisor = new();
                _jobObservation = await JobObservationService.GetJobObservationById(JobObservationId, true, true, true, false, true);

                if (_jobObservation == null)
                {
                    NoData = true;
                    _jobObservation = new();
                    StateHasChanged();

                }
                else
                {

                    _checklistCategoriesAndQuestions = await JobStructureCategoriesService.GetChecklistCategories(true);
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
                                questionAnswers[question.QuestionID] = item;
                            }
                        }
                    }

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


                    _plants = await PlantServices.GetPlants();
                    //_products = await ProductService.GetProducts();
                    _areas = await AreaServices.GetAreas(_jobObservation.PlantId);
                    _distributions = await DistributionService.GetDistributionsWithCollections(_jobObservation.PlantId, _jobObservation.AreaId);

                    _products = _distributions[_distributions.FindIndex(d => d.DistributionId == _jobObservation.DistributionId)].Products;


                    _operations = _distributions[_distributions.FindIndex(d => d.DistributionId == _jobObservation.DistributionId)].Operations;


                    var groupedOperations = _operations
                        .GroupBy(op => op.ProductName)
                        .Select(g => new ProductAndStandardTime
                        {
                            ProductName = g.Key,
                            StandardTime = g.Select(op => op.StandardTime).FirstOrDefault()
                        })
                        .ToList();

                    int count = Math.Min(groupedOperations.Count, 5);
                    _productAndSpecification = new ProductAndStandardTime[count];

                    for (int i = 0; i < count; i++)
                    {
                        var productName = groupedOperations[i].ProductName;

                        var standardTimeParts = groupedOperations[i].StandardTime.Split('§');
                        if (decimal.TryParse(standardTimeParts[0], out decimal standardTimeValue))
                        {
                            var roundedStandardTime = Math.Round(standardTimeValue, 2).ToString("F2");

                            _productAndSpecification[i] = new ProductAndStandardTime
                            {
                                ProductName = productName,
                                StandardTime = roundedStandardTime
                            };
                        }
                        else
                        {
                            _productAndSpecification[i] = new ProductAndStandardTime
                            {
                                ProductName = productName,
                                StandardTime = "0.00"
                            };
                        }
                    }


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
                        _jobObservation.TaktTime = _jobObservation.TaktTime.Replace(",", ".");
                        taktTime = double.Parse(_jobObservation.TaktTime, CultureInfo.InvariantCulture);
                    }

                    if (string.IsNullOrEmpty(_jobObservation.HOEStandardTimes) || _jobObservation.HOEStandardTimes.Contains("|"))
                    {
                        hoeStandardTime = 0.0;
                    }
                    else
                    {
                        _jobObservation.HOEStandardTimes = _jobObservation.HOEStandardTimes.Replace(",", ".");
                        hoeStandardTime = double.Parse(_jobObservation.HOEStandardTimes, CultureInfo.InvariantCulture);
                    }

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

                    ids = _jobObservation.SectionIds.Split('|');

                    jobProductId = _jobObservation.ProductId != null ? (int)_jobObservation.ProductId : 0;
                    showLoading = false;

                    var selectedProduct = _products.FirstOrDefault(p => p.ProductId == jobProductId);
                    if (jobProductId != 0)
                        _filteredOperations = _operations.Where(op => op.ProductName != null && op.ProductName.Contains(selectedProduct.Code)).ToList();


                    if (!string.IsNullOrEmpty(_jobObservation.ProductIds))
                    {
                        string[] productIdsArray = _jobObservation.ProductIds.Split('|');

                        for (int i = 0; i < 5; i++)
                        {
                            if (i < productIdsArray.Length && int.TryParse(productIdsArray[i].Trim(), out int productId))
                            {
                                jobProductIds[i] = productId;
                            }
                            else
                            {
                                jobProductIds[i] = 0;
                            }
                        }

                        for (int i = 0; i < 5; i++)
                        {

                            var prodName = _products.FirstOrDefault(p => p.ProductId == jobProductIds[i]);
                            _specifications[i] = new();

                            if (prodName != null)
                            {
                                var op = _operations.Where(o => o.OperationId == _jobObservation.Operations?.FirstOrDefault().OperationId).FirstOrDefault(p => p.ProductName == prodName?.Code);

                                if (op != null && !string.IsNullOrEmpty(op.NameTime))
                                {

                                    var names = op.NameTime.Replace(',', '.').Split("§");
                                    for (int j = 0; j < 5; j++)
                                    {
                                        if (!string.IsNullOrEmpty(names[j]))
                                        {
                                            _specifications[i].Add(names[j]);
                                        }
                                    }
                                }
                            }
                        }
                    }

                    StepsNumber = ConvertStringToArray(_jobObservation?.StepsNumber);
                    DoubleManagment = ConvertStringToArray(_jobObservation?.DoubleManagment);
                    Waiting = ConvertStringToArray(_jobObservation?.Waiting);

                    if (_jobObservation.SignatureImage != null && _jobObservation.SignatureImage.ContentType == "image/png")
                    {
                        var imageUrl = await FilesServices.ShowOperatorSignature(_jobObservation.SignatureImage.FileUploadId);
                        currentImage = imageUrl;
                        operatorSignatureSigned = true;
                    }

                    if (user.UserType == 1)
                    {
                        _supervisors = await UsersService.GetUsersByUserTypeInPlantAndArea(_jobObservation.PlantId, _jobObservation.AreaId, 3, false, false);
                        _supervisors = _supervisors.OrderBy(s => s.Name).ToList();

                    }

                    _jobObservation.TaktTime = "1.46";
                    taktTime = 1.46;

                    if (!string.IsNullOrEmpty(_jobObservation.OperationTimesJson) && _jobObservation.OperationTimesJson != "||||")
                    {
                        OperationTimes = JsonSerializer.Deserialize<Dictionary<string, double[]>>(_jobObservation.OperationTimesJson);
                        if (OperationTimes != null && OperationTimes.ContainsKey("CycleTime") && OperationTimes.ContainsKey("WaitingTime"))
                        {
                            for (int i = 0; i < 5; i++)
                            {
                                if (i < OperationTimes["CycleTime"].Length)
                                {
                                    CycleTimes[i] = OperationTimes["CycleTime"][i].ToString();
                                }
                                else
                                {
                                    CycleTimes[i] = "0";
                                }

                                // Accede a los valores de WaitingTime
                                if (i < OperationTimes["WaitingTime"].Length)
                                {
                                    WaitingTimes[i] = OperationTimes["WaitingTime"][i].ToString();
                                }
                                else
                                {
                                    WaitingTimes[i] = "0";
                                }
                            }
                        }

                    }


                    if (!string.IsNullOrEmpty(_jobObservation.ProductSpecifications) &&
                        _jobObservation.ProductSpecifications != "||||")
                    {
                        string[] specificationsArray = _jobObservation.ProductSpecifications.Split('|');

                        for (int i = 0; i < specificationsArray.Length && i < productSpecification.Length; i++)
                        {
                            productSpecification[i] = specificationsArray[i];
                        }
                    }
                    else
                    {
                        for (int i = 0; i < productSpecification.Length; i++)
                        {
                            productSpecification[i] = "";
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
                                && job.DistributionId == _jobObservation.DistributionId && job.Operations?.FirstOrDefault()?.OperationId == _jobObservation.Operations?.FirstOrDefault()?.OperationId)
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
                        rootNodeCCP = TreeServices.Make_Tree_CCP(CCPFolders.operation);
                    }
                    else
                    {
                        folderCCPError = true;
                    }

                    if (searchAssychart)
                    {
                        listFilter = _assychart.RoutesProductsAssyChart.Where(r => r.Code.ToLower().Contains(_jobObservation.Operations?.FirstOrDefault()?.Code.ToLower(), StringComparison.OrdinalIgnoreCase)).ToList();
                        FilterOperation = true;
                    }
                }

            }

          
            StateHasChanged();
        }

        private int?[] ConvertStringToArray(string stringValue) =>
            string.IsNullOrEmpty(stringValue)
                ? new int?[5]
                : stringValue.Split('|')
                    .Select(s => int.TryParse(s, out var result) ? (int?)result : null)
                    .ToArray();

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
        private async void OpenCommentDialog()
        {
            var parameters = new DialogParameters { { "_jobObservation", _jobObservation }, { "ChangeDate", EventCallback.Factory.Create(this, ChangeDate) } };
            var dialog = await DialogService.ShowAsync<ChangeDate_Dialog>("", parameters, dialogCommentOptions);
            await dialog.Result;
        }
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
                    _ = await GenerateLups();
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

                    _ = await GenerateLups();
                    Snackbar.Clear();
                    Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                    Snackbar.Add(Localizer["DateChangeInJob"] + $" {_jobObservation.JobObservationId}", Severity.Info);
                    NavigationManager.NavigateTo("/jobobservation");
                }
                else
                    await JSRuntime.InvokeVoidAsync("alert", "Update failed!"); // Alert
            }
        }


        private string BuildOperationTimesJson()
        {
            var operationTimes = new
            {
                CycleTime = OperationTimes["CycleTime"],
                WaitingTime = OperationTimes["WaitingTime"]
            };

            return JsonSerializer.Serialize(operationTimes);
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

            if (string.IsNullOrEmpty(_jobObservation.OperatorSignature))
            {
                currentImage = "";
            }

            if (_jobObservation.OperatorId != new int() && !string.IsNullOrEmpty(_jobObservation.OperatorSignature))
            {
                User operatorUser = await UsersService.GetUser(_jobObservation.OperatorId);

                if (_jobObservation.OperatorSignature != operatorUser.Payroll.ToString())
                {
                    Snackbar.Clear();
                    Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                    Snackbar.Add($"Operator Signature doesn't match", Severity.Error);

                    return;
                }
                if (currentImage == "")
                {
                    Snackbar.Clear();
                    Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                    Snackbar.Add($"Operator Signature is missing", Severity.Error);
                    return;
                }
                else if (!(_jobObservation.SignatureImage != null && _jobObservation.SignatureImage.ContentType == "image/png"))
                {
                    await GenerateOperatorSignatureImage();
                }
            }

            startHour = DateTime.Now.TimeOfDay;


            _jobObservation.OperationTimesJson = BuildOperationTimesJson();
            _jobObservation.StepsNumber = StepsNumber[0] + "|" + StepsNumber[1] + "|" + StepsNumber[2] + "|" + StepsNumber[3] + "|" + StepsNumber[4];
            _jobObservation.DoubleManagment = DoubleManagment[0] + "|" + DoubleManagment[1] + "|" + DoubleManagment[2] + "|" + DoubleManagment[3] + "|" + DoubleManagment[4];
            _jobObservation.Waiting = Waiting[0] + "|" + Waiting[1] + "|" + Waiting[2] + "|" + Waiting[3] + "|" + Waiting[4];
            _jobObservation.TaktTime = taktTime.ToString();
            _jobObservation.HOEStandardTimes = hoeStandardTime.ToString();
            _jobObservation.KpiId = kpiID;
            _jobObservation.ProductId = jobProductId;
            _jobObservation.ProductIds = string.Join("|", jobProductIds);
            _jobObservation.ProductSpecifications = string.Join("|", productSpecification);

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
                    _ = await GenerateLups();
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
                    _ = await GenerateLups();
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


        public bool ChecklistQuestionsValidation()
        {
            foreach (var (category, section) in _checklistCategoriesAndQuestions.Select((category, section) => (category, section)))
            {
                if (ids.Any(id => id == category.JobCategoryStructureId.ToString()))
                {
                    if (category.Type == StructureType.Checklist)
                    {
                        if (category.ChecklistQuestions.Count > 0)
                        {
                            foreach (var question in category.ChecklistQuestions)
                            {
                                int secNum = section;
                                if (_jobObservation.ChecklistAnswers.Any(ck => ck.QuestionID == question.QuestionID))
                                {
                                    ChecklistAnswer answer = _jobObservation.ChecklistAnswers.ToList().Find(ck => ck.QuestionID == question.QuestionID);
                                    if (string.IsNullOrEmpty(answer.Answer))
                                    {
                                        Snackbar.Add(Localizer["answer all the questions"], Severity.Error);
                                        return true; 
                                    }
                                }
                                else
                                {
                                    ChecklistAnswer answer = questionAnswers[question.QuestionID];
                                    if (string.IsNullOrEmpty(answer.Answer))
                                    {
                                        Snackbar.Add(Localizer["answer all the questions"], Severity.Error);
                                        return true;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return false;
        }

        //Under Review Job observation
        public async void UnderReviewJobObservation()
        {
            Snackbar.Clear();
            Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;

            if (ChecklistQuestionsValidation())
            {
                return;
            }

            if (_jobObservation.OperatorSignature == null || _jobObservation.OperatorSignature == "")
            {
                Snackbar.Add(Localizer["operatorsignaturemiss"] + $"!", Severity.Error);
                return;
            }

            if (_jobObservation.OperatorSignature != _jobObservation.Operator.Payroll.ToString())
            {
                Snackbar.Add(Localizer["operatorsignaturenotmarch"], Severity.Error);
                return;
            }
            if (currentImage == "")
            {
                Snackbar.Add($"Operator Signature is missing", Severity.Error);
                return;
            }
            else if (!(_jobObservation.SignatureImage != null && _jobObservation.SignatureImage.ContentType == "image/png"))
            {
                await GenerateOperatorSignatureImage();
            }

            _jobObservation.OperationTimesJson = BuildOperationTimesJson();

            _jobObservation.StepsNumber = StepsNumber[0] + "|" + StepsNumber[1] + "|" + StepsNumber[2] + "|" + StepsNumber[3] + "|" + StepsNumber[4];
            _jobObservation.DoubleManagment = DoubleManagment[0] + "|" + DoubleManagment[1] + "|" + DoubleManagment[2] + "|" + DoubleManagment[3] + "|" + DoubleManagment[4];
            _jobObservation.Waiting = Waiting[0] + "|" + Waiting[1] + "|" + Waiting[2] + "|" + Waiting[3] + "|" + Waiting[4];
            _jobObservation.TaktTime = taktTime.ToString();
            _jobObservation.HOEStandardTimes = hoeStandardTime.ToString();
            _jobObservation.KpiId = kpiID;
            _jobObservation.ProductId = jobProductId;
            _jobObservation.ProductIds = string.Join("|", jobProductIds);
            _jobObservation.ProductSpecifications = string.Join("|", productSpecification);

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
                    _ = await GenerateLups();
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
                    _ = await GenerateLups();
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
            if (ChecklistQuestionsValidation())
            {
                return;
            }

            if (_jobObservation.SsvCommentary == null || _jobObservation.SsvCommentary == "")
            {
                Snackbar.Clear();
                Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                Snackbar.Add($"You need to add a commentary to reject the job observation", Severity.Error);
                return;
            }
            if (_jobObservation.OperatorSignature == null || _jobObservation.OperatorSignature == "")
            {
                Snackbar.Clear();
                Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                Snackbar.Add(Localizer["operatorsignaturemiss"] + $"!", Severity.Error);
                return;
            }

            if (_jobObservation.OperatorSignature != _jobObservation.Operator.Payroll.ToString())
            {
                Snackbar.Clear();
                Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                Snackbar.Add(Localizer["operatorsignaturenotmarch"], Severity.Error);
                return;
            }
            if (currentImage == "")
            {
                Snackbar.Clear();
                Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                Snackbar.Add($"Operator Signature is missing", Severity.Error);
                return;
            }
            else if (!(_jobObservation.SignatureImage != null && _jobObservation.SignatureImage.ContentType == "image/png"))
            {
                await GenerateOperatorSignatureImage();
            }


            _jobObservation.OperationTimesJson = BuildOperationTimesJson();

            _jobObservation.StepsNumber = StepsNumber[0] + "|" + StepsNumber[1] + "|" + StepsNumber[2] + "|" + StepsNumber[3] + "|" + StepsNumber[4];
            _jobObservation.DoubleManagment = DoubleManagment[0] + "|" + DoubleManagment[1] + "|" + DoubleManagment[2] + "|" + DoubleManagment[3] + "|" + DoubleManagment[4];
            _jobObservation.Waiting = Waiting[0] + "|" + Waiting[1] + "|" + Waiting[2] + "|" + Waiting[3] + "|" + Waiting[4];
            _jobObservation.TaktTime = taktTime.ToString();
            _jobObservation.HOEStandardTimes = hoeStandardTime.ToString();
            _jobObservation.KpiId = kpiID;
            _jobObservation.ProductId = jobProductId;
            _jobObservation.ProductIds = string.Join("|", jobProductIds);
            _jobObservation.ProductSpecifications = string.Join("|", productSpecification);

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
                    _ = await GenerateLups();
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
                    _ = await GenerateLups();
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
        private DialogOptions dialogSignOptions = new() { CloseOnEscapeKey = true, MaxWidth = MaxWidth.ExtraSmall, FullWidth = true };
        private async void OpenSignComment()
        {
            var parameters = new DialogParameters
            {
                { "userName", user.Name }
            };
            var dialog = await DialogService.ShowAsync<SignJobObservation_Dialog>("", parameters, dialogSignOptions);
            var result = await dialog.Result;
            if (!result.Canceled)
            {
                int option = (int)result.Data;
                switch (option)
                {
                    case 0:
                        Reject();
                        break;
                    case 1:
                        await SignDate();
                        break;
                    default:
                        return;
                }
            }
        }

        public async Task SignDate()
        {
            if (ChecklistQuestionsValidation())
            {
                return;
            }

            if (_jobObservation.OperatorSignature == null || _jobObservation.OperatorSignature == "")
            {
                Snackbar.Clear();
                Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                Snackbar.Add(Localizer["operatorsignaturemiss"] + $"!", Severity.Error);
                return;
            }

            if (_jobObservation.OperatorSignature != _jobObservation.Operator.Payroll.ToString())
            {
                Snackbar.Clear();
                Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                Snackbar.Add(Localizer["operatorsignaturenotmarch"], Severity.Error);
                return;
            }
            if (currentImage == "")
            {
                Snackbar.Clear();
                Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                Snackbar.Add($"Operator Signature is missing", Severity.Error);
                return;
            }
            else if (!(_jobObservation.SignatureImage != null && _jobObservation.SignatureImage.ContentType == "image/png"))
            {
                await GenerateOperatorSignatureImage();
            }

            endHour = DateTime.Now.TimeOfDay;

            _jobObservation.OperationTimesJson = BuildOperationTimesJson();

            _jobObservation.StepsNumber = StepsNumber[0] + "|" + StepsNumber[1] + "|" + StepsNumber[2] + "|" + StepsNumber[3] + "|" + StepsNumber[4];
            _jobObservation.DoubleManagment = DoubleManagment[0] + "|" + DoubleManagment[1] + "|" + DoubleManagment[2] + "|" + DoubleManagment[3] + "|" + DoubleManagment[4];
            _jobObservation.Waiting = Waiting[0] + "|" + Waiting[1] + "|" + Waiting[2] + "|" + Waiting[3] + "|" + Waiting[4];
            _jobObservation.TaktTime = taktTime.ToString();
            _jobObservation.HOEStandardTimes = hoeStandardTime.ToString();
            _jobObservation.KpiId = kpiID;
            _jobObservation.ProductId = jobProductId;
            _jobObservation.ProductIds = string.Join("|", jobProductIds);
            _jobObservation.ProductSpecifications = string.Join("|", productSpecification);

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

                    _ = await GenerateLups();
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
                _jobObservation.FinishedDate = DateTime.Now;

                _ = await GenerateChecklistAnswers();

                var result = await JobObservationService.UpdateJobObservation(_jobObservation, user.ObjectId);



                if (result)
                {
                    _ = await GenerateLups();
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
            _jobObservation.Operations = new List<Operation>();
            _jobObservation.OperatorId = 0;
            _jobObservation.SupervisorId = 0;
            jobProductId = 0;
            _specifications = new();
            _assychart = null;

            _areas = await AreaServices.GetAreas(_jobObservation.PlantId);
            _areas = _areas.OrderBy(a => a.Description).ToList();
        }

        private async void ShowDistributions()
        {
            _jobObservation.SupervisorId = 0;
            _supervisors.Clear();
            _assychart = null;
            jobProductId = 0;
            _specifications = new();
            if (user.UserType == 1)
            {
                _supervisors = await UsersService.GetUsersByUserTypeInPlantAndArea(_jobObservation.PlantId, _jobObservation.AreaId, 3, false, false);
                _supervisors = _supervisors.OrderBy(s => s.Name).ToList();

            }
            else if (user.UserType == 2)
            {
                _supervisors = new();
                _supervisors = await UsersService.GetSubordinates(user.UserId, false);
                _supervisors = _supervisors.OrderBy(s => s.Name).ToList();

            }

            _jobObservation.OperatorId = 0;
            _jobObservation.DistributionId = 0;
            _jobObservation.Operations = new List<Operation>();
            jobProductId = 0;

            _distributions = await DistributionService.GetDistributionsWithCollections(_jobObservation.PlantId, _jobObservation.AreaId);
            _distributions = _distributions.OrderBy(d => d.Description).ToList();

            StateHasChanged();
        }

        private async void ShowOperators()
        {
            if (_jobObservation.DistributionId != 0 && _jobObservation.Operations?.FirstOrDefault()?.OperationId != 0)
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


        private async void ShowPastJobObservations()
        {
            flag = true;
            if (_jobObservation.Operations.Count() > 0)
                operation = await OperationService.GetOperationById(_jobObservation.PlantId, _jobObservation.AreaId, _jobObservation.DistributionId, (int)_jobObservation.Operations?.FirstOrDefault()?.OperationId);

            pastjobObservations = new();
            pastLup = new();
            if (user != null)
            {
                pastJobs = await JobObservationService.GetAllJobObservations();

                foreach (var job in pastJobs)
                {
                    if (job.SupervisorId == _jobObservation.SupervisorId && Convert.ToDateTime(job.StartDate?.ToShortDateString()).Date <= Convert.ToDateTime(_jobObservation.StartDate?.ToShortDateString()).Date
                        && job.DistributionId == _jobObservation.DistributionId && job.Operations?.FirstOrDefault()?.OperationId == _jobObservation.Operations?.FirstOrDefault()?.OperationId)
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


        private async void ShowOperations()
        {

            jobProductId = 0;
            _specifications = new();
            _operations = await OperationService.GetOperations(_jobObservation.PlantId, _jobObservation.AreaId, _jobObservation.DistributionId);
        }

        private async void UpdateOperation(int id)
        {
           
            jobProductId = 0;
            _specifications = new();
        }

        public async void ModifyKPI(int kpi, int catId)
        {
            kpiID = kpi;
            var specialEntry = _checklistCategoriesAndQuestions.First(p => p.JobCategoryStructureId == catId).ChecklistQuestions.First(p => p.CategorySequence == 6).QuestionID;

            var Comentary = kpi switch { 1 => "S&P", 2 => "Q", 3 => "D", 4 => "C", 5 => "E", 6 => "Other", _ => "S&P/Q" };

            questionAnswers[specialEntry].CommentarySV = Comentary;
            questionAnswers[specialEntry].Answer = "YES";
        }

        public async void ModifyKPIByQuestion(int kpi, int catId)
        {
            kpiID = kpi;
            var specialEntry = _checklistCategoriesAndQuestions.First(p => p.JobCategoryStructureId == catId).ChecklistQuestions.First(p => p.CategorySequence == 6).QuestionID;
            questionAnswers[specialEntry].Answer = "YES";
        }

        //Temp Lup
        private async void OpenTempCameraDialog()
        {

            var parameters = new DialogParameters {
                { "Prompt", $"LUP Evidence" },
                { "returnFrame", EventCallback.Factory.Create<string>(this, GetCurrentFrame) } };
            var dialog = await DialogService.ShowAsync<AnswerCamera_Dialog>("", parameters, dialogCameraOptions);
            await dialog.Result;
        }

        private void SetLup(Lup lup)
        {
            selectedLup = lup;
        }

        public void DeleteTempLup(Lup lup)
        {
            if (_tempLup.ContainsKey(lup))
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
            else
            {
                Console.WriteLine("El lup no existe en _tempLup.");
            }
        }

        private void RemoveTempImage(Lup lup, int index)
        {
            if (index >= 0 && index < _tempLup[lup].Count)
            {
                _tempLup[lup].RemoveAt(index);

                StateHasChanged();
            }
        }

        //Past Job Observations Modal
        private async void OpenDialogPastJobObservations()
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

            var parameters = new DialogParameters {
                { "DistDesc", _distributions.First(p=>p.DistributionId == _jobObservation.DistributionId) },
                { "OperaDesc", operation.Description },
                { "pastjobObservations", pastjobObservations },
                { "pastLup", pastLup }
            };
            var dialog = await DialogService.ShowAsync<PastJobObs_Dialog>("", parameters, dialogPastJobObservations);
            await dialog.Result;
        }
        private async void OpenDialogLup(int id)
        {
            DialogOptions dialogLup = new() { CloseOnEscapeKey = true, MaxWidth = MaxWidth.Large, FullWidth = true };
            var parameters = new DialogParameters { { "lupId", id } };
            var dialog = DialogService.Show<OpenLup_Dialog>("", parameters, dialogLup);
            await dialog.Result;
        }

        void EditLup(int lupId)
        {
            NavigationManager.NavigateTo($"lup/updatelup/{lupId}");
        }

        async Task DeleteLup(int deleteLupId)
        {
            await LupService.DeleteLup(deleteLupId);

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
                        && job.DistributionId == _jobObservation.DistributionId && job.Operations?.FirstOrDefault()?.OperationId == _jobObservation.Operations?.FirstOrDefault()?.OperationId)
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

            StateHasChanged();
        }

        void GoToJobObservation(int jobObservationId)
        {
            NavigationManager.NavigateTo($"/");
            NavigationManager.NavigateTo($"jobobservation/updatejobobservation/{jobObservationId}");
        }

        private void UpdateValue(int operationId, int cycleIndex, double newValue)
        {
            //if (!OperationTimes.ContainsKey(operationId))
            //{
            //    OperationTimes[operationId] = new Dictionary<int, double>();
            //}

            //OperationTimes[operationId][cycleIndex] = newValue;
            //StateHasChanged();

            //Console.WriteLine("Diccionario actualizado:");
            //foreach (var kvp in OperationTimes)
            //{
            //    Console.WriteLine($"OperationId: {kvp.Key}");
            //    foreach (var cycleKvp in kvp.Value)
            //    {
            //        Console.WriteLine($"  CycleIndex: {cycleKvp.Key}, Value: {cycleKvp.Value}");
            //    }
            //}

        }

        private string elapsedTime2 = "00:00:00.000";
        private DateTime startTime2;
        private bool isTimerRunning2 = false;
        private System.Timers.Timer timer2;

        private int currentOperationIndex = 0;
        private int currentCycle = 0;
        private string cronometerTime = "0.00";

        private double previousOperationTime = 0.0;

        UpdateJobObsCmpts.Timer Timer;
        private void NextOperation()
        {
            double elapsedCentiseconds = Timer.GetElapsedCentiseconds();

            if (isWaitingTimeActive)
            {
                double cycleTime = double.Parse(CycleTimes[currentCycle], CultureInfo.InvariantCulture);
                double waitingTime = Math.Round(elapsedCentiseconds - cycleTime, 2);

                WaitingTimes[currentCycle] = waitingTime.ToString();
                OperationTimes["WaitingTime"][currentCycle] = waitingTime;

                isWaitingTimeActive = false;
            }
            else
            {
                var cycleTime = Math.Round(elapsedCentiseconds, 2);
                CycleTimes[currentCycle] = cycleTime.ToString();
                OperationTimes["CycleTime"][currentCycle] = cycleTime;

            }
            currentCycle++;

            if (currentCycle > 4)
            {
                currentCycle = 0;
            }

            //SetAsCurrentJobObservation();
            StateHasChanged();
        }

        private void WaitingTime()
        {
            double elapsedCentiseconds = Timer.GetElapsedCentiseconds();
            var cycleTime = Math.Round(elapsedCentiseconds, 2);

            CycleTimes[currentCycle] = cycleTime.ToString();
            OperationTimes["CycleTime"][currentCycle] = cycleTime;

            isWaitingTimeActive = true;
            Waiting[currentCycle] = 1;

            StateHasChanged();
        }

        public void ChangeCycle(int cycle)
        {
            currentCycle = cycle;
        }

        bool errorflag = false;

        public bool checkIfError(int i, int value)
        {
            if (errorflag)
            {
                bool check = currentCycle - 1 == i;
                bool chekV = value == 0;
                return check && chekV;
            }
            else
            {
                return false;
            }
        }

        private void PauseTimer()
        {
            isTimerRunning2 = false;
            currentOperationIndex = 0;

            previousOperationTime = 0.0;
            //foreach (var operationId in OperationTimes.Keys)
            //{
            //    OperationTimes[operationId][currentCycle] = 0.0;
            //}
        }

        private void StartOver()
        {
            //currentCycle = 1;
            cronometerTime = "0.00";
            isTimerRunning2 = false;
            currentOperationIndex = 0;

            previousOperationTime = 0.0;
            foreach (var operationId in OperationTimes.Keys)
            {
                //for (int cycle = 1; cycle <= 5; cycle++)
                //{
                OperationTimes[operationId][currentCycle] = 0.0;
                //}
            }


            StateHasChanged();
        }




        public void actualizarCampo()
        {
            //if (currentOperationIndex < _operations.Count)
            //{
            //    var currentOperation = _operations[currentOperationIndex];
            //    OperationTimes[currentOperation.OperationId][currentCycle] = GetRandomNumber(0.1, 0.9);

            //    currentOperationIndex++;

            //    if (currentOperationIndex >= _operations.Count && currentCycle < 5)
            //    {
            //        currentCycle++;
            //        currentOperationIndex = 0;
            //    }

            //    StateHasChanged();
            //}
            //else
            //{
            //    currentCycle = 1;
            //    currentOperationIndex = 0;
            //    StateHasChanged();
            //}
        }

        private async Task ShowSpecifications(int id, int productIndex)
        {
            jobProductIds[productIndex] = id;
            _specifications[productIndex] = new();



            var prodName = _products.FirstOrDefault(p => p.ProductId == jobProductIds[productIndex]);


            if (prodName != null)
            {
                var op = _operations.Where(o => o.OperationId == _jobObservation.Operations?.FirstOrDefault().OperationId).FirstOrDefault(p => p.ProductName == prodName?.Code);

                if (op != null && !string.IsNullOrEmpty(op.NameTime))
                {

                    var names = op.NameTime.Replace(',', '.').Split("§");
                    for (int i = 0; i < 5; i++)
                    {
                        if (!string.IsNullOrEmpty(names[i]))
                        {
                            _specifications[productIndex].Add(names[i]);
                        }
                    }
                }
            }

            StateHasChanged();
        }

        public void InitializeCycleTimes(string specification, int index)
        {
            productSpecification[index] = specification;
            _filteredOperations = new();
            StepsNumber = new int?[5];
            DoubleManagment = new int?[5];
            Waiting = new int?[5];

            var selectedProduct = _products.FirstOrDefault(p => p.ProductId == jobProductId);
            if (selectedProduct != null)
            {
                _filteredOperations = _operations.Where(op => op.ProductName != null && op.ProductName.Contains(selectedProduct.Code)).ToList();

            }
            var standardTimeIndex = _specifications[index].FindIndex(s => s == productSpecification[index]);

            foreach (var op in _filteredOperations)
            {
                if (op.StandardTime != null)
                {

                    var hoeTimes = op.StandardTime.Replace(',', '.').Split("§");
                    hoeStandardTime = double.Parse(hoeTimes[standardTimeIndex], CultureInfo.InvariantCulture);
                    hoeStandardTime = Math.Round(hoeStandardTime, 2);
                    Console.WriteLine(hoeStandardTime);
                }
            }
        }


        private double GetRandomNumber(double min, double max)
        {
            Random random = new Random();
            return random.NextDouble() * (max - min) + min;
        }

        private void HandleKeyDown(KeyboardEventArgs args)
        {
            if (isTimerRunning2 && args.Code == "KeyN")
            {
                NextOperation();
            }
        }

        private CDMS_CCP_Archives CcpFilesInFolder = new CDMS_CCP_Archives();
        private CDMS_HOE_Archives HoeFilesInFolder = new CDMS_HOE_Archives();
        private CDMS_GOS_Archives GosFilesInFolder = new CDMS_GOS_Archives();

        private DialogOptions dialogOptions = new() { CloseOnEscapeKey = true, MaxWidth = MaxWidth.Large, FullWidth = true };

        public void ShowHourMessage()
        {
            Snackbar.Clear();
            Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
            Snackbar.Add(Localizer["btnChangeHour"], Severity.Info);
        }


        //Delete lup
        private async void OpenDeleteDialog(int deleteId)
        {
            var parameters = new DialogParameters { { "Name", user.Name }, { "lupId", deleteId }, { "DeleteLup", EventCallback.Factory.Create<int>(this, DeleteLup) } };
            var dialog = await DialogService.ShowAsync<Delete_Dialog>("", parameters, dialogDeleteOptions);
            await dialog.Result;

        }
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

        int SOSCodePathId { get; set; } = 0;
        string SosPanelOpen { get; set; } = "";
        private async Task<AsyncVoidMethodBuilder> OpenDialogCodePath(SOSCodePath itemselected, int panelSelect)
        {

            ShowLoading = true;
            SOSCodePathId = itemselected.SOSCodePathId;
            switch (panelSelect)
            {
                case 1:
                    SosPanelOpen = "HOE";
                    break;

                case 2:
                    SosPanelOpen = "CCP";
                    break;

                case 3:
                    SosPanelOpen = "GOS";
                    break;

                case 4:
                    SosPanelOpen = "HOE_CD";
                    break;

                case 5:
                    SosPanelOpen = "CCP_CD";
                    break;

                case 6:
                    SosPanelOpen = "GOS_CD";
                    break;


            }

            CodePathModalDisplay = true;
            StateHasChanged();
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
        ChecklistAnswer SelectedAnswer { get; set; }
        private async void OpenCameraAnswerDialog(ChecklistAnswer item)
        {
            var parameters = new DialogParameters { { "Prompt", $"Evidence for {item.Prompt}" }, { "returnFrame", EventCallback.Factory.Create<string>(this, GetCurrentFrameAnswer) } };
            SelectedAnswer = item;
            var dialog = await DialogService.ShowAsync<AnswerCamera_Dialog>("", parameters, dialogCameraOptions);
            await dialog.Result;
        }

        //Camera
        private DialogOptions dialogCameraOptions = new() { CloseOnEscapeKey = true, MaxWidth = MaxWidth.Medium, FullWidth = true, CloseButton = true, DisableBackdropClick = true };

        private List<string> capturedImages = new List<string>();

        private async void OpenCameraDialog()
        {
            var parameters = new DialogParameters { { "Prompt", $"LUP Evidence" }, { "returnFrame", EventCallback.Factory.Create<string>(this, GetCurrentFrame) } };
            var dialog = await DialogService.ShowAsync<AnswerCamera_Dialog>("", parameters, dialogCameraOptions);
            await dialog.Result;
        }

        private string? cameraId = null;

        private int frameCount;

        private async void GetCurrentFrame(string imageData)
        {
            if (!string.IsNullOrEmpty(imageData))
            {
                if (selectedLup != null && _tempLup.ContainsKey(selectedLup))
                {
                    var lupKey = _tempLup.FirstOrDefault(l => l.Key == selectedLup);
                    _tempLup[lupKey.Key].Add(imageData);
                    selectedLup = null;
                }
                else
                {
                    capturedImages.Add(imageData);
                }
            }
            StateHasChanged();
        }

        private async void GetCurrentFrameAnswer(string imageData)
        {

            if (!string.IsNullOrEmpty(imageData))
            {
                SelectedAnswer.capturedImages.Add(imageData);
            }
            SelectedAnswer.Edited = true;

            StateHasChanged();
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

        private async Task UploadEvidence(int lupPhotosId)
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






        //Guide Modal
        MudTabs guideTabs;

        private int selectedPillar = 0;

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
                    if ((question.Value.Answer != "" || question.Value.Edited) && !_jobObservation.ChecklistAnswers.Any(cka => cka.QuestionID == question.Key))
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



        private async Task<AsyncVoidMethodBuilder> GenerateLups()
        {
            if (_tempLup?.Count > 0)
            {
                foreach (var lup in _tempLup)
                {
                    using var content = new MultipartFormDataContent();

                    foreach (var imageData in lup.Value)
                    {
                        if (!string.IsNullOrEmpty(imageData))
                        {
                            var base64Data = imageData.Replace("data:image/png;base64,", "");

                            if (IsValidBase64String(base64Data))
                            {
                                var imageBytes = Convert.FromBase64String(base64Data);
                                var imageStream = new MemoryStream(imageBytes);
                                imageStream.Position = 0;
                                var fileContent = new StreamContent(imageStream);
                                fileContent.Headers.ContentType = new MediaTypeHeaderValue("image/png");

                                content.Add(fileContent, "LupFiles", "CameraEvidences.png");
                            }
                            else
                            {
                                Snackbar.Clear();
                                Snackbar.Add("Invalid image data, Update Evidences", Severity.Error);
                            }
                        }
                        else
                        {
                            Snackbar.Clear();
                            Snackbar.Add("No image data to upload, Update Evidences", Severity.Warning);
                        }
                    }

                    lup.Key.JobObservationId = _jobObservation.JobObservationId;
                    lup.Key.IsActive = true;
                    lup.Key.CreatedDate = DateTime.Now;

                    startHour = DateTime.Now.TimeOfDay;

                    if (CultureInfo.CurrentCulture.Name == "en-US")
                    {
                        var formatedStartDate = lup.Key.CreatedDate;

                        var EnglishStartDate = formatedStartDate?.Month.ToString() + "/" + formatedStartDate?.Day.ToString() + "/" + formatedStartDate?.Year.ToString();
                        lup.Key.CreatedDate = DateTime.ParseExact(EnglishStartDate, "M/d/yyyy", CultureInfo.InvariantCulture);

                        hour1 = lup.Key.CreatedDate?.ToShortDateString() + $" {startHour}";


                        if (DateTime.TryParseExact(hour1, $"M/d/yyyy HH:mm:ss", null, DateTimeStyles.None, out newDate1))
                        {
                            Console.WriteLine(newDate1);
                        }
                        else
                        {
                            Console.WriteLine("Unable to parse '{0}'", hour1);
                        }

                        lup.Key.CreatedDate = newDate1;
                    }
                    else
                    {
                        hour1 = lup.Key.CreatedDate?.ToShortDateString() + $" {startHour?.ToString("hh\\:mm\\:ss")}";

                        if (DateTime.TryParseExact(hour1, $"d/M/yyyy HH:mm:ss", null, DateTimeStyles.None, out newDate1))
                        {
                            Console.WriteLine(newDate1);
                        }
                        else
                            Console.WriteLine("Unable to parse '{0}'", hour1);

                        lup.Key.CreatedDate = newDate1;
                    }

                    Lup sendLup = new Lup
                    {
                        LupId = 0,
                        JobObservationId = lup.Key.JobObservationId,
                        Oportunity = lup.Key.Oportunity,
                        IsActive = lup.Key.IsActive,
                        Pillar = lup.Key.Pillar,
                        Q3 = null,
                        Q4 = null,
                        Justification = null,
                        Status = 1,
                        StatusOKNG = LUPStatus.Percent0,
                        CreatedDate = lup.Key.CreatedDate,
                        EndDate = null,
                        DepartmentId = null,
                        StdChange = null,
                        StdUpdate = null,
                        ChecklistQuestionId = null,
                        Observer = null
                    };



                    content.Add(new StringContent(sendLup.JobObservationId.ToString()), "LupCmp.JobObservationId");
                    content.Add(new StringContent(sendLup.Oportunity ?? ""), "LupCmp.Oportunity");
                    content.Add(new StringContent(sendLup.IsActive.ToString()), "LupCmp.IsActive");
                    content.Add(new StringContent(sendLup.Pillar.ToString()), "LupCmp.Pillar");
                    content.Add(new StringContent(sendLup.Status.ToString()), "LupCmp.Status");
                    content.Add(new StringContent(sendLup.StatusOKNG.ToString()), "LupCmp.StatusOKNG");
                    content.Add(new StringContent(sendLup.CreatedDate.ToString()), "LupCmp.CreatedDate");


                    var result1 = await LupService.CreateEvidencesLup(content);

                    if (result1 != null)
                    {
                        Snackbar.Add($"Job observation Question item Update Evidences", Severity.Info);
                    }
                    else
                    {
                        Snackbar.Add($"Error in Question Update Evidences", Severity.Error);
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

        private int photoIndex = 0;

        private async void OpenPhotoDialog(int index, ChecklistAnswer item)
        {
            var parameters = new DialogParameters { { "photoIndex", index }, { "SelectedAnswer", item }, { "imageUrls", imageUrls } };
            SelectedAnswer = item;
            photoIndex = index;
            var dialog = await DialogService.ShowAsync<Photo_Dialog>("", parameters, dialogPhotoOptions);
            await dialog.Result;
        }

        private void HandleSignatureSaved()
        {
            currentImage = _signatureImageService.GetImage();

        }

        private void HandleClearSignature()
        {
            Console.WriteLine("Clean");
            currentImage = "";
        }

        private async Task<AsyncVoidMethodBuilder> GenerateOperatorSignatureImage()
        {
            if (!string.IsNullOrEmpty(currentImage))
            {
                var base64Data = currentImage.Replace("data:image/png;base64,", "");

                if (IsValidBase64String(base64Data))
                {
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

                    content.Add(content: new StringContent(_jobObservation.JobObservationId.ToString()), name: "JobObservationId");
                    var result1 = await JobObservationService.CreateOperatorSignature(content);

                    if (result1 != null)
                    {
                        Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                        Snackbar.Add($"Job observation operator signature Added", Severity.Info);
                    }
                    else
                    {
                        Snackbar.Clear();
                        Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                        Snackbar.Add($"Error in Job observation operator signature", Severity.Error);
                    }
                }
                else
                {
                    Snackbar.Clear();
                    Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                    Snackbar.Add("Invalid image data", Severity.Error);
                }
            }


            return new AsyncVoidMethodBuilder();
        }

        //LUP
        public async void AddTempLup(int pillar)
        {
            Snackbar.Clear();
            Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
            if (_jobObservation.SupervisorId == 0)
            {
                Snackbar.Add($"First select a Supervisor", Severity.Error);
                return;
            }

            List<Lup> lupsToAdd = new List<Lup>();

            switch (pillar)
            {
                case 1:
                    if (area_ListS != null && area_ListS.Count > 0)
                    {
                        foreach (var lupOpportunity in area_ListS)
                        {
                            Lup newLup = new Lup
                            {
                                ChecklistQuestionId = lupOpportunity.QuestionID,
                                Oportunity = ObjectCloner.ObjectCloner.DeepClone(lupOpportunity.Opportunity)
                            };
                            lupsToAdd.Add(newLup);
                        }

                        area_ListS.Clear();
                    }
                    else
                    {
                        Snackbar.Add($"Error S Area is empty", Severity.Error);
                        return;
                    }
                    break;
                case 2:
                    if (area_ListQ != null && area_ListQ.Count > 0)
                    {
                        foreach (var lupOpportunity in area_ListQ)
                        {
                            Lup newLup = new Lup
                            {
                                ChecklistQuestionId = lupOpportunity.QuestionID,
                                Oportunity = ObjectCloner.ObjectCloner.DeepClone(lupOpportunity.Opportunity)
                            };
                            lupsToAdd.Add(newLup);
                        }
                        area_ListQ.Clear();
                    }
                    else
                    {
                        Snackbar.Add($"Error Q Area is empty", Severity.Error);
                        return;
                    }
                    break;
                case 3:
                    if (area_ListD != null && area_ListD.Count > 0)
                    {
                        foreach (var lupOpportunity in area_ListD)
                        {
                            Lup newLup = new Lup
                            {
                                ChecklistQuestionId = lupOpportunity.QuestionID,
                                Oportunity = ObjectCloner.ObjectCloner.DeepClone(lupOpportunity.Opportunity)
                            };
                            lupsToAdd.Add(newLup);
                        }
                        area_ListD.Clear();
                    }
                    else
                    {
                        Snackbar.Add($"Error D Area is empty", Severity.Error);
                        return;
                    }
                    break;
                case 4:
                    if (area_ListC != null && area_ListC.Count > 0)
                    {
                        foreach (var lupOpportunity in area_ListC)
                        {
                            Lup newLup = new Lup
                            {
                                ChecklistQuestionId = lupOpportunity.QuestionID,
                                Oportunity = ObjectCloner.ObjectCloner.DeepClone(lupOpportunity.Opportunity)
                            };
                            lupsToAdd.Add(newLup);
                        }
                        area_ListC.Clear();
                    }
                    else
                    {
                        Snackbar.Add($"Error C Area is empty", Severity.Error);
                        return;
                    }
                    break;
                case 5:
                    if (area_ListOther != null && area_ListOther.Count > 0)
                    {
                        foreach (var lupOpportunity in area_ListOther)
                        {
                            Lup newLup = new Lup
                            {
                                ChecklistQuestionId = lupOpportunity.QuestionID,
                                Oportunity = ObjectCloner.ObjectCloner.DeepClone(lupOpportunity.Opportunity)
                            };
                            lupsToAdd.Add(newLup);
                        }

                        area_ListOther.Clear();
                    }
                    else
                    {
                        Snackbar.Add($"Error Others Area is empty", Severity.Error);
                        return;
                    }
                    break;

            }


            foreach (Lup LupItem in lupsToAdd)
            {
                LupItem.Observer = _jobObservation.Supervisor.Name;

                LupItem.JobObservationId = _jobObservation.JobObservationId;
                LupItem.Pillar = pillar;
                LupItem.Status = 1;
                LupItem.CreatedDate = DateTime.Now;
                LupItem.IsActive = true;

                if (!_tempLup.ContainsKey(LupItem))
                {
                    _tempLup.Add(LupItem, new List<string>());
                }
            }
            lup = new();

            Snackbar.Add($"Lup item added", Severity.Info);

        }

        //Questions and answers

        private async void AddLupOpportunity(ChecklistAnswer item, int section, ChecklistQuestion question)
        {

            Snackbar.Configuration.MaxDisplayedSnackbars = 5;
            Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
            var notGood = currentLanguage == "es-ES" ? question.NotGood : question.NotGoodEN;
            foreach (var pillar in question.Pillars)
            {
                var lupOpportunity = new LupOpportunity
                {
                    QuestionID = question.QuestionID,
                    Opportunity = $"{section}.{question.CategorySequence}- " + notGood,
                };

                switch (pillar)
                {
                    case 1:
                        areaS = notGood;
                        area_ListS?.Add(lupOpportunity);
                        Snackbar.Add("LUP added in Safety & Environment Pillar SECTION 3", Severity.Warning);
                        break;
                    case 2:
                        areaQ = notGood;
                        area_ListQ?.Add(lupOpportunity);
                        Snackbar.Add("LUP added in Quality Pillar SECTION 3", Severity.Warning);
                        break;
                    case 3:
                        areaD = notGood;
                        area_ListD?.Add(lupOpportunity);
                        Snackbar.Add("LUP added in Delivery Pillar SECTION 3", Severity.Warning);
                        break;
                    case 4:
                        areaC = notGood;
                        area_ListC?.Add(lupOpportunity);
                        Snackbar.Add("LUP added in Cost Pillar SECTION 3", Severity.Warning);
                        break;
                    case 5:
                        areaOther = notGood;
                        area_ListOther?.Add(lupOpportunity);
                        Snackbar.Add("LUP added in Other Pillar SECTION 3", Severity.Warning);
                        break;

                }
            }

            lupInsidences = await LupService.GetAllLupInsidences(question.QuestionID, _jobObservation.SupervisorId, _jobObservation.DistributionId);
            if (lupInsidences != null && lupInsidences.Count > 0)
            {
                foreach (var lup in lupInsidences)
                {
                    Console.WriteLine(lup.Oportunity);
                }
                DialogOptions dialogActiveLupItemsOptions = new() { CloseOnEscapeKey = true, MaxWidth = MaxWidth.Medium, FullWidth = true, CloseButton = true, DisableBackdropClick = true };
                var parameters = new DialogParameters { { "lupInsidences", lupInsidences } };
                var dialog = await DialogService.ShowAsync<ActiveLUPItems_Dialog>("", parameters, dialogActiveLupItemsOptions);
                await dialog.Result;
            }



            item.Show = true;
            item.Edited = true;

            StateHasChanged();
            base.StateHasChanged();
        }

        private async Task AnswerChangeOption(string option, int id, int section, int catId)
        {
            var answer = questionAnswers[id];
            answer.Answer = option;

            var entry = _checklistCategoriesAndQuestions.First(p => p.JobCategoryStructureId == catId).ChecklistQuestions.First(p => p.QuestionID == id);
            if (section == 1 && (entry.CategorySequence == 4 || entry.CategorySequence == 5))
            {
                var extraEntrySec = entry.CategorySequence == 4 ? 5 : 4;
                var specialEntry = _checklistCategoriesAndQuestions.First(p => p.JobCategoryStructureId == catId).ChecklistQuestions.First(p => p.CategorySequence == 6).QuestionID;
                
                var extraEntry = _checklistCategoriesAndQuestions.First(p => p.JobCategoryStructureId == catId).ChecklistQuestions.First(p => p.CategorySequence == extraEntrySec);

                string kpi = "";
                if (entry.CategorySequence == 4)
                {
                    kpi = option == "YES" ? "S&P" : "";
                    if (questionAnswers[extraEntry.QuestionID].Answer == "YES")
                        kpi += kpi.IsNullOrEmpty() ? "Q" : "/Q";
                }
                else //if(entry.CategorySequence == 5)
                {
                    if (questionAnswers[extraEntry.QuestionID].Answer == "YES")
                        kpi += "S&P";
                    kpi += option == "YES" ? kpi.IsNullOrEmpty() ? "Q" : "/Q" : "";
                }


                questionAnswers[specialEntry].CommentarySV = kpi;
                questionAnswers[specialEntry].Answer = "YES";

                ModifyKPIByQuestion(kpi switch { "" => 0, "S&P" => 1, "Q" => 2, _ => 7 }, catId);

            }
            //SetAsCurrentJobObservation();
        }

        private async void RemoveLupOppportunity(ChecklistAnswer item, int section, ChecklistQuestion question)
        {
            Console.WriteLine("passed");
            int removed = 0;
            foreach (var pillar in question.Pillars)
            {
                switch (pillar)
                {
                    case 1:
                        removed = area_ListS.RemoveAll(q => q.QuestionID == question.QuestionID);
                        if (removed > 0)
                        {
                            Snackbar.Add("LUP removed in Safety & Environment Pillar SECTION 3", Severity.Warning);
                        }
                        break;
                    case 2:
                        removed = area_ListQ.RemoveAll(q => q.QuestionID == question.QuestionID);
                        if (removed > 0)
                        {
                            Snackbar.Add("LUP removed in Quality Pillar SECTION 3", Severity.Warning);
                        }
                        break;
                    case 3:
                        removed = area_ListD.RemoveAll(q => q.QuestionID == question.QuestionID);
                        if (removed > 0)
                        {
                            Snackbar.Add("LUP removed in Delivery Pillar SECTION 3", Severity.Warning);
                        }
                        break;
                    case 4:
                        removed = area_ListC.RemoveAll(q => q.QuestionID == question.QuestionID);
                        if (removed > 0)
                        {
                            Snackbar.Add("LUP removed in Cost Pillar SECTION 3", Severity.Warning);
                        }
                        break;
                    case 5:
                        removed = area_ListOther.RemoveAll(q => q.QuestionID == question.QuestionID);
                        if (removed > 0)
                        {
                            Snackbar.Add("LUP removed in Other Pillar SECTION 3", Severity.Warning);
                        }
                        break;

                }
            }

            item.Show = true;
            item.Edited = true;

            StateHasChanged();
            base.StateHasChanged();
        }

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

        private void UpdateListEntry(ChecklistQuestion question, int secNum, string currentComment)
        {
            var notGood = currentLanguage == "es-ES" ? question.NotGood : question.NotGoodEN;
            string searchString = $"{secNum}.{question.CategorySequence}- {notGood}";
            string fullEntry = $"{secNum}.{question.CategorySequence}- {notGood}";

            if (!string.IsNullOrEmpty(currentComment))
            {
                fullEntry += $", comentario: {currentComment}";
            }

            Snackbar.Clear();
            Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;


            foreach (var pillarId in question.Pillars)
            {
                switch (pillarId)
                {
                    case 1:
                        int index1 = area_ListS?.FindIndex(l => l.QuestionID == question.QuestionID && l.Opportunity.Contains(searchString)) ?? -1;
                        if (area_ListS != null && index1 != -1)
                        {
                            area_ListS[index1].Opportunity = fullEntry;
                            Snackbar.Add($"Commentary added to Lup", Severity.Success);
                        }
                        break;
                    case 2:
                        int index2 = area_ListQ?.FindIndex(l => l.QuestionID == question.QuestionID && l.Opportunity.Contains(searchString)) ?? -1;
                        if (area_ListQ != null && index2 != -1)
                        {
                            area_ListQ[index2].Opportunity = fullEntry;
                            Snackbar.Add($"Commentary added to Lup in Quality", Severity.Success);
                        }
                        break;
                    case 3:
                        int index3 = area_ListD?.FindIndex(l => l.QuestionID == question.QuestionID && l.Opportunity.Contains(searchString)) ?? -1;
                        if (area_ListD != null && index3 != -1)
                        {
                            area_ListD[index3].Opportunity = fullEntry;
                            Snackbar.Add($"Commentary added to Lup in Delivery", Severity.Success);
                        }
                        break;
                    case 4:
                        int index4 = area_ListC?.FindIndex(l => l.QuestionID == question.QuestionID && l.Opportunity.Contains(searchString)) ?? -1;
                        if (area_ListC != null && index4 != -1)
                        {
                            area_ListC[index4].Opportunity = fullEntry;
                            Snackbar.Add($"Commentary added to Lup in Cost", Severity.Success);
                        }
                        break;
                    case 5:
                        int index5 = area_ListOther?.FindIndex(l => l.QuestionID == question.QuestionID && l.Opportunity.Contains(searchString)) ?? -1;
                        if (area_ListOther != null && index5 != -1)
                        {
                            area_ListOther[index5].Opportunity = fullEntry;
                            Snackbar.Add($"Commentary added to Lup in Other", Severity.Success);
                        }
                        break;
                }
            }
        }
        bool ShowOperationsDialog { get; set; } = false;



        private async void UpdateShowOperations()
        {
            ShowOperationsDialog = true;

            StateHasChanged();

        }

        private async void CloseOperations()
        {
            ShowOperationsDialog = false;

            StateHasChanged();

        }

        private string searchTerm = "";
        private IEnumerable<Operation> FilteredOperations =>
            _operations.Where(op =>
                string.IsNullOrEmpty(searchTerm) ||
                (op.Code?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ?? false) ||
                (op.Description?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ?? false));


    }//end class

}//end namespace
