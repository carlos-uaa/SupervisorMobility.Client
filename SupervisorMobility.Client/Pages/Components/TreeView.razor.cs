using Microsoft.JSInterop;
using SupervisorMobility.Client.Data.Entities.TreeStruct;
using MudBlazor;

namespace SupervisorMobility.Client.Pages.Components
{
    public partial class TreeView
    {
        [Parameter]
        public int Path_Id { get; set; }
        [Parameter]
        public string? panel { get; set; }

        private SOSCodePath Path_Item { get; set; }
        private int selectPanel { get; set; } = 0;
        private bool ShowLoading = true;
        private bool ShowLoadingFiles = true;
        private bool Files_In_CCP = true;
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

            try
            {
                Path_Item = await TreeServices.getCodePath(Path_Id);

                await TreeServices.setNodesByPath(Path_Item);


                Node_CCP = TreeServices.getNodeCCP();
                Node_GOS = TreeServices.getNodeGOS();
                Node_HOE = TreeServices.getNodeHOE();
                Node_CCP_CD = TreeServices.getNodeCCP_CD();
                Node_GOS_CD = TreeServices.getNodeGOS_CD();
                Node_HOE_CD = TreeServices.getNodeHOE_CD();


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
            }
            StateHasChanged();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {

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
        private async Task DownloadFileFromURL_HOE(string urlroute, string namefile)
        {
            var fileName = namefile;
            var fileURL = urlroute;
            await JS.InvokeVoidAsync("triggerFileDownload", fileName, fileURL);
        }

        private async Task DownloadFileFromURL_CCP(string urlroute, string namefile)
        {

            CDMS_DownloadFile DownloadLink = await CDMSServices.GetDownloadLinkCCP(urlroute);

            if (DownloadLink is not null)
            {
                var fileName = namefile;
                var fileURL = DownloadLink?.operation.URL;

                Console.WriteLine($"NamekEY: {DownloadLink?.operation.NameDocKey}");

                try
                {
                    var result = await JS.InvokeAsync<string>("triggerFileDownloadAndWaitForConfirmation", fileName, fileURL);
                    if (result == "File downloaded successfully")
                    {
                        var DeleteTemp = await CDMSServices.DeleteFileTempCCP(DownloadLink?.operation.NameDocKey);
                        if (DeleteTemp is not null)
                        {
                            Console.WriteLine($"Download GOS - fileDownlaod Succes");
                        }
                    }

                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error In Download Gos File: {ex.Message} ");
                }
            }
        }//end download file ccp

        private async Task DownloadFileFromURL_GOS(string urlroute, string namefile)
        {
            CDMS_DownloadFile DownloadLink = await CDMSServices.GetDownloadLinkGOS(urlroute);

            if (DownloadLink is not null)
            {
                var fileName = namefile;
                var fileURL = DownloadLink?.operation.URL;

                Console.WriteLine($"NamekEY: {DownloadLink?.operation.NameDocKey}");

                try
                {
                    var result = await JS.InvokeAsync<string>("triggerFileDownloadAndWaitForConfirmation", fileName, fileURL);
                    if (result == "File downloaded successfully")
                    {
                        var DeleteTemp = await CDMSServices.DeleteFileTempGOS(DownloadLink?.operation.NameDocKey);
                        if (DeleteTemp is not null)
                        {
                            Console.WriteLine($"Download GOS - fileDownlaod Succes");
                        }
                    }

                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error In Download Gos File: {ex.Message} ");
                }
            }

        }//end download file gos

        //end fragment code
    }
}