using DocumentFormat.OpenXml.Vml.Spreadsheet;
using FuzzyString;
using Microsoft.JSInterop;
using MudBlazor;
using SupervisorMobility.Client.Data.Entities;
using SupervisorMobility.Client.Data.Entities.TreeStruct;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Security.Policy;
using static SupervisorMobility.Client.Pages.Inicio.JobObservationPage.CreateJobObservationNew;

namespace SupervisorMobility.Client.Pages.Inicio.JobObservationPage
{
    public partial class JobObservationDetails
    {

        [Parameter]
        public int JobObservationId { get; set; }
        public JobObservation _jobObservation { get; set; } = new();
        List<Product> _products { get; set; } = new();
        public Lup lup { get; set; } = new();
        //Lup Modal
        private bool visible = false;
        private int lupId;

        private DialogOptions dialogOptions = new() { CloseOnEscapeKey = true, MaxWidth = MaxWidth.Large, FullWidth = true };

        private AssyChart _assychart { get; set; } = new AssyChart();
        int SOSCodePathId { get; set; } = 0;
        string SosPanelOpen { get; set; } = "";
        //Glosary
        private List<Glosary> glosary = new();
        private Dictionary<string, Glosary> _glosaryInfo;

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



        private bool searchAssychart = false;

        private string messageErrorFolders;

        private List<SOSCodePath> listFilter = new();
        bool FilterOperation = false;

        //Operator user
        public List<User> users = new();
        public List<User> operatorUsers = new();


        //Edit Date
        TimeSpan? changeStartHour { get; set; }
        TimeSpan? changeEndHour { get; set; }

        string cycle1Color = "";
        string cycle2Color = "";
        string cycle3Color = "";
        string cycle4Color = "";
        string cycle5Color = "";

        public string[] questions = new string[5];

        public double taktTime { get; set; }
        public double hoeStandardTime { get; set; }
        public int kpiID = 0;
        public int auxErgonomicsLevel = 0;

        public List<JobCategoryStructure> _checklistCategoriesAndQuestions { get; set; } = new();
        public List<ChecklistAnswer> _checklistAnswers { get; set; } = new();
        private Dictionary<int, string> questionResponses = new Dictionary<int, string>();

        private Dictionary<int, ChecklistAnswer> questionAnswers = new Dictionary<int, ChecklistAnswer>();
        Dictionary<int, string> imageUrls = new Dictionary<int, string>();

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

        public int jobProductId = 0;

        List<Lup> SSV_LupList = new();
        List<Distribution> _distributions = new();
        List<Operation> _operations = new();
        List<Operation> _filteredOperations = new();

        bool showLoading = true;
        private string currentImage = "";
        string currentLanguage = "es-ES";
        bool NoData { get; set; } = false;
        private IList<string> _sourceMsgLoading = new List<string>();
        private IList<Color> _Colors = new List<Color>() { Color.Default, Color.Primary, Color.Secondary, Color.Success, Color.Info, Color.Default, Color.Primary, Color.Secondary, Color.Success, Color.Info };


        private bool if_pick_Distribution = false;
        private int productId = 0;
        public int idFilter;

        //Show Photo
        private DialogOptions dialogPhotoOptions = new() { CloseOnEscapeKey = true, MaxWidth = MaxWidth.Medium, FullWidth = true, CloseButton = true };

        private bool visiblePhoto = false;

        private int?[] ConvertStringToArray(string stringValue) =>
            string.IsNullOrEmpty(stringValue)
                ? new int?[5]
                : stringValue.Split('|')
                    .Select(s => int.TryParse(s, out var result) ? (int?)result : null)
                    .ToArray();

        bool CodePathModalDisplay = false;

        private int photoIndex = 0;
        ChecklistAnswer SelectedAnswer { get; set; }



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
            _jobObservation.Supervisor = new();
            _jobObservation.Operator = new();

            glosary = await GlosaryService.GetGlosary();
            _glosaryInfo = glosary.ToDictionary(x => x.Name, x => x);

            _jobObservation = await JobObservationService.GetJobObservationById(JobObservationId, true, true, true, includeCkAnswers: true);

            StateHasChanged();
            if (_jobObservation != null)
            {
                //_jobObservation = await JobObservationService.GetJobObservationById(JobObservationId);
                _products = await ProductService.GetProducts();

                _distributions = await DistributionService.GetDistributionsWithCollections(_jobObservation.PlantId, _jobObservation.AreaId);
                _distributions = _distributions.OrderBy(d => d.Description).ToList();
                _operations = _distributions[_distributions.FindIndex(d => d.DistributionId == _jobObservation.DistributionId)].Operations;
                _operations = _operations.OrderBy(o => o.Description).ToList();

                if (_operations == null)
                {
                    _operations = new List<Operation>();
                }

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


                _checklistCategoriesAndQuestions = await JobStructureCategoriesService.GetChecklistCategories(true);
                _checklistAnswers = await ChecklistAnswerServices.GetAllChecklistAnswersByJobObservationId(JobObservationId);

                //jobProductId = _jobObservation.ProductId != null ? (int)_jobObservation.ProductId : 0;

                SSV_LupList = _jobObservation.Lup.Where(l => !l.EndDate.HasValue).ToList();


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
                            if (_operations != null && _jobObservation.Operations != null)
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
                }


                StepsNumber = ConvertStringToArray(_jobObservation?.StepsNumber);
                DoubleManagment = ConvertStringToArray(_jobObservation?.DoubleManagment);
                Waiting = ConvertStringToArray(_jobObservation?.Waiting);

                foreach (var category in _checklistCategoriesAndQuestions)
                {
                    foreach (var question in category.ChecklistQuestions)
                    {
                        if (_jobObservation.ChecklistAnswers.Any(cka => cka.QuestionID == question.QuestionID))
                        {
                            var item = _jobObservation.ChecklistAnswers.ToList().Find(cka => cka.QuestionID == question.QuestionID);
                            if (item.Evidences.Count > 0)
                            {
                                item.Show = true;
                                foreach (var evidence in item.Evidences)
                                {
                                    var imageUrl = await FilesServices.ShowImageEvidence(evidence.FileUploadId);
                                    imageUrls[evidence.FileUploadId] = imageUrl;

                                }
                            }

                        }
                        else
                        {
                            ChecklistAnswer newChAnswer = new();
                            newChAnswer.JobObservationId = _jobObservation.JobObservationId;
                            newChAnswer.QuestionID = question.QuestionID;
                            newChAnswer.Prompt = question.Prompt;
                            questionAnswers.Add(question.QuestionID, newChAnswer);
                        }
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

                if (!string.IsNullOrEmpty(_jobObservation.OperationTimesJson) && _jobObservation.OperationTimesJson != "||||")
                {
                    var operationTimes = JsonSerializer.Deserialize<Dictionary<string, double[]>>(_jobObservation.OperationTimesJson);
                    if (operationTimes != null && operationTimes.ContainsKey("CycleTime") && operationTimes.ContainsKey("WaitingTime"))
                    {
                        for (int i = 0; i < 5; i++)
                        {
                            if (i < operationTimes["CycleTime"].Length)
                            {
                                CycleTimes[i] = operationTimes["CycleTime"][i].ToString();
                            }
                            else
                            {
                                CycleTimes[i] = "0";
                            }

                            // Accede a los valores de WaitingTime
                            if (i < operationTimes["WaitingTime"].Length)
                            {
                                WaitingTimes[i] = operationTimes["WaitingTime"][i].ToString();
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


                showLoading = false;

                if (_jobObservation.SignatureImage != null && _jobObservation.SignatureImage.ContentType == "image/png")
                {
                    var imageUrl = await FilesServices.ShowOperatorSignature(_jobObservation.SignatureImage.FileUploadId);
                    currentImage = imageUrl;
                }

                StateHasChanged();

                startHour = _jobObservation.StartDate?.TimeOfDay;
                endHour = _jobObservation.EndDate?.TimeOfDay;

                changeStartHour = _jobObservation.PlannedStartDate?.TimeOfDay;
                changeEndHour = _jobObservation.PlannedEndDate?.TimeOfDay;

                if (_jobObservation.PlantId != 0)
                {
                    if (_jobObservation.AreaId != 0)
                    {
                        if (_jobObservation.DistributionId != 0)
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
                                messageErrorFolders = Localizer["jobObservationDoesNotContainAValidOperation"];
                            }
                        }
                        else
                        {
                            messageErrorFolders = Localizer["jobObservationDoesNotContainAValidDistribution"];
                        }
                    }
                    else
                    {
                        messageErrorFolders = Localizer["jobObservationDoesNotContainAValidArea"];
                    }
                }
                else
                {
                    messageErrorFolders = Localizer["jobObservationDoesNotContainAValidPlant"];
                }

                if (searchAssychart && _assychart.RoutesProductsAssyChart?.Count() > 0 && _jobObservation.Operations?.Count() > 0)
                {
                    var firstOperation = _jobObservation.Operations.FirstOrDefault();
                    if (firstOperation?.Code != null)
                    {
                        listFilter = _assychart.RoutesProductsAssyChart
                            .Where(r => r.Code.ToLower().Contains(firstOperation.Code.ToLower(), StringComparison.OrdinalIgnoreCase))
                            .ToList();
                        FilterOperation = true;
                    }
                    else
                    {
                        listFilter = new List<SOSCodePath>();
                        FilterOperation = false;
                    }
                }
            }
            else
            {
                NoData = true;
            }

        }//end on inizialized 

        void history()
        {
            NavigationManager.NavigateTo($"jobobservation/history/{JobObservationId}");
        }
        void Closed(MudChip chip)
        {
            // react to chip closed
        }

        private async Task<AsyncVoidMethodBuilder> OpenDialogCodePath(SOSCodePath itemselected, int panelSelect)
        {

            showLoading = true;
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


        private async void CloseModalFiles()
        {
            CodePathModalDisplay = false;

            StateHasChanged();

        }


        private void OpenPhotoDialog(int index, ChecklistAnswer item)
        {
            SelectedAnswer = item;
            photoIndex = index;
            visiblePhoto = true;

        }

        private async Task DownloadFile(int fileId, string filename)
        {
            await FilesServices.DownloadFileEvidence(fileId, filename);
        }
    }
}
