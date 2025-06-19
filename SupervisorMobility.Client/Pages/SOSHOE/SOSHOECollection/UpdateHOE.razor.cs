using BlazorCameraStreamer;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using MudBlazor;
using System.Globalization;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;

namespace SupervisorMobility.Client.Pages.SOSHOE.SOSHOECollection
{
    public partial class UpdateHOE
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

        private string BaseText = "Este es un texto de ejemplo donde los t�rminos ser�n resaltados.";
        private string HighlightedText;

        public string productSide = string.Empty;
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
        public Dictionary<string, (string, string)> PreviousVideo = new Dictionary<string, (string, string)>();
        public Dictionary<string, (string, string)> MediaUris = new Dictionary<string, (string, string)>();
        public List<int> OldVideoRemoved = new List<int>();

        //Commentaries
        public class ItemModel
        {
            public int CommentaryId { get; set; }
            public string Commentary { get; set; }
            public bool IsActive { get; set; } = true;

        }

        List<ItemModel> items = new List<ItemModel>();
        List<ItemModel> tempItems = new List<ItemModel>();


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


        //Users
        List<User> _supervisors { get; set; } = new();
        List<User> _Seniorsupervisors { get; set; } = new();


        private readonly List<int> Cycles = Enumerable.Range(1, 3000).ToList();
        int cycleId = 0;



        //Show Evidence 
        private DialogOptions dialogEvidenceOptions = new() { CloseOnEscapeKey = true, MaxWidth = MaxWidth.Medium, FullWidth = true, CloseButton = true };

        private bool visibleEvidence = false;

        private int photoIndex = 0;
        private void OpenEvidenceDialog(int index, bool OldImgOrigin = true)
        {
            if (!OldImgOrigin) index += (PreviousImages.Count);
            photoIndex = index;
            visibleEvidence = true;

        }


        private DialogOptions dialogFileExplorerOptions = new() { CloseOnEscapeKey = true, MaxWidth = MaxWidth.Large, FullWidth = true, CloseButton = true };
        private bool IsGOS = false;
        bool showFileExplorer = false;
        private List<(object, string)> finalFilesSelection = new List<(object, string)>();

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

        int analysisTabsIndex = 0;

        #endregion
        //Loading
        private IList<string> _sourceMsgLoading = new List<string>();
        private IList<Color> _Colors = new List<Color>() { Color.Default, Color.Primary, Color.Secondary, Color.Success, Color.Info, Color.Default, Color.Primary, Color.Secondary, Color.Success, Color.Info };
        public bool ShowLoading = true;
        public bool AddAnalisis = false;
        public bool DeleteAnalisis = false;
        public bool UpdateButton = false;

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
                new BreadcrumbItem(text: Localizer["homeSOSHOE"], href: "/soshoe"),
                new BreadcrumbItem(text: Localizer["hoe"], href: "/soshoe/Hub" ),
                new BreadcrumbItem(text: Localizer["update"], href: "", disabled:true )
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
                if (tool != null)
                {
                    //_toolsIds.Add(tool.ToolId);
                    ToolUsed? toolItem = _sosHub.ToolsUsed.FirstOrDefault(t => t.ToolId == tool.ToolId);
                    if (toolItem != null)
                    {
                        toolItem.IsActive = true;
                    }
                    else
                    {
                        ToolUsed ToolToAdd = new ToolUsed();

                        ToolToAdd.ToolId = tool.ToolId;
                        ToolToAdd.Tool = tool;
                        ToolToAdd.Quantity = 1;
                        ToolToAdd.IsActive = true;

                        _sosHub.ToolsUsed?.Add(ToolToAdd);
                    }

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
                var toolItem = _sosHub.ToolsUsed.FirstOrDefault(t => t.ToolId == toolId);
                toolItem.IsActive = false;

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
                return _filteredMaterials.Select(t => t.PartName);

            return _filteredMaterials.Where(x => x.PartName.Contains(value, StringComparison.OrdinalIgnoreCase)).Select(t => t.PartName);
        }

        private bool IsExistingMaterial => _filteredMaterials.Any(t => t.PartName.Equals(selectedMaterialName, StringComparison.OrdinalIgnoreCase));

        private void AddSelectedMaterial()
        {
            if (!string.IsNullOrWhiteSpace(selectedMaterialName))
            {
                var material = _filteredMaterials.FirstOrDefault(t => t.PartName.Equals(selectedMaterialName, StringComparison.OrdinalIgnoreCase));
                if (material != null)
                {
                    MaterialUsed? materialItem = _sosHub.MaterialsUsed.FirstOrDefault(t => t.MaterialId == material.MaterialId);
                    if (materialItem != null)
                    {
                        materialItem.IsActive = true;
                    }
                    else
                    {
                        MaterialUsed materialToAdd = new MaterialUsed();

                        materialToAdd.MaterialId = material.MaterialId;
                        materialToAdd.Material = material;
                        materialToAdd.Quantity = 1;
                        materialToAdd.IsActive = true;

                        _sosHub.MaterialsUsed?.Add(materialToAdd);
                    }

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
                var materialItem = _sosHub.MaterialsUsed.FirstOrDefault(t => t.MaterialId == materialId);
                materialItem.IsActive = false;

                _filteredMaterials.Add(material);
                _filteredMaterials = _filteredMaterials.OrderBy(d => d.PartName).ToList();
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
                _materials = _materials.OrderBy(d => d.PartName).ToList();

                _filteredMaterials = new List<Material>(_materials);
                _filteredMaterials.RemoveAll(material => _materialsIds.Contains(material.MaterialId));
                _filteredMaterials = _filteredMaterials.OrderBy(d => d.PartName).ToList();

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
            if (resourceType == "Tools")
            {
                newToolName = selectedToolName;
                selectedToolName = string.Empty;
            }
            else if (resourceType == "Equipment")
            {
                newEquipmentName = selectedEquipmentName;
                selectedEquipmentName = string.Empty;
            }
            else if (resourceType == "Materials")
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

        //Update SOS HUB and validations
        #region Update SOSHUB
        private string ValidateSosHubForm()
        {


            //if (RawAnalisis.Count == 0 && _sosHub.Sections.Count == 0 && _sosHub.SOSDistribution.Count == 0)
            //{
            //    return "Write down at least one analysis first";
            //}

            if (string.IsNullOrEmpty(_sosHub.ProcessSheet))
            {
                return "Write down the Process Sheet plan first";
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
            if (stationId == new int())
            {
                return "First select a Station!";
            }
            if (plantId == new int())
            {
                return "First select a Plant!";
            }
            if (areaId == new int())
            {
                return "First select a Area!";
            }
            if (distributionId == new int())
            {
                return "First select a Distribution!";
            }


            if (_sosHub.ApproverOwners.Count == 0)
            {
                return "First add one Owner!";
            }

            if (_sosHub.ReviewerEditors.Count == 0)
            {
                return "First add one Editor!";
            } 
            
            if (_sosHub.AppliedModels.Count == 0)
            {
                return "First add one Product!";
            }
            return string.Empty;
        }

        private async Task UpdateSOSHub()
        {
            UpdateButton = true;
            Snackbar.Clear();
            Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;

            var validationMessage = ValidateSosHubForm();
            if (!string.IsNullOrEmpty(validationMessage))
            {
                Snackbar.Add(validationMessage, Severity.Warning);
                UpdateButton = false;
                return;
            }

            //_sosHub.AppliedModelId = productId;
            _sosHub.IsActive = true;
            _sosHub.TrainingTime = $"{cycleId} {(cycleId == 1 ? "cycle" : "cycles")}";
            _sosHub.CreatedDate = createdDateTime;
            _sosHub.ModifiedDate = modifiedDateTime;
            _sosHub.PlantId = plantId;
            _sosHub.AreaId = areaId;
            _sosHub.DistributionId = distributionId;
            _sosHub.StationId = stationId;
            _sosHub.DepartmentId = departmentId;
            _sosHub.SafetyEquipment = _equipment.Where(equipment => _equipmentIds.Contains(equipment.EquipmentId)).ToList();


            var combinedList = RawAnalisisBk.Union(RawAnalisis).ToList();
            foreach (var raw in combinedList)
            {
                if (!_sosHub.AnalysesBkup.Any(a => a == raw))
                {
                    //    _sosHub.AnalysesBkup.Find(a => a.Text == raw).IsActive = true;
                    //}
                    //else
                    //{
                    AnalysisBkup analysisBkup = new AnalysisBkup();
                    analysisBkup.Text = raw.Text;
                    analysisBkup.IsActive = true;
                    _sosHub.AnalysesBkup.Add(analysisBkup);
                }
            }

            FinalizeList();

            await GenerateSOSHUBCommentaries();

            _sosHub.ModifiedDate = System.DateTime.Now;

            var result = await SOSHubServices.UpdateSOSHub(_sosHub);

            if (result != null)
            {
                Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                Snackbar.Add($"SOS Updated! (y)", Severity.Info);

                _sosHub = result;
                _ = await UploadEvidence();

                NavigationManager.NavigateTo("/soshoe/Hub");
            }
            else
                await JSRuntime.InvokeVoidAsync("alert", "Error al actualizar!");
            UpdateButton = false;
        }

        public async Task<AsyncVoidMethodBuilder> GenerateSOSHUBCommentaries()
        {
            _sosHub.ProcessSheetCommentary?.Clear();
            foreach (var item in items)
            {
                var processSheetCommentary = new Commentary
                {
                    CommentaryId = item.CommentaryId,
                    Comment = item.Commentary,
                    IsActive = item.IsActive
                };

                _sosHub.ProcessSheetCommentary?.Add(processSheetCommentary);
            }

            foreach (var item in tempItems)
            {
                var processSheetCommentary = new Commentary
                {
                    CommentaryId = 0,
                    Comment = item.Commentary,
                    IsActive = true
                };

                _sosHub.ProcessSheetCommentary?.Add(processSheetCommentary);
            }

            return new AsyncVoidMethodBuilder();
        }

        public async Task<AsyncVoidMethodBuilder> SetUserInfo()
        {
            _products = await ProductsServices.GetProducts();
            _tools = await ToolsServices.GetTools();
            _tools = _tools.OrderBy(t => t.ToolName).ToList();
            _filteredTools = new List<Tool>(_tools);

            _equipment = await EquipmentsServices.GetEquipments();
            _equipment = _equipment.OrderBy(e => e.EquipmentName).ToList();
            _filteredEquipment = new List<Equipment>(_equipment);

            _materials = await MaterialsServices.GetMaterials();
            _materials = _materials.OrderBy(m => m.PartName).ToList();
            _filteredMaterials = new List<Material>(_materials);

            _plants = await PlantServices.GetPlants();
            _plants = _plants.OrderBy(p => p.Description).ToList();

            _departments = await DepartmentServices.GetDepartments();
            _departments = _departments.OrderBy(d => d.Description).ToList();

            _stations = await StationServices.GetStations();
            _stations = _stations.OrderBy(s => s.Description).ToList();



            _sosHub = await SOSHubServices.GetSOSHub(SOSHubId, true, true, true, true, true, true, true, true, includePeople: true, includeDocuments: true, includeCollections: true, includeModel: true);
            StateHasChanged();

            if(_sosHub.StationId != 0)
                _sosHub.Station = _stations.Find(s => s.StationId == _sosHub.StationId);


            if (_sosHub.ProcessSheetCommentary != null && _sosHub.ProcessSheetCommentary.Count > 0)
            {
                foreach (var comment in _sosHub.ProcessSheetCommentary)
                {
                    if (comment.IsActive != null && comment.IsActive == true)
                    {
                        var item = new ItemModel
                        {
                            CommentaryId = comment.CommentaryId,
                            Commentary = comment.Comment,
                        };
                        items.Add(item);
                    }
                }
            }




            if (_sosHub.AnalysesBkup != null && _sosHub.AnalysesBkup.Count > 0)
            {
                var listTextSections = _sosHub.Sections.SelectMany(section => section.Analyses).Select(analysis => analysis.Text).ToList();

                foreach (var backup in _sosHub.AnalysesBkup)
                {
                    if (!listTextSections.Contains(backup.Text))
                    {
                        RawAnalisis.Add(backup);
                    }
                    RawAnalisisBk.Add(backup);
                }
            }

            if(_sosHub.SOSDistribution.Count > 0)
            {
                _sosHub.Sections = _sosHub.Sections.OrderBy(s => s.SecuenceDist).ToList();
            }

            if (_sosHub.Images != null && _sosHub.Images.Count > 0)
            {

                foreach (var sosImage in _sosHub.Images)
                {
                    var image = await SOSHubServices.ShowImageSosHub(sosImage.FileUploadId);
                    PreviousImages.Add((sosImage.FileUploadId, image));
                }
            }

            if (_sosHub.Videos != null && _sosHub.Videos.Count > 0)
            {
                foreach (var sosVideo in _sosHub.Videos)
                {
                    var video = await SOSHubServices.ShowVideoSosHub(sosVideo.FileUploadId);
                    PreviousVideo.Add(sosVideo.FileUploadId.ToString(), (sosVideo.FileName, video));
                }
            }

            if (_sosHub.SafetyEquipment != null && _sosHub.SafetyEquipment.Count > 0)
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

            PopulateList();

            _filteredTools = _filteredTools.Where(t => !_toolsIds.Contains(t.ToolId)).ToList();
            _filteredEquipment = _filteredEquipment.Where(e => !_equipmentIds.Contains(e.EquipmentId)).ToList();
            _filteredMaterials = _filteredMaterials.Where(m => !_materialsIds.Contains(m.MaterialId)).ToList();

            plantId = _sosHub.PlantId ?? plantId;
            areaId = _sosHub.AreaId ?? areaId;

            if (user.UserType == 1)
            {
                if (plantId != new int())
                {
                    _areas = await AreaServices.GetAreas(plantId);
                    _areas = _areas.OrderBy(a => a.Description).ToList();
                }

                _Seniorsupervisors = new();
                _Seniorsupervisors = await UsersService.GetUsersByUserTypeInPlant(plantId, 2, true, false);
                _Seniorsupervisors = _Seniorsupervisors
                     .Where(s => s.Areas != null && s.Areas.Any(a => a.AreaId == areaId))
                     .OrderBy(s => s.Name)
                     .ToList();



                _supervisors = await UsersService.GetUsersByUserTypeInPlantAndArea(plantId, areaId, 3, true, false);
                _supervisors = _supervisors.OrderBy(s => s.Name).ToList();

            }
            else if (user.UserType == 2)
            {
                _Seniorsupervisors = new List<User>();
                _Seniorsupervisors = await UsersService.GetUsersByUserTypeInPlant(plantId, 2, true, false);
                _Seniorsupervisors = _Seniorsupervisors
                     .Where(s => s.Areas != null && s.Areas.Any(a => a.AreaId == areaId))
                     .OrderBy(s => s.Name)
                     .ToList();

                _supervisors = new();
                _supervisors = await UsersService.GetSubordinates(user.UserId, true);
                _supervisors = _supervisors.OrderBy(s => s.Name).ToList();


                _areas = user.Areas?.ToList();
                _areas = _areas.OrderBy(a => a.Description).ToList();

            }
            else if (user.UserType == 3)
            {
                _Seniorsupervisors = new List<User>();
                _Seniorsupervisors = await UsersService.GetUsersByUserTypeInPlant(plantId, 2, true, false);
                _Seniorsupervisors = _Seniorsupervisors
                     .Where(s => s.Areas != null && s.Areas.Any(a => a.AreaId == areaId))
                     .OrderBy(s => s.Name)
                     .ToList();


                _supervisors = new();
                _supervisors = await UsersService.GetSubordinates(user.Superior.UserId, true);
                _supervisors = _supervisors.OrderBy(s => s.Name).ToList();

                _areas = await AreaServices.GetAreas(plantId);
                _areas = _areas.OrderBy(a => a.Description).ToList();

            }

            foreach (User ssv in _sosHub.ApproverOwners)
            {
                var index = _Seniorsupervisors.FindIndex(u => u.UserId == ssv.UserId);

                if (index >= 0)
                {
                    _Seniorsupervisors.RemoveAt(index);
                }

            }

            foreach (User sv in _sosHub.ReviewerEditors)
            {
                var index = _supervisors.FindIndex(u => u.UserId == sv.UserId);

                if (index >= 0)
                {
                    _supervisors.RemoveAt(index);
                }
            }
            
            foreach (Product model in _sosHub.AppliedModels)
            {
                var index = _products.FindIndex(u => u.ProductId == model.ProductId);

                if (index >= 0)
                {
                    _products.RemoveAt(index);
                }
            }


            if (plantId != 0 && areaId != 0)
            {
                _distributions = await DistributionServices.GetDistributionsWithCollections(plantId, areaId);
                _distributions = _distributions.OrderBy(d => d.Description).ToList();
            }

            distributionId = _sosHub.DistributionId ?? distributionId;
            departmentId = _sosHub.DepartmentId ?? departmentId;
            //productId = _sosHub.AppliedModelId ?? productId;

            stationId = _sosHub.StationId ?? stationId;

            productSide = _sosHub.Folio.Split('-')[2] ?? productSide;

            cycleId = _sosHub.TrainingTime != null ? GetCycleId(_sosHub.TrainingTime) : 0;
            StateHasChanged();

            return new AsyncVoidMethodBuilder();
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
                throw new FormatException("El formato de TrainingTime no es v�lido.");
            }
        }
        private async void ShowSupervisors()
        {
            GenerateFolio();


            distributionId = 0;
            _distributions.Clear();

            _Seniorsupervisors.Clear();
            _supervisors.Clear();
          

            if (user.UserType == 1)
            {
             
                _Seniorsupervisors = await UsersService.GetUsersByUserTypeInPlant(plantId, 2, true, false);
                _Seniorsupervisors = _Seniorsupervisors
                     .Where(s => s.Areas != null )
                     .OrderBy(s => s.Name)
                     .ToList();

               
                _supervisors = await UsersService.GetUsersByUserTypeInPlantAndArea(plantId, areaId, 3, true, false);
                _supervisors = _supervisors.OrderBy(s => s.Name).ToList();

            }
            else if (user.UserType == 2)
            {
                //_Seniorsupervisors = new List<User>();
                _Seniorsupervisors = await UsersService.GetUsersByUserTypeInPlant(plantId, 2, true, false);
                _Seniorsupervisors = _Seniorsupervisors
                     .Where(s => s.Areas != null && s.Areas.Any(a => a.AreaId == areaId))
                     .OrderBy(s => s.Name)
                     .ToList();

                //_supervisors = new();
                _supervisors = await UsersService.GetSubordinates(user.UserId, true);
                _supervisors = _supervisors.OrderBy(s => s.Name).ToList();

            }
            else if (user.UserType == 3)
            {
                //_Seniorsupervisors = new List<User>();
                _Seniorsupervisors = await UsersService.GetUsersByUserTypeInPlant(plantId, 2, true, false);
                _Seniorsupervisors = _Seniorsupervisors
                     .Where(s => s.Areas != null && s.Areas.Any(a => a.AreaId == areaId))
                     .OrderBy(s => s.Name)
                     .ToList();


                //_supervisors = new();
                _supervisors = await UsersService.GetSubordinates(user.Superior.UserId, true);
                _supervisors = _supervisors.OrderBy(s => s.Name).ToList();
            }


            _distributions = await DistributionServices.GetDistributionsWithCollections(plantId, areaId);
            _distributions = _distributions.OrderBy(d => d.Description).ToList();

            StateHasChanged();
        }

        #endregion
        #region Station
        private string newStationName;
        private string selectedStationName;

        private async Task<IEnumerable<Station>> SearchStation(string searchString)
        {
            if (string.IsNullOrWhiteSpace(searchString))
                return _stations.Select(t => t);

            return _stations.Where(x => x.Code.Contains(searchString, StringComparison.OrdinalIgnoreCase)).Select(t => t);
        }

       
        private async void HandleStationCreated(bool isCreated)
        {
            Snackbar.Clear();
            Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
            if (isCreated)
            {
                newStationName = string.Empty;
                visibleResources = false;
                resourceType = "";
                _stations = await StationServices.GetStations();
                _stations = _stations.OrderBy(d => d.Description).ToList();


                Snackbar.Add($"{Localizer1["StationCreateSucces"]}", Severity.Info);
                StateHasChanged();
            }
            else
            {
                Snackbar.Add($"{Localizer1["StationCreateError"]}", Severity.Error);
            }
        }


   

        private void StationTextChanged(string inputText)
        {

            selectedStationName = _stations.Any(s => s.Code == inputText || s.Description == inputText || inputText == $"{s.Code} - {s.Description}") ? "" : inputText;

            if (selectedStationName == "")
            {
                Station? stat = _stations.Find(s => s.Code == inputText || s.Description == inputText || inputText == $"{s.Code} - {s.Description}");
                if (stat != null)
                {

                    stationId = stat != null ? stat.StationId : 0;
                    _sosHub.Station = stat;

                    GenerateFolio();

                }
            }

            StateHasChanged();
        }
        private void StationDelete()
        {
            stationId = 0;
            selectedStationName = "";
        }

        private bool IsExistingStation => _stations.Any(t => t.Description.Equals(selectedStationName, StringComparison.OrdinalIgnoreCase));

        #endregion
        //Show areas
        #region Areas
        private async void ShowAreas()
        {
            areaId = 0;
            distributionId = 0;
            _distributions.Clear();
            _supervisors.Clear();
            _areas = await AreaServices.GetAreas(plantId);
            _areas = _areas.OrderBy(a => a.Description).ToList();
            StateHasChanged();
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
        private DialogOptions dialogCameraOptions = new() { CloseOnEscapeKey = true, MaxWidth = MaxWidth.Medium, FullWidth = true, CloseButton = true, DisableBackdropClick = true };

        private List<(int, string)> PreviousImages = new List<(int, string)>();//id, b64 string
        private List<string> capturedImages = new List<string>();
        private List<int> OldImageRemoved = new();

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
            await UpdateRemovedFiles();

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

            if (MediaUris.Count > 0)
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

            if (fileNames.Count > 0)
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

        private async Task UpdateRemovedFiles()
        {
            int failed = 0;
            int status;
            if (OldImageRemoved.Any())
            {
                foreach (var file in OldImageRemoved)
                {
                    var result = await SOSHubServices.RemoveImageFromSOSData(_sosHub.SOSHubId, file);
                    if (!result)
                    {
                        failed++;
                    }
                }
                status = failed == 0 ? 0 : failed < OldImageRemoved.Count ? 1 : 2;
                RemoveMultiMediaSnackbarMessage("images", status);
                failed = 0;
            }
            if (OldVideoRemoved.Any())
            {
                foreach (var file in OldVideoRemoved)
                {
                    var result = await SOSHubServices.RemoveVideoFromSOSData(_sosHub.SOSHubId, file);
                    if (!result)
                    {
                        failed++;
                    }
                }
                status = failed == 0 ? 0 : failed < OldVideoRemoved.Count ? 1 : 2;
                RemoveMultiMediaSnackbarMessage("videos", status);
            }
        }

        private void RemoveMultiMediaSnackbarMessage(string iov, int failed)
        {
            Severity color = Severity.Normal; string Msg2 = string.Empty;
            switch (failed)
            {
                case 0: color = Severity.Success; Msg2 = $"Previous {iov} removed"; break;
                case 1: color = Severity.Warning; Msg2 = $"Failed to remove some {iov}"; break;
                case 2: color = Severity.Error; Msg2 = $"Failed to remove previous {iov}"; break;
            }

            Snackbar.Add(Msg2, color);
        }

        private void RemoveOldVideo(string Id)
        {
            int fileId = int.Parse(Id);
            OldVideoRemoved.Add(fileId);
            PreviousVideo.Remove(fileId.ToString());
        }
        private void RemoveOldImage(int fileId)
        {
            OldImageRemoved.Add(fileId);
            PreviousImages.RemoveAll(p => p.Item1 == fileId);
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
            tempItems.Add(new ItemModel());
        }

        void RemoveItem(ItemModel item, bool isTemp)
        {
            Console.WriteLine(item.Commentary);
            if (isTemp)
            {
                Console.WriteLine("a");
            }
            if (isTemp)
            {
                tempItems.Remove(item);
            }
            else
            {
                int index = items.IndexOf(item);

                if (index != -1)
                {
                    items[index].IsActive = false;
                }
            }
        }

        #endregion

        //Common direction files
        #region CommonDirection
        public void PopulateList()
        {
            _sosHub.CommonDirection ??= new List<CommonDirection>();
            foreach (var item in _sosHub.CommonDirection)
            {
                switch (item.type)
                {
                    case 1://GOS
                        GOSDocument tempGos = new GOSDocument { ID_DOC = item.DOC_ID, Nombre = item.name };
                        finalFilesSelection.Add((tempGos, item.route));
                        break;
                    case 2://CCP
                        CCPDocument tempCcp = new CCPDocument { ID_DOC = item.DOC_ID, Nombre = item.name };
                        finalFilesSelection.Add((tempCcp, item.route));
                        break;
                }
            }
        }
        public void FinalizeList()
        {
            foreach (var file in finalFilesSelection)
            {
                int id = 0;
                string name = string.Empty;
                int type = 0;
                switch (file.Item1)
                {
                    case GOSDocument gosDoc:
                        id = gosDoc.ID_DOC;
                        name = gosDoc.Nombre;
                        type = 1;
                        break;
                    case CCPDocument ccpDoc:
                        id = ccpDoc.ID_DOC;
                        name = ccpDoc.Nombre;
                        type = 2;
                        break;
                    default:
                        //ignore it
                        break;
                }
                if (id != 0 && !_sosHub.CommonDirection.Where(p => p.DOC_ID == id).Any())
                {
                    _sosHub.CommonDirection.Add(new CommonDirection { DOC_ID = id, name = name, route = file.Item2, type = type });
                }
            }
            _sosHub.CommonDirection.Where(p => !finalFilesSelection.Where(j => (int)j.Item1.GetType().GetProperty("ID_DOC")!.GetValue(j.Item1)! == p.DOC_ID).Any()).ToList().ForEach(p => p.IsActive = false);
        }
        public void ShowFileExplorerDialog(bool isGos)
        {
            IsGOS = isGos;
            showFileExplorer = true;
        }

        private void CloseFileExplorerDialog()
        {
            showFileExplorer = false;
        }

        private void HandleFinalFilesSelectionChanged(List<(object, string)> finalFiles)
        {
            finalFilesSelection = finalFiles;
            StateHasChanged();
        }

        private void ManipulateFinalList(object file, bool operation, bool isFinalList = false)
        {

            var currentId = (int)file.GetType().GetProperty("ID_DOC")!.GetValue(file)!;
            finalFilesSelection.RemoveAll(p => (int)p.Item1.GetType().GetProperty("ID_DOC")!.GetValue(p.Item1)! == currentId);

        }

        private void DownloadDocument(object document)
        {
            switch (document)
            {
                case GOSDocument gosDoc:
                    CDMSServices.GetDownloadLinkGOS(gosDoc.ID_DOC, gosDoc.Nombre);
                    break;

                case CCPDocument ccpDoc:
                    CDMSServices.GetDownloadLinkCCP(ccpDoc.ID_DOC, ccpDoc.Nombre); break;
                default:
                    //fail mesage
                    break;
            }
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
        [Inject]
        private IDialogService DialogService { get; set; }
        public List<AnalysisBkup> RawAnalisis { get; set; } = new List<AnalysisBkup>();
        public List<AnalysisBkup> RawAnalisisBk { get; set; } = new List<AnalysisBkup>();

        private IEnumerable<AnalysisBkup> _selectedValues = new List<AnalysisBkup>();


        public string stepName { get; set; } = "";
        bool showAddStepDialog = false;


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



        void CreateBakup()
        {
            if (RawAnalisis.Count > 0 && RawAnalisis.Count > RawAnalisisBk.Count)
            {
                RawAnalisisBk = ObjectCloner.ObjectCloner.DeepClone(RawAnalisis);
                foreach (var raw in RawAnalisisBk)
                {
                    if (_sosHub.AnalysesBkup.Any(a => a == raw))
                    {
                        _sosHub.AnalysesBkup.Find(a => a == raw).IsActive = true;
                    }
                    else
                    {
                        AnalysisBkup analysisBkup = new AnalysisBkup();
                        analysisBkup.AnalysisBkupId = raw.AnalysisBkupId;
                        analysisBkup.Text = raw.Text;
                        analysisBkup.IsActive = true;
                        _sosHub.AnalysesBkup.Add(analysisBkup);
                    }
                }


            }
        }
        async void RestoreBakup()
        {
            bool? result = await DialogService.ShowMessageBox(
            "Warning",
            "Deleting can be clar a sections!",
            yesText: "Delete!", cancelText: "Cancel");

            var state = result == null ? "Canceled" : "Deleted!";
            if (state == "Deleted!")
            {
                RawAnalisis = ObjectCloner.ObjectCloner.DeepClone(RawAnalisisBk);
                RawAnalisisBk.Clear();
                _sosHub.Sections.Clear();
                StateHasChanged();
            }


        }

        void AddRawItem()
        {
            AddAnalisis = true;
            StateHasChanged();

            RawAnalisis.Add(new AnalysisBkup());

            AddAnalisis = false;
            StateHasChanged();
        }

        void RemoveRawItem(AnalysisBkup item)
        {
            DeleteAnalisis = true;
            StateHasChanged();
            if (RawAnalisis.Count > 0)
            {
                RawAnalisis.Remove(item);
                var analisis = _sosHub.AnalysesBkup.FirstOrDefault(a => a == item);
                if (analisis != null)
                {
                    analisis.IsActive = false;
                }
                DeleteAnalisis = false;
                StateHasChanged();
            }
        }
        void RemoveSectionItem(Section item)
        {
            if (_sosHub.Sections.Count > 0)
            {
                // Obtener los textos de los análisis de la sección que se va a eliminar
                var textsToReinsert = item.Analyses.Select(analysis => analysis.Text).ToList();

                // Para cada texto, encontrar su posición correcta en RawAnalisis según RawAnalisisBk
                foreach (var text in textsToReinsert)
                {
                    // Verificar si el texto existe en RawAnalisisBk
                    var analysisToInsert = RawAnalisisBk.FirstOrDefault(a => a.Text == text);
                    if (analysisToInsert != null)
                    {
                        int indexInsert = RawAnalisisBk.FindIndex(a => a.Text == text);

                        // Encontrar el índice donde insertar en RawAnalisis
                        int insertPosition = RawAnalisis
                            .Select((value, index) => new { Value = value, Index = index })
                            .Where(x => RawAnalisisBk.FindIndex(a => a.Text == x.Value.Text) >= indexInsert)
                            .Select(x => x.Index)
                            .DefaultIfEmpty(RawAnalisis.Count)
                            .First();

                        // Insertar el AnalysisBkup en la posición correcta
                        RawAnalisis.Insert(insertPosition, analysisToInsert);
                    }
                    else
                    {
                        AnalysisBkup newToAdd = new();
                        newToAdd.Text = text;
                        RawAnalisis.Add(newToAdd);
                    }
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
        public async Task ConfirmStepAsync()
        {
            if (!string.IsNullOrEmpty(stepName))
            {
                Section sectionToAdd = new Section { Step = stepName };

                foreach (AnalysisBkup item in _selectedValues)
                {
                    Analysis analysisToAdd = ProcessText(item.Text);

                    sectionToAdd.Analyses.Add(analysisToAdd);
                    RawAnalisis.Remove(RawAnalisis.First(a => a.Text == item.Text));
                }

                _sosHub.Sections.Add(sectionToAdd);

                // Reset variables
                stepName = string.Empty;
                _selectedValues = new List<AnalysisBkup>();
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

        private Analysis ProcessText(string text)
        {
            Analysis analysis = new Analysis { Text = text };

            // Remove asterisks from BaseText
            BaseText = Regex.Replace(text, @"\*", "").ToString();

            // Split text into segments
            var segments = text.Split(new[] { '-' }, StringSplitOptions.RemoveEmptyEntries)
                               .Select(segmentText => CreateSegment(segmentText))
                               .ToList();

            analysis.CriticalPoints = segments.SelectMany(segment => segment.CriticalPoints).ToList();
            analysis.Reasons = Enumerable.Repeat(string.Empty, analysis.CriticalPoints.Count).ToList();

            return analysis;
        }

        private Segment CreateSegment(string segmentText)
        {
            Segment segment = new Segment { Analysis = segmentText };

            // Extract MainPoint
            var mainPointRegex = new Regex(@"#(.*?)#");
            var mainPointMatch = mainPointRegex.Match(segmentText);
            segment.MainPoint = mainPointMatch.Success ? mainPointMatch.Groups[1].Value.Trim() : string.Empty;

            // Extract CriticalPoints
            var criticalPointsRegex = new Regex(@"\*(.*?)\*");
            var criticalPointMatches = criticalPointsRegex.Matches(segmentText);

            foreach (Match match in criticalPointMatches)
            {
                if (match.Success)
                {
                    segment.CriticalPoints.Add(match.Groups[1].Value.Trim());
                }
            }

            return segment;
        }
        #endregion

        private void GenerateFolio()
        {

            string? areaCode = _areas.Find(a => a.AreaId == areaId)?.Code;
            string? stationCode = _stations.Find(s => s.StationId == stationId)?.Code;
            string? sideCode = productSide;
            string? modelCode = _sosHub.AppliedModels != null ? string.Join("/", _sosHub.AppliedModels.Select(m => m.Code)) : "";


            _sosHub.Folio = string.Concat(areaCode, "-", stationCode, "-", sideCode, "-", modelCode);
            StateHasChanged();
        }


        private bool cantCreate = false;
        private async void OnSelectedUserFunction(User element, int type)
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
                    selectedEditorOfList = element;

                    if (selectedEditorOfList != new User())
                    {
                        ActiveAddEditor = false;
                    }
                    else
                    {
                        ActiveAddEditor = true;
                    }
                    break;
            }

        }

        #region ApproberOwners

        private User selectedOwnerOfList = null;
        private bool ActiveAddOwner = false;

        private void DeleteOwnerList(User selection)
        {
            _sosHub.ApproverOwners?.Remove(selection);
            _Seniorsupervisors.Add(selection);
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

                _Seniorsupervisors.Remove(selection);

                selectedOwnerOfList = null;
                ActiveAddOwner = true;
            }

            cantCreate = _sosHub.ApproverOwners.Count == 0;

            StateHasChanged();
        }


        #endregion
        #region ReviewerEditor


        private User selectedEditorOfList = null;
        private bool ActiveAddEditor = false;


        private void DeleteEditorList(User selection)
        {
            _sosHub.ReviewerEditors?.Remove(selection);
            _supervisors.Add(selection);
            cantCreate = _sosHub.ReviewerEditors.Count == 0;
            StateHasChanged();
        }


        private void AddEditor(User selection)
        {
            if (_sosHub.ReviewerEditors == null)
            {
                _sosHub.ReviewerEditors = new List<User>();
            }


            if (selectedEditorOfList != null && !_sosHub.ReviewerEditors.Contains(selection))
            {

                _sosHub.ReviewerEditors.Add(selection);

                _supervisors.Remove(selection);

                selectedEditorOfList = null;
                ActiveAddEditor = true;
            }

            cantCreate = _sosHub.ReviewerEditors.Count == 0;

            StateHasChanged();
        }


        #endregion
        private async Task<IEnumerable<User>> SearchSupervisorsUsers(string value)
        {
            // In real life use an asynchronous function for fetching data from an api.
            // await Task.Delay(1000);

            // if text is null or empty, show complete list
            if (string.IsNullOrEmpty(value))
                return _supervisors;

            return _supervisors.Where(x => x.Name.Contains(value, StringComparison.InvariantCultureIgnoreCase));
        }

        private async Task<IEnumerable<User>> SearchSeniorSupervisorsUsers(string value)
        {
            // In real life use an asynchronous function for fetching data from an api.
            // await Task.Delay(1000);

            // if text is null or empty, show complete list
            if (string.IsNullOrEmpty(value))
                return _Seniorsupervisors;

            return _Seniorsupervisors.Where(x => x.Name.Contains(value, StringComparison.InvariantCultureIgnoreCase));
        }

        #region AppliedModels
        private Product selectedProductOfList = null;
        private bool ActiveAddProduct = false;


        private void DeleteProductList(Product selection)
        {
            _sosHub.AppliedModels?.Remove(selection);
            _products.Add(selection);
            cantCreate = _sosHub.AppliedModels.Count == 0;
            GenerateFolio();
            StateHasChanged();
        }


        private void AddProduc(Product selection)
        {
            if (_sosHub.AppliedModels == null)
            {
                _sosHub.AppliedModels = new List<Product>();
            }


            if (selectedProductOfList != null && !_sosHub.AppliedModels.Contains(selection))
            {

                _sosHub.AppliedModels.Add(selection);

                _products.Remove(selection);

                selectedProductOfList = null;
                ActiveAddProduct = true;

                if (_sosHub.AppliedModels.Count >= 1)
                {
                    GenerateFolio();
                }

            }

            cantCreate = _sosHub.AppliedModels.Count == 0;

            StateHasChanged();
        }

        private async Task<IEnumerable<Product>> SearchProduct(string value)
        {

            if (string.IsNullOrEmpty(value))
                return _products;

            return _products.Where(x => x.Code.Contains(value, StringComparison.InvariantCultureIgnoreCase));
        }
        private async void OnSelectedProductFunction(Product element)
        {
            selectedProductOfList = element;

            if (selectedProductOfList != new Product())
            {
                ActiveAddProduct = false;
            }
            else
            {
                ActiveAddProduct = true;
            }

        }

        #endregion 

    }
}