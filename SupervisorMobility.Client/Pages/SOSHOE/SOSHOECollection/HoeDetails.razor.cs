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
        List<Department> _departments = new();
        int plantId = 0;
        int areaId = 0;
        int departmentId = 0;


        List<string> allCriticalPoints = new List<string>();
        private string BaseText = "Este es un texto de ejemplo donde los términos serán resaltados.";
        private string HighlightedText;
        private string SelectedTerm;


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

        #endregion

        protected async override Task OnInitializedAsync()
        {

            _links = new List<BreadcrumbItem>
                {
                    new BreadcrumbItem(text: Localizer["home"], href: "/"),
                    new BreadcrumbItem(text: Localizer["hoe"], href: "/sosHub"),
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



            SetUserInfo();

        }

    
        //Create SOS HUB and validations
        #region Create SOSHUB

        public async void SetUserInfo()
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

            StateHasChanged();
            _sosHub = await SOSHubServices.GetSOSHub(SOSHubId, true, true, true, true, true, true, true, true);


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



            areaId = (int)_sosHub.AreaId;
            departmentId = (int)_sosHub.DepartmentId;
            productId = (int)_sosHub.AppliedModelId;

            supervisorEditorId = (int)_sosHub.EditorId;
            supervisorOwnerId = (int)_sosHub.OwnerId;

            cycleId = GetCycleId(_sosHub.TrainingTime);
            StateHasChanged();

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
                throw new FormatException("El formato de TrainingTime no es válido.");
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


        //Highlight analysis text
        #region Highlight
        private void HighlightTerm(string term)
        {
            visibleStepsDialog = false;
            base.StateHasChanged();

            SelectedTerm = term;
            HighlightedText = ApplyHighlights(BaseText, allCriticalPoints, SelectedTerm);
            visibleStepsDialog = true;
            base.StateHasChanged();

        }

        private string ApplyHighlights(string text, List<string> terms, string highlightTerm)
        {

            if (string.IsNullOrEmpty(highlightTerm))
            {
                StateHasChanged();
                return text;
            }

            var normalizedText = Normalize(text);
            var normalizedHighlightTerm = Normalize(highlightTerm);

            var result = new StringBuilder(text);

            foreach (var term in terms)
            {
                var normalizedTerm = Normalize(term);

                if (normalizedTerm == normalizedHighlightTerm)
                {
                    result = new StringBuilder(ReplaceInsensitive(result.ToString(), term, $"<mark>{term}</mark>", normalizedTerm));
                }
            }
            StateHasChanged();

            return result.ToString();
        }

        private static string Normalize(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return string.Empty;
            }

            input = input.Normalize(NormalizationForm.FormD)
                         .Where(c => CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                         .Aggregate(new StringBuilder(), (sb, c) => sb.Append(c))
                         .ToString()
                         .ToLowerInvariant();
           
            return input;
        }

        private static string ReplaceInsensitive(string text, string search, string replacement, string normalizedSearch)
        {
            string normalizedText = Normalize(text);
            
            return Regex.Replace(normalizedText, Regex.Escape(normalizedSearch), m =>
            {
                int startIndex = m.Index;
                string originalMatch = text.Substring(startIndex, search.Length);
                return $"<mark>{originalMatch}</mark>";
            }, RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
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
    }
}