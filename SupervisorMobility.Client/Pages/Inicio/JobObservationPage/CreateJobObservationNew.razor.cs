using BlazorCameraStreamer;
using Blazorise.Extensions;
using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using FuzzyString;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyModel;
using Microsoft.JSInterop;
using MudBlazor;
using Newtonsoft.Json.Linq;
using SupervisorMobility.Client.Data.Entities;
using SupervisorMobility.Client.Data.Entities.QuestionHelperEntities;
using SupervisorMobility.Client.Data.Entities.TreeStruct;
using SupervisorMobility.Client.Pages.Inicio.JobObservationPage.CreateJobObservation;
using SupervisorMobility.Client.Pages.Inicio.JobObservationPage.Modals;
using SupervisorMobility.Client.Pages.Inicio.SOSProgramPage.Dialogs;
using System.Globalization;
using System.Linq;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Text;
using System.Timers;

namespace SupervisorMobility.Client.Pages.Inicio.JobObservationPage
{
    public partial class CreateJobObservationNew
    {
        
        [Parameter]
        public string date { get; set; }
        public string hour1 { get; set; }
        public string hour2 { get; set; }
        TimeSpan? startHour = new TimeSpan(00, 00, 00);
        TimeSpan? endHour = new TimeSpan(00, 00, 00);
        DateTime newDate1;
        DateTime newDate2;

        List<Plant> _plants { get; set; } = new();
        List<Product> _products { get; set; } = new();
        List<Area> _areas = new();
        List<Distribution> _distributions = new();
        List<Operation> _operations = new();
        List<Operation> _filteredOperations = new();

        List<User> _supervisors { get; set; } = new();

        Dictionary<Lup, List<string>> _tempLup = new();

        Lup lup { get; set; } = new();
        List<List<string>> _specifications { get; set; } = new List<List<string>>
        {
            new List<string>(), // Product 1
            new List<string>(), // Product 2
            new List<string>(), // Product 3
            new List<string>(), // Product 4
            new List<string>()  // Product 5
        };

        public int[] jobProductIds = new int[5];
        public string[] operationSpecProduct = new string[5];

        AssyChart? _assychart { get; set; }


        public JobObservation _jobObservation { get; set; } = new();

        public string areaS;
        public string areaQ;
        public string areaD;
        public string areaC;
        public string areaOther;

        public string placeholder = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, " +
          "sed do eiusmod tempor incididuntut labore et dolore magna aliqua. Ut enim ad minim " +
          "veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo coe velit esse cillum";

        //Glosary
        private List<Glosary> glosary = new();
        private Dictionary<string, Glosary> _glosaryInfo;

        //Past Job observation
        private DialogOptions dialogPastJobObservations = new() { CloseOnEscapeKey = true, MaxWidth = MaxWidth.Large, FullWidth = true };

        public List<JobObservation> pastJobs = new();
        public List<JobObservation> pastjobObservations = new();
        public List<Lup> pastLup = new();
        public JobObservation pastJob = new();

        public Distribution distribution = new Distribution();
        public Operation? operation = new();

        public bool flag = false;
        public bool session = false;

        // Breadcrumb links
        private List<BreadcrumbItem> _links;


        //User
        private string json = string.Empty;
        public User user = new();

        //Operator user
        public List<User> _operators = new();
        public List<User> operatorUsers = new();

        public string[] questions = new string[5];
        public double taktTime { get; set; } = 1.46;
        public double hoeStandardTime { get; set; }
        public int kpiID = 0;
        public int auxErgonomicsLevel = 0;
        public int jobProductId = 0;

        public Dictionary<int, Dictionary<string, string[]>> imagesFromFile = new Dictionary<int, Dictionary<string, string[]>>();
        public Dictionary<int, List<string>> imagesFromCamera = new Dictionary<int, List<string>>();


        //Checklist Categories and questions
        public List<JobCategoryStructure> _checklistCategoriesAndQuestions { get; set; } = new();
        private Dictionary<int, string> questionResponses = new Dictionary<int, string>();
        private Dictionary<int, ChecklistAnswer> questionAnswers = new Dictionary<int, ChecklistAnswer>();
        private Dictionary<int, IEnumerable<string>> MultiQuestionAnswers { get; set; } = new();

        List<CategoryData> questionsData;

        public List<ChecklistAnswer> _checklistAnswers { get; set; } = new();

    

        string currentLanguage = "es-ES";

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

        private DialogOptions options = new DialogOptions() { CloseOnEscapeKey = false, DisableBackdropClick = true, CloseButton = false };

        List<Lup> lupInsidences = new();

        public Lup? selectedLup = null;

        public class ProductAndStandardTime
        {
            public string ProductName { get; set; }
            public string StandardTime { get; set; }
        }

        ProductAndStandardTime[] _productAndSpecification =
            new ProductAndStandardTime[5].Select(x => new ProductAndStandardTime()).ToArray();

        private bool isWaitingTimeActive = false;
        public string[] productSpecification = new string[5];

        bool ShowLoading = true;
        private IList<string> _sourceMsgLoading = new List<string>();
        private IList<MudBlazor.Color> _Colors = new List<MudBlazor.Color>() { MudBlazor.Color.Default, MudBlazor.Color.Primary, MudBlazor.Color.Secondary, MudBlazor.Color.Success, MudBlazor.Color.Info, MudBlazor.Color.Default, MudBlazor.Color.Primary, MudBlazor.Color.Secondary, MudBlazor.Color.Success, MudBlazor.Color.Info };
        private DialogOptions dialogOptions = new() { CloseOnEscapeKey = true, MaxWidth = MaxWidth.Large, FullWidth = true };

        protected async override Task OnInitializedAsync()
        {

            _sourceMsgLoading.Add($"{Localizer1["Loading1"]}");
            _sourceMsgLoading.Add($"{Localizer1["Loading2"]}");
            _sourceMsgLoading.Add($"{Localizer1["Loading3"]}");
            _sourceMsgLoading.Add($"{Localizer1["Loading4"]}");
            _sourceMsgLoading.Add($"{Localizer1["Loading5"]}");
            _sourceMsgLoading.Add($"{Localizer1["Loading6"]}");
            _sourceMsgLoading.Add($"{Localizer1["Loading7"]}");    
            _sourceMsgLoading.Add($"{Localizer1["Loading8"]}");
            _sourceMsgLoading.Add($"{Localizer1["Loading9"]}");
            _sourceMsgLoading.Add($"{Localizer1["Loading10"]}");
            _sourceMsgLoading.Add($"{Localizer1["Loading11"]}");

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

            _plants = await PlantServices.GetPlants();
            _plants = _plants.OrderBy(p => p.Description).ToList();
            _checklistCategoriesAndQuestions = await JobStructureCategoriesService.GetChecklistCategories(true);


            await GetUserAsync();

            bool confirm = false;
            if (checkForStoragedValues())
            {
                //if (!await SessionStorage.ContainKeyAsync("CJO"))
                //{
                    var parameters = new DialogParameters
                    {
                        { "ContentText", "You had a previous unsaved Job Observation \n Do you wish to continue with it?" },
                        { "ButtonText", "Continue" },
                        { "CancelText", "New JO" }
                    };

                    var dialog = await DialogService.ShowAsync<Shared.Confirmation>("Load Data?", parameters, options);
                    var result = await dialog.Result;

                    if (!result.Canceled)
                    {
                        confirm = (bool)result.Data;
                        //await SessionStorage.SetItemAsync("CJO", true);
                    }
                    else
                    {
                        confirm = false;
                    }
                //}
                //else
                //{
                //    confirm = true;
                //}
            }

            if (confirm)
            {
                session = true;

                try
                {
                    _jobObservation = await LocalStorage.GetItemAsync<JobObservation>("JobObs") ?? throw new ArgumentNullException("Error Retriving Job Observation", nameof(_jobObservation));                     
                    //if (ot) { ot = false; OperationTimes = await LocalStorage.GetItemAsync<Dictionary<int, Dictionary<int, double>>>("OpTimes") ?? new(); Console.WriteLine(OperationTimes);  }
                    var _tempLupList = await LocalStorage.GetItemAsync<List<KeyValuePair<Lup, List<string>>>>("LupToAdd") ?? new(); _tempLup = _tempLupList.ToDictionary(pair => pair.Key, pair => pair.Value);
                    area_ListS = await LocalStorage.GetItemAsync<List<LupOpportunity>>("area_ListS") ?? new(); 
                    area_ListQ = await LocalStorage.GetItemAsync<List<LupOpportunity>>("area_ListQ") ?? new(); 
                    area_ListD = await LocalStorage.GetItemAsync<List<LupOpportunity>>("area_ListD") ?? new(); 
                    area_ListC = await LocalStorage.GetItemAsync<List<LupOpportunity>>("area_ListC") ?? new(); 
                    area_ListOther = await LocalStorage.GetItemAsync<List<LupOpportunity>>("area_ListOther") ?? new(); 
                    imagesFromFile = await LocalStorage.GetItemAsync<Dictionary<int, Dictionary<string, string[]>>>("QAnsImgFF") ?? new(); 
                    imagesFromCamera = await LocalStorage.GetItemAsync<Dictionary<int, List<string>>>("QAnsImgFC") ?? new(); 
                    currentImage = await LocalStorage.GetItemAsync<string>("SignatureImg") ?? string.Empty; 
                    questionAnswers = await LocalStorage.GetItemAsync<Dictionary<int, ChecklistAnswer>>("QAns") ?? new(); 
                    MultiQuestionAnswers = await LocalStorage.GetItemAsync<Dictionary<int, IEnumerable<string>>>("MQAns") ?? new(); 
                    taktTime = await LocalStorage.GetItemAsync<double?>("taktTime") ?? 1.46; 
                    StepsNumber = await LocalStorage.GetItemAsync<int?[]>("StepsNumber") ?? new int?[5];
                    CycleTimes = await LocalStorage.GetItemAsync<string?[]>("CycleTimes") ?? new string?[5]; 
                    jobProductIds = await LocalStorage.GetItemAsync<int[]>("JobProductsIds") ?? new int[5];
                    _specifications = await LocalStorage.GetItemAsync<List<List<string>>>("Specifications") ?? new List<List<string>>();
                    productSpecification = await LocalStorage.GetItemAsync<string[]>("ProductSpecifications") ?? new string[5];
                    _products= await LocalStorage.GetItemAsync<List<Product>>("Products") ?? new();
                    operationSpecProduct = await LocalStorage.GetItemAsync<string[]>("OperationSpecProduct") ?? new string[5];

                    DoubleManagment = await LocalStorage.GetItemAsync<int?[]>("DblManagement") ?? new int?[5]; 
                    Waiting = await LocalStorage.GetItemAsync<int?[]>("Waiting") ?? new int?[5]; 
                    currentCycle = await LocalStorage.GetItemAsync<int?>("CC") ?? 0; 
                    hoeStandardTime = await LocalStorage.GetItemAsync<double?>("HoeStandardTime") ?? 0.0; 

                    //jobProductId = _jobObservation.ProductId ?? 0;
                    kpiID = _jobObservation.KpiId ?? 0;

                    bool skipQA = !questionAnswers.Any();
                    bool skipIFF = !imagesFromFile.Any();
                    bool skipIFC = !imagesFromCamera.Any();
                    bool skipMQA = !MultiQuestionAnswers.Any();

                    foreach (var category in _checklistCategoriesAndQuestions)
                    {
                        foreach (var question in category.ChecklistQuestions)
                        {
                            ChecklistAnswer newChAnswer = new();
                            newChAnswer.JobObservationId = _jobObservation.JobObservationId;
                            newChAnswer.QuestionID = question.QuestionID;
                            newChAnswer.Prompt = question.Prompt;
                            if (skipQA) questionAnswers.Add(question.QuestionID, newChAnswer);
                            if (skipIFF) { imagesFromFile.Add(question.QuestionID, new()); }
                            else if (!skipQA)
                            {
                                foreach (var item in imagesFromFile[question.QuestionID])
                                    questionAnswers[question.QuestionID].MediaUris.Add(item.Value[1]);
                            }
                            if (skipIFC) { imagesFromCamera.Add(question.QuestionID, new()); }
                            else if (!skipQA) { questionAnswers[question.QuestionID].capturedImages.AddRange(imagesFromCamera[question.QuestionID]); }
                            if (question.Type?.Code == "MCM" && skipMQA) MultiQuestionAnswers.Add(question.QuestionID, new List<string>());
                        }
                    }

                    await InitializeCollectionsWithPreviousData();

                    //if (_jobObservation.ProductId != null)
                    //{
                    //    var selectedProduct = _products.FirstOrDefault(p => p.ProductId == jobProductId);
                    //    if (selectedProduct != null)
                    //    {
                    //        _filteredOperations = _operations.Where(op => op.ProductName != null && op.ProductName.Contains(selectedProduct.Code)).ToList();
                    //    }
                    //}

                    Snackbar.Clear();
                    Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                    Snackbar.Add("Loaded previous work", Severity.Info);

                    //ShowLoading = false;
                    StateHasChanged();
                }
                catch (Exception e)
                {
                    Snackbar.Clear();
                    Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                    Snackbar.Add(e.Message, Severity.Error);

                    await InitializeJobObservation();
                }
            }
            else
            {
                ClearJOStorage();
                await InitializeJobObservation();
            }

            glosary = await GlosaryService.GetGlosary();
            _glosaryInfo = glosary.ToDictionary(x => x.Name, x => x);

            questionsData = new();

            foreach (var category in _checklistCategoriesAndQuestions.Where(c=>c.Type == StructureType.Checklist))
            {
                var tempCD = new CategoryData();
                tempCD.CategoryId = category.JobCategoryStructureId;
                foreach (var _question in category.ChecklistQuestions)
                {
                    var tempCQD = new CategoryQuestionData();
                    if (_question.Actions != null && _question.Actions.Any())
                    {
                        foreachLoop:
                        foreach (var (item, index) in _question.Actions.Select((item, index) => (item, index)))
                        {
                            var act = item.Split("℘", StringSplitOptions.RemoveEmptyEntries);
                            if (act.Length >= 2)
                            {
                                List<string> newQuestions = act[0].Split("⁂", StringSplitOptions.RemoveEmptyEntries).ToList();
                                List<string> newActions = act[1].Split("⁂").ToList();
                                if (!tempCQD.QuestionContent.ContainsKey(index))
                                {
                                    tempCQD.QuestionContent[index] = (new Dictionary<int, QuestionData>(), new Dictionary<int, ActionData>());
                                }
                                for (int i = 0; i < newQuestions.Count; i++)
                                {
                                    var temp = newQuestions[i].Split("§", StringSplitOptions.RemoveEmptyEntries);
                                    if(temp.Length < 3)
                                    {
                                        tempCQD.QuestionContent.Remove(index);
                                        goto foreachLoop;
                                    }
                                    if (!tempCQD.QuestionContent[index].Questions.ContainsKey(i))
                                    {
                                        tempCQD.QuestionContent[index].Questions[i] = new QuestionData(); // Default value
                                    }
                                    tempCQD.QuestionContent[index].Questions[i].QuestionId = temp.Length > 0 ? Int32.Parse(temp[0]) : 0;
                                    tempCQD.QuestionContent[index].Questions[i].Comparator = temp.Length > 1 ? temp[1] : "";
                                    tempCQD.QuestionContent[index].Questions[i].QstOption = temp.Length > 2 ? temp[2] : "";

                                }
                                for (int i = 0; i < newActions.Count; i++)
                                {
                                    var temp = newActions[i].Split("§", StringSplitOptions.RemoveEmptyEntries);
                                    if ((temp.Length > 0 && (temp[0] == "SET" || temp[0] == "DBLOPT")) && temp.Length < 2)
                                    {
                                        tempCQD.QuestionContent.Remove(index);
                                        goto foreachLoop;
                                    }
                                    if (!tempCQD.QuestionContent[index].Actions.ContainsKey(i))
                                    {
                                        tempCQD.QuestionContent[index].Actions[i] = new ActionData(); // Default value
                                    }
                                    tempCQD.QuestionContent[index].Actions[i].Operation = temp.Length > 0 ? temp[0] : "";
                                    tempCQD.QuestionContent[index].Actions[i].Value = temp.Length > 1 ? temp[1] : "";

                                }
                            }
                        }
                        tempCD.QuestionsInCategory[_question.QuestionID] = tempCQD;
                    }
                }
                if(tempCD.QuestionsInCategory.Any()) questionsData.Add(tempCD);
            }

            ShowLoading = false;
            StateHasChanged();

    }

        private async Task InitializeJobObservation()
        {
            _jobObservation.Supervisor = new();

            date = date.Replace("-", "/");

            _jobObservation.IsActive = true;
            _jobObservation.StartDate = DateTime.ParseExact(date, "d/M/yyyy", null);
            _jobObservation.EndDate = DateTime.ParseExact(date, "d/M/yyyy", null);
            _jobObservation.Option = 1;

            //_plants = await PlantServices.GetPlants();
            //_plants = _plants.OrderBy(p => p.Description).ToList();
            //_checklistCategoriesAndQuestions = await JobStructureCategoriesService.GetChecklistCategories(true);

            string jobCategoryStructureIds = "";

            bool skipQA = !questionAnswers.Any();
            bool skipIFF = !imagesFromFile.Any();
            bool skipIFC = !imagesFromCamera.Any();
            bool skipMQA = !MultiQuestionAnswers.Any();

            foreach (var category in _checklistCategoriesAndQuestions)
            {
                jobCategoryStructureIds += category.JobCategoryStructureId + "|";
                foreach (var question in category.ChecklistQuestions)
                {
                    ChecklistAnswer newChAnswer = new();
                    newChAnswer.JobObservationId = _jobObservation.JobObservationId;
                    newChAnswer.QuestionID = question.QuestionID;
                    newChAnswer.Prompt = question.Prompt;
                    if (skipQA) questionAnswers.Add(question.QuestionID, newChAnswer);
                    if (skipIFF) imagesFromFile.Add(question.QuestionID, new ());
                    if (skipIFC) imagesFromCamera.Add(question.QuestionID, new ());
                    if (question.Type?.Code == "MCM" && skipMQA) MultiQuestionAnswers.Add(question.QuestionID, new List<string>());
                }
            }

            if (!string.IsNullOrEmpty(jobCategoryStructureIds))
            {
                jobCategoryStructureIds = jobCategoryStructureIds.TrimEnd('|');
            }

            _jobObservation.SectionIds = jobCategoryStructureIds;

            //await GetUserAsync();

            ShowLoading = false;
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


                if (user.UserType == 2)
                {
                    _jobObservation.PlantId = user.PlantId.HasValue ? (int)user.PlantId : 0;
                    _jobObservation.AreaId = 0;
                    _jobObservation.SupervisorId = 0;
                    //_allSupervisors = await UsersService.GetUsersByType(3);
                    //_operators = await UsersService.GetUsersByType(4);

                    return;
                }
                else
                {
                    if (user.UserType == 1 && PatPlantId.IsNullOrEmpty())
                    {
                        _jobObservation.PlantId = 0;
                        _jobObservation.AreaId = 0;
                        _jobObservation.SupervisorId = 0;

                        return;
                    }

                    if (!PatPlantId.IsNullOrEmpty())
                    {
                        _jobObservation.PlantId = int.Parse(PatPlantId ?? "0");

                        _jobObservation.AreaId = int.Parse(PatAreaId ?? "0");

                        _areas = await AreaServices.GetAreas(_jobObservation.PlantId);
                        _areas = _areas.OrderBy(a => a.Description).ToList();

                        _jobObservation.SupervisorId = int.Parse(PatSupervisorId ?? "0");

                        _jobObservation.Supervisor = await UsersService.GetUser(_jobObservation.SupervisorId);

                        _distributions = await DistributionService.GetDistributionsWithCollections(_jobObservation.PlantId, _jobObservation.AreaId);
                        _distributions = _distributions.OrderBy(d => d.Description).ToList();

                        _jobObservation.DistributionId = int.Parse(PatDistributionId ?? "0");

                        _products = _distributions[_distributions.FindIndex(d => d.DistributionId == _jobObservation.DistributionId)].Products;
                        _products = _products.OrderBy(p => p.Description).ToList();

                        _operations = _distributions[_distributions.FindIndex(d => d.DistributionId == _jobObservation.DistributionId)].Operations;
                        _operations = _operations.OrderBy(o => o.Description).ToList();

                        //_jobObservation.OperationId = int.Parse(PatOperationId ?? "0");

                        if (user.UserType == 1)
                        {
                            _supervisors = await UsersService.GetUsersByUserTypeInPlantAndArea(_jobObservation.PlantId, _jobObservation.AreaId, 3, false, false);
                            _supervisors = _supervisors.OrderBy(s => s.Name).ToList();
                        }

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

                        _jobObservation.OperatorId = int.Parse(PatOperatorId ?? "0");
                    }
                    else
                    {
                        _jobObservation.PlantId = user.PlantId.HasValue ? (int)user.PlantId : 0;
                        _jobObservation.AreaId = user.AreaId.HasValue ? (int)user.AreaId : 0;


                        _areas = await AreaServices.GetAreas(_jobObservation.PlantId);
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
            _jobObservation.Operations = new List<Operation>();
            _jobObservation.OperatorId = 0;
            _jobObservation.SupervisorId = 0;
            _assychart = null;
            jobProductId = 0;
            _specifications = new List<List<string>>
            {
                new List<string>(), // Product 1
                new List<string>(), // Product 2
                new List<string>(), // Product 3
                new List<string>(), // Product 4
                new List<string>()  // Product 5
            };
            await LocalStorage.SetItemAsync("JobObs", _jobObservation);

            //SetAsCurrentJobObservation();

            _areas = await AreaServices.GetAreas(_jobObservation.PlantId);
            _areas = _areas.OrderBy(a => a.Description).ToList();

            StateHasChanged();

        }



        private async void ShowDistributions()
        {
            _jobObservation.SupervisorId = 0;
            _supervisors.Clear();
            _assychart = null;
            jobProductId = 0;
            _specifications = new();
            productSpecification = new string[5];

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

            await LocalStorage.SetItemAsync("JobObs", _jobObservation);
            //SetAsCurrentJobObservation();

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
            await JobObservationContext_OnFieldChanged();
            StateHasChanged();
        }

        private async void ShowOperations()
        {
            _assychart = null;
            _specifications = new List<List<string>> {
                new List<string>(), // Product 1
                new List<string>(), // Product 2
                new List<string>(), // Product 3
                new List<string>(), // Product 4
                new List<string>()  // Product 5
            };
            jobProductId = 0;
            productSpecification = new string[5];

            _products = _distributions[_distributions.FindIndex(d => d.DistributionId == _jobObservation.DistributionId)].Products;
            _products = _products.OrderBy(p => p.Description).ToList();

            _jobObservation.Operations = new List<Operation>();

            _operations = _distributions[_distributions.FindIndex(d => d.DistributionId == _jobObservation.DistributionId)].Operations;
            _operations = _operations.OrderBy(o => o.Description).ToList();

            _assychart = await AssychartsServices.GetAssyChartJobObservation(_jobObservation.PlantId, _jobObservation.AreaId, _jobObservation.DistributionId);
            if (_assychart != null && _assychart.ErgonomicsLevel != null)
            {
                auxErgonomicsLevel = (int)_assychart.ErgonomicsLevel;
            }

            await LocalStorage.SetItemAsync("JobObs", _jobObservation);
            //SetAsCurrentJobObservation();

            var groupedOperations = _operations
                .SelectMany(op => op.ProductName.Split('§').Select(product => new { Product = product, Operation = op }))
                .GroupBy(x => x.Product)
                .Select(g => new ProductAndStandardTime
                {
                    ProductName = g.Key,
                    StandardTime = g.Select(x => x.Operation.StandardTime).FirstOrDefault()
                })
                .ToList();

            int count = Math.Min(groupedOperations.Count, 5);
            _productAndSpecification = new ProductAndStandardTime[5];

            for (int i = 0; i < 5; i++)
            {
                
                    _productAndSpecification[i] = new ProductAndStandardTime
                    {
                        ProductName = "",
                        StandardTime = "0.00"
                    };
                
            }



            await Task.Delay(150);

            distribution = await DistributionService.GetDistributionById(_jobObservation.PlantId, _jobObservation.AreaId, _jobObservation.DistributionId);
            StateHasChanged();
        }


        private Dictionary<string, double[]> OperationTimes = new Dictionary<string, double[]>
        {
            { "CycleTime", new double[5] },
            { "WaitingTime", new double[5] }
        }; 
        private int?[] StepsNumber = new int?[5];
        private int?[] DoubleManagment = new int?[5];
        private int?[] Waiting = new int?[5];
        private string?[] CycleTimes = new string?[5] { "", "", "", "", "" };
        private string?[] WaitingTimes = new string?[5] { "", "", "", "", "" };

        private bool isTimerRunning2 = false;

        private int currentOperationIndex = 0;
        private int currentCycle = 0;

        private double previousOperationTime = 0.0;

        CreateJobObservation.Timer Timer;

        private async void NextOperation()
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

            SyncLocalStorage.SetItem("CC", currentCycle);
            await LocalStorage.SetItemAsync("CycleTimes", CycleTimes);

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
            //_ = StoreWaiting(1, currentCycle);

            StateHasChanged();
        }


        public void ChangeCycle(int cycle)
        {
            currentCycle = cycle;
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
            isTimerRunning2 = false;
            currentOperationIndex = 0;

            previousOperationTime = 0.0;
            foreach (var operationId in OperationTimes.Keys)
            {
                for (int cycle = 0; cycle < 5; cycle++)
                {
                    OperationTimes[operationId][cycle] = 0.0;
                }
            }

            try
            {
                SyncLocalStorage.SetItem("OpTimes", OperationTimes);
                SyncLocalStorage.RemoveItem("StepsNumber");
                SyncLocalStorage.RemoveItem("DblManagement");
                SyncLocalStorage.RemoveItem("Waiting");
            }
            catch (Exception e)
            {
                Console.WriteLine("Item not found or already deleted, " + e.Message);
            }

            StateHasChanged();
        }




        public void actualizarCampo()
        {
            if (currentOperationIndex < _operations.Count)
            {
                var currentOperation = _operations[currentOperationIndex];
                //OperationTimes[currentOperation.OperationId][currentCycle] = GetRandomNumber(0.1, 0.9);

                currentOperationIndex++;

                if (currentOperationIndex >= _operations.Count && currentCycle < 5)
                {
                    currentCycle++;
                    currentOperationIndex = 0;
                }

                StateHasChanged();
            }
            else
            {
                currentCycle = 1;
                currentOperationIndex = 0;
                StateHasChanged();
            }
        }

        List<bool> loadSpec = new List<bool>() { false,false,false,false,false };
        public void InitializeCycleTimes(string specification, int index)
        {
            loadSpec[index] = true;
            StateHasChanged();
            Console.WriteLine($"Spect: {specification}");
            productSpecification[index] = specification;
            _filteredOperations = new();
            //StepsNumber = new int?[5];
            //DoubleManagment = new int?[5];
            //Waiting = new int?[5]; 

            var selectedProduct = _products.FirstOrDefault(p => p.ProductId == jobProductIds[index]);

            Console.WriteLine($"Prdo: {selectedProduct.Code}");
            if (selectedProduct != null)
            {
                var op = string.IsNullOrEmpty(specification) ? _operations.FirstOrDefault(o => o.ProductName?.Contains(selectedProduct.Code) == true) :  _operations.FirstOrDefault(o => o.ProductName?.Contains(selectedProduct.Code) == true && o.NameTime.Contains(specification) == true );
                Console.WriteLine($"Op: {op?.Description}");
                if (op != null && !string.IsNullOrEmpty(op.StandardTime))
                {
                    var standardTimeDict = JsonSerializer.Deserialize<Dictionary<string, string>>(op.StandardTime);
                    if (standardTimeDict != null && standardTimeDict.ContainsKey(selectedProduct.Code))
                    {
                        var standardTimeParts = standardTimeDict[selectedProduct.Code].Split('§');

                        int indexOfSpec = string.IsNullOrEmpty(specification) ? 0 : Array.IndexOf(_specifications[index].ToArray(), specification);
                        Console.WriteLine($"IndexOfSpec: {indexOfSpec}");

                        if (decimal.TryParse(standardTimeParts[indexOfSpec], out decimal standardTimeValue))
                        {
                            var roundedStandardTime = Math.Round(standardTimeValue, 2).ToString("F2");
                            Console.WriteLine($"{specification} {selectedProduct.Code}: {roundedStandardTime}");

                            _productAndSpecification[index] = new ProductAndStandardTime
                            {
                                ProductName = selectedProduct.Code,
                                StandardTime = roundedStandardTime
                            };
                        }else
                        {
                            Console.WriteLine($"{selectedProduct.Code}: Invalid StandardTime");
                            _productAndSpecification[index] = new ProductAndStandardTime
                            {
                                ProductName = selectedProduct.Code,
                                StandardTime = "0.00"
                            };
                        }
                    }
                }
            }


            _jobObservation.ModelsSpecification = string.Join("|", productSpecification);

            SyncLocalStorage.SetItem("HoeStandardTime", hoeStandardTime);
            SyncLocalStorage.SetItem("Specifications", _specifications);
            SyncLocalStorage.SetItem("ProductSpecifications", productSpecification);
            SyncLocalStorage.SetItem("OperationSpecProduct", operationSpecProduct);
            //SyncLocalStorage.SetItem("JobObs", _jobObservation);
            //SyncLocalStorage.SetItem("OpTimes", OperationTimes);
            //SetAsCurrentJobObservation();
            loadSpec[index] = false;
            StateHasChanged();
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

        private async Task ShowSpecifications(int id_Product, int productIndex)
        {
            jobProductIds[productIndex] = id_Product;
            _specifications[productIndex] = new();

            var prodName = _products.FirstOrDefault(p => p.ProductId == jobProductIds[productIndex]);
            Console.WriteLine($"Prod select: {prodName?.Code}");

            if (prodName != null)
            {
                Operation? op = null;

                if (_jobObservation.Operations != null && _jobObservation.Operations?.Count() > 0)
                {
                    Console.WriteLine("Operacion en Job");
                    var firstOperation = _jobObservation.Operations.FirstOrDefault();
                    if (firstOperation != null)
                    {
                        op = _operations.FirstOrDefault(o => o.OperationId == firstOperation.OperationId && o.ProductName?.Split('§').Contains(prodName.Code) == true);
                    }

                   if (op == null)
                    {
                        foreach (var jobOp in _jobObservation.Operations)
                        {
                            op = _operations.FirstOrDefault(o => o.OperationId == jobOp.OperationId && o.ProductName?.Split('§').Contains(prodName.Code) == true);
                            if (op != null)
                            {
                                break;
                            }
                        }
                    }
                }

                if (op == null)
                {
                    op = _operations.FirstOrDefault(o => o.ProductName?.Split('§').Contains(prodName.Code) == true);
                }

                if (op != null && !string.IsNullOrEmpty(op.NameTime))
                {
                    Console.WriteLine($"Operacion encontrada {op.Description}");

                    operationSpecProduct[productIndex] = op.Description;

                    Dictionary<string, List<string>> NameTimeList = new Dictionary<string, List<string>>();
                    var nameTimeDict = JsonSerializer.Deserialize<Dictionary<string, string>>(op.NameTime);
                    foreach (var kvp in nameTimeDict)
                    {
                        NameTimeList[kvp.Key] = kvp.Value.Split('§').ToList();
                    }

                    var names = NameTimeList[prodName.Code];


                    for (int i = 0; i < 5; i++)
                    {
                        if (!string.IsNullOrEmpty(names[i]))
                        {
                            _specifications[productIndex].Add(names[i]);
                        }
                    }

                    var standardTimeDict = JsonSerializer.Deserialize<Dictionary<string, string>>(op.StandardTime);
                    if (standardTimeDict != null && standardTimeDict.ContainsKey(prodName.Code))
                    {
                        var standardTimeParts = standardTimeDict[prodName.Code].Split('§');

                        
                        if (decimal.TryParse(standardTimeParts[0], out decimal standardTimeValue))
                        {
                            var roundedStandardTime = Math.Round(standardTimeValue, 2).ToString("F2");
                           
                            _productAndSpecification[productIndex] = new ProductAndStandardTime
                            {
                                ProductName = prodName.Code,
                                StandardTime = roundedStandardTime
                            };
                        }
                        else
                        {
                            
                            _productAndSpecification[productIndex] = new ProductAndStandardTime
                            {
                                ProductName = prodName.Code,
                                StandardTime = "0.00"
                            };
                        }
                    }


                }
            }



            //_jobObservation.ProductId = id;
            await LocalStorage.SetItemAsync("JobProductsIds", jobProductIds);
            await LocalStorage.SetItemAsync("Specifications", _specifications);
            await LocalStorage.SetItemAsync("Products", _products);

            await LocalStorage.SetItemAsync("JobObs", _jobObservation);
            StateHasChanged();
        }

        private async void ShowPastJobObservations()
        {
            flag = true;

            if (_jobObservation.Operations?.Count() > 0)
                operation = await OperationService.GetOperationById(_jobObservation.PlantId, _jobObservation.AreaId, _jobObservation.DistributionId, (int)_jobObservation.Operations?.FirstOrDefault().OperationId);

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


        private async Task CreateNewJobObservation()
        {
            if (_jobObservation.Option == 3 && _jobObservation.Anomaly.IsNullOrEmpty())
            {
                Snackbar.Clear();
                Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                Snackbar.Add($"Write down the anomaly first", Severity.Error);
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
                    Snackbar.Add($"Operator payroll doesn't match", Severity.Error);

                    return;
                }
                if (currentImage == "")
                {
                    Snackbar.Clear();
                    Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                    Snackbar.Add($"Operator Signature is missing", Severity.Error);
                    return;
                }
            }
            //Eventual
            _jobObservation.Type = 2;
            _jobObservation.Status = 1;

            await FillJobObservationData();

            var result = await JobObservationService.CreateJobObservationWithLup(_jobObservation);

            //var result = await JobObservationService.CreateJobObservation(_jobObservation);
            if (result != null)
            {
                Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                Snackbar.Add($"Job Observation Created", Severity.Info);

                _jobObservation = result;
                _ = await GenerateChecklistAnswers();
                _ = await GenerateLups();
                _ = await GenerateOperatorSignatureImage();

                ClearJOStorage();

                NavigationManager.NavigateTo("/jobobservation");
            }
            else
                await JSRuntime.InvokeVoidAsync("alert", "Error en los datos!"); // Alert

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

        public async Task FillJobObservationData()
        {
            //_jobObservation.OperationId = 0;

            _jobObservation.OperationTimesJson = BuildOperationTimesJson();
            _jobObservation.ModelsSpecification = string.Join("|", productSpecification);
            _jobObservation.StepsNumber = string.Join("|", StepsNumber);
            _jobObservation.DoubleManagment = string.Join("|", DoubleManagment);
            _jobObservation.Waiting = string.Join("|", Waiting);
            _jobObservation.TaktTime = taktTime.ToString().Replace(",", ".");
            _jobObservation.HOEStandardTimes = hoeStandardTime.ToString().Replace(",", ".");
            _jobObservation.KpiId = kpiID;
            //_jobObservation.ProductId = jobProductId;
            _jobObservation.ProductIds = string.Join("|", jobProductIds);
            _jobObservation.ProductSpecifications = string.Join("|", productSpecification);

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
                    Console.WriteLine("Unable to parse '{0}'", hour1);
                    return;
                }

                if (DateTime.TryParseExact(hour2, $"M/d/yyyy HH:mm:ss", null, DateTimeStyles.None, out newDate2))
                {
                    Console.WriteLine(newDate2);
                }
                else
                {
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

            await TransformToSingle();

            //_jobObservation.Lup = _tempLup.Keys.ToList();
            foreach (var question in questionAnswers)
            {
                if (question.Value.Answer != "" || question.Value.Edited)
                {
                    _jobObservation.ChecklistAnswers?.Add(question.Value);
                }
            }
            await Task.CompletedTask;
        }


        void CancelCreateJobObservation()
        {
            ClearJOStorage();
            NavigationManager.NavigateTo("/jobobservation");
        }

        //Lup
        public void AddTempLup(int pillar)
        {
            if (_jobObservation.SupervisorId == 0)
            {
                Snackbar.Clear();
                Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                Snackbar.Add("First select a Supervisor", Severity.Error);
                return;
            }
            Snackbar.Clear();
            Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;

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

                        try
                        {
                            SyncLocalStorage.RemoveItem("area_ListS");
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("Item not found or already deleted", e.Message);
                        }

                        area_ListS.Clear();
                    }
                    else
                    {
                        Snackbar.Add("Error S Area is empty", Severity.Error);
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

                        try
                        {
                            SyncLocalStorage.RemoveItem("area_ListQ");
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("Item not found or already deleted", e.Message);
                        }

                        area_ListQ.Clear();
                    }
                    else
                    {
                        Snackbar.Add("Error Q Area is empty", Severity.Error);
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

                        try
                        {
                            SyncLocalStorage.RemoveItem("area_ListD");
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("Item not found or already deleted", e.Message);
                        }

                        area_ListD.Clear();
                    }
                    else
                    {
                        Snackbar.Add("Error D Area is empty", Severity.Error);
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

                        try
                        {
                            SyncLocalStorage.RemoveItem("area_ListC");
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("Item not found or already deleted", e.Message);
                        }

                        area_ListC.Clear();
                    }
                    else
                    {
                        Snackbar.Add("Error C Area is empty", Severity.Error);
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

                        try
                        {
                            SyncLocalStorage.RemoveItem("area_ListOther");
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("Item not found or already deleted", e.Message);
                        }

                        area_ListOther.Clear();
                    }
                    else
                    {

                        Snackbar.Add("Error Others Area is empty", Severity.Error);
                        return;
                    }
                    break;

                default:
                    Snackbar.Add("Invalid pillar", Severity.Error);
                    return;
            }

            User? svAux = _supervisors?.Find(u => _jobObservation.SupervisorId == u.UserId);

            foreach (Lup lupItem in lupsToAdd)
            {
                lupItem.Observer = svAux?.Name ?? "";
                lupItem.JobObservationId = 0;
                lupItem.Pillar = pillar;
                lupItem.Status = 1;
                lupItem.CreatedDate = DateTime.Now;
                lupItem.IsActive = true;

                if (!_tempLup.ContainsKey(lupItem))
                {
                    _tempLup.Add(lupItem, new List<string>());
                }
            }
            SyncLocalStorage.SetItem("LupToAdd", _tempLup.ToList());
            lup = new Lup();

            Snackbar.Add("Lup item added", Severity.Info);
        }

        private void RemoveFromList(int pilarId, int indexRemove)
        {
            switch (pilarId)
            {
                case 1:
                    area_ListS?.RemoveAt(indexRemove);
                    SyncLocalStorage.SetItem("area_ListS", area_ListS);
                    Snackbar.Add("LUP remove in Safety & Environment Pillar SECTION 3", Severity.Warning);
                    break;
                case 2:
                    area_ListQ?.RemoveAt(indexRemove);
                    SyncLocalStorage.SetItem("area_ListQ", area_ListQ);
                    Snackbar.Add("LUP remove in Quality Pillar SECTION 3", Severity.Warning);
                    break;
                case 3:
                    area_ListD?.RemoveAt(indexRemove);
                    SyncLocalStorage.SetItem("area_ListD", area_ListD);
                    Snackbar.Add("LUP remove in Delivery Pillar SECTION 3", Severity.Warning);
                    break;
                case 4:
                    area_ListC?.RemoveAt(indexRemove);
                    SyncLocalStorage.SetItem("area_ListC", area_ListC);
                    Snackbar.Add("LUP remove in Cost Pillar SECTION 3", Severity.Warning);
                    break;
                case 5:
                    area_ListOther?.RemoveAt(indexRemove);
                    SyncLocalStorage.SetItem("area_ListOther", area_ListOther);
                    Snackbar.Add("LUP remove in Other Pillar SECTION 3", Severity.Warning);
                    break;
            }

            base.StateHasChanged();
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

                SyncLocalStorage.SetItem("LupToAdd", _tempLup.ToList());
            }
            else
            {
                Console.WriteLine("El lup no existe en _tempLup.");
            }
        }


        //Past Job observation

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
                { "DistDesc", distribution.Description },
                { "OperaDesc", operation.Description },
                { "pastjobObservations", pastjobObservations },
                { "pastLup", pastLup }
            };
            var dialog = await DialogService.ShowAsync<PastJobObs_Dialog>("", parameters, dialogPastJobObservations);
            await dialog.Result;
        }


        //In progress
        private async Task SaveProgressJobObservation()
        {
            if (_jobObservation.DistributionId == new int())
            {
                Snackbar.Clear();
                Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                Snackbar.Add($"First select a distribution!", Severity.Error);
                return;
            }

            if (_jobObservation.OperatorId == new int())
            {
                Snackbar.Clear();
                Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                Snackbar.Add($"First select an operator!", Severity.Error);
                return;
            }
            if (_jobObservation.Option == 3 && _jobObservation.Anomaly.IsNullOrEmpty())
            {
                Snackbar.Clear();
                Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                Snackbar.Add($"Write down the anomaly first", Severity.Error);
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
            }

            //Eventual
            _jobObservation.Type = 2;
            _jobObservation.Status = 2;
            if (_jobObservation.Justification == "")
            {
                _jobObservation.Justification = null;
            }


            await FillJobObservationData();

            var result = await JobObservationService.CreateJobObservationWithLup(_jobObservation);
            //var result = await JobObservationService.CreateJobObservation(_jobObservation);
            if (result != null)
            {
                Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                Snackbar.Add($"Job Observation Created", Severity.Info);
                _jobObservation = result;
                _ = await GenerateChecklistAnswers();
                _ = await GenerateLups();
                _ = await GenerateOperatorSignatureImage();

                ClearJOStorage();

                NavigationManager.NavigateTo("/jobobservation");
            }
            else
                await JSRuntime.InvokeVoidAsync("alert", "Error en los datos!"); // Alert
        }

        public bool ChecklistQuestionsValidation()
        {
            foreach (var (category, section) in _checklistCategoriesAndQuestions.Select((category, section) => (category, section)))
            {
                if (category.Type == StructureType.Checklist)
                {
                    if (category.ChecklistQuestions.Count > 0)
                    {
                        foreach (var question in category.ChecklistQuestions)
                        {
                            int secNum = section;
                            if (_jobObservation.ChecklistAnswers != null && _jobObservation.ChecklistAnswers.Any(ck => ck.QuestionID == question.QuestionID))
                            {
                                ChecklistAnswer? answer = _jobObservation.ChecklistAnswers?.ToList().Find(ck => ck.QuestionID == question.QuestionID);
                                if (answer != null && string.IsNullOrEmpty(answer.Answer))
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

            return false;
        }



        //Under Review Job observation
        public async void UnderReviewJobObservation()
        {

            if (ChecklistQuestionsValidation())
            {
                return;
            }

            if (_jobObservation.DistributionId == new int())
            {
                Snackbar.Clear();
                Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                Snackbar.Add($"First select a distribution!", Severity.Error);
                return;
            }

            if (_jobObservation.OperatorId == new int())
            {
                Snackbar.Clear();
                Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                Snackbar.Add($"First select an operator!", Severity.Error);
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
            if (currentImage == "")
            {
                Snackbar.Clear();
                Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                Snackbar.Add($"Operator Signature is missing", Severity.Error);
                return;
            }


            //Eventual
            _jobObservation.Type = 2;
            _jobObservation.Status = 4;


            await FillJobObservationData();

            var result = await JobObservationService.CreateJobObservationWithLup(_jobObservation);
            //var result = await JobObservationService.CreateJobObservation(_jobObservation);
            if (result != null)
            {
                Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                Snackbar.Add($"Job Observation Created", Severity.Info);

                _jobObservation = result;
                _ = await GenerateChecklistAnswers();
                _ = await GenerateLups();
                _ = await GenerateOperatorSignatureImage();

                ClearJOStorage();

                NavigationManager.NavigateTo("/jobobservation");
            }
            else
                await JSRuntime.InvokeVoidAsync("alert", "Error en los datos!"); // Alert
        }


        //Reject Job observation
        async void Reject()
        {
            if (ChecklistQuestionsValidation())
            {
                return;
            }

            if (_jobObservation.DistributionId == new int())
            {
                Snackbar.Clear();
                Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                Snackbar.Add($"First select a distribution!", Severity.Error);
                return;
            }

            if (_jobObservation.OperatorId == new int())
            {
                Snackbar.Clear();
                Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                Snackbar.Add($"First select an operator!", Severity.Error);
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
                Snackbar.Add($"Operator's Signature is missing!", Severity.Error);
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
            if (currentImage == "")
            {
                Snackbar.Clear();
                Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                Snackbar.Add($"Operator Signature is missing", Severity.Error);
                return;
            }

            //Eventual
            _jobObservation.Type = 2;
            _jobObservation.Status = 5;


            await FillJobObservationData();

            var result = await JobObservationService.CreateJobObservationWithLup(_jobObservation);
            //var result = await JobObservationService.CreateJobObservation(_jobObservation);
            if (result != null)
            {
                Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                Snackbar.Add($"Job Observation Created", Severity.Info);

                _jobObservation = result;
                _ = await GenerateChecklistAnswers();
                _ = await GenerateLups();
                _ = await GenerateOperatorSignatureImage();
                ClearJOStorage();
                NavigationManager.NavigateTo("/jobobservation");
            }
            else
                await JSRuntime.InvokeVoidAsync("alert", "Error en los datos!"); // Alert
        }





        //Finished Job observation
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
        private DialogOptions dialogSignOptions = new() { CloseOnEscapeKey = true, MaxWidth = MaxWidth.ExtraSmall, FullWidth = true };

        public async Task SignDate()
        {
            if (ChecklistQuestionsValidation())
            {
                return;
            }

            if (_jobObservation.DistributionId == new int())
            {
                Snackbar.Clear();
                Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                Snackbar.Add($"First select a distribution!", Severity.Error);
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
                return;
            }
            if (_jobObservation.OperatorSignature == null || _jobObservation.OperatorSignature == "")
            {
                Snackbar.Clear();
                Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                Snackbar.Add($"Operator's Signature is missing!", Severity.Error);
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
            if (currentImage == "")
            {
                Snackbar.Clear();
                Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                Snackbar.Add($"Operator Signature is missing", Severity.Error);
                return;
            }


            //Eventual
            _jobObservation.Type = 2;

            _jobObservation.SsvSignature = "Signed";
            _jobObservation.Status = 6;
            endHour = DateTime.Now.TimeOfDay;

            //_jobObservation.OperationId = 0;
            _jobObservation.OperationTimesJson = BuildOperationTimesJson();
            //_jobObservation.ModelsSpecification = productSpecification;
            _jobObservation.StepsNumber = StepsNumber[0] + "|" + StepsNumber[1] + "|" + StepsNumber[2] + "|" + StepsNumber[3] + "|" + StepsNumber[4];
            _jobObservation.DoubleManagment = DoubleManagment[0] + "|" + DoubleManagment[1] + "|" + DoubleManagment[2] + "|" + DoubleManagment[3] + "|" + DoubleManagment[4];
            _jobObservation.Waiting = Waiting[0] + "|" + Waiting[1] + "|" + Waiting[2] + "|" + Waiting[3] + "|" + Waiting[4];
            _jobObservation.TaktTime = taktTime.ToString();
            _jobObservation.HOEStandardTimes = hoeStandardTime.ToString();
            _jobObservation.KpiId = kpiID;
            //_jobObservation.ProductId = jobProductId;


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

            //_jobObservation.Lup = _tempLup.Keys.ToList();
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
                _ = await GenerateLups();
                _ = await GenerateOperatorSignatureImage();
                ClearJOStorage();
                NavigationManager.NavigateTo("/jobobservation");
            }
            else
                await JSRuntime.InvokeVoidAsync("alert", "Error en los datos!"); // Alert
        }

        public int idFilter;          
        private List<SOSCodePath> listFilter = new();
        bool FilterOperation = false;

             //Guide Modal
        MudTabs guideTabs;

        //Questions and answers

        private async Task AddLupOpportunity(ChecklistAnswer item, int section, ChecklistQuestion question)
        {
            // get incidences
            Snackbar.Configuration.MaxDisplayedSnackbars = 5;
            Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
            var notGood = currentLanguage == "es-ES" ? question.NotGood : question.NotGoodEN;

            if (question.Pillars != null)
            {
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
                            SyncLocalStorage.SetItem("area_ListS", area_ListS);
                            Snackbar.Add("LUP added in Safety & Environment Pillar SECTION 3", Severity.Warning);
                            break;
                        case 2:
                            areaQ = notGood;
                            area_ListQ?.Add(lupOpportunity);
                            SyncLocalStorage.SetItem("area_ListQ", area_ListQ);
                            Snackbar.Add("LUP added in Quality Pillar SECTION 3", Severity.Warning);
                            break;
                        case 3:
                            areaD = notGood;
                            area_ListD?.Add(lupOpportunity);
                            SyncLocalStorage.SetItem("area_ListD", area_ListD);
                            Snackbar.Add("LUP added in Delivery Pillar SECTION 3", Severity.Warning);
                            break;
                        case 4:
                            areaC = notGood;
                            area_ListC?.Add(lupOpportunity);
                            SyncLocalStorage.SetItem("area_ListC", area_ListC);
                            Snackbar.Add("LUP added in Cost Pillar SECTION 3", Severity.Warning);
                            break;
                        case 5:
                            areaOther = notGood;
                            area_ListOther?.Add(lupOpportunity);
                            SyncLocalStorage.SetItem("area_ListOther", area_ListOther);
                            Snackbar.Add("LUP added in Other Pillar SECTION 3", Severity.Warning);
                            break;
                    }
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

            //SetAsCurrentJobObservation();

            item.Show = true;
            item.Edited = true;

            StateHasChanged();
            base.StateHasChanged();
        }

        private async void RemoveLupOppportunity(ChecklistAnswer item, int section, ChecklistQuestion question)
        {
            int removed = 0;
            if (question.Pillars != null)
            {
                foreach (var pillar in question.Pillars)
                {
                    switch (pillar)
                    {
                        case 1:
                            removed = area_ListS.RemoveAll(q => q.QuestionID == question.QuestionID);
                            if (removed > 0)
                            {
                                SyncLocalStorage.SetItem("area_ListS", area_ListS);
                                Snackbar.Add("LUP removed in Safety & Environment Pillar SECTION 3", Severity.Warning);
                            }
                            break;
                        case 2:
                            removed = area_ListQ.RemoveAll(q => q.QuestionID == question.QuestionID);
                            if (removed > 0)
                            {
                                SyncLocalStorage.SetItem("area_ListQ", area_ListQ);
                                Snackbar.Add("LUP removed in Quality Pillar SECTION 3", Severity.Warning);
                            }
                            break;
                        case 3:
                            removed = area_ListD.RemoveAll(q => q.QuestionID == question.QuestionID);
                            if (removed > 0)
                            {
                                SyncLocalStorage.SetItem("area_ListD", area_ListD);
                                Snackbar.Add("LUP removed in Delivery Pillar SECTION 3", Severity.Warning);
                            }
                            break;
                        case 4:
                            removed = area_ListC.RemoveAll(q => q.QuestionID == question.QuestionID);
                            if (removed > 0)
                            {
                                SyncLocalStorage.SetItem("area_ListC", area_ListC);
                                Snackbar.Add("LUP removed in Cost Pillar SECTION 3", Severity.Warning);
                            }
                            break;
                        case 5:
                            removed = area_ListOther.RemoveAll(q => q.QuestionID == question.QuestionID);
                            if (removed > 0)
                            {
                                SyncLocalStorage.SetItem("area_ListOther", area_ListOther);
                                Snackbar.Add("LUP removed in Other Pillar SECTION 3", Severity.Warning);
                            }
                            break;
                    }
                }
            }

            //SetAsCurrentJobObservation();

            StateHasChanged();
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

            if (question.Pillars != null)
            {
                foreach (var pillarId in question.Pillars)
                {
                    switch (pillarId)
                    {
                        case 1:
                            int index1 = area_ListS?.FindIndex(l => l.QuestionID == question.QuestionID && l.Opportunity.Contains(searchString)) ?? -1;
                            if (area_ListS != null && index1 != -1)
                            {
                                area_ListS[index1].Opportunity = fullEntry;
                                SyncLocalStorage.SetItem("area_ListS", area_ListS);
                                Snackbar.Add($"Commentary added to Lup in Safety & Environment", Severity.Success);
                            }
                            break;
                        case 2:
                            int index2 = area_ListQ?.FindIndex(l => l.QuestionID == question.QuestionID && l.Opportunity.Contains(searchString)) ?? -1;
                            if (area_ListQ != null && index2 != -1)
                            {
                                area_ListQ[index2].Opportunity = fullEntry;
                                SyncLocalStorage.SetItem("area_ListQ", area_ListQ);
                                Snackbar.Add($"Commentary added to Lup in Quality", Severity.Success);
                            }
                            break;
                        case 3:
                            int index3 = area_ListD?.FindIndex(l => l.QuestionID == question.QuestionID && l.Opportunity.Contains(searchString)) ?? -1;
                            if (area_ListD != null && index3 != -1)
                            {
                                area_ListD[index3].Opportunity = fullEntry;
                                SyncLocalStorage.SetItem("area_ListD", area_ListD);
                                Snackbar.Add($"Commentary added to Lup in Delivery", Severity.Success);
                            }
                            break;
                        case 4:
                            int index4 = area_ListC?.FindIndex(l => l.QuestionID == question.QuestionID && l.Opportunity.Contains(searchString)) ?? -1;
                            if (area_ListC != null && index4 != -1)
                            {
                                area_ListC[index4].Opportunity = fullEntry;
                                SyncLocalStorage.SetItem("area_ListC", area_ListC);
                                Snackbar.Add($"Commentary added to Lup in Cost", Severity.Success);
                            }
                            break;
                        case 5:
                            int index5 = area_ListOther?.FindIndex(l => l.QuestionID == question.QuestionID && l.Opportunity.Contains(searchString)) ?? -1;
                            if (area_ListOther != null && index5 != -1)
                            {
                                area_ListOther[index5].Opportunity = fullEntry;
                                SyncLocalStorage.SetItem("area_ListOther", area_ListOther);
                                Snackbar.Add($"Commentary added to Lup in Other", Severity.Success);
                            }
                            break;
                    }
                }
            }
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

                    var teno = new string[] { e.File.ContentType, MediaUri };

                    imagesFromFile[item.QuestionID].Add(e.File.Name, teno);
                    await LocalStorage.SetItemAsync("QAnsImgFF", imagesFromFile);
                }
            }

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

        private string? cameraId = null;

        private int frameCount;
        private async void GetCurrentFrame(string imageData)
        {
            if (!string.IsNullOrEmpty(imageData))
            {
                if(selectedLup != null && _tempLup.ContainsKey(selectedLup))
                {
                    var lupKey = _tempLup.FirstOrDefault(l => l.Key == selectedLup);
                    _tempLup[lupKey.Key].Add(imageData);
                    selectedLup = null;
                }
            }
            StateHasChanged();
        }

        public int lupPhotosId = 0;
        public string oportunity = "";
        public int photosPillar = 0;


        private string imageData;

        private async void GetCurrentFrameAnswer(string data)
        {
            imageData = data;

            if (!string.IsNullOrEmpty(imageData))
            {
                SelectedAnswer.capturedImages.Add(imageData);
                imagesFromCamera[SelectedAnswer.QuestionID].Add(imageData);
                await LocalStorage.SetItemAsync("QAnsImgFC", imagesFromCamera);
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

        private async Task RemoveImageAnswer(ChecklistAnswer item, int index)
        {
            if (index >= 0 && index < item.capturedImages.Count)
            {
                imagesFromCamera[item.QuestionID].Remove(item.capturedImages.ElementAt(index));
                await LocalStorage.SetItemAsync("QAnsImgFC", imagesFromCamera);
                item.capturedImages.RemoveAt(index);
            }
            base.StateHasChanged();
        }

        private async Task RemoveImageFileAnswer(ChecklistAnswer item, int index)
        {
            if (index >= 0 && index < item.MediaUris.Count)
            {
                var temp = imagesFromFile[item.QuestionID].ElementAt(index).Key;
                imagesFromFile[item.QuestionID].Remove(temp);
                await LocalStorage.SetItemAsync("QAnsImgFF", imagesFromFile);
                try
                {
                    item?.capturedImagesFiles?.RemoveAt(index);
                }
                catch(Exception ex)
                {
                    Console.WriteLine("Nutin'", ex.Message);
                }
                item?.MediaUris.RemoveAt(index);

            }
            base.StateHasChanged();
        }

        private async Task<AsyncVoidMethodBuilder> GenerateChecklistAnswers()
        {
            if (_jobObservation.ChecklistAnswers?.Count > 0)
            {
                foreach ((ChecklistAnswer question, int index) in _jobObservation.ChecklistAnswers.Select((question, index) => (question, index)))
                {
                    ChecklistAnswer answer = questionAnswers[question.QuestionID];

                    if (answer.Edited)
                    {
                        using var content = new MultipartFormDataContent();

                        if (answer.MediaUris.Count > 0)
                            for (int i = 0; i < answer.MediaUris.Count; i++)
                            {
                                if (!string.IsNullOrEmpty(answer.MediaUris[i]))
                                {
                                    var imageData = answer.MediaUris[i];
                                    string base64Data = "";
                                    if (imageData.Contains("data:image/png;base64,"))
                                    {
                                        base64Data = imageData.Replace("data:image/png;base64,", "");
                                    }
                                    else if (imageData.Contains("data:image/jpeg;base64,"))
                                    {
                                        base64Data = imageData.Replace("data:image/jpeg;base64,", "");
                                    }
                                    else if (imageData.Contains("data:image/jpg;base64,"))
                                    {
                                        base64Data = imageData.Replace("data:image/jpg;base64,", "");
                                    }
                                    else if (imageData.Contains("data:image/gif;base64,"))
                                    {
                                        base64Data = imageData.Replace("data:image/gif;base64,", "");
                                    }
                                    else if (imageData.Contains("data:image/svg+xml;base64,"))
                                    {
                                        base64Data = imageData.Replace("data:image/svg+xml;base64,", "");
                                    }

                                    if (!IsValidBase64String(base64Data))
                                    {
                                        Snackbar.Clear();
                                        Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                                        Snackbar.Add("Invalid image data", Severity.Error);
                                        continue;
                                    }

                                    var filename = imagesFromFile[answer.QuestionID].ElementAt(i);

                                    if (IsValidBase64String(base64Data))
                                    {
                                        // Convierte base64Data en bytes
                                        var imageBytes = Convert.FromBase64String(base64Data);

                                        var imageStream = new MemoryStream(imageBytes);
                                        imageStream.Position = 0;
                                        var fileContent = new StreamContent(imageStream);
                                        fileContent.Headers.ContentType = new MediaTypeHeaderValue(filename.Value[0]);

                                        content.Add(
                                            content: fileContent,
                                            name: "Files",
                                            fileName: filename.Key);

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
                        if (!string.IsNullOrEmpty(question.CommentarySV))
                            content.Add(new StringContent(question.CommentarySV), "checklistAnswer.CommentarySV");

                        if (!string.IsNullOrEmpty(question.CommentarySSV))
                            content.Add(new StringContent(question.CommentarySSV), "checklistAnswer.CommentarySSV");



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



                Lup sendLup = new Lup {
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


        private void RemoveTempImage(Lup lup, int index)
        {
            if (index >= 0 && index < _tempLup[lup].Count)
            {
                _tempLup[lup].RemoveAt(index);

                StateHasChanged();
            }
        }


        public bool visibleOperatorSignature = false;

        private string currentImage = "";

        private void HandleSignatureSaved()
        {
            currentImage = _signatureImageService.GetImage();
            SyncLocalStorage.SetItemAsString("SignatureImg", currentImage);
        }

        private void HandleClearSignature()
        {
            currentImage = "";
            SyncLocalStorage.RemoveItem("SignatureImg");
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

        private async Task JobObservationContext_OnFieldChanged()
        {
            await LocalStorage.SetItemAsync("JobObs", _jobObservation);
            //SetAsCurrentJobObservation();
        }

        private async Task UpdateOperator()
        {
            var operatorUser = operatorUsers.FirstOrDefault(p => p.UserId == _jobObservation.OperatorId);
            if (operatorUser != null)
            {
                _jobObservation.Operator = operatorUser;
            }
            await LocalStorage.SetItemAsync("JobObs", _jobObservation);
            //SetAsCurrentJobObservation();
        }

        private async Task UpdateOperation()
        {
            jobProductId = 0;
            _specifications = new List<List<string>> { new List<string>(), new List<string>(), new List<string>(), new List<string>(), new List<string>() };
            //productSpecification = string.Empty;
            //var operation = _operations.FirstOrDefault(p => p.OperationId == _jobObservation.OperationId);
            //if (operation != null)
            //{
            //    _jobObservation.Operation = operation;
            //}

            await LocalStorage.SetItemAsync("JobObs", _jobObservation);
            //SetAsCurrentJobObservation();
            StateHasChanged();
        }

        private async Task OnKPIChange(int id, int catId)
        {
            var specialEntry = _checklistCategoriesAndQuestions.First(p => p.JobCategoryStructureId == catId).ChecklistQuestions.First(p => p.CategorySequence == 6).QuestionID;
            _jobObservation.KpiId = kpiID = id;

            var Comentary = id switch { 1 => "S&P", 2 => "Q", 3 => "D", 4 => "C", 5 => "E", 6 => "Other", _ => "S&P/Q" };

            questionAnswers[specialEntry].CommentarySV = Comentary;
            questionAnswers[specialEntry].Answer = "YES";
            await JobObservationContext_OnFieldChanged(); 
        }

        private async Task OnKPIChangeByQuestion(int id, int catId)
        {
            var specialEntry = _checklistCategoriesAndQuestions.First(p => p.JobCategoryStructureId == catId).ChecklistQuestions.First(p => p.CategorySequence == 6).QuestionID;
            _jobObservation.KpiId = kpiID = id;

            questionAnswers[specialEntry].Answer = "YES";
            await JobObservationContext_OnFieldChanged();
        }

        private async Task OnStartDateChanged(DateTime? newDate)
        {
            _jobObservation.StartDate = newDate;
            await LocalStorage.SetItemAsync("JobObs", _jobObservation);
            //SetAsCurrentJobObservation();
        }

        private async Task OnEndDateChanged(DateTime? newDate)
        {
            _jobObservation.EndDate = newDate;
            await LocalStorage.SetItemAsync("JobObs", _jobObservation);
            //SetAsCurrentJobObservation();
        }

        private async Task ChangeOption(int option)
        {
            _jobObservation.Option = option;
            await LocalStorage.SetItemAsync("JobObs", _jobObservation);
            //SetAsCurrentJobObservation();
        }

        private async Task AnswerChangeOption(string option, int id, int section, int catId)
        {
            questionAnswers[id].Answer = option;
            await LocalStorage.SetItemAsync("QAns", questionAnswers);
            //SetAsCurrentJobObservation();
        }
        private async Task MultiAnswerChangeOption(IEnumerable<string> options, int id)
        {
            MultiQuestionAnswers[id] = options;
            questionAnswers[id].Answer = string.Join(",", options);
            await LocalStorage.SetItemAsync("MQAns", MultiQuestionAnswers);
            //SetAsCurrentJobObservation();
        }
        private async Task TransformToSingle()
        {
            foreach (var pair in MultiQuestionAnswers)
            {
                questionAnswers[pair.Key].Answer = string.Join(",", pair.Value);
            }
        }
        private async Task AnswerComentaryUpdate()
        {
            await LocalStorage.SetItemAsync("QAns", questionAnswers);
            //SetAsCurrentJobObservation();
        }

        private async Task ChangeTaktTime(double newTime)
        {
            taktTime = newTime;
            await LocalStorage.SetItemAsync("taktTime", taktTime);
            //SetAsCurrentJobObservation();
        }

        private async Task StoreSteps(int? value, int index)
        {
            StepsNumber[index] = value;
            await LocalStorage.SetItemAsync("StepsNumber", StepsNumber);
            //SetAsCurrentJobObservation();
        }

        private async Task StoreManagement(int? value, int index)
        {
            DoubleManagment[index] = value;
            await LocalStorage.SetItemAsync("DblManagement", DoubleManagment);
            //SetAsCurrentJobObservation();
        }

        private async Task StoreWaiting(int? value, int index)
        {
            Waiting[index] = value;
            await LocalStorage.SetItemAsync("Waiting", Waiting);
            //SetAsCurrentJobObservation();
        }


        private async Task UpdateAreaLists(int pillar)
        {
            switch (pillar)
            {
                case 1:
                    await LocalStorage.SetItemAsync("area_ListS", area_ListS);
                    break;
                case 2:
                    await LocalStorage.SetItemAsync("area_ListQ", area_ListQ);
                    break;
                case 3:
                    await LocalStorage.SetItemAsync("area_ListD", area_ListD);
                    break;
                case 4:
                    await LocalStorage.SetItemAsync("area_ListC", area_ListC);
                    break;
                case 5:
                    await LocalStorage.SetItemAsync("area_ListOther", area_ListOther);
                    break;
                default:
                    return;
            }
            //SetAsCurrentJobObservation();
        }

        public async Task AddDisabledOptions(int categoryId, ChecklistQuestion question, string value)
        {
            var cat = _checklistCategoriesAndQuestions.FirstOrDefault(p => p.JobCategoryStructureId == categoryId);
            var qst = cat?.ChecklistQuestions.FirstOrDefault(x => x.QuestionID == question.QuestionID);

            qst?.DisabledOptions.Add(value);
        }
        public async Task RemoveDisabledOptions(int categoryId, ChecklistQuestion question, string value)
        {
            var cat = _checklistCategoriesAndQuestions.FirstOrDefault(p => p.JobCategoryStructureId == categoryId);
            var qst = cat?.ChecklistQuestions.FirstOrDefault(x => x.QuestionID == question.QuestionID);

            qst?.DisabledOptions.Remove(value);
        }

        public async Task DisableEnableQuestion(int categoryId, ChecklistQuestion question, bool value)
        {
            var cat = _checklistCategoriesAndQuestions.FirstOrDefault(p => p.JobCategoryStructureId == categoryId);
            var qst = cat?.ChecklistQuestions.FirstOrDefault(x => x.QuestionID == question.QuestionID);

            qst.Disable = value;
        }

        //private void SetAsCurrentJobObservation()
        //{
        //    if (!session)
        //    {
        //        SyncSessionStorage.SetItem("CJO", session = true);
        //    }
        //}

        private bool checkForStoragedValues()
        {
            bool[] conditions = {
                SyncLocalStorage.ContainKey("JobObs")
                , SyncLocalStorage.ContainKey("OpTimes")
                , SyncLocalStorage.ContainKey("LupToAdd")
                , SyncLocalStorage.ContainKey("area_ListS")
                , SyncLocalStorage.ContainKey("area_ListQ")
                , SyncLocalStorage.ContainKey("area_ListD")
                , SyncLocalStorage.ContainKey("area_ListC")
                , SyncLocalStorage.ContainKey("area__ListOther")
                , SyncLocalStorage.ContainKey("QAnsImgFF")
                , SyncLocalStorage.ContainKey("QAnsImgFC")
                , SyncLocalStorage.ContainKey("SignatureImg")
                , SyncLocalStorage.ContainKey("QAns")
                , SyncLocalStorage.ContainKey("MQAns")
                , SyncLocalStorage.ContainKey("taktTime")
                , SyncLocalStorage.ContainKey("HoeStandardTime")
                , SyncLocalStorage.ContainKey("StepsNumber")
                , SyncLocalStorage.ContainKey("JobProductsIds")
                , SyncLocalStorage.ContainKey("DblManagement")
                , SyncLocalStorage.ContainKey("Waiting")
            };
            return conditions.Any(p => p == true);
        }

        private async Task InitializeCollectionsWithPreviousData()
        {
            if (_jobObservation.PlantId != 0)
            {
                _areas = await AreaServices.GetAreas(_jobObservation.PlantId);
                _areas = _areas.OrderBy(a => a.Description).ToList();
            }

            _supervisors = new();
            _assychart = null;
            if (user.UserType == 1)
            {
                if (_jobObservation.AreaId != 0)
                {
                    _supervisors = await UsersService.GetUsersByUserTypeInPlantAndArea(_jobObservation.PlantId, _jobObservation.AreaId, 3, false, false);
                    _supervisors = _supervisors.OrderBy(s => s.Name).ToList();
                }
            }
            else if (user.UserType == 2)
            {
                _supervisors = await UsersService.GetSubordinates(user.UserId, false);
                _supervisors = _supervisors.OrderBy(s => s.Name).ToList();

            }

            if (_jobObservation.PlantId != 0 && _jobObservation.AreaId != 0)
            {
                _distributions = await DistributionService.GetDistributionsWithCollections(_jobObservation.PlantId, _jobObservation.AreaId);
                _distributions = _distributions.OrderBy(d => d.Description).ToList();
            }

            if (_jobObservation.DistributionId != 0 && _jobObservation.Operations?.FirstOrDefault()?.OperationId != 0)
                ShowPastJobObservations();

            if (_jobObservation.SupervisorId != 0)
            {
                if (user.UserType == 1 || user.UserType == 2)
                {
                    _operators = await UsersService.GetSubordinates(_jobObservation.SupervisorId, false);
                    _operators = _operators.OrderBy(o => o.Name).ToList();
                }

                operatorUsers = new();
                //operator User
                foreach (var operatorUser in _operators)
                {
                    if (operatorUser.AreaId == _jobObservation.AreaId && operatorUser.SuperiorId == _jobObservation.SupervisorId)
                    {
                        operatorUsers.Add(operatorUser);
                    }
                }
            }

            if (_jobObservation.DistributionId != 0 && _distributions?.Count > 0)
            {
                _products = _distributions[_distributions.FindIndex(d => d.DistributionId == _jobObservation.DistributionId)].Products;
                _products = _products.OrderBy(p => p.Description).ToList();

                _operations = _distributions[_distributions.FindIndex(d => d.DistributionId == _jobObservation.DistributionId)].Operations;
                _operations = _operations.OrderBy(o => o.Description).ToList();

                var groupedOperations = _operations
                    .SelectMany(op => op.ProductName.Split('§').Select(product => new { Product = product, Operation = op }))
                    .GroupBy(x => x.Product)
                    .Select(g => new ProductAndStandardTime
                    {
                        ProductName = g.Key,
                        StandardTime = g.Select(x => x.Operation.StandardTime).FirstOrDefault()
                    })
                    .ToList();

                int count = Math.Min(groupedOperations.Count, 5);
                _productAndSpecification = new ProductAndStandardTime[5];

                for (int i = 0; i < 5; i++)
                {
                    var selectedProduct = _products.FirstOrDefault(p => p.ProductId == jobProductIds[i]);

                    var productName = selectedProduct.Code;

                    var operation = string.IsNullOrEmpty(productSpecification[i]) ? _operations.FirstOrDefault(o => o.ProductName?.Split('§').Contains(selectedProduct.Code) == true) : _operations.FirstOrDefault(o => o.ProductName?.Split('§').Contains(selectedProduct.Code) == true && o.NameTime.Contains(productSpecification[i]) == true);
                   
                    var standardTimeDict = JsonSerializer.Deserialize<Dictionary<string, string>>(operation.StandardTime);

                    var standardTimeParts = standardTimeDict[productName].Split('§');

                    int indexOfSpec = string.IsNullOrEmpty(productSpecification[i]) ? 0 : Array.IndexOf(_specifications[i].ToArray(), productSpecification[i]);

                    if (decimal.TryParse(standardTimeParts[indexOfSpec], out decimal standardTimeValue))
                    {
                        var roundedStandardTime = Math.Round(standardTimeValue, 2).ToString("F2");
                        Console.WriteLine($"{productSpecification[i]} {productName}: {roundedStandardTime}");

                        _productAndSpecification[i] = new ProductAndStandardTime
                        {
                            ProductName = productName,
                            StandardTime = roundedStandardTime
                        };
                    }
                    else
                    {
                        Console.WriteLine($"{productName}: Invalid StandardTime");
                        _productAndSpecification[i] = new ProductAndStandardTime
                        {
                            ProductName = productName,
                            StandardTime = "0.00"
                        };
                    }
                }


                if (_jobObservation.PlantId != 0 && _jobObservation.AreaId != 0)
                {
                    _assychart = await AssychartsServices.GetAssyChartJobObservation(_jobObservation.PlantId, _jobObservation.AreaId, _jobObservation.DistributionId);
                    if (_assychart != null && _assychart.ErgonomicsLevel != null)
                    {
                        auxErgonomicsLevel = (int)_assychart.ErgonomicsLevel;
                    }

                    distribution = await DistributionService.GetDistributionById(_jobObservation.PlantId, _jobObservation.AreaId, _jobObservation.DistributionId);
                }
            }

            //if (_jobObservation.ProductId != null)
            //{
            //    _specifications = new();
            //    var prodName = _products.FirstOrDefault(p => p.ProductId == jobProductId);
            //    if (prodName != null)
            //    {
            //        var op = _operations.FirstOrDefault(p => p.ProductName == prodName?.Code);
            //        if (op != null && !string.IsNullOrEmpty(op.NameTime))
            //        {
            //            var names = op.NameTime.Replace(',', '.').Split("§");
            //            for (int i = 0; i < 5; i++)
            //            {
            //                if (!string.IsNullOrEmpty(names[i]))
            //                {
            //                    _specifications.Add(names[i]);
            //                }
            //            }

            //        }
            //    }
            //}
            
            StateHasChanged();
        }

        private void ClearJOStorage()
        {
            SyncLocalStorage.RemoveItems(new string[]{ 
                "JobObs","OpTimes","LupToAdd","area_ListS","area_ListQ",
                "area_ListD","area_ListC","area_ListOther","QAnsImgFF",
                "QAnsImgFC", "SignatureImg", "QAns", "taktTime", "HoeStandardTime", "StepsNumber", "OperationSpecProduct",
                "DblManagement", "Waiting", "CC", "CJO", "JobProductsIds", "Specifications", 
                "ProductSpecifications", "Products", "CycleTimes", "MQAns"});
        }
    }
}
