using Microsoft.JSInterop;
using SupervisorMobility.Client.Data.Entities.TreeStruct;
using MudBlazor;
using System.Runtime.CompilerServices;
using System.Diagnostics;

namespace SupervisorMobility.Client.Pages.Components
{
    public partial class TreeView
    {
        [Parameter]
        public int Path_Id { get; set; }
        [Parameter]
        public string? panel { get; set; }
        public string? searchCodeString { get; set; }

        private SOSCodePath Path_Item { get; set; }
        private MudTabs selectPanel { get; set; }
        MudTabPanel panel_HOE;
        MudTabPanel panel_CCP;
        MudTabPanel panel_GOS;
        MudTabPanel panel_HOE_CD;
        MudTabPanel panel_CCP_CD;
        MudTabPanel panel_GOS_CD;
        private bool ShowLoading = true;
        //CCP
        private TreeItemData Node_CCP { get; set; } = new();
        private TreeItemData Node_CCP_CD { get; set; } = new();
        //GOS
        private TreeItemData Node_GOS { get; set; } = new();
        private TreeItemData Node_GOS_CD { get; set; } = new();
        //HOE
        private TreeItemData Node_HOE { get; set; } = new();
        private TreeItemData Node_HOE_CD { get; set; } = new();


        private IList<string> _sourceMsgLoading = new List<string>();
        private IList<Color> _Colors = new List<Color>() { Color.Default, Color.Primary, Color.Secondary, Color.Success, Color.Info, Color.Default, Color.Primary, Color.Secondary, Color.Success, Color.Info };

        //New File Explorer
        TreeItemData rootNodeHOE { get; set; } = new TreeItemData();
        TreeItemData rootNodeCCP { get; set; } = new TreeItemData();
        TreeItemData rootNodeGOS { get; set; } = new TreeItemData();
        TreeItemData SelectedNodeHOE { get; set; }
        TreeItemData SelectedNodeCCP { get; set; }
        TreeItemData SelectedNodeGOS { get; set; }

        CDMS_HOE_Directory HOEFolders { get; set; } = new CDMS_HOE_Directory();
        CDMS_GOS_Directory GOSFolders { get; set; } = new CDMS_GOS_Directory();
        CDMS_CCP_Directory CCPFolders { get; set; } = new CDMS_CCP_Directory();

        bool isHoeFolder = false;
        bool isGosFolder = false;
        bool isCcpFolder = false;
        private CDMS_HOE_Archives? HoeFilesInFolder;
        private CDMS_CCP_Archives? CcpFilesInFolder;
        private CDMS_GOS_Archives? GosFilesInFolder;

        private bool folderHoeError = false;
        private bool folderCCPError = false;
        private bool folderGOSError = false;

        public bool IsHOE { get; set; } = false;
        public bool IsGOS { get; set; } = false;
        public bool IsCCP { get; set; } = false;
        public bool IsHOECD { get; set; } = false;
        public bool IsGOSCD { get; set; } = false;
        public bool IsCCPCD { get; set; } = false;
        public bool ChangeDirectory { get; set; } = true;
        private string _buttonText = "CDMS_File_Explorer";

        //Tabs
        private List<TreeItemData> openTabs = new List<TreeItemData>();
        private int activeTabIndex = 0;

        private string searchString = "";

        bool LoadingContents = false;
        private bool isFinalPath = false;


        protected async override Task OnInitializedAsync()
        {
            ShowLoading = true;
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

            StateHasChanged();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {

        }

        protected override async Task OnParametersSetAsync()
        {
            ShowLoading = true;

            try
            {
                //Path_Item = await TreeServices.getCodePath(Path_Id);
                await TreeServices.InitializeTreeData();
                Path_Item = await AssyChartServices.GetCodePath(Path_Id);
                searchCodeString = Path_Item.Code;
                await TreeServices.setNodesByPath(Path_Item);


                rootNodeHOE = await TreeServices.getRootHOE();
                rootNodeGOS = await TreeServices.getRootGOS();
                rootNodeCCP = await TreeServices.getRootCCP();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error on OnInitializedAsync");
                Console.WriteLine(ex);
            }
            finally
            {
                switch (panel)
                {
                    case "HOE":
                        SetButtonText(0);
                        break;
                    case "GOS":
                        SetButtonText(1);
                        break;
                    case "CCP":
                        SetButtonText(2);
                        break;
                    case "HOE_CD":
                        SetButtonText(3);
                        break;
                    case "GOS_CD":
                        SetButtonText(4);
                        break;
                    case "CCP_CD":
                        SetButtonText(5);
                        break;
                    default:
                        SetButtonText(0);
                        break;
                }
                ShowLoading = false;

            }
        }

        private async Task GetHOETab(TreeItemData item)
        {
            try
            {
                await TreeServices.GetFilesInNodeHOE(item.Is_Directory ? item.TreeItems.FirstOrDefault() : item);
            }
            catch (Exception ex)
            {
                Console.WriteLine("HOE Final, has is directory");
                Console.WriteLine(ex.Message);
            }
            finally
            {
                await TreeServices.GetFilesInNodeHOE(item);
            }
        }

        private async Task GetGOSTab(TreeItemData item)
        {
            try
            {
                await TreeServices.GetFilesInNodeGOS(item.Is_Directory ? item.TreeItems.First() : item);
            }
            catch (Exception ex)
            {
                Console.WriteLine("HOE Final, has is directory");
                Console.WriteLine(ex.Message);
            }
            finally
            {
                await TreeServices.GetFilesInNodeGOS(item);
            }
        }
        private async Task DownloadFileFromURL_HOE(string urlroute, string namefile)
        {
            _ = await CDMSServices.Download_DeleteFileTempHOE(namefile, urlroute);
               
            //var fileName = namefile;
            //var fileURL = urlroute;
            //await JS.InvokeVoidAsync("triggerFileDownload", fileName, fileURL);
        }

        private async Task DownloadFileFromURL_CCP(int urlroute, string namefile)
        {
            _ = await CDMSServices.GetDownloadLinkCCP(urlroute, namefile);
        }//end download file ccp

        private async Task DownloadFileFromURL_GOS(int ID, string namefile)
        {
            _ = await CDMSServices.GetDownloadLinkGOS(ID, namefile);
        }//end download file gos

        private async Task<AsyncVoidMethodBuilder> SearchFunction()
        {
                try
                {
                    if(Node_CCP != null && Node_GOS != null && Node_HOE != null)
                        ShowLoading = true;
                    
                StateHasChanged();

                    //if (string.IsNullOrEmpty(searchCodeString))
                    //{
                    //    if (Path_Item.HOE != "")
                    //        HoeFilesInFolder = ObjectCloner.ObjectCloner.DeepClone(AuxHoeFilesInFolder);

                    //    if (Path_Item.GOS != "")
                    //        GosFilesInFolder = ObjectCloner.ObjectCloner.DeepClone(AuxGosFilesInFolder);

                    //    if (Path_Item.CCP != "")
                    //        CcpFilesInFolder = ObjectCloner.ObjectCloner.DeepClone(AuxCcpFilesInFolder);

                    //    if (Path_Item.CommonDirectionHOE != "")
                    //        HoeFilesInFolderCD = ObjectCloner.ObjectCloner.DeepClone(AuxHoeFilesInFolderCD);

                    //    if (Path_Item.CommonDirectionGOS != "")
                    //        GosFilesInFolderCD = ObjectCloner.ObjectCloner.DeepClone(AuxGosFilesInFolderCD);

                    //    if (Path_Item.CommonDirectionCCP != "")
                    //        CcpFilesInFolderCD = ObjectCloner.ObjectCloner.DeepClone(AuxCcpFilesInFolderCD);
                    //}
                    //else
                    //{
                    //    if (Path_Item.HOE != "")
                    //    {
                    //        HoeFilesInFolder = ObjectCloner.ObjectCloner.DeepClone(AuxHoeFilesInFolder);
                    //        HoeFilesInFolder.operation = HoeFilesInFolder.operation.Where(x => x.Nombre.ToLower().Contains(searchCodeString.ToLower(), StringComparison.InvariantCultureIgnoreCase)).ToList();
                    //    }

                    //    if (Path_Item.GOS != "")
                    //    {
                    //        GosFilesInFolder = ObjectCloner.ObjectCloner.DeepClone(AuxGosFilesInFolder);
                    //        GosFilesInFolder.operation = AuxGosFilesInFolder.operation.Where(x => x.Nombre.ToLower().Contains(searchCodeString.ToLower(), StringComparison.InvariantCultureIgnoreCase)).ToList();
                    //    }

                    //    if (Path_Item.CCP != "")
                    //    {
                    //        CcpFilesInFolder = ObjectCloner.ObjectCloner.DeepClone(AuxCcpFilesInFolder);
                    //        CcpFilesInFolder.operation = CcpFilesInFolder.operation.Where(x => x.Nombre.ToLower().Contains(searchCodeString.ToLower(), StringComparison.InvariantCultureIgnoreCase)).ToList();
                    //    }

                    //    if (Path_Item.CommonDirectionHOE != "")
                    //    {
                    //        HoeFilesInFolderCD = ObjectCloner.ObjectCloner.DeepClone(AuxHoeFilesInFolderCD);
                    //        HoeFilesInFolderCD.operation = HoeFilesInFolderCD.operation.Where(x => x.Nombre.ToLower().Contains(searchCodeString.ToLower(), StringComparison.InvariantCultureIgnoreCase)).ToList();
                    //    }

                    //    if (Path_Item.CommonDirectionGOS != "")
                    //    {
                    //        GosFilesInFolderCD = ObjectCloner.ObjectCloner.DeepClone(AuxGosFilesInFolderCD);
                    //        GosFilesInFolderCD.operation = GosFilesInFolderCD.operation.Where(x => x.Nombre.ToLower().Contains(searchCodeString.ToLower(), StringComparison.InvariantCultureIgnoreCase)).ToList();
                    //    }

                    //    if (Path_Item.CommonDirectionCCP != "")
                    //    {
                    //        CcpFilesInFolderCD = ObjectCloner.ObjectCloner.DeepClone(AuxCcpFilesInFolderCD);
                    //        CcpFilesInFolderCD.operation = CcpFilesInFolderCD.operation.Where(x => x.Nombre.ToLower().Contains(searchCodeString.ToLower(), StringComparison.InvariantCultureIgnoreCase)).ToList();
                    //    }

                    //}

                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error Filter: {ex.Message}");
                }
                finally
                {
                if (Node_CCP != null && Node_GOS != null && Node_HOE != null)
                    ShowLoading = false;

                StateHasChanged();
                }
           

        return new AsyncVoidMethodBuilder();
        }//end searchsring

        private void SetButtonText(int id)
        {
            switch (id)
            {
                case 0:
                    _buttonText = "HOE";
                    IsHOE = true;
                    IsCCP = false;
                    IsGOS = false;
                    ChangeFileExplorer();
                    break;
                case 1:
                    _buttonText = "GOS";
                    IsHOE = false;
                    IsGOS = true;
                    IsCCP = false;
                    ChangeFileExplorer();
                    break;
                case 2:
                    _buttonText = "CCP";
                    IsHOE = false;
                    IsGOS = false;
                    IsCCP = true;
                    ChangeFileExplorer();
                    break;
                case 3:
                    _buttonText = "HOECD";
                    IsHOE = true;
                    IsCCP = false;
                    IsGOS = false;
                    IsHOECD = true;
                    IsCCPCD = false;
                    IsGOSCD = false;
                    ChangeFileExplorer();
                    break;
                case 4:
                    _buttonText = "GOSCD";
                    IsHOE = false;
                    IsGOS = true;
                    IsCCP = false;
                    IsHOECD = false;
                    IsGOSCD = true;
                    IsCCPCD = false;
                    ChangeFileExplorer();
                    break;
                case 5:
                    _buttonText = "CCPCD";
                    IsHOE = false;
                    IsGOS = false;
                    IsCCP = true;
                    IsHOECD = false;
                    IsGOSCD = false;
                    IsCCPCD = true;
                    ChangeFileExplorer();
                    break;
            }
        }
        public void ChangeFileExplorer()
        {
            isFinalPath = false;

            searchString = "";
            openTabs.Clear();
        

            if (IsHOE && !IsHOECD)
            {
                var tabs = TreeServices.FindAncestorsByPath(rootNodeHOE, Path_Item.HOE);
                openTabs.AddRange(tabs);
                OnNodeClick(openTabs.Last());
            }
            if (IsGOS && !IsGOSCD)
            {
                var tabs = TreeServices.FindAncestorsByPath(rootNodeGOS, Path_Item.GOS);
                openTabs.AddRange(tabs);
                OnNodeClick(openTabs.Last());
            }
             if (IsCCP && !IsCCPCD)
            {
                var tabs = TreeServices.FindAncestorsByPath(rootNodeCCP, Path_Item.CCP);
                openTabs.AddRange(tabs);
                OnNodeClick(openTabs.Last());
            }
            if (IsHOECD)
            {
                var tabs = TreeServices.FindAncestorsByPath(rootNodeHOE, Path_Item.CommonDirectionHOE);
                openTabs.AddRange(tabs);
                OnNodeClick(openTabs.Last());
            }
            if (IsGOSCD)
            {
                var tabs = TreeServices.FindAncestorsByPath(rootNodeGOS, Path_Item.CommonDirectionGOS);
                openTabs.AddRange(tabs);
                OnNodeClick(openTabs.Last());
            }
             if (IsCCPCD)
            {
                var tabs = TreeServices.FindAncestorsByPath(rootNodeCCP, Path_Item.CommonDirectionCCP);
                openTabs.AddRange(tabs);
                OnNodeClick(openTabs.Last());
            }

            StateHasChanged();
        }

        private void RecreateFileExplorer()
        {
            isFinalPath = false;
            searchString = "";
            openTabs = openTabs.Take(activeTabIndex + 1).ToList();
        }

        private async void OnNodeClick(TreeItemData clickedNode)
        {
            isFinalPath = false;

            LoadingContents = true;
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
                    isFinalPath = true;


                    if (IsHOE)
                    {
                        HoeFilesInFolder = await CDMSServices.GetFilesHOE(clickedNode.Path);
                     

                    }
                    else if (IsGOS)
                    {
                        GosFilesInFolder = await CDMSServices.GetFilesGOS(clickedNode.Path);
                        
                    }
                    else if (IsCCP)
                    {
                        CcpFilesInFolder = await CDMSServices.GetFilesCCP(clickedNode.Path);
                        
                    }
                }
            }
            LoadingContents = false;
            StateHasChanged();
        }

        private string GetIcon(TreeItemData item)
        {
            return Icons.Material.Filled.Folder;
        }

        private string GetFileIcon(object key)
        {
            return Icons.Material.Outlined.InsertDriveFile;
        }

        private Color GetIconColor(bool Is_Directory)
        {
            if (!Is_Directory)
            {
                return Color.Info;
            }

            return Color.Warning;
        }

        private bool FilterHOEFunc(HOEDocument element)
        {
            if (string.IsNullOrWhiteSpace(searchString))
                return true;
            if (element.Nombre.ToString().Contains(searchString, StringComparison.OrdinalIgnoreCase))
                return true;
            return false;
        }

        private bool FilterGOSFunc(GOSDocument element)
        {
            if (string.IsNullOrWhiteSpace(searchString))
                return true;
            if (element.Nombre.ToString().Contains(searchString, StringComparison.OrdinalIgnoreCase))
                return true;
            return false;
        }

        private bool FilterCCPFunc(CCPDocument element)
        {
            if (string.IsNullOrWhiteSpace(searchString))
                return true;
            if (element.Nombre.ToString().Contains(searchString, StringComparison.OrdinalIgnoreCase))
                return true;
            return false;
        }
    }
    //end fragment code
}