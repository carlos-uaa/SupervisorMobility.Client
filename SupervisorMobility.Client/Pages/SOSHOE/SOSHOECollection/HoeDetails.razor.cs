using BlazorCameraStreamer;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using MudBlazor;
using SupervisorMobility.Client.Data.Entities.SOSAnalysis_Process;
using SupervisorMobility.Client.Services.SOS_Services.ToolServices;
using System;
using System.Formats.Asn1;
using System.Globalization;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
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

        List<Material> _materials { get; set; } = new();
        List<int> _materialsIds = new();
        private string selectedMaterialName;

        List<Equipment> _equipment = new();
        List<int> _equipmentIds = new();
        private string selectedEquipmentName;

        private List<Tool> _tools = new();
        private List<int> _toolsIds = new();
        private string selectedToolName;

        int analysisTabsIndex = 0;


        //Analysis
        [Inject]
        private IDialogService DialogService { get; set; }
        public List<string> RawAnalisis { get; set; } = new List<string>();
        public List<string> RawAnalisisBk { get; set; } = new List<string>();

        private IEnumerable<string> _selectedValues = new List<string>();


        public string stepName { get; set; } = "";
        bool showAddStepDialog = false;


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
                    new BreadcrumbItem(text: Localizer["home"], href: "/SOSHOE"),
                    new BreadcrumbItem(text: Localizer["hoe"], href: "/SOSHOE/Hub"),
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

            _tools = await ToolsServices.GetTools();
            _tools = _tools.OrderBy(t => t.ToolName).ToList();

            _equipment = await EquipmentsServices.GetEquipments();
            _equipment = _equipment.OrderBy(e => e.EquipmentName).ToList();

            _materials = await MaterialsServices.GetMaterials();
            _materials = _materials.OrderBy(m => m.MaterialName).ToList();

            _plants = await PlantServices.GetPlants();
            _plants = _plants.OrderBy(p => p.Description).ToList();

            _departments = await DepartmentServices.GetDepartments();
            _departments = _departments.OrderBy(d => d.Description).ToList();
            _stations = await StationServices.GetStations();
            _stations = _stations.OrderBy(s => s.Description).ToList();
            StateHasChanged();
            _sosHub = await SOSHubServices.GetSOSHub(SOSHubId, true, true, true, true, true, true, true, true, includeDocuments:true);


            if (_sosHub.AnalysesBkup != null && _sosHub.AnalysesBkup.Count > 0)
            {
                foreach (var backup in _sosHub.AnalysesBkup)
                {
                    RawAnalisisBk.Add(backup.Text);
                    RawAnalisis.Add(backup.Text); 
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

            if(_sosHub.SafetyEquipment != null && _sosHub.SafetyEquipment.Count > 0)
            {
                _equipmentIds = _sosHub.SafetyEquipment.Select(s => s.EquipmentId).ToList();
            }
            if (_sosHub.ToolsUsed != null && _sosHub.ToolsUsed.Count > 0)
            {
                _toolsIds = _sosHub.ToolsUsed.Select(s => s.ToolId).ToList();
            }
            if (_sosHub.MaterialsUsed != null && _sosHub.MaterialsUsed.Count > 0)
            {
                _materialsIds = _sosHub.MaterialsUsed.Select(s => s.MaterialId).ToList();
            }

            if (_sosHub.PlantId != null)
            {
                plantId = (int)_sosHub.PlantId;
            }

            if (user.UserType == 1)
            {
                if(plantId != new int())
                {
                    _areas = await AreaServices.GetAreas(plantId);
                    _areas = _areas.OrderBy(a => a.Description).ToList();
                }
                _supervisors = await UsersService.GetUsersByType( 3, false, false);
                _supervisors = _supervisors.OrderBy(s => s.Name).ToList();
            }
            if (user.UserType == 2)
            {

                _areas = user.Areas.ToList();
                foreach (var sv in user.Subordinates.ToList())
                {
                    _supervisors.Add(sv);
                }

            }
            else if (user.UserType == 3)
            {
                _areas = await AreaServices.GetAreas(plantId);
                _areas = _areas.OrderBy(a => a.Description).ToList();

                _supervisors.Add(user);
            }

            if (plantId != 0 && areaId != 0)
            {
                _distributions = await DistributionServices.GetDistributionsWithCollections(plantId, areaId);
                _distributions = _distributions.OrderBy(d => d.Description).ToList();
            }

            distributionId = _sosHub.DistributionId ?? distributionId;
            areaId = _sosHub.AreaId ?? areaId;
            stationId = _sosHub.StationId ?? stationId;
            departmentId = _sosHub.DepartmentId ?? departmentId;
            productId = _sosHub.AppliedModelId ?? productId;
            supervisorEditorId = _sosHub.EditorId ?? supervisorEditorId;
            supervisorOwnerId = _sosHub.OwnerId ?? supervisorOwnerId;

            cycleId = _sosHub.TrainingTime != null ? GetCycleId(_sosHub.TrainingTime) : 0;

            StateHasChanged();
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

        private static string ReplaceInsensitive(string text, string search)
        {
            if (string.IsNullOrEmpty(search))
            {
                return text;
            }

            string normalizedText = Normalize(text);
            string normalizedSearch = Normalize(search);
            var result = Regex.Replace(normalizedText, Regex.Escape(normalizedSearch), m =>
            {
                int startIndex = normalizedText.IndexOf(m.Value, m.Index, StringComparison.OrdinalIgnoreCase);
                string originalMatch = text.Substring(startIndex, search.Length);
                return $"<mark>{originalMatch}</mark>";
            }, RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
            return result;
        }

        private string GetAnalisisText(int sectionIndex, int analisisIndex)
        {
            return _sosHub.Sections[sectionIndex].Analyses[analisisIndex].Text;
        }

        //private string GetHighlightedText(int sectionIndex, int analisisIndex)
        //{
        //    //return _sosHub.Sections[sectionIndex].Analyses[analisisIndex].CriticalPoint;
        //}

        void CreateBakup()
        {
            if (RawAnalisis.Count > 0)
            {
                RawAnalisisBk = ObjectCloner.ObjectCloner.DeepClone(RawAnalisis);
                foreach (var raw in RawAnalisisBk)
                {
                    AnalysisBkup analysisBkup = new AnalysisBkup();
                    analysisBkup.Text = raw;
                    analysisBkup.IsActive = true;
                    _sosHub.AnalysesBkup.Add(analysisBkup);
                }
            }
        }
        void RestoreBakup()
        {
            RawAnalisis = ObjectCloner.ObjectCloner.DeepClone(RawAnalisisBk);
            _sosHub.Sections.Clear();
            _sosHub.AnalysesBkup.Clear();
        }
        void AddRawItem()
        {
            RawAnalisis.Add("");
        }

        void RemoveRawItem(string item)
        {
            if (RawAnalisis.Count > 1)
            {
                RawAnalisis.Remove(item);
            }
        }
        void RemoveSectionItem(Section item)
        {

            if (_sosHub.Sections.Count > 0)
            {
                var textsToReinsert = item.Analyses.Select(analisis => analisis.Text).ToList();

                foreach (var text in textsToReinsert)
                {
                    int indexinsert = RawAnalisisBk.IndexOf(text);

                    int insertPosition = RawAnalisis
                        .Select((value, index) => new { Value = value, Index = index })
                        .Where(x => RawAnalisisBk.IndexOf(x.Value) >= indexinsert)
                        .Select(x => x.Index)
                        .DefaultIfEmpty(RawAnalisis.Count)
                        .First();
                    // Insertar el texto en la posición correcta
                    RawAnalisis.Insert(insertPosition, text);
                }

                // Eliminar la sección de Sections
                _sosHub.Sections.Remove(item);
            }
        }

        public void AddStep()
        {
            showAddStepDialog = true;
        }

        private void CloseStepDialog()
        {
            showAddStepDialog = false;
        }
        public async void confirmStep()
        {

            if (!string.IsNullOrEmpty(stepName))
            {

                Section SectiontoAdd = new Section();
                foreach (string item in _selectedValues)
                {
                    Analysis ToAdd = new Analysis();
                    ToAdd.Text = item;
                    SectiontoAdd.Analyses.Add(ToAdd);
                    RawAnalisis.Remove(item);
                }

                SectiontoAdd.Step = stepName;

                _sosHub.Sections.Add(SectiontoAdd);

                stepName = string.Empty;
                _selectedValues = new List<string>();
                CloseStepDialog();
            }
            else
            {
                bool? result = await DialogService.ShowMessageBox(
                   "Warning",
                    "Es necesario el texto!",
                   yesText: "Ok!");
                var state = result == null ? "Canceled" : "Deleted!";
                StateHasChanged();
            }

        }
        #endregion

        void HoeHistory()
        {
            NavigationManager.NavigateTo($"/SOSHOE/Hub/Details/{SOSHubId}/History");
        }
    }
}