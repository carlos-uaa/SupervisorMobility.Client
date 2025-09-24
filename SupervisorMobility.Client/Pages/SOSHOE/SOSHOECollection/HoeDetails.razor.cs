using BlazorCameraStreamer;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.JSInterop;
using MudBlazor;
using SupervisorMobility.Client.Data.Entities;
using System.Globalization;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;

namespace SupervisorMobility.Client.Pages.SOSHOE.SOSHOECollection
{
    public partial class HoeDetails
    {
        bool Dev_env { get; set; }

        #region Variables
        [Parameter]
        public int SOSHubId { get; set; }

        private SOSHub _sosHub = new();

        private List<BreadcrumbItem> _links;
        private System.DateTime? createdDateTime = System.DateTime.Now;
        private System.DateTime? modifiedDateTime = System.DateTime.Now;

        //User
        private string json = string.Empty;
        public User user = new();
        public bool logged = false;

        public int userType = 0;
        public string productSide = string.Empty;

        private List<Segment> segments = new List<Segment>();
        private List<Product> _products = new List<Product>();

        List<Plant> _plants { get; set; } = new();
        List<Area> _areas = new();
        List<Distribution> _distributions = new();
        List<Department> _departments = new();
        List<Station> _stations { get; set; } = new();
        int plantId = 0;
        int areaId = 0;
        int distributionId = 0;
        int departmentId = 0;
        int stationId = 0;


        private string HighlightedText;

        PAT _pat = new();
        public int ssvId;

        public int productId = 0;
        public class Segment
        {
            public string Analysis { get; set; }
            public string MainPoint { get; set; }
            public List<string> CriticalPoints { get; set; } = new List<string>();
        }

        private bool visibleStepsDialog = false;
        private bool visibleImagesDialog = false;
        private bool visibleVideosDialog = false;


        private DialogOptions dialogStepsOptions = new() { CloseOnEscapeKey = false, MaxWidth = MaxWidth.Medium, FullWidth = true, DisableBackdropClick = true, CloseButton = true };
        private DialogOptions dialogImagesOptions = new() { CloseOnEscapeKey = false, MaxWidth = MaxWidth.Medium, FullWidth = true, DisableBackdropClick = true, CloseButton = true };
        private DialogOptions dialogVideosOptions = new() { CloseOnEscapeKey = false, MaxWidth = MaxWidth.Medium, FullWidth = true, DisableBackdropClick = true, CloseButton = true };


        //Videos
        public Dictionary<string, (string, string)> MediaUris = new Dictionary<string, (string, string)>();

        private class FileToDisplay
        {
            public string name { get; set; }
            public string ftype { get; set; }
            public string message { get; set; }
        }



        //Users
        List<User> _supervisors { get; set; } = new();
        Dictionary<int, List<User>> _operators { get; set; } = new();


        private readonly List<int> Cycles = Enumerable.Range(1, 3000).ToList();
        int cycleId = 0;


        //Show Evidence 
        private DialogOptions dialogEvidenceOptions = new() { CloseOnEscapeKey = true, MaxWidth = MaxWidth.Medium, FullWidth = true, CloseButton = true };

        private bool visibleEvidence = false;

        private int photoIndex = 0;
        private void OpenEvidenceDialog(int index)
        {
            photoIndex = index;
            visibleEvidence = true;

        }


        //Tools, materials and equipment
        int analysisTabsIndex = 0;


        //Analysis
        [Inject]
        private IDialogService DialogService { get; set; }
        public List<AnalysisBkup> RawAnalisis { get; set; } = new List<AnalysisBkup>();

        private IEnumerable<string> _selectedValues = new List<string>();


        public string stepName { get; set; } = "";
        bool showAddStepDialog = false;

        public List<Object> Documents { get; set; } = new List<Object>();


        #endregion
        private IList<string> _sourceMsgLoading = new List<string>();
        private IList<Color> _Colors = new List<Color>() { Color.Default, Color.Primary, Color.Secondary, Color.Success, Color.Info, Color.Default, Color.Primary, Color.Secondary, Color.Success, Color.Info };
        public bool ShowLoading = true;
        protected async override Task OnInitializedAsync()
        {
            Dev_env = Environment.IsDevelopment();

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

            _links = new List<BreadcrumbItem>
                {
                    new BreadcrumbItem(text: Localizer["home"], href: "/soshoe"),
                    new BreadcrumbItem(text: Localizer["hoe"], href: "/soshoe/Hub"),
                    new BreadcrumbItem(text: Localizer["details"] + " Collection: " + SOSHubId, href: "", disabled: true)
                };
            BreadcrumbServices.UpdateBreadcrumbs(_links);
            await GetUserAsync();
            logged = await HasPropertyAsync();
            if (!logged)
            {
                Snackbar.Clear();
                Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                Snackbar.Add($"Error You have to log in", Severity.Error);
                NavigationManager.NavigateTo($"/");
            }



            await SetUserInfo();

            if (_sosHub.Folio.Contains("-L-"))
            {
                productSide = "L";
            }
            else if (_sosHub.Folio.Contains("-R-"))
            {
                productSide = "R";
            }

            ShowLoading = false;
            StateHasChanged();
        }



        #region Initialize SOSHUB

        public async Task<AsyncVoidMethodBuilder> SetUserInfo()
        {
            _products = await ProductsServices.GetProducts();

            _plants = await PlantServices.GetPlants();
            _plants = _plants.OrderBy(p => p.Description).ToList();

            _departments = await DepartmentServices.GetDepartments();
            _departments = _departments.OrderBy(d => d.Description).ToList();
            _stations = await StationServices.GetStations();
            _stations = _stations.OrderBy(s => s.Description).ToList();
            StateHasChanged();

            //Subhub get
            _sosHub = await SOSHubServices.GetSOSHub(SOSHubId, true, true, true, true, true, true, true, true, includeModel: true, includePeople: true, includeDocuments: true, includeCollections: true, includePats: true);


            if (_sosHub.AnalysesBkup != null && _sosHub.AnalysesBkup.Count > 0)
            {
                var listTextSections = _sosHub.Sections.SelectMany(section => section.Analyses).Select(analysis => analysis.Text).ToList();

                foreach (var backup in _sosHub.AnalysesBkup)
                {
                    if (!listTextSections.Contains(backup.Text))
                    {
                        RawAnalisis.Add(backup);
                    }
                }
            }


            if (_sosHub.Images != null && _sosHub.Images.Count > 0)
            {

                foreach (var sosImage in _sosHub.Images)
                {
                    var image = await SOSHubServices.ShowImageSosHub(sosImage.FileUploadId);
                    capturedImages.Add(image);
                }
            }

            if (_sosHub.Videos != null && _sosHub.Videos.Count > 0)
            {
                foreach (var sosVideo in _sosHub.Videos)
                {
                    var video = await SOSHubServices.ShowVideoSosHub(sosVideo.FileUploadId);
                    MediaUris.Add(sosVideo.FileUploadId.ToString(), (sosVideo.FileName, video));
                }
            }


            if (_sosHub.PlantId != null)
            {
                plantId = (int)_sosHub.PlantId;
                areaId = _sosHub.AreaId ?? areaId;
                distributionId = _sosHub.DistributionId ?? distributionId;
            }



            switch (user.UserType)
            {
                case 1:
                    if (plantId != new int())
                    {
                        _areas = await AreaServices.GetAreas(plantId);
                        _areas = _areas.OrderBy(a => a.Description).ToList();
                    }
                    _supervisors = await UsersService.GetUsersByUserTypeInPlant(plantId, 3, false, false);
                    _supervisors = _supervisors.OrderBy(s => s.Name).ToList();
                    break;
                case 2:
                    _areas = user.Areas.ToList();
                    foreach (var sv in user.Subordinates.ToList())
                    {
                        _supervisors.Add(sv);
                    }
                    break;
                case 3:
                    _areas = await AreaServices.GetAreas(plantId);
                    _areas = _areas.OrderBy(a => a.Description).ToList();

                    _supervisors.Add(user);
                    break;
            }



            if (plantId != 0 && areaId != 0)
            {
                _distributions = await DistributionServices.GetDistributionsWithCollections(plantId, areaId);
                _distributions = _distributions.OrderBy(d => d.Description).ToList();
            }

            stationId = _sosHub.StationId ?? stationId;
            departmentId = _sosHub.DepartmentId ?? departmentId;


            cycleId = _sosHub.TrainingTime ?? 0;


            //_sosHub.AppliedModel = _products.Find(p => p.ProductId == _sosHub.AppliedModelId);

            foreach (var analysis in _sosHub.SOSAnalysis)
            {
                Documents.Add(analysis);
            }

            foreach (var combination in _sosHub.SOSCombination)
            {
                Documents.Add(combination);
            }

            foreach (var distribution in _sosHub.SOSDistribution)
            {
                if (distribution.SOSHubId == _sosHub.SOSHubId) Documents.Add(distribution);
            }

            foreach (var flow in _sosHub.SOSFlow)
            {
                Documents.Add(flow);
            }

            foreach (var sequence in _sosHub.SOSSequence)
            {
                Documents.Add(sequence);
            }

            foreach (var pat in _sosHub.PATs)
            {
                Documents.Add(pat);
            }

            if (_sosHub.Hci != null)
            {
                Documents.Add(_sosHub.Hci);
            }

            StateHasChanged();
            //faltan a�adir los diagramas

            return new AsyncVoidMethodBuilder();
        }

        private void DownloadDocument(CommonDirection document)
        {
            switch (document.type)
            {
                case 1:
                    CDMSServices.GetDownloadLinkGOS(document.DOC_ID, document.name); break;
                case 2:
                    CDMSServices.GetDownloadLinkCCP(document.DOC_ID, document.name); break;
                default:
                    //fail mesage
                    break;
            }
        }

        public static int GetCycleId(string trainingTime)
        {
            string cycleIdString = trainingTime.Split(' ').First();

            if (int.TryParse(cycleIdString, out int cycleId))
            {
                return cycleId;
            }
            else
            {
                return 0;
            }
        }


        #endregion

        //Local storage user
        #region LocalStorageUser
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

        #endregion

        //Analize text and steps
        #region Steps

        public void ShowStepsDialog()
        {
            visibleStepsDialog = true;
        }

        void CloseStepsDialog()
        {
            visibleStepsDialog = false;
        }

        #endregion

        //Camera
        #region Camera

        private List<string> capturedImages = new List<string>();

        private int imageIndex = 0;

        private CameraStreamer CameraStreamerReference;

        private string imageData;


        private async void Stop()
        {
            await CameraStreamerReference.StopAsync();
        }


        public void ShowImagesDialog()
        {
            visibleImagesDialog = true;
        }

        void CloseImagesDialog()
        {
            visibleImagesDialog = false;
        }

        #endregion


        //Videos Add and delete
        #region Videos
        public void ShowVideosDialog()
        {
            visibleVideosDialog = true;
        }

        void CloseVideosDialog()
        {
            visibleVideosDialog = false;
        }


        #endregion

        //Training Time Cycles
        #region TrainingTime

        private string ReturnCycles(int cycle)
        {
            if (cycle == 1)
            {
                return $"{cycle} cycle";
            }
            else
            {
                return $"{cycle} cycles";
            }
        }


        private Task<IEnumerable<int>> SearchSupervisors(string searchString)
        {
            IEnumerable<int> result;

            if (string.IsNullOrEmpty(searchString))
            {
                result = _supervisors.Select(x => x.UserId);
            }
            else
            {
                result = _supervisors
                    .Where(x => x.Name.Contains(searchString, StringComparison.InvariantCultureIgnoreCase))
                    .Select(x => x.UserId);
            }

            return Task.FromResult(result);
        }

        private Task<IEnumerable<int>> SearchApproverOwners(string searchString)
        {
            IEnumerable<int> result;

            if (string.IsNullOrEmpty(searchString))
            {
                result = _sosHub.ApproverOwners?.Select(x => x.UserId);
            }
            else
            {
                result = _sosHub.ApproverOwners?
                    .Where(x => x.Name.Contains(searchString, StringComparison.InvariantCultureIgnoreCase))
                    .Select(x => x.UserId);
            }

            return Task.FromResult(result);
        }
        private Task<IEnumerable<int>> SearchReviewerEditors(string searchString)
        {
            IEnumerable<int> result;

            if (string.IsNullOrEmpty(searchString))
            {
                result = _sosHub.ReviewerEditors?.Select(x => x.UserId);
            }
            else
            {
                result = _sosHub.ReviewerEditors?
                    .Where(x => x.Name.Contains(searchString, StringComparison.InvariantCultureIgnoreCase))
                    .Select(x => x.UserId);
            }

            return Task.FromResult(result);
        }



        int OperatorTurn1 { get; set; } = 0;
        int OperatorTurn2 { get; set; } = 0;
        int OperatorTurn3 { get; set; } = 0;
        int SupervisorTurn1 { get; set; } = 0;
        int SupervisorTurn2 { get; set; } = 0;
        int SupervisorTurn3 { get; set; } = 0;


        private async void ShowOperators(int value, int TurnId)
        {

            switch (TurnId)
            {
                case 0:
                    SupervisorTurn1 = value;
                    if (value == 0)
                    {
                        OperatorTurn1 = 0;
                    }
                    break;
                case 1:
                    SupervisorTurn2 = value;
                    if (value == 0)
                    {
                        OperatorTurn2 = 0;
                    }
                    break;
                case 2:
                    SupervisorTurn3 = value;
                    if (value == 0)
                    {
                        OperatorTurn3 = 0;
                    }
                    break;
            }


            if (user.UserType == 1)
            {
                List<User> _oper = await UsersService.GetUsersByUserTypeInPlantAndArea(plantId, areaId, 4, false, false);
                _oper = _oper.OrderBy(s => s.Name).ToList();

                if (_operators.ContainsKey(TurnId))
                {
                    _operators[TurnId] = _oper;
                }
                else
                {
                    _operators.Add(TurnId, _oper);
                }

            }
            else if (user.UserType == 2)
            {

                List<User> _oper = new();
                switch (TurnId)
                {
                    case 0:
                        _oper = await UsersService.GetSubordinates(SupervisorTurn1, false);
                        break;
                    case 1:
                        _oper = await UsersService.GetSubordinates(SupervisorTurn2, false);
                        break;
                    case 2:
                        _oper = await UsersService.GetSubordinates(SupervisorTurn3, false);
                        break;
                }

                _oper = _oper.OrderBy(s => s.Name).ToList();

                if (_operators.ContainsKey(TurnId))
                {
                    _operators[TurnId] = _oper;
                }
                else
                {
                    _operators.Add(TurnId, _oper);
                }
            }

            StateHasChanged();
        }

        private Task<IEnumerable<int>> SearchOperator(string searchString, int turn)
        {
            IEnumerable<int> result;

            if (string.IsNullOrEmpty(searchString))
            {
                result = _operators[turn].Select(x => x.UserId);
            }
            else
            {
                result = _operators[turn]
                    .Where(x => x.Name.Contains(searchString, StringComparison.InvariantCultureIgnoreCase))
                    .Select(x => x.UserId);
            }

            return Task.FromResult(result);
        }



        private async Task<IEnumerable<int>> SearchCycles(string searchString)
        {
            if (string.IsNullOrEmpty(searchString) || !int.TryParse(searchString, out var cycle))
            {
                return Cycles;
            }
            else
            {
                return Cycles.Where(h => h == cycle);
            }
        }

        #endregion


        //Analysis Steps Critical Points
        #region Analysis


        private string GetFormatedAnalisisText(int sectionIndex, int analisisIndex)
        {
            string BaseText = Regex.Replace(_sosHub.Sections[sectionIndex].Analyses[analisisIndex].Text, @"\*", "").ToString();

            return BaseText;
        }


        private MarkupString GenerateHighlightedText(string text, List<string> criticalPoints)
        {
            if (string.IsNullOrEmpty(text) || criticalPoints == null || criticalPoints.Count == 0)
            {
                return new MarkupString(text);
            }

            var normalizedText = Normalize(text);
            var builder = new StringBuilder();
            var currentIndex = 0;

            foreach (var criticalPoint in criticalPoints)
            {
                var normalizedCriticalPoint = Normalize(criticalPoint);
                var match = Regex.Match(normalizedText, Regex.Escape(normalizedCriticalPoint), RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);

                if (match.Success)
                {
                    var startIndex = match.Index;
                    var endIndex = startIndex + criticalPoint.Length;

                    // Agregar el texto normal antes del punto cr�tico
                    builder.Append(text.Substring(currentIndex, startIndex - currentIndex));

                    // Agregar el punto cr�tico resaltado
                    builder.Append($"<mark>{text.Substring(startIndex, endIndex - startIndex)}</mark>");

                    currentIndex = endIndex;
                }
            }

            // Agregar el texto normal despu�s del �ltimo punto cr�tico
            builder.Append(text.Substring(currentIndex));

            return new MarkupString(builder.ToString());
        }

        private static string Normalize(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return string.Empty;
            }

            return input.Normalize(NormalizationForm.FormD).Where(c => CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark).Aggregate(new StringBuilder(), (sb, c) => sb.Append(c)).ToString().ToLowerInvariant();
        }



        public static string ReasonFormat(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return input;
            }

            if (!input.StartsWith("("))
            {
                input = "(" + input;
            }

            if (!input.EndsWith(")"))
            {
                input = input + ")";
            }

            return input;
        }


        private void CloseStepDialog()
        {
            showAddStepDialog = false;
        }

        #endregion

        void HoeHistory()
        {
            NavigationManager.NavigateTo($"/soshoe/Hub/Details/{SOSHubId}/History");
        }

        void UpdateHoe(int HoeId)
        {
            NavigationManager.NavigateTo($"/soshoe/Hub/Update/{HoeId}");
        }

        //Generate Documents
        #region Generate Documents
        public bool ShowGenerateDialog = false;
        private int selectedIndexPageGenerate = 0;
        private DialogOptions dialogPagesOptions = new() { CloseOnEscapeKey = true, MaxWidth = MaxWidth.Large, FullWidth = true, CloseButton = true };
        public async void GoToPageGenerate(int indexPage)
        {
            selectedIndexPageGenerate = indexPage;
            ShowGenerateDialog = false;

            _sosHub.Plant = _plants.FirstOrDefault(p => p.PlantId == plantId);
            _sosHub.Area = _areas.FirstOrDefault(a => a.AreaId == areaId);
            _sosHub.Distribution = _distributions.FirstOrDefault(d => d.DistributionId == distributionId);
            _sosHub.Department = _departments.FirstOrDefault(d => d.DepartmentId == departmentId);
            _sosHub.Station = _stations.FirstOrDefault(s => s.StationId == stationId);


            var parameters = new DialogParameters { { "selectedIndexPageGenerate", indexPage }, { "user", user }, { "SOSHubId", SOSHubId },
                { "_sosHub", _sosHub }, { "_supervisors", _supervisors }, { "_plants", _plants }, { "_areas", _areas }, { "_distributions", _distributions }, { "_departments", _departments },
                { "_stations", _stations }
            };

            var dialog = await DialogService.ShowAsync<GeneralComponents.GenerateComponent>("", parameters, dialogPagesOptions);
            var result = await dialog.Result;

        }

        #endregion

        #region DocumentsFunctionsSecction
        public void Details<T>(int id) where T : class
        {
            if (typeof(T) == typeof(SOSAnalysis))
            {
                NavigationManager.NavigateTo($"/soshoe/Analysis/Details/{id}");
            }
            else if (typeof(T) == typeof(SOSCombination))
            {
                NavigationManager.NavigateTo($"/soshoe/Combination/Details/{id}");
            }
            else if (typeof(T) == typeof(SOSDistribution))
            {
                NavigationManager.NavigateTo($"/soshoe/Distribution/Details/{id}");
            }
            else if (typeof(T) == typeof(SOSFlow))
            {
                NavigationManager.NavigateTo($"/soshoe/Flow/Details/{id}");
            }
            else if (typeof(T) == typeof(SOSSequence))
            {
                NavigationManager.NavigateTo($"/soshoe/Sequence/Details/{id}");
            }
            else if (typeof(T) == typeof(PAT))
            {
                NavigationManager.NavigateTo($"/PAT/{id}");
            }
            else if (typeof(T) == typeof(HCI))
            {
                NavigationManager.NavigateTo($"/HCI/Details/{id}");
            }
            else if (typeof(T) == typeof(SOSSynopticTableofControlPoints))
            {
                NavigationManager.NavigateTo($"/soshoe/SynopticControlPoints/Details/{id}");
            }
            else if (typeof(T) == typeof(SOSSynopticTableofOperatingRequirements))
            {
                NavigationManager.NavigateTo($"/soshoe/SynopticRequirements/Details/{id}");
            }
            // A�adir m�s casos seg�n sea necesario
        }

        public void Update<T>(int id) where T : class
        {
            if (typeof(T) == typeof(SOSAnalysis))
            {
                NavigationManager.NavigateTo($"/soshoe/Analysis/Update/{id}");
            }
            else if (typeof(T) == typeof(SOSCombination))
            {
                NavigationManager.NavigateTo($"/soshoe/Combination/Update/{id}");
            }
            else if (typeof(T) == typeof(SOSDistribution))
            {
                NavigationManager.NavigateTo($"/soshoe/Distribution/Update/{id}");
            }
            else if (typeof(T) == typeof(SOSFlow))
            {
                NavigationManager.NavigateTo($"/soshoe/Flow/Update/{id}");
            }
            else if (typeof(T) == typeof(SOSSequence))
            {
                NavigationManager.NavigateTo($"/soshoe/Sequence/Update/{id}");
            }
            else if (typeof(T) == typeof(PAT))
            {
                NavigationManager.NavigateTo($"/PAT/Update/{id}");
            }
            else if (typeof(T) == typeof(HCI))
            {
                NavigationManager.NavigateTo($"/HCI/update/{id}");
            }
            else if (typeof(T) == typeof(SOSSynopticTableofControlPoints))
            {
                NavigationManager.NavigateTo($"/soshoe/SynopticControlPoints/Update/{id}");
            }
            else if (typeof(T) == typeof(SOSSynopticTableofOperatingRequirements))
            {
                NavigationManager.NavigateTo($"/soshoe/SynopticRequirements/Update/{id}");
            }
            // A�adir m�s casos seg�n sea necesario
        }

        private MudMessageBox _DeleteAnalysis;
        private MudMessageBox _DeleteCombination;
        private MudMessageBox _DeleteDistribution;
        private MudMessageBox _DeleteFlow;
        private MudMessageBox _DeleteSequence;
        private MudMessageBox _DeletePat;

        public async void Delete<T>(int id) where T : class
        {
            if (typeof(T) == typeof(SOSAnalysis))
            {
                bool? result = await _DeleteAnalysis.Show();
                var confirm = result is null ? "Canceled" : "Deleted!";
                StateHasChanged();

                if (confirm == "Deleted!")
                {
                    bool res = await SOSAnalysisServices.DeleteSOSAnalysis(id);
                    if (res)
                    {

                        _sosHub.SOSAnalysis.RemoveAll(Analysis => Analysis.SOSAnalysisId == id);
                        Documents.RemoveAll(doc => doc is SOSAnalysis analysis && analysis.SOSAnalysisId == id);


                        Snackbar.Clear();
                        Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                        Snackbar.Add($"{Localizer["succesRemoveAnalysis"]}", Severity.Info);
                    }

                }
            }
            else if (typeof(T) == typeof(SOSCombination))
            {
                bool? result = await _DeleteCombination.Show();
                var confirm = result is null ? "Canceled" : "Deleted!";
                StateHasChanged();

                if (confirm == "Deleted!")
                {

                    bool res = await SOSCombinationServices.DeleteSOSCombination(id);
                    if (res)
                    {
                        _sosHub.SOSCombination.RemoveAll(Combination => Combination.SOSCombinationId == id);

                        Documents.RemoveAll(doc => doc is SOSFlow Flow && Flow.SOSFlowId == id);

                        Snackbar.Clear();
                        Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                        Snackbar.Add($"{Localizer["succesRemoveCombination"]}", Severity.Info);
                    }
                }
            }
            else if (typeof(T) == typeof(SOSDistribution))
            {
                bool? result = await _DeleteDistribution.Show();
                var confirm = result is null ? "Canceled" : "Deleted!";
                StateHasChanged();

                if (confirm == "Deleted!")
                {

                    bool res = await SOSDistributionServices.DeleteSOSDistribution(id);
                    if (res)
                    {
                        _sosHub.SOSDistribution.RemoveAll(Distribution => Distribution.SOSDistributionId == id);

                        Documents.RemoveAll(doc => doc is SOSDistribution Distribution && Distribution.SOSDistributionId == id);
                        Snackbar.Clear();
                        Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                        Snackbar.Add($"{Localizer["succesRemoveDistribution"]}", Severity.Info);
                    }
                }
            }
            else if (typeof(T) == typeof(SOSFlow))
            {
                bool? result = await _DeleteFlow.Show();
                var confirm = result is null ? "Canceled" : "Deleted!";
                StateHasChanged();

                if (confirm == "Deleted!")
                {

                    bool res = await SOSFlowServices.DeleteSOSFlow(id);
                    if (res)
                    {
                        _sosHub.SOSFlow.RemoveAll(Flow => Flow.SOSFlowId == id);

                        Documents.RemoveAll(doc => doc is SOSFlow Flow && Flow.SOSFlowId == id);

                        Snackbar.Clear();
                        Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                        Snackbar.Add($"{Localizer["succesRemoveFlow"]}", Severity.Info);
                    }
                }
            }
            else if (typeof(T) == typeof(SOSSequence))
            {
                bool? result = await _DeleteSequence.Show();
                var confirm = result is null ? "Canceled" : "Deleted!";
                StateHasChanged();

                if (confirm == "Deleted!")
                {

                    bool res = await SOSSequenceServices.DeleteSOSSequence(id);
                    if (res)
                    {
                        _sosHub.SOSSequence.RemoveAll(Sequence => Sequence.SOSSequenceId == id);

                        Documents.RemoveAll(doc => doc is SOSSequence Sequence && Sequence.SOSSequenceId == id);
                        Snackbar.Clear();
                        Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                        Snackbar.Add($"{Localizer["succesRemoveSequence"]}", Severity.Info);
                    }
                }
            }
            else if (typeof(T) == typeof(PAT))
            {
                bool? result = await _DeletePat.Show();
                var confirm = result is null ? "Canceled" : "Deleted!";
                StateHasChanged();

                if (confirm == "Deleted!")
                {

                    bool res = await PATServices.DeletePat(id);
                    if (res)
                    {
                        _sosHub.PATs.RemoveAll(pat => pat.PATid == id);

                        Documents.RemoveAll(doc => doc is PAT pat && pat.PATid == id);
                        Snackbar.Clear();
                        Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                        Snackbar.Add($"{Localizer["succesRemovePat"]}", Severity.Info);
                    }
                }
            }


            StateHasChanged();
        }

        private int selectedRowNumber = -1;
        private int SosDocId = -1;
        private MudTable<Object> SelectTableEventDocument;
        private int selectedRowNumberPat = -1;
        private MudTable<Object> SelectTableEventDocumentPat;

        private void RowClickEventDocument(TableRowClickEventArgs<object> args)
        {

            if (selectedRowNumber == SelectTableEventDocument.Items.ToList().IndexOf(args.Item))
            {

                var method = GetType().GetMethod(nameof(Details), BindingFlags.Instance | BindingFlags.Public);

                var generic = method.MakeGenericMethod(args.Item.GetType());
                generic.Invoke(this, new object[] { SosDocId });


                //if (args.Item is SOSAnalysis)
                //{
                //    NavigationManager.NavigateTo($"/soshoe/Analysis/Details/{SosDocId}");
                //}
                //else if (args.Item is SOSCombination)
                //{
                //    NavigationManager.NavigateTo($"/soshoe/Combination/Details/{SosDocId}");
                //}
                //else if (args.Item is SOSDistribution)
                //{
                //    NavigationManager.NavigateTo($"/soshoe/Distribution/Details/{SosDocId}");
                //}
                //else if (args.Item is SOSFlow)
                //{
                //    NavigationManager.NavigateTo($"/soshoe/Flow/Details/{SosDocId}");
                //}
                //else if (args.Item is SOSSequence)
                //{
                //    NavigationManager.NavigateTo($"/soshoe/Sequence/Details/{SosDocId}");
                //}  
                //else if (args.Item is PAT)
                //{
                //    NavigationManager.NavigateTo($"/PAT/{SosDocId}");
                //}
            }
            else
            {
                SelectTableEventDocument.SelectedItem = args.Item;
                selectedRowNumber = SelectTableEventDocument.Items.ToList().IndexOf(args.Item);
            }
        }

        private string SelectedRowDocument(Object element, int rowNumber)
        {
            if (selectedRowNumber == rowNumber)
            {
                if (element is SOSAnalysis analysis)
                {
                    SosDocId = analysis.SOSAnalysisId;
                }
                else if (element is SOSCombination combination)
                {
                    SosDocId = combination.SOSCombinationId;
                }
                else if (element is SOSDistribution distribution)
                {
                    SosDocId = distribution.SOSDistributionId;
                }
                else if (element is SOSFlow flow)
                {
                    SosDocId = flow.SOSFlowId;
                }
                else if (element is SOSSequence sequence)
                {
                    SosDocId = sequence.SOSSequenceId;
                }
                else if (element is PAT pat)
                {
                    SosDocId = pat.PATid;
                }
                else if (element is HCI hci)
                {
                    SosDocId = hci.HCIId;
                }
                else if (element is SOSSynopticTableofControlPoints csro)
                {
                    SosDocId = csro.SOSSynopticTableofControlPointsId;
                }
                else if (element is SOSSynopticTableofOperatingRequirements cscp)
                {
                    SosDocId = cscp.SOSSynopticTableofOperatingRequirementsId;
                }
                return "selected"; // Mantener la fila seleccionada
            }
            else if (SelectTableEventDocument.SelectedItem != null && SelectTableEventDocument.SelectedItem.Equals(element))
            {
                if (element is SOSAnalysis analysis)
                {
                    SosDocId = analysis.SOSAnalysisId;
                }
                else if (element is SOSCombination combination)
                {
                    SosDocId = combination.SOSCombinationId;
                }
                else if (element is SOSDistribution distribution)
                {
                    SosDocId = distribution.SOSDistributionId;
                }
                else if (element is SOSFlow flow)
                {
                    SosDocId = flow.SOSFlowId;
                }
                else if (element is SOSSequence sequence)
                {
                    SosDocId = sequence.SOSSequenceId;
                }
                else if (element is PAT pat)
                {
                    SosDocId = pat.PATid;
                }
                else if (element is HCI hci)
                {
                    SosDocId = hci.HCIId;
                }
                else if (element is SOSSynopticTableofControlPoints csro)
                {
                    SosDocId = csro.SOSSynopticTableofControlPointsId;
                }
                else if (element is SOSSynopticTableofOperatingRequirements cscp)
                {
                    SosDocId = cscp.SOSSynopticTableofOperatingRequirementsId;
                }

                selectedRowNumber = rowNumber;
                return "selected";
            }
            else
            {
                return string.Empty;
            }
        }


        private void RowClickEventDocumentPat(TableRowClickEventArgs<object> args)
        {

            if (selectedRowNumberPat == SelectTableEventDocumentPat.Items.ToList().IndexOf(args.Item))
            {
                if (args.Item is PAT)
                {
                    NavigationManager.NavigateTo($"/PAT/{SosDocId}");
                }
            }
            else
            {
                SelectTableEventDocumentPat.SelectedItem = args.Item;
                selectedRowNumberPat = SelectTableEventDocumentPat.Items.ToList().IndexOf(args.Item);
            }
        }

        private string SelectedRowDocumentPat(Object element, int rowNumber)
        {
            if (selectedRowNumberPat == rowNumber)
            {
                if (element is PAT pat)
                {
                    SosDocId = pat.PATid;
                }
                return "selected"; // Mantener la fila seleccionada
            }
            else if (SelectTableEventDocumentPat.SelectedItem != null && SelectTableEventDocumentPat.SelectedItem.Equals(element))
            {
                if (element is PAT pat)
                {
                    SosDocId = pat.PATid;
                }

                selectedRowNumberPat = rowNumber;
                return "selected";
            }
            else
            {
                return string.Empty;
            }
        }
        #endregion

        #region ApproberOwners

        private User selectedOwnerOfList = null;
        private User selectedApproverOfList = null;
        private bool ActiveAddOwner = false;
        private bool ActiveAddApprover = false;

        private bool cantCreate = false;

        private async void OnSelectedOwnerFunction(User element, int type)
        {
            switch (type)
            {
                case 1:

                    selectedOwnerOfList = element;


                    if (selectedOwnerOfList != new User())
                    {
                        ActiveAddOwner = false;
                    }
                    else
                    {
                        ActiveAddOwner = true;
                    }
                    break;

                case 2:
                    selectedApproverOfList = element;


                    if (selectedApproverOfList != new User())
                    {
                        ActiveAddApprover = false;
                    }
                    else
                    {
                        ActiveAddApprover = true;
                    }
                    break;
            }

        }

        private void DeleteOwnerList(User selection)
        {
            _sosHub.ApproverOwners?.Remove(selection);
            _supervisors.Add(selection);
            cantCreate = _sosHub.ApproverOwners.Count == 0;
            StateHasChanged();
        }


        private void AddOwner(User selection)
        {
            if (_sosHub.ApproverOwners == null)
            {
                _sosHub.ApproverOwners = new List<User>();
            }


            if (selectedOwnerOfList != null && !_sosHub.ApproverOwners.Contains(selection))
            {

                _sosHub.ApproverOwners.Add(selection);

                _supervisors.Remove(selection);

                selectedOwnerOfList = null;
                ActiveAddOwner = true;
            }

            cantCreate = _sosHub.ApproverOwners.Count == 0;

            StateHasChanged();
        }


        #endregion

        #region PAT



        private async Task OnAplicationDateChanged(DateTime? newDate)
        {
            _pat.AplicationDate = newDate;
            StateHasChanged();
        }


        #endregion
    }
}