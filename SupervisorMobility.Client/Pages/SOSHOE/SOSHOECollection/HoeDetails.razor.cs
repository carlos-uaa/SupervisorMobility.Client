using BlazorCameraStreamer;
using Blazorise.Extensions;
using DocumentFormat.OpenXml.EMMA;
using DocumentFormat.OpenXml.InkML;
using DocumentFormat.OpenXml.Office2010.PowerPoint;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using MudBlazor;
using SupervisorMobility.Client.Data.Entities;
using SupervisorMobility.Client.Data.Entities.SOS_Process;
using SupervisorMobility.Client.Services.SOS_Services.ToolServices;
using System;
using System.Formats.Asn1;
using System.Globalization;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using static MudBlazor.CategoryTypes;
using static MudBlazor.FilterOperator;

namespace SupervisorMobility.Client.Pages.SOSHOE.SOSHOECollection
{
    public partial class HoeDetails
    {
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
        int supervisorOwnerId = 0;
        int supervisorEditorId = 0;

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
                    new BreadcrumbItem(text: Localizer["details"] + SOSHubId, href: "", disabled: true)
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
            ShowLoading = false;
        }


        //Create SOS HUB and validations
        #region Create SOSHUB

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
            _sosHub = await SOSHubServices.GetSOSHub(SOSHubId, true, true, true, true, true, true, true, true, includeDocuments: true, includeCollections: true);




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
            productId = _sosHub.AppliedModelId ?? productId;
            supervisorEditorId = _sosHub.EditorId ?? supervisorEditorId;
            supervisorOwnerId = _sosHub.OwnerId ?? supervisorOwnerId;

            cycleId = _sosHub.TrainingTime != null ? GetCycleId(_sosHub.TrainingTime) : 0;

            StateHasChanged();

            _sosHub.AppliedModel = _products.Find(p => p.ProductId == _sosHub.AppliedModelId);

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
                Documents.Add(distribution);
            }

            foreach (var flow in _sosHub.SOSFlow)
            {
                Documents.Add(flow);
            }

            foreach (var sequence in _sosHub.SOSSequence)
            {
                Documents.Add(sequence);
            }

            //faltan añadir los diagramas

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
                                _oper = await UsersService.GetSubordinates(SupervisorTurn1 ,false);
                        break;
                        case 1:
                                _oper = await UsersService.GetSubordinates(SupervisorTurn2 ,false);
                        break;
                    case 2:
                                _oper = await UsersService.GetSubordinates(SupervisorTurn3 ,false);
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

                    // Agregar el texto normal antes del punto crítico
                    builder.Append(text.Substring(currentIndex, startIndex - currentIndex));

                    // Agregar el punto crítico resaltado
                    builder.Append($"<mark>{text.Substring(startIndex, endIndex - startIndex)}</mark>");

                    currentIndex = endIndex;
                }
            }

            // Agregar el texto normal después del último punto crítico
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
        private DialogOptions dialogPagesOptions = new() { CloseOnEscapeKey = true, FullWidth = true };
        public bool ShowGenerateDialog = false;
        public bool ShowPagesGenerate = false;
        private int selectedIndexPageGenerate = 0;
        public void GoToPageGenerate(int indexPage)
        {
            selectedIndexPageGenerate = indexPage;
            dialogPagesOptions.MaxWidth = MaxWidth.Medium;
            switch (selectedIndexPageGenerate)
            {
                case 1:
                    if (_sosHub.SOSAnalysis.Count > 0)
                    {
                        _sosAnalysis = _sosHub.SOSAnalysis.FirstOrDefault();
                        if (_sosAnalysis.SOSAnalysisId != 0 && _sosAnalysis.AnalysisLogbooks.Count > 0)
                        {
                            ApproverAnalysisId = (int)(_sosAnalysis.AnalysisLogbooks.Last().Status != 2 ? _sosAnalysis.AnalysisLogbooks.Last().ApproverId : 0);
                            ReviewerAnalysisId = (int)(_sosAnalysis.AnalysisLogbooks.Last().Status != 2 ? _sosAnalysis.AnalysisLogbooks.Last().ReviewerId : 0);
                        }

                        if (_sosAnalysis.AnalysisLogbooks.Count == 0)
                        {
                            _sosAnalysis.AnalysisLogbooks.Add(new SOSAnalysisLogbook());
                        }
                    }
                    break; 
                case 2:

                    if (_sosHub.SOSCombination.Count > 0)
                    {
                        _sosCombination = _sosHub.SOSCombination.FirstOrDefault() ?? new SOSCombination();
                        if (_sosCombination.SOSCombinationId != 0 && _sosCombination.CombinationLogbooks.Count > 0)
                        {
                            ApproverCombinationId = (int)(_sosCombination.CombinationLogbooks.Last().Status != 2 ? _sosCombination.CombinationLogbooks.Last().ApproverId : 0);
                            ReviewerCombinationId = (int)(_sosCombination.CombinationLogbooks.Last().Status != 2 ? _sosCombination.CombinationLogbooks.Last().ReviewerId : 0);
                        }

                        if (_sosCombination.CombinationLogbooks.Count == 0)
                        {
                            _sosCombination.CombinationLogbooks.Add(new SOSCombinationLogbook());
                        }

                        if (_sosCombination.Turns?.Count >= 1)
                        {
                            OperatorTurn1 = (int)_sosCombination.Turns.ElementAt(0).OperatorId;
                            SupervisorTurn1 = (int)_sosCombination.Turns.ElementAt(0).SupervisorId;
                        }

                        if (_sosCombination.Turns?.Count >= 2)
                        {
                            OperatorTurn2 = (int)_sosCombination.Turns.ElementAt(1).OperatorId;
                            SupervisorTurn2 = (int)_sosCombination.Turns.ElementAt(1).SupervisorId;
                        }

                        if (_sosCombination.Turns?.Count >= 3)
                        {
                            OperatorTurn3 = (int)_sosCombination.Turns.ElementAt(2).OperatorId;
                            SupervisorTurn3 = (int)_sosCombination.Turns.ElementAt(2).SupervisorId;
                        }

                    }
                    break;

                        case 3:
                    if (_sosHub.SOSDistribution.Count > 0)
                    {
                        _sosDistribution = _sosHub.SOSDistribution.FirstOrDefault();
                        if (_sosDistribution.SOSDistributionId != 0 && _sosDistribution.DistributionLogbooks.Count > 0)
                        {
                            ApproverDistributionId = (int)(_sosDistribution.DistributionLogbooks.Last().Status != 2 ? _sosDistribution.DistributionLogbooks.Last().ApproverId : 0);
                            ReviewerDistributionId = (int)(_sosDistribution.DistributionLogbooks.Last().Status != 2 ? _sosDistribution.DistributionLogbooks.Last().ReviewerId : 0);
                        }

                        if (_sosDistribution.DistributionLogbooks.Count == 0)
                        {
                            _sosDistribution.DistributionLogbooks.Add(new SOSDistributionLogbook());
                        }

                        if (_sosDistribution.Turns?.Count >= 1)
                        {
                            OperatorTurn1 = (int)_sosDistribution.Turns.ElementAt(0).OperatorId;
                            SupervisorTurn1 = (int)_sosDistribution.Turns.ElementAt(0).SupervisorId;
                        }

                        if (_sosDistribution.Turns?.Count >= 2)
                        {
                            OperatorTurn2 = (int)_sosDistribution.Turns.ElementAt(1).OperatorId;
                            SupervisorTurn2 = (int)_sosDistribution.Turns.ElementAt(1).SupervisorId;
                        }

                        if (_sosDistribution.Turns?.Count >= 3)
                        {
                            OperatorTurn3 = (int)_sosDistribution.Turns.ElementAt(2).OperatorId;
                            SupervisorTurn3 = (int)_sosDistribution.Turns.ElementAt(2).SupervisorId;
                        }
                    }
                    break;
                        case 4:
                    if (_sosHub.SOSFlow.Count > 0)
                    {
                        _sosFlow = _sosHub.SOSFlow.FirstOrDefault();
                        if (_sosFlow.SOSFlowId != 0 && _sosFlow.FlowLogbooks.Count > 0)
                        {
                            ApproverFlowId = (int)(_sosFlow.FlowLogbooks.Last().Status != 2 ? _sosFlow.FlowLogbooks.Last().ApproverId : 0);
                            ReviewerFlowId = (int)(_sosFlow.FlowLogbooks.Last().Status != 2 ? _sosFlow.FlowLogbooks.Last().ReviewerId : 0);
                        }

                        if (_sosFlow.FlowLogbooks.Count == 0)
                        {
                            _sosFlow.FlowLogbooks.Add(new SOSFlowLogbook());
                        }
                    }
                    break;
                case 5:
                    if (_sosHub.SOSSequence.Count > 0)
                    {
                        _sosSequence = _sosHub.SOSSequence.FirstOrDefault();
                        if (_sosSequence.SOSSequenceId != 0 && _sosSequence.SequenceLogbooks.Count > 0)
                        {
                            ApproverSequenceId = (int)(_sosSequence.SequenceLogbooks.Last().Status != 2 ? _sosSequence.SequenceLogbooks.Last().ApproverId : 0);
                            ReviewerSequenceId = (int)(_sosSequence.SequenceLogbooks.Last().Status != 2 ? _sosSequence.SequenceLogbooks.Last().ReviewerId : 0);
                        }

                        if (_sosSequence.SequenceLogbooks.Count == 0)
                        {
                            _sosSequence.SequenceLogbooks.Add(new SOSSequenceLogbook());
                        }
                    }
                    break;
            }

            ShowGenerateDialog = false;
            ShowPagesGenerate = true;

        }


        public void ReturnToGenerateindex()
        {
            selectedIndexPageGenerate = 0;
            ShowPagesGenerate = false;
            ShowGenerateDialog = true;
        }

        #endregion
        #region generateAnalysis
        SOSAnalysis _sosAnalysis { get; set; } = new SOSAnalysis();
        SOSAnalysisLogbook loganalysis { get; set; } = new SOSAnalysisLogbook();
        int ApproverAnalysisId = 0;
        int ReviewerAnalysisId = 0;

        public async void GenerateAnalysis()
        {

            if (_sosHub.SOSAnalysis.Count > 0)
            {
                //REVISION
                if (ReviewerAnalysisId == 0 || ApproverAnalysisId == 0)
                {
                    bool? result = await DialogService.ShowMessageBox(
                       "Warning",
                       ApproverAnalysisId == 0 ? "Es necesario el aprobador" : "Es necesario seleccionar el editor (elaboro)!",
                       yesText: "Ok!");
                    var state = result == null ? "Canceled" : "Deleted!";
                    StateHasChanged();
                    return;
                }

                _sosAnalysis = _sosHub.SOSAnalysis.First();

                if (_sosAnalysis.AnalysisLogbooks.First().SOSAnalysisLogbookId == 0)
                {
                    _sosAnalysis.AnalysisLogbooks.Clear();
                }

                loganalysis.NoRevision = _sosAnalysis.AnalysisLogbooks?.Count();
                loganalysis.ApproverId = ApproverAnalysisId;
                loganalysis.ReviewerId = ReviewerAnalysisId;
                loganalysis.Date = System.DateTime.Now;
                loganalysis.Status = 1;
                loganalysis.IsActive = true;
                if (_sosAnalysis.AnalysisLogbooks == null)
                {
                    _sosAnalysis.AnalysisLogbooks = new List<SOSAnalysisLogbook>();
                }

                _sosAnalysis.AnalysisLogbooks.Add(loganalysis);

                var Gen_sosAnalysis = await SOSHubServices.GenerateAnalysis(SOSHubId, _sosAnalysis);

                Snackbar.Clear();
                Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                if (Gen_sosAnalysis != 0)
                {
                    Snackbar.Add($"{Localizer["_sosAnalysisGeneratedSucces"]}", Severity.Info);
                    ShowPagesGenerate = false;
                    NavigationManager.NavigateTo($"/soshoe/Analysis/Details/{_sosAnalysis.SOSAnalysisId}");
                    _sosAnalysis = new SOSAnalysis();
                    //Pregutar si quiere ver el analisis generado
                }
                else
                {
                    Snackbar.Add($"{Localizer["Fail_sosAnalysisGeneratedSucces"]}", Severity.Error);
                }

                StateHasChanged();

            }
            else
            {
                if (ReviewerAnalysisId == 0 || ApproverAnalysisId == 0)
                {
                    bool? result = await DialogService.ShowMessageBox(
                       "Warning",
                       string.IsNullOrEmpty(_sosAnalysis.OperationName) ? "Es necesario el aproador" : "Es necesario seleccionar el editor (elaboro)!",
                       yesText: "Ok!");
                    var state = result == null ? "Canceled" : "Deleted!";
                    StateHasChanged();
                }
                else if (string.IsNullOrEmpty(_sosAnalysis.ProcessName) || string.IsNullOrEmpty(_sosAnalysis.InternalControlNumber))
                {
                    bool? result = await DialogService.ShowMessageBox(
                      "Warning",
                      string.IsNullOrEmpty(_sosAnalysis.ProcessName) ? "Es necesario el nombre de Proceso" : "Es necesario el nombre del proceso!",
                      yesText: "Ok!");
                    var state = result == null ? "Canceled" : "Deleted!";
                    StateHasChanged();
                }
                else if (string.IsNullOrEmpty(_sosAnalysis.OperationName))
                {
                    bool? result = await DialogService.ShowMessageBox(
                      "Warning",
                       "Es necesario el nombre de operacion!",
                      yesText: "Ok!");
                    var state = result == null ? "Canceled" : "Deleted!";
                    StateHasChanged();
                }
                else
                {
                    loganalysis.NoRevision = 0;
                    loganalysis.ReviewerId = supervisorOwnerId;
                    loganalysis.ApproverId = ReviewerAnalysisId;
                    loganalysis.Date = System.DateTime.Now;
                    loganalysis.Status = 1;
                    loganalysis.IsActive = true;
                    if (_sosAnalysis.AnalysisLogbooks == null)
                    {
                        _sosAnalysis.AnalysisLogbooks = new List<SOSAnalysisLogbook>();
                    }
                    _sosAnalysis.AnalysisLogbooks.Add(loganalysis);

                    var Gen_sosAnalysis = await SOSHubServices.GenerateAnalysis(SOSHubId, _sosAnalysis);

                    Snackbar.Clear();
                    Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                    if (Gen_sosAnalysis != 0)
                    {
                        Snackbar.Add($"{Localizer["_sosAnalysisGeneratedSucces"]}", Severity.Info);
                        ShowPagesGenerate = false;
                        NavigationManager.NavigateTo($"/soshoe/Analysis/Details/{Gen_sosAnalysis}");
                        _sosAnalysis = new SOSAnalysis();
                        //Pregutar si quiere ver el analisis generado
                    }
                    else
                    {
                        Snackbar.Add($"{Localizer["Fail_sosAnalysisGeneratedSucces"]}", Severity.Error);
                    }

                    StateHasChanged();
                }
            }


        }

        #endregion
        #region generateCombination
        SOSCombination _sosCombination { get; set; } = new SOSCombination();
        int ApproverDocCombinationId = 0;
        int ReviewerDocCombinationId = 0;
        int ReviewerHYDocCombinationId = 0;

        SOSCombinationLogbook logCombination { get; set; } = new SOSCombinationLogbook();
        int ApproverCombinationId = 0;
        int ReviewerCombinationId = 0;  


        public async void GenerateCombination()
        {

            if (_sosHub.SOSCombination.Count > 0)
            {
                //REVISION
                if (ReviewerCombinationId == 0 || ApproverCombinationId == 0)
                {
                    bool? result = await DialogService.ShowMessageBox(
                       "Warning",
                       ApproverCombinationId == 0 ? "Es necesario el aprobador" : "Es necesario seleccionar el editor (elaboro)!",
                       yesText: "Ok!");
                    var state = result == null ? "Canceled" : "Deleted!";
                    StateHasChanged();
                    return;
                }

                _sosCombination = _sosHub.SOSCombination.First();

                if (_sosCombination.CombinationLogbooks.First().SOSCombinationLogbookId == 0)
                {
                    _sosCombination.CombinationLogbooks.Clear();
                }

                logCombination.NoRevision = _sosCombination.CombinationLogbooks?.Count();
                logCombination.ApproverId = ApproverCombinationId;
                logCombination.ReviewerId = ReviewerCombinationId;
                logCombination.Date = System.DateTime.Now;
                logCombination.Status = 1;
                logCombination.IsActive = true;
                if (_sosCombination.CombinationLogbooks == null)
                {
                    _sosCombination.CombinationLogbooks = new List<SOSCombinationLogbook>();
                }


                _sosCombination.CombinationLogbooks.Add(logCombination);

                var Gen_sosCombination = await SOSHubServices.GenerateCombination(SOSHubId, _sosCombination);

                Snackbar.Clear();
                Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                if (Gen_sosCombination != 0)
                {
                    Snackbar.Add($"{Localizer["_sosCombinationGeneratedSucces"]}", Severity.Info);
                    ShowPagesGenerate = false;
                    NavigationManager.NavigateTo($"/soshoe/Combination/Details/{_sosCombination.SOSCombinationId}");
                    _sosCombination = new SOSCombination();
                    //Pregutar si quiere ver el analisis generado
                }
                else
                {
                    Snackbar.Add($"{Localizer["Fail_sosCombinationGeneratedSucces"]}", Severity.Error);
                }

                StateHasChanged();

            }
            else
            {
                if (ReviewerCombinationId == 0 || ApproverCombinationId == 0)
                {
                    bool? result = await DialogService.ShowMessageBox(
                       "Warning",
                       string.IsNullOrEmpty(_sosCombination.OperationName) ? "Es necesario el aproador" : "Es necesario seleccionar el editor (elaboro)!",
                       yesText: "Ok!");
                    var state = result == null ? "Canceled" : "Deleted!";
                    StateHasChanged();
                }
                else if (string.IsNullOrEmpty(_sosCombination.ProcessName) || string.IsNullOrEmpty(_sosCombination.InternalControlNumber))
                {
                    bool? result = await DialogService.ShowMessageBox(
                      "Warning",
                      string.IsNullOrEmpty(_sosCombination.ProcessName) ? "Es necesario el nombre de Proceso" : "Es necesario el nombre del proceso!",
                      yesText: "Ok!");
                    var state = result == null ? "Canceled" : "Deleted!";
                    StateHasChanged();
                }
                else if (string.IsNullOrEmpty(_sosCombination.OperationName))
                {
                    bool? result = await DialogService.ShowMessageBox(
                      "Warning",
                       "Es necesario el nombre de operacion!",
                      yesText: "Ok!");
                    var state = result == null ? "Canceled" : "Deleted!";
                    StateHasChanged();
                }
                else
                {
                    logCombination.NoRevision = 0;
                    logCombination.ReviewerId = supervisorOwnerId;
                    logCombination.ApproverId = ReviewerCombinationId;
                    logCombination.Date = System.DateTime.Now;
                    logCombination.Status = 1;
                    logCombination.IsActive = true;
                    if (_sosCombination.CombinationLogbooks == null)
                    {
                        _sosCombination.CombinationLogbooks = new List<SOSCombinationLogbook>();
                    }

                    _sosCombination.CombinationLogbooks.Add(logCombination);

                    _sosCombination.ReviewerId = ReviewerDocCombinationId;
                    _sosCombination.ReviewerHSId = ReviewerHYDocCombinationId;
                    _sosCombination.ApproverId = ApproverDocCombinationId;


                    if (SupervisorTurn1 != 0 && OperatorTurn1 != 0)
                    {
                        _sosCombination.Turns.ElementAt(0).SupervisorId = SupervisorTurn1;
                        _sosCombination.Turns.ElementAt(0).OperatorId = OperatorTurn1;
                    }

                    if (SupervisorTurn2 != 0 && OperatorTurn2 != 0)
                    {
                        _sosCombination.Turns.ElementAt(1).SupervisorId = SupervisorTurn2;
                        _sosCombination.Turns.ElementAt(1).OperatorId = OperatorTurn2;
                    }

                    if (SupervisorTurn3 != 0 && OperatorTurn3 != 0)
                    {
                        _sosCombination.Turns.ElementAt(2).SupervisorId = SupervisorTurn3;
                        _sosCombination.Turns.ElementAt(2).OperatorId = OperatorTurn3;
                    }

                    var Gen_sosCombination = await SOSHubServices.GenerateCombination(SOSHubId, _sosCombination);

                    Snackbar.Clear();
                    Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                    if (Gen_sosCombination != 0)
                    {
                        Snackbar.Add($"{Localizer["_sosCombinationGeneratedSucces"]}", Severity.Info);
                        ShowPagesGenerate = false;
                        NavigationManager.NavigateTo($"/soshoe/Combination/Details/{Gen_sosCombination}");
                        _sosCombination = new SOSCombination();
                        //Pregutar si quiere ver el analisis generado
                    }
                    else
                    {
                        Snackbar.Add($"{Localizer["Fail_sosCombinationGeneratedSucces"]}", Severity.Error);
                    }

                    StateHasChanged();
                }
            }


        }

        private void AddCombinationTurn()
        {
            if(_sosCombination.Turns == null)
            {
                _sosCombination.Turns = new List<Turn>();
            }

            string[] numbers = new string[]{"ero", "ndo", "ero","rto"};
            Turn toCreate = new Turn();
            toCreate.TurnType = (_sosCombination.Turns.Count + 1).ToString() + numbers[(int)_sosCombination.Turns.Count()];
            _sosCombination.Turns?.Add(toCreate);
        }


        #endregion
        #region generateDistribution
        SOSDistribution _sosDistribution { get; set; } = new SOSDistribution();
        SOSDistributionLogbook logDistribution { get; set; } = new SOSDistributionLogbook();
        int ApproverDistributionId = 0;
        int ReviewerDistributionId = 0;    
        int ApproverDocDistributionId = 0;
        int ReviewerDocDistributionId = 0;

        public async void GenerateDistribution()
        {

            if (_sosHub.SOSDistribution.Count > 0)
            {
                //REVISION
                if (ReviewerDistributionId == 0 || ApproverDistributionId == 0)
                {
                    bool? result = await DialogService.ShowMessageBox(
                       "Warning",
                       ApproverDistributionId == 0 ? "Es necesario el aprobador" : "Es necesario seleccionar el editor (elaboro)!",
                       yesText: "Ok!");
                    var state = result == null ? "Canceled" : "Deleted!";
                    StateHasChanged();
                    return;
                }

                _sosDistribution = _sosHub.SOSDistribution.First();

                if (_sosDistribution.DistributionLogbooks.First().SOSDistributionLogbookId == 0)
                {
                    _sosDistribution.DistributionLogbooks.Clear();
                }

                logDistribution.NoRevision = _sosDistribution.DistributionLogbooks?.Count();
                logDistribution.ApproverId = ApproverDistributionId;
                logDistribution.ReviewerId = ReviewerDistributionId;
                logDistribution.Date = System.DateTime.Now;
                logDistribution.Status = 1;
                logDistribution.IsActive = true;
                if (_sosDistribution.DistributionLogbooks == null)
                {
                    _sosDistribution.DistributionLogbooks = new List<SOSDistributionLogbook>();
                }

                _sosDistribution.DistributionLogbooks.Add(logDistribution);

                var Gen_sosDistribution = await SOSHubServices.GenerateDistribution(SOSHubId, _sosDistribution);

                Snackbar.Clear();
                Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                if (Gen_sosDistribution != 0)
                {
                    Snackbar.Add($"{Localizer["_sosDistributionGeneratedSucces"]}", Severity.Info);
                    ShowPagesGenerate = false;
                    NavigationManager.NavigateTo($"/soshoe/Distribution/Details/{_sosDistribution.SOSDistributionId}");
                    _sosDistribution = new SOSDistribution();
                    //Pregutar si quiere ver el analisis generado
                }
                else
                {
                    Snackbar.Add($"{Localizer["Fail_sosDistributionGeneratedSucces"]}", Severity.Error);
                }

                StateHasChanged();

            }
            else
            {
                if (ReviewerDistributionId == 0 || ApproverDistributionId == 0)
                {
                    bool? result = await DialogService.ShowMessageBox(
                       "Warning",
                       string.IsNullOrEmpty(_sosDistribution.OperationName) ? "Es necesario el aproador" : "Es necesario seleccionar el editor (elaboro)!",
                       yesText: "Ok!");
                    var state = result == null ? "Canceled" : "Deleted!";
                    StateHasChanged();
                }
                else if (string.IsNullOrEmpty(_sosDistribution.ProcessName) || string.IsNullOrEmpty(_sosDistribution.InternalControlNumber))
                {
                    bool? result = await DialogService.ShowMessageBox(
                      "Warning",
                      string.IsNullOrEmpty(_sosDistribution.ProcessName) ? "Es necesario el nombre de Proceso" : "Es necesario el nombre del proceso!",
                      yesText: "Ok!");
                    var state = result == null ? "Canceled" : "Deleted!";
                    StateHasChanged();
                }
                else if (string.IsNullOrEmpty(_sosDistribution.OperationName))
                {
                    bool? result = await DialogService.ShowMessageBox(
                      "Warning",
                       "Es necesario el nombre de operacion!",
                      yesText: "Ok!");
                    var state = result == null ? "Canceled" : "Deleted!";
                    StateHasChanged();
                }
                else
                {
                    logDistribution.NoRevision = 0;
                    logDistribution.ReviewerId = supervisorOwnerId;
                    logDistribution.ApproverId = ReviewerDistributionId;
                    logDistribution.Date = System.DateTime.Now;
                    logDistribution.Status = 1;
                    logDistribution.IsActive = true;
                    if (_sosDistribution.DistributionLogbooks == null)
                    {
                        _sosDistribution.DistributionLogbooks = new List<SOSDistributionLogbook>();
                    }
                    _sosDistribution.DistributionLogbooks.Add(logDistribution);

                    var Gen_sosDistribution = await SOSHubServices.GenerateDistribution(SOSHubId, _sosDistribution);

                    Snackbar.Clear();
                    Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                    if (Gen_sosDistribution != 0)
                    {
                        Snackbar.Add($"{Localizer["_sosDistributionGeneratedSucces"]}", Severity.Info);
                        ShowPagesGenerate = false;
                        NavigationManager.NavigateTo($"/soshoe/Distribution/Details/{Gen_sosDistribution}");
                        _sosDistribution = new SOSDistribution();
                        //Pregutar si quiere ver el analisis generado
                    }
                    else
                    {
                        Snackbar.Add($"{Localizer["Fail_sosDistributionGeneratedSucces"]}", Severity.Error);
                    }

                    StateHasChanged();
                }
            }


        }

        private void AddDistributionTurn()
        {
            if (_sosDistribution.Turns == null)
            {
                _sosDistribution.Turns = new List<Turn>();
            }

            string[] numbers = new string[] { "ero", "ndo", "ero", "rto" };
            Turn toCreate = new Turn();
            toCreate.TurnType = (_sosDistribution.Turns.Count + 1).ToString() + numbers[(int)_sosDistribution.Turns.Count()];
            _sosDistribution.Turns?.Add(toCreate);
        }
        #endregion
        #region generateFlow
        SOSFlow _sosFlow { get; set; } = new SOSFlow();
        SOSFlowLogbook logFlow { get; set; } = new SOSFlowLogbook();
        int ApproverFlowId = 0;
        int ReviewerFlowId = 0;

        public async void GenerateFlow()
        {

            if (_sosHub.SOSFlow.Count > 0)
            {
                //REVISION
                if (ReviewerFlowId == 0 || ApproverFlowId == 0)
                {
                    bool? result = await DialogService.ShowMessageBox(
                       "Warning",
                       ApproverFlowId == 0 ? "Es necesario el aprobador" : "Es necesario seleccionar el editor (elaboro)!",
                       yesText: "Ok!");
                    var state = result == null ? "Canceled" : "Deleted!";
                    StateHasChanged();
                    return;
                }

                _sosFlow = _sosHub.SOSFlow.First();

                if (_sosFlow.FlowLogbooks.First().SOSFlowLogbookId == 0)
                {
                    _sosFlow.FlowLogbooks.Clear();
                }

                logFlow.NoRevision = _sosFlow.FlowLogbooks?.Count();
                logFlow.ApproverId = ApproverFlowId;
                logFlow.ReviewerId = ReviewerFlowId;
                logFlow.Date = System.DateTime.Now;
                logFlow.Status = 1;
                logFlow.IsActive = true;
                if (_sosFlow.FlowLogbooks == null)
                {
                    _sosFlow.FlowLogbooks = new List<SOSFlowLogbook>();
                }

                _sosFlow.FlowLogbooks.Add(logFlow);

                var Gen_sosFlow = await SOSHubServices.GenerateFlow(SOSHubId, _sosFlow);

                Snackbar.Clear();
                Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                if (Gen_sosFlow != 0)
                {
                    Snackbar.Add($"{Localizer["_sosFlowGeneratedSucces"]}", Severity.Info);
                    ShowPagesGenerate = false;
                    NavigationManager.NavigateTo($"/soshoe/Flow/Details/{_sosFlow.SOSFlowId}");
                    _sosFlow = new SOSFlow();
                    //Pregutar si quiere ver el analisis generado
                }
                else
                {
                    Snackbar.Add($"{Localizer["Fail_sosFlowGeneratedSucces"]}", Severity.Error);
                }

                StateHasChanged();

            }
            else
            {
                if (ReviewerFlowId == 0 || ApproverFlowId == 0)
                {
                    bool? result = await DialogService.ShowMessageBox(
                       "Warning",
                       string.IsNullOrEmpty(_sosFlow.OperationName) ? "Es necesario el aproador" : "Es necesario seleccionar el editor (elaboro)!",
                       yesText: "Ok!");
                    var state = result == null ? "Canceled" : "Deleted!";
                    StateHasChanged();
                }
                else if (string.IsNullOrEmpty(_sosFlow.ProcessName) || string.IsNullOrEmpty(_sosFlow.InternalControlNumber))
                {
                    bool? result = await DialogService.ShowMessageBox(
                      "Warning",
                      string.IsNullOrEmpty(_sosFlow.ProcessName) ? "Es necesario el nombre de Proceso" : "Es necesario el nombre del proceso!",
                      yesText: "Ok!");
                    var state = result == null ? "Canceled" : "Deleted!";
                    StateHasChanged();
                }
                else if (string.IsNullOrEmpty(_sosFlow.OperationName))
                {
                    bool? result = await DialogService.ShowMessageBox(
                      "Warning",
                       "Es necesario el nombre de operacion!",
                      yesText: "Ok!");
                    var state = result == null ? "Canceled" : "Deleted!";
                    StateHasChanged();
                }
                else
                {
                    logFlow.NoRevision = 0;
                    logFlow.ReviewerId = supervisorOwnerId;
                    logFlow.ApproverId = ReviewerFlowId;
                    logFlow.Date = System.DateTime.Now;
                    logFlow.Status = 1;
                    logFlow.IsActive = true;
                    if (_sosFlow.FlowLogbooks == null)
                    {
                        _sosFlow.FlowLogbooks = new List<SOSFlowLogbook>();
                    }
                    _sosFlow.FlowLogbooks.Add(logFlow);

                    var Gen_sosFlow = await SOSHubServices.GenerateFlow(SOSHubId, _sosFlow);

                    Snackbar.Clear();
                    Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                    if (Gen_sosFlow != 0)
                    {
                        Snackbar.Add($"{Localizer["_sosFlowGeneratedSucces"]}", Severity.Info);
                        ShowPagesGenerate = false;
                        NavigationManager.NavigateTo($"/soshoe/Flow/Details/{Gen_sosFlow}");
                        _sosFlow = new SOSFlow();
                        //Pregutar si quiere ver el analisis generado
                    }
                    else
                    {
                        Snackbar.Add($"{Localizer["Fail_sosFlowGeneratedSucces"]}", Severity.Error);
                    }

                    StateHasChanged();
                }
            }


        }

        #endregion
        #region generateSequence
        SOSSequence _sosSequence { get; set; } = new SOSSequence();
        SOSSequenceLogbook logSequence { get; set; } = new SOSSequenceLogbook();
        int ApproverSequenceId = 0;
        int ReviewerSequenceId = 0;

        public async void GenerateSequence()
        {

            if (_sosHub.SOSSequence.Count > 0)
            {
                //REVISION
                if (ReviewerSequenceId == 0 || ApproverSequenceId == 0)
                {
                    bool? result = await DialogService.ShowMessageBox(
                       "Warning",
                       ApproverSequenceId == 0 ? "Es necesario el aprobador" : "Es necesario seleccionar el editor (elaboro)!",
                       yesText: "Ok!");
                    var state = result == null ? "Canceled" : "Deleted!";
                    StateHasChanged();
                    return;
                }

                _sosSequence = _sosHub.SOSSequence.First();

                if (_sosSequence.SequenceLogbooks.First().SOSSequenceLogbookId == 0)
                {
                    _sosSequence.SequenceLogbooks.Clear();
                }

                logSequence.NoRevision = _sosSequence.SequenceLogbooks?.Count();
                logSequence.ApproverId = ApproverSequenceId;
                logSequence.ReviewerId = ReviewerSequenceId;
                logSequence.Date = System.DateTime.Now;
                logSequence.Status = 1;
                logSequence.IsActive = true;
                if (_sosSequence.SequenceLogbooks == null)
                {
                    _sosSequence.SequenceLogbooks = new List<SOSSequenceLogbook>();
                }

                _sosSequence.SequenceLogbooks.Add(logSequence);

                var Gen_sosSequence = await SOSHubServices.GenerateSequence(SOSHubId, _sosSequence);

                Snackbar.Clear();
                Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                if (Gen_sosSequence != 0)
                {
                    Snackbar.Add($"{Localizer["_sosSequenceGeneratedSucces"]}", Severity.Info);
                    ShowPagesGenerate = false;
                    NavigationManager.NavigateTo($"/soshoe/Sequence/Details/{_sosSequence.SOSSequenceId}");
                    _sosSequence = new SOSSequence();
                    //Pregutar si quiere ver el analisis generado
                }
                else
                {
                    Snackbar.Add($"{Localizer["Fail_sosSequenceGeneratedSucces"]}", Severity.Error);
                }

                StateHasChanged();

            }
            else
            {
                if (ReviewerSequenceId == 0 || ApproverSequenceId == 0)
                {
                    bool? result = await DialogService.ShowMessageBox(
                       "Warning",
                       string.IsNullOrEmpty(_sosSequence.OperationName) ? "Es necesario el aproador" : "Es necesario seleccionar el editor (elaboro)!",
                       yesText: "Ok!");
                    var state = result == null ? "Canceled" : "Deleted!";
                    StateHasChanged();
                }
                else if (string.IsNullOrEmpty(_sosSequence.ProcessName) || string.IsNullOrEmpty(_sosSequence.InternalControlNumber))
                {
                    bool? result = await DialogService.ShowMessageBox(
                      "Warning",
                      string.IsNullOrEmpty(_sosSequence.ProcessName) ? "Es necesario el nombre de Proceso" : "Es necesario el nombre del proceso!",
                      yesText: "Ok!");
                    var state = result == null ? "Canceled" : "Deleted!";
                    StateHasChanged();
                }
                else if (string.IsNullOrEmpty(_sosSequence.OperationName))
                {
                    bool? result = await DialogService.ShowMessageBox(
                      "Warning",
                       "Es necesario el nombre de operacion!",
                      yesText: "Ok!");
                    var state = result == null ? "Canceled" : "Deleted!";
                    StateHasChanged();
                }
                else
                {
                    logSequence.NoRevision = 0;
                    logSequence.ApproverId = ApproverSequenceId;
                    logSequence.ReviewerId = ReviewerSequenceId;
                    logSequence.Date = System.DateTime.Now;
                    logSequence.Status = 1;
                    logSequence.IsActive = true;
                    if (_sosSequence.SequenceLogbooks == null)
                    {
                        _sosSequence.SequenceLogbooks = new List<SOSSequenceLogbook>();
                    }
                    _sosSequence.SequenceLogbooks.Add(logSequence);

                    var Gen_sosSequence = await SOSHubServices.GenerateSequence(SOSHubId, _sosSequence);

                    Snackbar.Clear();
                    Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                    if (Gen_sosSequence != 0)
                    {
                        Snackbar.Add($"{Localizer["_sosSequenceGeneratedSucces"]}", Severity.Info);
                        ShowPagesGenerate = false;
                        NavigationManager.NavigateTo($"/soshoe/Sequence/Details/{Gen_sosSequence}");
                        _sosSequence = new SOSSequence();
                        //Pregutar si quiere ver el analisis generado
                    }
                    else
                    {
                        Snackbar.Add($"{Localizer["Fail_sosSequenceGeneratedSucces"]}", Severity.Error);
                    }

                    StateHasChanged();
                }
            }


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
            // Añadir más casos según sea necesario
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
            // Añadir más casos según sea necesario
        }

        private MudMessageBox _DeleteAnalysis;
        private MudMessageBox _DeleteCombination;
        private MudMessageBox _DeleteDistribution;
        private MudMessageBox _DeleteFlow;
        private MudMessageBox _DeleteSequence;

        public async void Delete<T>(int id) where T : class
        {
            if (typeof(T) == typeof(SOSAnalysis))
            {
                bool? result = await _DeleteAnalysis.Show();
                var confirm = result is null ? "Canceled" : "Deleted!";
                StateHasChanged();

                if (confirm == "Deleted!")
                {
                    _sosHub.SOSAnalysis.RemoveAll(Analysis => Analysis.SOSAnalysisId == id);
                    await SOSAnalysisServices.DeleteSOSAnalysis(id);
                }
            }
            else if (typeof(T) == typeof(SOSCombination))
            {
                bool? result = await _DeleteCombination.Show();
                var confirm = result is null ? "Canceled" : "Deleted!";
                StateHasChanged();

                if (confirm == "Deleted!")
                {
                    _sosHub.SOSCombination.RemoveAll(combination => combination.SOSCombinationId == id);
                    await SOSAnalysisServices.DeleteSOSAnalysis(id);
                }
            }
            else if (typeof(T) == typeof(SOSDistribution))
            {
            }
            else if (typeof(T) == typeof(SOSFlow))
            {
            }
            else if (typeof(T) == typeof(SOSSequence))
            {
                bool? result = await _DeleteAnalysis.Show();
                var confirm = result is null ? "Canceled" : "Deleted!";
                StateHasChanged();

                if (confirm == "Deleted!")
                {
                    _sosHub.SOSSequence.RemoveAll(Sequence => Sequence.SOSSequenceId == id);
                    await SOSSequenceServices.DeleteSOSSequence(id);
                }
            }


        }

        private int selectedRowNumber = -1;
        private int SosDocId = -1;
        private MudTable<Object> SelectTableEventDocument;

        private void RowClickEventDocument(TableRowClickEventArgs<object> args)
        {


            if (selectedRowNumber == SelectTableEventDocument.Items.ToList().IndexOf(args.Item))
            {
                //HoeDetails(args.Item.SOSHubId);
                //aqui va los details

                if (args.Item is SOSAnalysis)
                {
                    NavigationManager.NavigateTo($"/soshoe/Analysis/Details/{SosDocId}");
                }
                else if (args.Item is SOSCombination)
                {
                    NavigationManager.NavigateTo($"/soshoe/Combination/Details/{SosDocId}");
                }
                else if (args.Item is SOSDistribution)
                {
                    NavigationManager.NavigateTo($"/soshoe/Distribution/Details/{SosDocId}");
                }
                else if (args.Item is SOSFlow)
                {
                    NavigationManager.NavigateTo($"/soshoe/Flow/Details/{SosDocId}");
                }
                else if (args.Item is SOSSequence)
                {
                    NavigationManager.NavigateTo($"/soshoe/Sequence/Details/{SosDocId}");
                }
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

                selectedRowNumber = rowNumber;
                return "selected";
            }
            else
            {
                return string.Empty;
            }
        }
        #endregion

    }
}