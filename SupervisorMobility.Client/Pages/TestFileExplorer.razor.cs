using global::System;
using global::System.Collections.Generic;
using global::System.Linq;
using global::System.Threading.Tasks;
using global::Microsoft.AspNetCore.Components;
using System.Net.Http;
using System.Net.Http.Json;
using Microsoft.JSInterop;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.Web.Virtualization;
using Microsoft.AspNetCore.Components.WebAssembly.Http;
using SupervisorMobility.Client;
using SupervisorMobility.Client.Shared;
using SupervisorMobility.Client.Services;
using SupervisorMobility.Client.Data.Resources;
using Microsoft.Extensions.Localization;
using BlazorCameraStreamer;
using Blazored.SessionStorage;
using MudBlazor;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using SupervisorMobility.Client.Data.Entities.TreeStruct;
using SupervisorMobility.Client.Services.AreaService;
using SupervisorMobility.Client.Services.DistributionService;
using SupervisorMobility.Client.Services.PlantService;
using SupervisorMobility.Client.Services.ProductsService;
using DocumentFormat.OpenXml.Wordprocessing;
using Color = MudBlazor.Color;

namespace SupervisorMobility.Client.Pages
{
    public partial class TestFileExplorer
    {

        [Parameter]
        public bool IsGOS { get; set; } = true;

        bool ShowLoading = true;
        bool LoadingContents = false;
        bool Find_all_tree = false;
        bool Find_Product = false;
        bool ShowMoreInfo = false;

        bool if_add_CD_CCP = false;
        bool if_add_CD_GOS = false;
        bool if_add_CD_HOE = false;


        private IList<string> _sourceMsgLoading = new List<string>();
        private IList<Color> _Colors = new List<Color>() { Color.Default, Color.Primary, Color.Secondary, Color.Success, Color.Info, Color.Default, Color.Primary, Color.Secondary, Color.Success, Color.Info };

        List<BreadcrumbItem> _links;

        private SOSCodePath SosCode = new SOSCodePath();


        TreeItemData rootNodeCCP { get; set; } = new TreeItemData();
        TreeItemData rootNodeGOS { get; set; } = new TreeItemData();
        TreeItemData rootNodeHOE { get; set; } = new TreeItemData();
        TreeItemData SelectedNodeCCP { get; set; }
        TreeItemData SelectedNodeGOS { get; set; }
        TreeItemData SelectedNodeHOE { get; set; }

        CDMS_GOS_Directory GOSFolders { get; set; } = new CDMS_GOS_Directory();
        CDMS_CCP_Directory CCPFolders { get; set; } = new CDMS_CCP_Directory();
        CDMS_HOE_Directory HOEFolders { get; set; } = new CDMS_HOE_Directory();

        bool isGosFolder = false;
        bool isCcpFolder = false;
        bool isHoeFolder = false;
        private CDMS_CCP_Archives? CcpFilesInFolder;
        private CDMS_HOE_Archives? HoeFilesInFolder;
        private CDMS_GOS_Archives? GosFilesInFolder;

        private bool folderCCPError = false;
        private bool folderHOEError = false;
        private bool folderGOSError = false;

        MudMessageBox HOEmbox { get; set; }
        MudMessageBox CCPmbox { get; set; }
        MudMessageBox GOSmbox { get; set; }

        public bool HOEModalDisplay { get; set; } = false;

        private DialogOptions dialogOptions = new() { CloseOnEscapeKey = true, MaxWidth = MaxWidth.ExtraExtraLarge, FullWidth = true, Position = DialogPosition.TopCenter, DisableBackdropClick = true, CloseButton = true };


        private List<Product> _products = new List<Product>();
        private List<Plant> _plants = new List<Plant>();
        private Dictionary<int, List<Area>> _areas = new Dictionary<int, List<Area>>();
        private Dictionary<int, Dictionary<int, List<Distribution>>> _distributions = new Dictionary<int, Dictionary<int, List<Distribution>>>();

        private Plant _plant = new Plant();
        private Area _area = new Area();
        private Distribution _distribution = new Distribution();
        private Product _product = new Product();

        int IndexPlant = -1;
        int IndexArea = -1;
        int IndexDistr = -1;
        int IndexProd = -1;

        private string hoverIcon = Icons.Material.Filled.FolderOpen;
        private string defaultIcon = Icons.Material.Filled.Folder;

        private Dictionary<TreeItemData, bool> hoverStates = new Dictionary<TreeItemData, bool>();
        private Dictionary<object, (bool, bool)> fileHoverStates = new Dictionary<object, (bool, bool)>();
        private List<object> finalFilesSelection = new List<object>();
        private List<object> removeFilesSelection = new List<object>();

        //Tabs
        private List<TreeItemData> openTabs = new List<TreeItemData>();
        private int activeTabIndex = 0;

        private string searchString = "";
        public bool showGOS { get; set; }


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
                new BreadcrumbItem(text: Localizer["home"], href: "/"),
                new BreadcrumbItem(text: Localizer["configuration"], href: "/configuration"),
                new BreadcrumbItem(text: Localizer["PathsRoute"], href: "/PathsRoute"),
                new BreadcrumbItem(text: Localizer["PathsRouteCreate"], href: "/PathsRoute", disabled: true),
            };


            try
            {
                _products = await ProductsServices.GetProducts();

                _plants = await PlantsServices.GetPlants();
                foreach (var plant in _plants)
                {
                    var areas = await AreasServices.GetAreas(plant.PlantId);
                    _areas.Add(plant.PlantId, areas);

                    var areaDistributions = new Dictionary<int, List<Distribution>>();
                    foreach (var area in areas)
                    {
                        var distributions = await DistributionsServices.GetDistributions(plant.PlantId, area.AreaId);
                        areaDistributions.Add(area.AreaId, distributions);
                    }
                    _distributions.Add(plant.PlantId, areaDistributions);
                }

                try
                {
                    GOSFolders = await CDMSServices.GetFoldersGOS();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error Get GOS Folder From CDMS");
                    Console.WriteLine(ex.Message);
                }

                if (GOSFolders != null)
                {
                    folderGOSError = false;
                    rootNodeGOS = TreeServices.Make_Tree_GOS(GOSFolders.operation);
                    if (IsGOS)
                    {
                        openTabs.Add(rootNodeGOS);
                    }
                }
                else
                {
                    folderGOSError = true;
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
                    if (!IsGOS)
                    {
                        openTabs.Add(rootNodeCCP);
                    }
                }
                else
                {
                    folderCCPError = true;
                }



            }
            catch (Exception ex)
            {

            }
            finally
            {
                ShowLoading = false;
            }

        }

        public void ChangeFileExplorer(bool toggled)
        {
            IsGOS = toggled;
            fileHoverStates.Clear();
            hoverStates.Clear();
            searchString = "";
            openTabs.Clear();
            foreach (var key in hoverStates.Keys.ToList())
            {
                hoverStates[key] = false;
            }

            if (IsGOS)
            {
                openTabs.Add(rootNodeGOS);
            }
            else
            {
                openTabs.Add(rootNodeCCP);
            }

            StateHasChanged();
        }

        private void OnSearchStringChanged(string value)
        {
            searchString = value;
            StateHasChanged();
        }

        private bool FilterFunc(TreeItemData element)
        {
            if (string.IsNullOrWhiteSpace(searchString))
                return true;
            if (element.Name.ToString().Contains(searchString, StringComparison.OrdinalIgnoreCase))
                return true;
            return false;
        }


        private void OnMouseOver(TreeItemData item)
        {
            hoverStates[item] = true;
        }

        private void OnMouseOut(TreeItemData item)
        {
            hoverStates[item] = false;
        }

        private void OnFileMouseOver(object item)
        {
            var temp = fileHoverStates[item];
            temp.Item1 = true;
            fileHoverStates[item] = temp;
        }

        private void OnFileMouseOut(object item)
        {
            var temp = fileHoverStates[item];
            temp.Item1 = false;
            fileHoverStates[item] = temp;
        }

        private string GetIcon(TreeItemData item)
        {
            return hoverStates.ContainsKey(item) && hoverStates[item] ? Icons.Material.Filled.FolderOpen : Icons.Material.Filled.Folder;
        }

        private string GetFileIcon(object key)
        {
            return fileHoverStates.ContainsKey(key) && fileHoverStates[key].Item1 ? Icons.Material.Filled.InsertDriveFile : Icons.Material.Outlined.InsertDriveFile;
        }

        private Color GetIconColor(bool Is_Directory)
        {
            if (!Is_Directory) 
            {
                return Color.Info;
            }

            return Color.Warning;
        }

        private async void OnNodeClick(TreeItemData clickedNode)
        {
            LoadingContents = true;
            fileHoverStates.Clear();
            hoverStates.Clear();
            searchString = "";
            if (clickedNode.Is_Directory)
            {
                int clickedNodeIndex = openTabs.IndexOf(clickedNode);
                if (clickedNodeIndex >= 0)
                {
                    openTabs = openTabs.Take(clickedNodeIndex + 1).ToList();
                }
                else
                {
                    openTabs.Add(clickedNode);
                }

                activeTabIndex = openTabs.IndexOf(clickedNode);

                if (!clickedNode.TreeItems.Any())
                {
                    if(IsGOS)
                    {
                        GosFilesInFolder = await CDMSServices.GetFilesGOS(clickedNode.Path);
                        foreach(var file in GosFilesInFolder.operation)
                        {
                            fileHoverStates.Add(file, (false, false));
                        }
                    }
                    else
                    {
                        CcpFilesInFolder = await CDMSServices.GetFilesCCP(clickedNode.Path);
                        foreach (var file in CcpFilesInFolder.operation)
                        {
                            fileHoverStates.Add(file, (false, false));
                        }
                    }
                }
            }
            LoadingContents = false;
            StateHasChanged();
        }

        private async void OnFileNodeClick(object clickedFile)
        {
            fileHoverStates[clickedFile] = fileHoverStates[clickedFile].Item2?(true, false):(true,true);
        }

        private Task HandleCheck(bool value, object item)
        {
            var temp = fileHoverStates[item];
            temp.Item2 = value;
            fileHoverStates[item] = temp;
            return Task.CompletedTask;
        }

        private async void AddFilesToList()
        {
            if (!fileHoverStates.Any(p=>p.Value.Item2 == true)) return;

            finalFilesSelection ??= new List<object>();
            finalFilesSelection.AddRange(fileHoverStates.Where(p=>p.Value.Item2).Select(key => key.Key).ToList());
        }

        private async void RemoveFilesFromList(List<object> Files)
        {
            if (!removeFilesSelection.Any() && !finalFilesSelection.Any()) return;

            finalFilesSelection.RemoveAll(item => removeFilesSelection.Contains(item));
        }

        private void RecreateFileExplorer()
        {
            foreach (var key in hoverStates.Keys.ToList())
            {
                hoverStates[key] = false;
            }
            searchString = "";
            openTabs = openTabs.Take(activeTabIndex + 1).ToList();
            fileHoverStates.Clear();
        }

        public async Task<HashSet<TreeItemData>> LoadServerData(TreeItemData parentNode)
        {
            await Task.Delay(50);
            return parentNode.TreeItems;
        }

        void DeleteGOSRoute()
        {
            SosCode.GOS = "";
        }
        void DeleteCCPRoute()
        {
            SosCode.CCP = "";
        }

    }
}