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
                Path_Item = await TreeServices.getCodePath(Path_Id);
                searchCodeString = Path_Item.Code;
                await TreeServices.setNodesByPath(Path_Item);


                Node_CCP = await TreeServices.getNodeCCP();
                Node_GOS = await TreeServices.getNodeGOS();
                Node_HOE = await TreeServices.getNodeHOE();
                Node_CCP_CD = await TreeServices.getNodeCCP_CD();
                Node_GOS_CD = await TreeServices.getNodeGOS_CD();
                Node_HOE_CD = await TreeServices.getNodeHOE_CD();


                if (Node_CCP != null)
                {
                    // El nodo fue encontrado, si tiene archivos los optenemos
                    Console.WriteLine("Name CCP: " + Node_CCP.Name);

                    await TreeServices.GetFilesInNodeCCP(Node_CCP.Is_Directory ? Node_CCP.TreeItems.FirstOrDefault() : Node_CCP);
                    StateHasChanged();
                }

                if (Node_GOS != null)
                {
                    // El nodo fue encontrado, si tiene archivos los optenemos
                    Console.WriteLine("Name GOS: " + Node_GOS.Name);
                    await TreeServices.GetFilesInNodeGOS(Node_GOS.Is_Directory ? Node_GOS.TreeItems.FirstOrDefault() : Node_GOS);
                    // await TreeServices.GetFilesInNodeCCP(Node_CCP);
                }

                if (Node_HOE != null)
                {
                    // El nodo fue encontrado, si tiene archivos los optenemos
                    Console.WriteLine("Name HOE: " + Node_HOE.Name);
                    await GetHOETab(Node_HOE);
                    StateHasChanged();
                }
                //CommonDirection
                if (Node_CCP_CD != null)
                {
                    // El nodo fue encontrado, si tiene archivos los optenemos
                    Console.WriteLine("Name CCP_CD: " + Node_CCP_CD.Name);

                    await TreeServices.GetFilesInNodeCCP(Node_CCP_CD.Is_Directory ? Node_CCP_CD.TreeItems.FirstOrDefault() : Node_CCP_CD);
                    StateHasChanged();
                }

                if (Node_GOS_CD != null)
                {
                    // El nodo fue encontrado, si tiene archivos los optenemos
                    Console.WriteLine("Name GOS_CD: " + Node_GOS_CD.Name);
                    await TreeServices.GetFilesInNodeGOS(Node_GOS_CD.Is_Directory ? Node_GOS_CD.TreeItems.FirstOrDefault() : Node_GOS_CD);
                    // await TreeServices.GetFilesInNodeCCP(Node_CCP);
                }

                if (Node_HOE_CD != null)
                {
                    // El nodo fue encontrado, si tiene archivos los optenemos
                    Console.WriteLine("Name HOE_CD: " + Node_HOE_CD.Name);
                    await GetHOETab(Node_HOE_CD);
                    StateHasChanged();
                }


            }
            catch (Exception ex)
            {
                Console.WriteLine("Error on OnInitializedAsync");
                Console.WriteLine(ex);
            }
            finally
            {
                ShowLoading = false;
                switch (panel)
                {
                    case "HOE":
                        if(Node_HOE != null)
                        {
                            selectPanel.ActivatePanel(panel_HOE);
                        }
                        break;
                    case "CCP":
                        if (Node_CCP != null)
                        {
                            selectPanel.ActivatePanel(panel_CCP);
                        }
                        break;

                    case "GOS":
                        if (Node_GOS != null)
                        {
                            selectPanel.ActivatePanel(panel_GOS);
                        }
                        break;
                    case "HOE_CD":
                        if (Node_HOE_CD != null)
                        {
                            selectPanel.ActivatePanel(panel_HOE_CD);
                        }
                        break;
                    case "CCP_CD":
                        if (Node_CCP_CD != null)
                        {
                            selectPanel.ActivatePanel(panel_CCP_CD);
                        }
                        break;

                    case "GOS_CD":
                        if (Node_GOS_CD != null)
                        {
                            selectPanel.ActivatePanel(panel_GOS_CD);
                        }
                        break;
                }
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
            var fileName = namefile;
            var fileURL = urlroute;
            await JS.InvokeVoidAsync("triggerFileDownload", fileName, fileURL);
        }

        private async Task DownloadFileFromURL_CCP(string urlroute, string namefile)
        {

            _ = await CDMSServices.GetDownloadLinkCCP(urlroute, namefile);

            //if (DownloadLink is not null)
            //{
            //    var fileName = namefile;
            //    var fileURL = DownloadLink?.operation.URL;

            //    Console.WriteLine($"NamekEY: {DownloadLink?.operation.NameDocKey}");

            //    try
            //    {
            //        var result = await JS.InvokeAsync<string>("triggerFileDownloadAndWaitForConfirmation", fileName, fileURL);
            //        if (result == "File downloaded successfully")
            //        {
            //            var DeleteTemp = await CDMSServices.DeleteFileTempCCP(DownloadLink?.operation.NameDocKey);
            //            if (DeleteTemp is not null)
            //            {
            //                Console.WriteLine($"Download GOS - fileDownlaod Succes");
            //            }
            //        }

            //    }
            //    catch (Exception ex)
            //    {
            //        Console.WriteLine($"Error In Download Gos File: {ex.Message} ");
            //    }
            //}
        }//end download file ccp

        private async Task DownloadFileFromURL_GOS(string urlroute, string namefile)
        {
            _ = await CDMSServices.GetDownloadLinkGOS(urlroute, namefile);

            //if (DownloadLink is not null)
            //{
            //    var fileName = namefile;
            //    var fileURL = DownloadLink?.operation.URL;

            //    Console.WriteLine($"NamekEY: {DownloadLink?.operation.NameDocKey}");

            //    try
            //    {
            //        var result = await JS.InvokeAsync<string>("triggerFileDownloadAndWaitForConfirmation", fileName, fileURL);
            //        if (result == "File downloaded successfully")
            //        {
            //            var DeleteTemp = await CDMSServices.DeleteFileTempGOS(DownloadLink?.operation.NameDocKey);
            //            if (DeleteTemp is not null)
            //            {
            //                Console.WriteLine($"Download GOS - fileDownlaod Succes");
            //            }
            //        }

            //    }
            //    catch (Exception ex)
            //    {
            //        Console.WriteLine($"Error In Download Gos File: {ex.Message} ");
            //    }
            //}

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
    }
        //end fragment code
}