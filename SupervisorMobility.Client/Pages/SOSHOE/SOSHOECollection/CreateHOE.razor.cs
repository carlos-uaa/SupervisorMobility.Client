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
    public partial class CreateHOE
    {
        #region Variables
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

        //Commentaries
        public class ItemModel
        {
            public string Commentary { get; set; }
        }

        //Common direction
        private static string DefaultDragClass = "relative rounded-lg border-2 border-dashed py-2 mt-4 mud-width-full mud-height-full z-10";
        private string DragClass = DefaultDragClass;

        private List<FileToDisplay> fileNames = new List<FileToDisplay>();
        private List<IBrowserFile> fileNames2 = new List<IBrowserFile>();
        private int height = 50;


        private int maxAllowedFiles = 10;
        private long maxFileSize = long.MaxValue;

        private class FileToDisplay
        {
            public string name { get; set; }
            public string ftype { get; set; }
            public string message { get; set; }
        }


        List<ItemModel> items = new List<ItemModel>();


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

        private string resourceType = "";

        List<Material> _materials { get; set; } = new();
        private List<Material> _filteredMaterials = new();
        List<int> _materialsIds = new();
        private string selectedMaterialName;
        private string newMaterialName;

        List<Equipment> _equipment = new();
        private List<Equipment> _filteredEquipment = new();
        List<int> _equipmentIds = new();
        private string selectedEquipmentName;
        private string newEquipmentName;

        private List<Tool> _tools = new();
        private List<Tool> _filteredTools = new();
        private List<int> _toolsIds = new();
        private string selectedToolName;
        private string newToolName;

        private DialogOptions dialogResourcesOptions = new() { CloseOnEscapeKey = true, MaxWidth = MaxWidth.Small, FullWidth = true, CloseButton = true };

        private bool visibleResources = false;

        #endregion

        protected async override Task OnInitializedAsync()
        {

            _links = new List<BreadcrumbItem>
                {
                    new BreadcrumbItem(text: Localizer["home"], href: "/"),
                    new BreadcrumbItem(text: Localizer["hoe"], href: "", disabled: true)
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



            AddItem();
            SetUserInfo();
            AddRawItem();

            StateHasChanged();
        }

        #region Tools

        private async Task<IEnumerable<string>> SearchTools(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return _filteredTools.Select(t => t.ToolName);

            return _filteredTools.Where(x => x.ToolName.Contains(value, StringComparison.OrdinalIgnoreCase)).Select(t => t.ToolName);
        }

        private bool IsExistingTool => _filteredTools.Any(t => t.ToolName.Equals(selectedToolName, StringComparison.OrdinalIgnoreCase));

        private void AddSelectedTool()
        {
            if (!string.IsNullOrWhiteSpace(selectedToolName))
            {
                var tool = _filteredTools.FirstOrDefault(t => t.ToolName.Equals(selectedToolName, StringComparison.OrdinalIgnoreCase));
                if (tool != null && !_toolsIds.Contains(tool.ToolId))
                {
                    _toolsIds.Add(tool.ToolId);
                    _filteredTools.Remove(tool);

                    selectedToolName = string.Empty;
                }
            }
        }

        private void RemoveTool(int toolId)
        {
            var tool = _tools.FirstOrDefault(t => t.ToolId == toolId);
            if (tool != null)
            {
                _toolsIds.Remove(toolId);
                _filteredTools.Add(tool);
                _filteredTools = _filteredTools.OrderBy(d => d.ToolName).ToList();

            }
        }

        private async void HandleToolCreated(bool isCreated)
        {
            Snackbar.Clear();
            Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
            if (isCreated)
            {
                newToolName = string.Empty;
                visibleResources = false;
                resourceType = "";
                _tools = await ToolsServices.GetTools();
                _tools = _tools.OrderBy(d => d.ToolName).ToList();

                _filteredTools = new List<Tool>(_tools);
                _filteredTools.RemoveAll(tool => _toolsIds.Contains(tool.ToolId));
                _filteredTools = _filteredTools.OrderBy(d => d.ToolName).ToList();

                Snackbar.Add($"{Localizer1["ToolCreateSucces"]}", Severity.Info);
                StateHasChanged();
            }
            else
            {
                Snackbar.Add($"{Localizer1["ToolCreateError"]}", Severity.Error);
            }
        }


        #endregion

        #region Material

        private async Task<IEnumerable<string>> SearchMaterials(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return _filteredMaterials.Select(t => t.MaterialName);

            return _filteredMaterials.Where(x => x.MaterialName.Contains(value, StringComparison.OrdinalIgnoreCase)).Select(t => t.MaterialName);
        }

        private bool IsExistingMaterial => _filteredMaterials.Any(t => t.MaterialName.Equals(selectedMaterialName, StringComparison.OrdinalIgnoreCase));

        private void AddSelectedMaterial()
        {
            if (!string.IsNullOrWhiteSpace(selectedMaterialName))
            {
                var material = _filteredMaterials.FirstOrDefault(t => t.MaterialName.Equals(selectedMaterialName, StringComparison.OrdinalIgnoreCase));
                if (material != null && !_materialsIds.Contains(material.MaterialId))
                {
                    _materialsIds.Add(material.MaterialId);
                    _filteredMaterials.Remove(material);

                    selectedMaterialName = string.Empty;
                }
            }
        }

        private void RemoveMaterial(int materialId)
        {
            var material = _materials.FirstOrDefault(t => t.MaterialId == materialId);
            if (material != null)
            {
                _materialsIds.Remove(materialId);
                _filteredMaterials.Add(material);
                _filteredMaterials = _filteredMaterials.OrderBy(d => d.MaterialName).ToList();

            }
        }

        private async void HandleMaterialCreated(bool isCreated)
        {
            Snackbar.Clear();
            Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
            if (isCreated)
            {
                newMaterialName = string.Empty;
                visibleResources = false;
                resourceType = "";
                _materials = await MaterialsServices.GetMaterials();
                _materials = _materials.OrderBy(d => d.MaterialName).ToList();

                _filteredMaterials = new List<Material>(_materials);
                _filteredMaterials.RemoveAll(material => _materialsIds.Contains(material.MaterialId));
                _filteredMaterials = _filteredMaterials.OrderBy(d => d.MaterialName).ToList();

                Snackbar.Add($"{Localizer1["MaterialCreateSucces"]}", Severity.Info);
                StateHasChanged();
            }
            else
            {
                Snackbar.Add($"{Localizer1["MaterialCreateError"]}", Severity.Error);
            }
        }


        #endregion

        #region Equipment

        private async Task<IEnumerable<string>> SearchEquipment(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return _filteredEquipment.Select(t => t.EquipmentName);

            return _filteredEquipment.Where(x => x.EquipmentName.Contains(value, StringComparison.OrdinalIgnoreCase)).Select(t => t.EquipmentName);
        }

        private bool IsExistingEquipment => _filteredEquipment.Any(t => t.EquipmentName.Equals(selectedEquipmentName, StringComparison.OrdinalIgnoreCase));

        private void AddSelectedEquipment()
        {
            if (!string.IsNullOrWhiteSpace(selectedEquipmentName))
            {
                var equipment = _filteredEquipment.FirstOrDefault(t => t.EquipmentName.Equals(selectedEquipmentName, StringComparison.OrdinalIgnoreCase));
                if (equipment != null && !_equipmentIds.Contains(equipment.EquipmentId))
                {
                    _equipmentIds.Add(equipment.EquipmentId);
                    _filteredEquipment.Remove(equipment);

                    selectedEquipmentName = string.Empty;
                }
            }
        }

        private void RemoveEquipment(int equipmentId)
        {
            var equipment = _equipment.FirstOrDefault(t => t.EquipmentId == equipmentId);
            if (equipment != null)
            {
                _equipmentIds.Remove(equipmentId);
                _filteredEquipment.Add(equipment);
                _filteredEquipment = _filteredEquipment.OrderBy(d => d.EquipmentName).ToList();

            }
        }

        private async void HandleEquipmentCreated(bool isCreated)
        {
            Snackbar.Clear();
            Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
            if (isCreated)
            {
                newEquipmentName = string.Empty;
                visibleResources = false;
                resourceType = "";
                _equipment = await EquipmentsServices.GetEquipments();
                _equipment = _equipment.OrderBy(d => d.EquipmentName).ToList();

                _filteredEquipment = new List<Equipment>(_equipment);
                _filteredEquipment.RemoveAll(equipment => _equipmentIds.Contains(equipment.EquipmentId));
                _filteredEquipment = _filteredEquipment.OrderBy(d => d.EquipmentName).ToList();

                Snackbar.Add($"{Localizer1["EquipmentCreateSucces"]}", Severity.Info);
                StateHasChanged();
            }
            else
            {
                Snackbar.Add($"{Localizer1["EquipmentCreateError"]}", Severity.Error);
            }
        }


        #endregion

        private void OpenResourceModal(string title)
        {
            resourceType = title;
            if(resourceType == "Tools")
            {
                newToolName = selectedToolName;
                selectedToolName = string.Empty;
            }
            else if(resourceType == "Equipment")
            {
                newEquipmentName = selectedEquipmentName;
                selectedEquipmentName = string.Empty;
            }
            else if(resourceType == "Materials")
            {
                newMaterialName = selectedMaterialName;
                selectedMaterialName = string.Empty;
            }
            visibleResources = true;
        }

        private void CloseResourcesDialog()
        {
            if (resourceType == "Tools")
            {
                newToolName = string.Empty;
            }
            else if (resourceType == "Equipment")
            {
                newEquipmentName = string.Empty;
            }
            else if (resourceType == "Materials")
            {
                newMaterialName = string.Empty;
            }
            visibleResources = false;
            resourceType = "";
        }
    
        //Create SOS HUB and validations
        #region Create SOSHUB
        private string ValidateSosHubForm()
        {
            if (RawAnalisis.Count == 0 && _sosHub.Sections.Count == 0)
            {
                return "Write down at least one analysis first";
            }
            if (string.IsNullOrEmpty(_sosHub.ProcessSheet))
            {
                return "Write down the Process Sheet plan first";
            }
            if (productId == new int())
            {
                return "First select a product!";
            }
            if (string.IsNullOrEmpty(_sosHub.SourcePlan))
            {
                return "Write down the source plan first";
            }
            if (string.IsNullOrEmpty(_sosHub.Plan))
            {
                return "Write down the plan first";
            }
            if (string.IsNullOrEmpty(_sosHub.Status))
            {
                return "First select a status!";
            }
            if (string.IsNullOrEmpty(_sosHub.OtherInformation))
            {
                return "First write down Other information first!";
            }
            if (departmentId == new int())
            {
                return "First select a Department!";
            }
            if (plantId == new int())
            {
                return "First select a Plant!";
            }
            if (areaId == new int())
            {
                return "First select a Area!";
            }
            if (supervisorOwnerId == new int())
            {
                return "First select a Owner!";
            }
            if (supervisorEditorId == new int())
            {
                return "First select a Editor!";
            }
            return string.Empty;
        }

        private async Task CreateNewSOSHub()
        {
            Snackbar.Clear();
            Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;

            var validationMessage = ValidateSosHubForm();
            if (!string.IsNullOrEmpty(validationMessage))
            {
                Snackbar.Add(validationMessage, Severity.Warning);
                return;
            }
            _sosHub.AppliedModelId = productId;
            _sosHub.IsActive = true;
            _sosHub.OwnerId = supervisorOwnerId;
            _sosHub.EditorId = supervisorEditorId;
            _sosHub.TrainingTime = $"{cycleId} {(cycleId == 1 ? "cycle" : "cycles")}";
            _sosHub.CreatedDate = createdDateTime;
            _sosHub.ModifiedDate = modifiedDateTime;
            _sosHub.DepartmentId = departmentId;
            _sosHub.PlantId = plantId;
            _sosHub.AreaId = areaId;
            _sosHub.ToolsUsed = _tools.Where(tool => _toolsIds.Contains(tool.ToolId)).ToList();
            _sosHub.MaterialsUsed = _materials.Where(material => _materialsIds.Contains(material.MaterialId)).ToList();
            _sosHub.SafetyEquipment = _equipment.Where(equipment => _equipmentIds.Contains(equipment.EquipmentId)).ToList();

            if(_sosHub.Sections.Count == 0 && RawAnalisis.Count > 0)
            {
                foreach (var raw in RawAnalisis)
                {
                    AnalysisBkup analysisBkupToADD = new AnalysisBkup();
                    analysisBkupToADD.Text = raw;
                    analysisBkupToADD.IsActive = true;
                    _sosHub.AnalysesBkup.Add(analysisBkupToADD);
                }
            }

            if (!(items == null || !items.Any()))
            {
                foreach (var item in items)
                {
                    if (string.IsNullOrEmpty(item.Commentary))
                    {
                        Snackbar.Add($"Write down the commentary first!", Severity.Warning);
                        return;
                    }
                    var processSheetCommentary = new Commentary
                    {
                        ComentaryId = 0,
                        Comment = item.Commentary,
                        IsActive = true
                    };
                    _sosHub.ProcessSheetCommentary.Add(processSheetCommentary);
                }
            }

            var result = await SOSHubServices.CreateSOScollection(_sosHub);

            if (result != null)
            {
                Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                Snackbar.Add($"SOS Created", Severity.Info);

                _sosHub = result;
                _ = await UploadEvidence();

                NavigationManager.NavigateTo("/hoe");
            }
            else
                await JSRuntime.InvokeVoidAsync("alert", "Error en los datos!");

        }

        public async void SetUserInfo()
        {
            _products = await ProductsServices.GetProducts();
            _sosHub.Plan = "[Current]";
            _sosHub.SourcePlan = "[Current]";

            _tools = await ToolsServices.GetTools();
            _tools = _tools.OrderBy(t => t.ToolName).ToList();
            _filteredTools = new List<Tool>(_tools);

            _equipment = await EquipmentsServices.GetEquipments();
            _equipment = _equipment.OrderBy(e => e.EquipmentName).ToList();
            _filteredEquipment = new List<Equipment>(_equipment);

            _materials = await MaterialsServices.GetMaterials();
            _materials = _materials.OrderBy(m => m.MaterialName).ToList();
            _filteredMaterials = new List<Material>(_materials);

            _plants = await PlantServices.GetPlants();
            _plants = _plants.OrderBy(p => p.Description).ToList();

            _departments = await DepartmentServices.GetDepartments();
            _departments = _departments.OrderBy(d => d.Description).ToList();


            if (user.UserType == 1)
            {
                _supervisors = await UsersService.GetUsersByType( 3, false, false);
                _supervisors = _supervisors.OrderBy(s => s.Name).ToList();
            }
            if (user.UserType == 2)
            {
                plantId = (int)user.PlantId;
                areaId = 0;

                _areas = user.Areas.ToList();
                foreach (var sv in user.Subordinates.ToList())
                {
                    _supervisors.Add(sv);
                }

            }
            else if (user.UserType == 3)
            {
                var plantId = (int)user.PlantId;
                var areaId = (int)user.AreaId;

                _areas = await AreaServices.GetAreas(plantId);
                _areas = _areas.OrderBy(a => a.Description).ToList();

                supervisorOwnerId = user.UserId;
                supervisorEditorId = user.UserId;

                _supervisors.Add(user);
            }
            StateHasChanged();

        }

        #endregion

        //Show areas
        #region Areas
            private async void ShowAreas()
        {
            areaId = 0;
            _areas = await AreaServices.GetAreas(plantId);
            _areas = _areas.OrderBy(a => a.Description).ToList();
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
        public void AnalyzeText()
        {
            //segments.Clear();
            //allCriticalPoints.Clear();
            //BaseText = Regex.Replace(_sosHub.OperationDescription, @"\*", "").ToString();

            //var segmentTexts = _sosHub.OperationDescription.Split(new[] { '-' }, StringSplitOptions.RemoveEmptyEntries);

            //foreach (var segmentText in segmentTexts)
            //{
            //    var segment = new Segment
            //    {
            //        Analysis = segmentText
            //    };

            //    var mainPointRegex = new Regex(@"#(.*?)#");
            //    var mainPointMatch = mainPointRegex.Match(segmentText);

            //    if (mainPointMatch.Success)
            //    {
            //        segment.MainPoint = mainPointMatch.Groups[1].Value.Trim();
            //    }
            //    else
            //    {
            //        segment.MainPoint = string.Empty;
            //    }

            //    var criticalPointsRegex = new Regex(@"\*(.*?)\*");
            //    var criticalPointMatches = criticalPointsRegex.Matches(segmentText);

            //    foreach (Match match in criticalPointMatches)
            //    {
            //        if (match.Success)
            //        {
            //            segment.CriticalPoints.Add(match.Groups[1].Value.Trim());
            //        }
            //    }

            //    segments.Add(segment);
            //}

            //allCriticalPoints = segments.SelectMany(segment => segment.CriticalPoints).ToList();
            //StateHasChanged();
        }

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
        private DialogOptions dialogCameraOptions = new() { CloseOnEscapeKey = true, MaxWidth = MaxWidth.Medium, FullWidth = true, CloseButton = true, DisableBackdropClick = true };

        private List<string> capturedImages = new List<string>();

        private bool visibleCamera = false;
        private int imageIndex = 0;

        private void OpenCameraDialog(int index)
        {
            imageIndex = index;
            visibleCamera = true;
        }

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
                if (imageIndex == 1)
                {
                    capturedImages.Add(imageData);
                }
            }
            visibleCamera = false;
            StateHasChanged();
            Stop();
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

        //Images
        #region Images
        private async Task AddImages(InputFileChangeEventArgs e, int type)
        {
            foreach (var file in e.GetMultipleFiles())
            {
                if (file.ContentType.StartsWith("image/"))
                {
                    using (Stream mediaStream = file.OpenReadStream(file.Size))
                    {
                        MemoryStream ms = new();
                        await mediaStream.CopyToAsync(ms);
                        string mediaUri = $"data:{file.ContentType};base64,{Convert.ToBase64String(ms.ToArray())}";

                        if (type == 1)
                        {
                            capturedImages.Add(mediaUri);
                        }
                    }
                }
            }
        }

        private void RemoveImage(int index, int imgIndex)
        {

            if (index >= 0 && index < capturedImages.Count)
            {
                if (imgIndex == 1)
                {
                    capturedImages.RemoveAt(index);
                }
            }
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

        #endregion

        //Upload Images Videos
        #region UploadEvidence
        private async Task<AsyncVoidMethodBuilder> UploadEvidence()
        {
            await UploadImages();
            await UploadVideos();
            await UploadFiles();

            return new AsyncVoidMethodBuilder();
        }


        private async Task UploadImages()
        {

            List<string> images = capturedImages;
            int sosHubId = _sosHub.SOSHubId;

            if (images.Count > 0)
            {
                foreach (var imageData in images)
                {
                    if (string.IsNullOrEmpty(imageData))
                    {
                        Snackbar.Clear();
                        Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                        Snackbar.Add("No image data to upload", Severity.Warning);
                        continue;
                    }

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

                    var imageBytes = Convert.FromBase64String(base64Data);

                    using var content = new MultipartFormDataContent();
                    var imageStream = new MemoryStream(imageBytes);
                    var fileContent = new StreamContent(imageStream);
                    fileContent.Headers.ContentType = new MediaTypeHeaderValue("image/png");

                    content.Add(fileContent, "\"file\"", "evidenceSosHub.png");


                    var result = await SOSHubServices.AddImageToSOSHub(content, sosHubId);

                    if (result is not null)
                    {
                        Snackbar.Configuration.MaxDisplayedSnackbars = 10;
                        Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                        Snackbar.Add("Image Added to HOE", Severity.Info);
                    }
                    else
                    {
                        Snackbar.Clear();
                        Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                        Snackbar.Add("Failed to upload Image to HOE", Severity.Error);
                    }

                }

                images.Clear();
            }

        }

        public async Task UploadVideos()
        {
            int sosHubId = _sosHub.SOSHubId;

            Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;

            if(MediaUris.Count > 0)
            {
                foreach (var item in MediaUris)
                {
                    var content = new MultipartFormDataContent();
                    string base64Data = item.Value.Item2.Split(',')[1];
                    if (string.IsNullOrEmpty(base64Data))
                    {
                        FileErrorMessage(item.Key);
                        continue;
                    }

                    try
                    {
                        var imageBytes = Convert.FromBase64String(base64Data);
                        var imageStream = new MemoryStream(imageBytes);
                        imageStream.Position = 0;
                        var fileContent = new StreamContent(imageStream);
                        fileContent.Headers.ContentType = new MediaTypeHeaderValue(item.Value.Item1);

                        content.Add(
                            content: fileContent,
                            name: "\"file\"",
                            fileName: item.Key);
                    }
                    catch (FormatException)
                    {
                        FileErrorMessage(item.Key);
                        continue;
                    }
                    catch (Exception)
                    {
                        Snackbar.Add($"Error converting video to uploadable data: {item.Key}", Severity.Error);
                    }

                    var result = await SOSHubServices.AddVideoToSOSHub(content, sosHubId);

                    if (result is not null)
                    {
                        Snackbar.Configuration.MaxDisplayedSnackbars = 10;
                        Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                        Snackbar.Add("Video Added to HOE", Severity.Info);
                    }
                    else
                    {
                        Snackbar.Clear();
                        Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                        Snackbar.Add("Failed to upload Video to HOE", Severity.Error);
                    }

                }
                MediaUris.Clear();
            }

        }

        private async Task UploadFiles()
        {
            int sosHubId = _sosHub.SOSHubId;
            Snackbar.Configuration.PositionClass = Defaults.Classes.Position.TopCenter;
            Console.WriteLine($" en carga {fileNames2.Count}");

            if(fileNames.Count > 0)
            {
                foreach (var file in fileNames2)
                {
                    using var content = new MultipartFormDataContent();
                    var fileContent = new StreamContent(file.OpenReadStream(maxFileSize));
                    fileContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);

                    content.Add(
                             content: fileContent,
                             name: "\"file\"",
                    fileName: file.Name);

                    var result = await SOSHubServices.AddCDToSOSHub(content, sosHubId);

                    if (result is not null)
                    {
                        Snackbar.Configuration.MaxDisplayedSnackbars = 10;
                        Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                        Snackbar.Add($"{file.Name} Added to HOE", Severity.Info);
                    }
                    else
                    {
                        Snackbar.Clear();
                        Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                        Snackbar.Add($"Failed to upload Evidence to HOE", Severity.Error);
                        break;
                    }
                }

                fileNames.Clear();
                fileNames2.Clear();
                StateHasChanged();

            }
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

        private async void AddVideos(InputFileChangeEventArgs e)
        {
            List<string[]> IgnoredFiles = new List<string[]>();
            string message;

            var files = e.GetMultipleFiles();
            foreach (var file in files)
            {
                string contentType = "";

                if (!file.ContentType.StartsWith("video/"))
                {
                    IgnoredFiles.Add(new string[] { file.Name, "Wrong Type" });
                    continue;
                }

                if (!MediaUris.Keys.Contains(file.Name))
                {
                    using (Stream mediaStream = file.OpenReadStream(file.Size))
                    {
                        MemoryStream ms = new();
                        await mediaStream.CopyToAsync(ms);
                        string MediaUri = $"data:{file.ContentType};base64,{Convert.ToBase64String(ms.ToArray())}";

                        MediaUris.Add(file.Name, (file.ContentType, MediaUri));
                    }
                }
            }

            if (IgnoredFiles.Count != 0)
            {
                string confirmMessage = $"The following files where ignored: ";
                foreach (string[] item in IgnoredFiles)
                {
                    confirmMessage += $"\n Name: {item[0]} \n Reason: {item[1]}";
                }
                await JSRuntime.InvokeAsync<bool>("confirm", confirmMessage);
            }

            StateHasChanged();
        }


        private void FileErrorMessage(string file)
        {
            Snackbar.Clear();
            Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
            Snackbar.Add($"File: {file} has invalid video data", Severity.Error);
        }

        private void DeleteItem(string key)
        {
            Console.WriteLine("Before deletion:");
            foreach (var uri in MediaUris)
            {
                Console.WriteLine($"{uri.Key}");
            }

            if (MediaUris.ContainsKey(key))
            {
                MediaUris.Remove(key);
                Console.WriteLine($"Deleted: {key}");
                StateHasChanged();
            }

            Console.WriteLine("After deletion:");
            foreach (var uri in MediaUris)
            {
                Console.WriteLine($"{uri.Key}");
            }

            InvokeAsync(StateHasChanged);
        }

        #endregion

        //Commentaries for process sheet
        #region Commentaries
        void AddItem()
        {
            items.Add(new ItemModel());
        }

        void RemoveItem(ItemModel item)
        {
            if (items.Count > 1)
            {
                items.Remove(item);
            }

        }

        #endregion

        //Common direction files
        #region CommonDirection

        private void OnInputFileChanged(InputFileChangeEventArgs e)
        {
            foreach (var file in e.GetMultipleFiles(maxAllowedFiles))
            {
                fileNames2.Add(file);
                fileNames.Add(new FileToDisplay() { name = file.Name, ftype = file.ContentType });
            }
            Console.WriteLine($"{fileNames2.Count}");


            height = 50 + (fileNames.Count * 33);
        }

        private async Task Clear()
        {
            fileNames.Clear();
            fileNames2.Clear();

            ClearDragClass();
            height = 50;
            await Task.Delay(100);
        }

        private void SetDragClass()
        {
            DragClass = $"{DefaultDragClass} mud-border-primary";
        }

        private void ClearDragClass()
        {
            DragClass = DefaultDragClass;
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

        //Analisis Steps Critical Points
        [Inject]
        private IDialogService DialogService { get; set; }
        public List<string> RawAnalisis { get; set; } = new List<string>();
        public List<string> RawAnalisisBk { get; set; } = new List<string>();

        private IEnumerable<string> _selectedValues = new List<string>();
      

        public string stepName { get; set; } = "";
        bool showAddStepDialog = false;
       

        private void ApplyHighlights(int sectionIndex, int analisisIndex)
        {
            var analisis = _sosHub.Sections[sectionIndex].Analyses[analisisIndex];
            var text = analisis.Text;
            var term = analisis.CriticalPoint;
            if (string.IsNullOrEmpty(term))
            {
                return;
            }

            var highlightedText = ReplaceInsensitive(text, term);
            analisis.Text = highlightedText;
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

        private string GetHighlightedText(int sectionIndex, int analisisIndex)
        {
            return _sosHub.Sections[sectionIndex].Analyses[analisisIndex].CriticalPoint;
        }

        void CreateBakup()
        {
            if (RawAnalisis.Count > 0)
            {
                RawAnalisisBk = ObjectCloner.ObjectCloner.DeepClone(RawAnalisis);
                foreach(var raw in RawAnalisisBk) { 
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
                // Obtener los textos de los análisis de la sección que se va a eliminar
                var textsToReinsert = item.Analyses.Select(analisis => analisis.Text).ToList();

                // Para cada texto, encontrar su posición correcta en RawAnalisis según RawAnalisisBk
                foreach (var text in textsToReinsert)
                {
                    int indexinsert = RawAnalisisBk.IndexOf(text);

                    // Encontrar el índice donde insertar en RawAnalisis
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
    }
}