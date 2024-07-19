using MudBlazor;
using SupervisorMobility.Client.Data.Entities.TreeStruct;
using Color = MudBlazor.Color;

namespace SupervisorMobility.Client.Pages
{
    public partial class TestFileExplorer
    {

        [Parameter]
        public bool IsGOS { get; set; } = true;

        [Parameter]
        public EventCallback<List<(object, string)>> OnFilesSelected { get; set; }

        [Parameter]
        public List<(object, string)> InitialFilesSelection { get; set; } = new List<(object, string)>();

        [Parameter]
        public Dictionary<object, (bool, bool)> FileHoverStates { get; set; } = new Dictionary<object, (bool, bool)>();


        bool ShowLoading = true;
        bool LoadingContents = false;
        bool Find_all_tree = false;
        bool Find_Product = false;
        bool ShowMoreInfo = false;

        private IList<string> _sourceMsgLoading = new List<string>();
        private IList<Color> _Colors = new List<Color>() { Color.Default, Color.Primary, Color.Secondary, Color.Success, Color.Info, Color.Default, Color.Primary, Color.Secondary, Color.Success, Color.Info };

        List<BreadcrumbItem> _links;

        TreeItemData rootNodeCCP { get; set; } = new TreeItemData();
        TreeItemData rootNodeGOS { get; set; } = new TreeItemData();
        TreeItemData SelectedNodeCCP { get; set; }
        TreeItemData SelectedNodeGOS { get; set; }

        CDMS_GOS_Directory GOSFolders { get; set; } = new CDMS_GOS_Directory();
        CDMS_CCP_Directory CCPFolders { get; set; } = new CDMS_CCP_Directory();

        bool isGosFolder = false;
        bool isCcpFolder = false;
        private CDMS_CCP_Archives? CcpFilesInFolder;
        private CDMS_GOS_Archives? GosFilesInFolder;

        private bool folderCCPError = false;
        private bool folderGOSError = false;

        private string hoverIcon = Icons.Material.Filled.FolderOpen;
        private string defaultIcon = Icons.Material.Filled.Folder;

        private Dictionary<TreeItemData, bool> hoverStates = new Dictionary<TreeItemData, bool>();
        private Dictionary<object, (bool, bool)> fileHoverStates = new Dictionary<object, (bool, bool)>();
        private List<(object, string)> finalFilesSelection = new List<(object, string)>();
        private List<object> removeFilesSelection = new List<object>();

        //Tabs
        private List<TreeItemData> openTabs = new List<TreeItemData>();
        private int activeTabIndex = 0;

        private string searchString = "";
        private bool isFinalPath = false;
        private string finalPath = "";

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
                new BreadcrumbItem(text: Localizer["File Explorer"], href: "/PathsRoute", disabled: true),
            };

            if (InitialFilesSelection != null)
            {
                finalFilesSelection = InitialFilesSelection;
                foreach (var file in finalFilesSelection)
                {
                    fileHoverStates[file.Item1] = (false, true);
                }
            }

            try
            {
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
            isFinalPath = false;
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
            if (temp.Item2) { return; }
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
            isFinalPath = false;
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
                    finalPath = clickedNode.Path;
                    isFinalPath = true;
                    if(IsGOS)
                    {
                        GosFilesInFolder = await CDMSServices.GetFilesGOS(clickedNode.Path);
                        foreach(var file in GosFilesInFolder.operation)
                        {
                            if(finalFilesSelection.Any(p=>file.ID_DOC == (int)p.Item1.GetType().GetProperty("ID_DOC")!.GetValue(p.Item1)!))
                            {
                                fileHoverStates.Add(file, (true,true));
                            }
                            else
                            {
                                fileHoverStates.Add(file, (false, false));
                            }
                        }
                    }
                    else
                    {
                        CcpFilesInFolder = await CDMSServices.GetFilesCCP(clickedNode.Path);
                        foreach (var file in CcpFilesInFolder.operation)
                        {
                            if (finalFilesSelection.Any(p => file.ID_DOC == (int)p.Item1.GetType().GetProperty("ID_DOC")!.GetValue(p.Item1)!))
                            {
                                fileHoverStates.Add(file, (true, true));
                            }
                            else
                            {
                                fileHoverStates.Add(file, (false, false));
                            }
                        }
                    }
                }
            }
            LoadingContents = false;
            StateHasChanged();
        }

        private async void OnFileNodeClick(object clickedFile)
        {
            fileHoverStates[clickedFile] = fileHoverStates[clickedFile].Item2?(true, false) :(true, true);
            ManipulateFinalList(clickedFile, fileHoverStates[clickedFile].Item2);
        }

        private Task HandleCheck(bool value, object item)
        {
            var temp = fileHoverStates[item];
            temp.Item2 = value;
            fileHoverStates[item] = temp;
            ManipulateFinalList(item, value);
            return Task.CompletedTask;
        }

        private void ManipulateFinalList(object file, bool operation, bool isFinalList = false)
        {
            if (operation)
            {
                finalFilesSelection.Add((file, finalPath));
            }
            else
            {
                var currentId = (int)file.GetType().GetProperty("ID_DOC")!.GetValue(file)!;
                finalFilesSelection.RemoveAll(p=> (int)p.Item1.GetType().GetProperty("ID_DOC")!.GetValue(p.Item1)! == currentId);

                if (isFinalList && fileHoverStates.Any())
                {
                    var key = fileHoverStates.FirstOrDefault(p => (int)p.Key.GetType().GetProperty("ID_DOC")!.GetValue(p.Key)! == currentId).Key;
                    if (key != null)
                    {
                        fileHoverStates[key] = (false, false);
                    }
                }
            }
            OnFilesSelected.InvokeAsync(finalFilesSelection);

        }

        private void DownloadDocument(object document)
        {
            //var id = (int)document.GetType().GetProperty("ID_DOC")!.GetValue(document)!;
            switch (document)
            {
                case GOSDocument gosDoc:
                    Console.WriteLine(gosDoc.ID_DOC);
                    //Call gos download method
                    break;
                case CCPDocument ccpDoc:
                    Console.WriteLine(ccpDoc.ID_DOC);
                    //Call ccp download method
                    break;
                default:
                    //fail mesage
                    break;
            }
        }

        private void RecreateFileExplorer()
        {
            isFinalPath = false;
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


    }
}