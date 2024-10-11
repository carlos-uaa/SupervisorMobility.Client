using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.Office.CustomUI;
using DocumentFormat.OpenXml.Office2013.Excel;
using MudBlazor;
using SupervisorMobility.Client.Data.Entities.TreeStruct;
using System.Xml.XPath;
using Color = MudBlazor.Color;

namespace SupervisorMobility.Client.Pages.Components
{
    public partial class PathFileExplorer
    {

        [Parameter]
        public bool IsHOE { get; set; } = false;
        [Parameter]
        public bool IsGOS { get; set; } = false;
        [Parameter]
        public bool IsCCP { get; set; } = false;
        [Parameter]
        public bool IsHOECD { get; set; } = false;
        [Parameter]
        public bool IsGOSCD { get; set; } = false;
        [Parameter]
        public bool IsCCPCD { get; set; } = false;

        [Parameter]
        public bool ChangeDirectory { get; set; } = true;

        [Parameter]
        public EventCallback<(object, string)> HandleFinaHOEChanged { get; set; }
        [Parameter]
        public EventCallback<(object, string)> HandleFinaGOSChanged { get; set; }
        [Parameter]
        public EventCallback<(object, string)> HandleFinaCCPChanged { get; set; }
        [Parameter]
        public EventCallback<(object, string)> HandleFinaHOECDChanged { get; set; }
        [Parameter]
        public EventCallback<(object, string)> HandleFinaGOSCDChanged { get; set; }
        [Parameter]
        public EventCallback<(object, string)> HandleFinaCCPCDChanged { get; set; }

        [Parameter]
        public (object, string) InitialFileFolderSelection { get; set; } = (null, string.Empty);

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

        private string hoverIcon = Icons.Material.Filled.FolderOpen;
        private string defaultIcon = Icons.Material.Filled.Folder;

        private Dictionary<TreeItemData, bool> hoverStates = new Dictionary<TreeItemData, bool>();
        private Dictionary<object, (bool, bool)> fileHoverStates = new Dictionary<object, (bool, bool)>();

        private (object, string) finalFilesSelection { get; set; } = (null, string.Empty);

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

            if (InitialFileFolderSelection != (null, ""))
            {

                    if(InitialFileFolderSelection.Item1 is string item)
                    {
                        //Carpeta o documento
                    }
                    else
                    {
                        // es una carpeta
                        //finalFilesSelection = InitialFileFolderSelection;
                    }

                    //varia del render para marcar la carpeta o documetos
                //foreach (var file in finalFilesSelection)
                //{
                //    fileHoverStates[file.Item1] = (false, true);
                //}
            }

            try
            {
                if (ChangeDirectory)
                {
                    try
                    {
                        HOEFolders = await CDMSServices.GetFoldersHOE();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error Get HOE Folder From CDMS");
                        Console.WriteLine(ex.Message);
                    }

                    if (HOEFolders != null)
                    {
                        folderHoeError = false;
                        rootNodeHOE = TreeServices.Make_Tree_HOE(HOEFolders.operation);
                        if (IsHOE)
                        {
                            openTabs.Add(rootNodeHOE);
                            foreach (var file in rootNodeHOE.TreeItems)
                            {
                                fileHoverStates[file] = (false, false);
                            }
                        }
                    }
                    else
                    {
                        folderHoeError = true;
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
                            foreach (var file in rootNodeGOS.TreeItems)
                            {
                                fileHoverStates[file] = (false, false);
                            }
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
                        if (IsCCP)
                        {
                            openTabs.Add(rootNodeCCP);
                            foreach (var file in rootNodeCCP.TreeItems)
                            {
                                fileHoverStates[file] = (false, false);
                            }
                        }
                    }
                    else
                    {
                        folderCCPError = true;
                    }

                }
                else if (IsHOE)
                {
                    try
                    {
                        HOEFolders = await CDMSServices.GetFoldersHOE();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error Get HOE Folder From CDMS");
                        Console.WriteLine(ex.Message);
                    }

                    if (HOEFolders != null)
                    {
                        folderHoeError = false;
                        rootNodeHOE = TreeServices.Make_Tree_HOE(HOEFolders.operation);
                        
                    }
                    else
                    {
                        folderHoeError = true;
                    }
                }
                else if (IsGOS)
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
                       
                    }
                    else
                    {
                        folderGOSError = true;
                    }
                }
                else if (IsCCP)
                {
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
                       
                    }
                    else
                    {
                        folderCCPError = true;
                    }
                }



            }
            catch (Exception ex)
            {

            }
            finally
            {
                ShowLoading = false;
                ChangeFileExplorer();
            }

        }

        private string _buttonText = "CDMS_File_Explorer";

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
            }
        }

        public void ChangeFileExplorer()
        {
            isFinalPath = false;
            finalFilesSelection = (null, "");
            fileHoverStates.Clear();
            hoverStates.Clear();
            searchString = "";
            openTabs.Clear();
            foreach (var key in hoverStates.Keys.ToList())
            {
                hoverStates[key] = false;
            }


          


            if (IsHOE && !IsHOECD)
            {
                var tabs = TreeServices.FindAncestorsByPath(rootNodeHOE, InitialFileFolderSelection.Item2);

                if(tabs.Count > 0)
                {
                    openTabs.AddRange(tabs);
                    OnNodeClick(openTabs.Last());
                }
                else
                {
                    openTabs.Add(rootNodeHOE);
                    foreach (var file in rootNodeHOE.TreeItems)
                    {
                        fileHoverStates[file] = (false, false);
                    }
                }
            }
            if (IsGOS && !IsGOSCD)
            {
                var tabs = TreeServices.FindAncestorsByPath(rootNodeGOS, InitialFileFolderSelection.Item2);

                if (tabs.Count > 0)
                {
                    openTabs.AddRange(tabs);
                    OnNodeClick(openTabs.Last());
                }
                else
                {
                    openTabs.Add(rootNodeGOS);
                    foreach (var file in rootNodeGOS.TreeItems)
                    {
                        fileHoverStates[file] = (false, false);
                    }
                }
            }
            if (IsCCP && !IsCCPCD)
            {
                var tabs = TreeServices.FindAncestorsByPath(rootNodeCCP, InitialFileFolderSelection.Item2);

                if (tabs.Count > 0)
                {
                    openTabs.AddRange(tabs);
                    OnNodeClick(openTabs.Last());
                }
                else
                {
                    openTabs.Add(rootNodeCCP);
                    foreach (var file in rootNodeCCP.TreeItems)
                    {
                        fileHoverStates[file] = (false, false);
                    }
                }
            }
            if (IsHOECD)
            {
                var tabs = TreeServices.FindAncestorsByPath(rootNodeHOE, InitialFileFolderSelection.Item2);

                if (tabs.Count > 0)
                {
                    openTabs.AddRange(tabs);
                    OnNodeClick(openTabs.Last());
                }
                else
                {
                    openTabs.Add(rootNodeHOE);
                    foreach (var file in rootNodeHOE.TreeItems)
                    {
                        fileHoverStates[file] = (false, false);
                    }
                };
            }
            if (IsGOSCD)
            {
                var tabs = TreeServices.FindAncestorsByPath(rootNodeGOS, InitialFileFolderSelection.Item2);

                if (tabs.Count > 0)
                {
                    openTabs.AddRange(tabs);
                    OnNodeClick(openTabs.Last());
                }
                else
                {
                    openTabs.Add(rootNodeGOS);
                    foreach (var file in rootNodeGOS.TreeItems)
                    {
                        fileHoverStates[file] = (false, false);
                    }
                }
            }
            if (IsCCPCD)
            {
                var tabs = TreeServices.FindAncestorsByPath(rootNodeCCP, InitialFileFolderSelection.Item2);

                if (tabs.Count > 0)
                {
                    openTabs.AddRange(tabs);
                    OnNodeClick(openTabs.Last());
                }
                else
                {
                    openTabs.Add(rootNodeCCP);
                    foreach (var file in rootNodeCCP.TreeItems)
                    {
                        fileHoverStates[file] = (false, false);
                    }
                }
            }


            StateHasChanged();
        }

        private void OnSearchStringChanged(string value)
        {
            searchString = value;
            StateHasChanged();
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

                    if (IsHOE)
                    {
                        HoeFilesInFolder = await CDMSServices.GetFilesHOE(clickedNode.Path);
                        foreach (var file in HoeFilesInFolder.operation)
                        {
                            if (finalFilesSelection.Item1 is not TreeItemData && finalFilesSelection != (null, "") && file.ID_DOC == (int)finalFilesSelection.Item1.GetType().GetProperty("ID_DOC")!.GetValue(finalFilesSelection.Item1)!)
                            {
                                fileHoverStates.Add(file, (true, true));
                            }
                            else
                            {
                                fileHoverStates.Add(file, (false, false));
                            }
                        }


                    }
                    else if (IsGOS)
                    {
                        GosFilesInFolder = await CDMSServices.GetFilesGOS(clickedNode.Path);
                        foreach (var file in GosFilesInFolder.operation)
                        {
                            if (finalFilesSelection.Item1 is not TreeItemData &&  finalFilesSelection != (null, "") && file.ID_DOC == (int)finalFilesSelection.Item1.GetType().GetProperty("ID_DOC")!.GetValue(finalFilesSelection.Item1)!)
                            {
                                fileHoverStates.Add(file, (true, true));
                            }
                            else
                            {
                                fileHoverStates.Add(file, (false, false));
                            }
                        }
                    }
                    else if (IsCCP)
                    {
                        CcpFilesInFolder = await CDMSServices.GetFilesCCP(clickedNode.Path);
                        foreach (var file in CcpFilesInFolder.operation)
                        {
                            if (finalFilesSelection.Item1 is not TreeItemData && finalFilesSelection != (null, "") && file.ID_DOC == (int)finalFilesSelection.Item1.GetType().GetProperty("ID_DOC")!.GetValue(finalFilesSelection.Item1)!)
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
                else
                {
                    foreach (var file in clickedNode.TreeItems)
                    {
                        //validacion de ruta ya seleccionada
                        fileHoverStates[file] = (false, false);
                    }
                }
            }
            LoadingContents = false;
            StateHasChanged();
        }

        private async void OnFileNodeClick(object clickedFile)
        {
            foreach (var key in fileHoverStates.Keys.ToList())
            {
                fileHoverStates[key] = (false, false);
            }

            fileHoverStates[clickedFile] = fileHoverStates[clickedFile].Item2 ? (true, false) : (true, true);
            ManipulateFinalList(clickedFile, fileHoverStates[clickedFile].Item2);

        }

        private Task HandleCheck(bool value, object item)
        {
                foreach (var key in fileHoverStates.Keys.ToList())
                {
                        fileHoverStates[key] = (false, false);
                }


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
                //finalFilesSelection.Add((file, finalPath));
                if(file is TreeItemData Folder)
                {
                    finalFilesSelection = (file, Folder.Path);
                }
                else
                {
                    finalFilesSelection = (file, finalPath);
                }
            }
            else
            {
                finalFilesSelection = (null, "");
                //finalFilesSelection.RemoveAll(p=> (int)p.Item1.GetType().GetProperty("ID_DOC")!.GetValue(p.Item1)! == currentId);

                int currentId = -1; 
                var property = file.GetType().GetProperty("ID_DOC");

                if (property != null)
                {
                    var value = property.GetValue(file);
                    if (value != null && int.TryParse(value.ToString(), out int id))
                    {
                        currentId = id; 
                    }
                }

                if (isFinalList && fileHoverStates.Any())
                {
                    if(currentId != -1)
                    {
                        var key = fileHoverStates.FirstOrDefault(p => (int)p.Key.GetType().GetProperty("ID_DOC")!.GetValue(p.Key)! == currentId).Key;
                        if (key != null)
                        {
                            fileHoverStates[key] = (false, false);
                        }
                    }
                    else
                    {
                        fileHoverStates[file] = (false, false);
                    }
             

                }
            }

            if (IsHOE && !IsHOECD)
            {
                HandleFinaHOEChanged.InvokeAsync(finalFilesSelection);
            }else if (IsGOS && !IsGOSCD)
            {
                HandleFinaGOSChanged.InvokeAsync(finalFilesSelection);

            }else if (IsCCP && !IsCCPCD)
            {
                HandleFinaCCPChanged.InvokeAsync(finalFilesSelection);
            }else if (IsHOECD)
            {
                HandleFinaHOECDChanged.InvokeAsync(finalFilesSelection);
            }else if (IsGOSCD)
            {
                HandleFinaGOSCDChanged.InvokeAsync(finalFilesSelection);

            }else if (IsCCPCD)
            {
                HandleFinaCCPCDChanged.InvokeAsync(finalFilesSelection);
            }


        }

        private void DownloadDocument(object document)
        {
            //var id = (int)document.GetType().GetProperty("ID_DOC")!.GetValue(document)!;
            switch (document)
            {
                case GOSDocument gosDoc:
                    Console.WriteLine(gosDoc.ID_DOC);
                    CDMSServices.GetDownloadLinkGOS(gosDoc.ID_DOC, gosDoc.Nombre);
                    break;

                case CCPDocument ccpDoc:
                    Console.WriteLine(ccpDoc.ID_DOC);
                    CDMSServices.GetDownloadLinkCCP(ccpDoc.ID_DOC, ccpDoc.Nombre); break;
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

            if (IsHOE)
            {
                foreach (var file in openTabs[activeTabIndex].TreeItems)
                {
                    fileHoverStates[file] = (false, false);
                }
            }
            if (IsGOS)
            {
                foreach (var file in openTabs[activeTabIndex].TreeItems)
                {
                    fileHoverStates[file] = (false, false);
                }
            }
            else if (IsCCP)
            {
                foreach (var file in openTabs[activeTabIndex].TreeItems)
                {
                    fileHoverStates[file] = (false, false);
                }
            }

        }

        public async Task<HashSet<TreeItemData>> LoadServerData(TreeItemData parentNode)
        {
            await Task.Delay(50);
            return parentNode.TreeItems;
        }


    }
}