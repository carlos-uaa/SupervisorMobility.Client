using Blazorise.Extensions;
using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.Vml.Spreadsheet;
using Microsoft.AspNetCore.Http;
using Microsoft.JSInterop;
using MudBlazor;
using SupervisorMobility.Client.Data.Entities;
using SupervisorMobility.Client.Data.Entities.TreeStruct;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace SupervisorMobility.Client.Pages.Inicio.JobObservationPage
{
    public partial class ProgrammedJobObservationDetails
    {

        [Parameter]
        public int JobObservationId { get; set; }


        [Parameter]
        public User LoggedUser { get; set; }

        [Parameter]
        public bool IsVisible { get; set; }

        [Parameter]
        public EventCallback<bool> IsVisibleChanged { get; set; }

        [Parameter]
        public string ProgrammedStartDate { get; set; }

        public DateTime? AuxProgrammedDate { get; set; }

        public JobObservation _jobObservation { get; set; } = new();

        private DialogOptions dialogOptions = new() { CloseOnEscapeKey = true, MaxWidth = MaxWidth.Large, FullWidth = true };
        public JobObservation _lupJobObservations { get; set; } = new();

        private AssyChart _assychart { get; set; } = new AssyChart();

        //Glosary
        private List<Glosary> glosary = new();
        private Dictionary<string, Glosary> _glosaryInfo;

        //Objects
        private bool dense = false;
        private bool hover = false;
        private bool ronly = false;

        public int plantId;
        public int areaId;
        public int distributionId;
        public int operationId;

        public DateTime? dateStart = DateTime.Today;
        public DateTime? dateEnd = DateTime.Today;
        DateTime LastdayYear = DateTime.Now;


        public string observer { get; set; } = "Juan";
        public string operator1 { get; set; } = "Pedro";

        int[] models = new int[5];
        string[] cycles = new string[5];

        private bool CcpDialog = false;
        private bool HoeDialog = false;
        private bool GosDialog = false;
        private bool searchAssychart = false;

        private CDMS_CCP_Archives? CcpFilesInFolder;
        private CDMS_HOE_Archives? HoeFilesInFolder;
        private CDMS_GOS_Archives? GosFilesInFolder;

        private bool folderError = false;
        private string messageErrorFolders;

        //Operator user
        public List<User> users = new();
        public List<User> operatorUsers = new();
        List<Plant> _plants { get; set; } = new();
        List<Area> _areas = new();
        List<Distribution> _distributions = new();
        List<Operation> _operations = new();


        TimeSpan? endHour;
        TimeSpan? startHour { get; set; }
        public string hour1 { get; set; }
        public string hour2 { get; set; }
        DateTime newDate1;
        DateTime newDate2;

        private List<SOSCodePath> listFilter = new();
        bool FilterOperation = false;

        private IList<string> _sourceMsgLoading = new List<string>();
        private IList<Color> _Colors = new List<Color>() { Color.Default, Color.Primary, Color.Secondary, Color.Success, Color.Info, Color.Default, Color.Primary, Color.Secondary, Color.Success, Color.Info };
        private CDMS_CCP_Archives? AuxCcpFilesInFolder;
        private CDMS_HOE_Archives? AuxHoeFilesInFolder;
        private CDMS_GOS_Archives? AuxGosFilesInFolder;
        //Error Display Rutes Select ONLY
        private bool folderCCPError = false;
        private bool folderHOEError = false;
        private bool folderGOSError = false;
        //Display Files Errors
        private bool folderErrorGOS = false;
        private bool folderErrorCCP = false;
        private bool folderErrorHOE = false;
        private string HOErute = "";
        private string CCPrute = "";
        private string GOSrute = "";
        //CommonDirection
        private bool folderErrorGOSCD = false;
        private bool folderErrorCCPCD = false;
        private bool folderErrorHOECD = false;
        private string HOEruteCD = "";
        private string CCPruteCD = "";
        private string GOSruteCD = "";
        //CommonDirection Files
        private CDMS_CCP_Archives? CcpFilesInFolderCD;
        private CDMS_HOE_Archives? HoeFilesInFolderCD;
        private CDMS_GOS_Archives? GosFilesInFolderCD;
        private CDMS_CCP_Archives? AuxCcpFilesInFolderCD;
        private CDMS_HOE_Archives? AuxHoeFilesInFolderCD;
        private CDMS_GOS_Archives? AuxGosFilesInFolderCD;


        private bool if_pick_Plant = false;
        private bool if_pick_Area = false;
        private bool if_pick_Distribution = false;
        private int productId = 0;
        public int idFilter;

        MudTabs FilesViewer;
        MudTabPanel HOE;
        MudTabPanel HOECD;
        MudTabPanel CCP;
        MudTabPanel CCPCD;
        MudTabPanel GOS;
        MudTabPanel GOSCD;

        public bool CodePathModalDisplay { get; set; } = false;
        private string searchCodeString = "";
        bool ShowLoading = true;

        SOSCodePath CodePathDialogDisplay { get; set; }


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


            //if (DateTime.TryParse(ProgrammedStartDate, CultureInfo.InvariantCulture, DateTimeStyles.None, out var AuxDate))
            //{
            //    Console.WriteLine($"Primera Conversion Exitosa" + AuxDate);
            //    AuxProgrammedDate = AuxDate;
            //    // La conversión fue exitosa, ahora puedes usar 'date'
            //}

            ProgrammedStartDate = ProgrammedStartDate.Replace("-", "/");
            AuxProgrammedDate = DateTime.ParseExact(ProgrammedStartDate, "d/M/yyyy", null);

            _jobObservation.Supervisor = new();
            _jobObservation.Operator = new();

            glosary = await GlosaryService.GetGlosary();
            _glosaryInfo = glosary.ToDictionary(x => x.Name, x => x);

            _jobObservation = await JobObservationService.GetJobObservationById(JobObservationId, true, true);

            _plants = await PlantServices.GetPlants();
            //_products = await ProductService.GetProducts();
            _areas = await AreaServices.GetAreas(_jobObservation.PlantId);
            _distributions = await DistributionService.GetDistributionsWithCollections(_jobObservation.PlantId, _jobObservation.AreaId);

            _operations = _distributions[_distributions.FindIndex(d => d.DistributionId == _jobObservation.DistributionId)].Operations;
            operatorUsers = await UsersService.GetSubordinates(_jobObservation.SupervisorId);
            StateHasChanged();


           

            if (ProgrammedStartDate != "" && ProgrammedStartDate != null)
            {
                _jobObservation.StartDate = AuxProgrammedDate;
                _jobObservation.EndDate = AuxProgrammedDate;
                LastdayYear = new DateTime(_jobObservation.StartDate.Value.Year, 12, 31);
            }
            else
            {
                startHour = _jobObservation.StartDate?.TimeOfDay;
                endHour = _jobObservation.EndDate?.TimeOfDay;

            }

            AssyFolders();
            ShowLoading = false;
        }

        private async Task AssyFolders() {

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

            if (_jobObservation.PlantId != 0)
            {
                if (_jobObservation.AreaId != 0)
                {
                    if (_jobObservation.DistributionId != 0)
                    {
                        if (_jobObservation.DistributionId != 0)
                        {

                            try
                            {
                                _assychart = await AssychartServices.GetAssyChartAdvance(_jobObservation.PlantId, _jobObservation.AreaId, _jobObservation.DistributionId, _jobObservation.OperationId);
                                if (_assychart == null)
                                    messageErrorFolders = Localizer["theFoldersWithTheInformationWereNotLocated"];
                                else
                                    searchAssychart = true;
                            }
                            catch (Exception ex)
                            {
                                messageErrorFolders = Localizer["theFoldersWithTheInformationWereNotLocated"];
                            }

                            if (_assychart == null)
                                messageErrorFolders = Localizer["theFoldersWithTheInformationWereNotLocated"];
                            else
                                searchAssychart = true;

                        }
                        else
                        {
                            messageErrorFolders = Localizer["jobObservationDoesNotContainAValidOperation"];
                        }
                    }
                    else
                    {
                        messageErrorFolders = Localizer["jobObservationDoesNotContainAValidDistribution"];
                    }
                }
                else
                {
                    messageErrorFolders = Localizer["jobObservationDoesNotContainAValidArea"];
                }
            }
            else
            {
                messageErrorFolders = Localizer["jobObservationDoesNotContainAValidPlant"];
            }

            if (searchAssychart)
            {
                listFilter = _assychart.RoutesProductsAssyChart.Where(r => r.Code.ToLower().Contains(_jobObservation.Operation.Code.ToLower(), StringComparison.OrdinalIgnoreCase)).ToList();
                FilterOperation = true;
            }
        }

        private void OnStartDateChanged(DateTime dt)
        {
            _jobObservation.StartDate = dt;
            _jobObservation.EndDate = dt;
        }

        private async Task PlanNewJobObservation()
        {
            if (_jobObservation.StartDate == null)
            {
                Snackbar.Clear();
                Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                Snackbar.Add($"Select the Start Date first", Severity.Warning);
                return;
            }
            if (_jobObservation.EndDate == null)
            {
                Snackbar.Clear();
                Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                Snackbar.Add($"Select the End Date first", Severity.Warning);
                return;
            }
            if (_jobObservation.OperationId == 0)
            {
                Snackbar.Clear();
                Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                Snackbar.Add($"Select the Operation first", Severity.Warning);
                return;
            }

            if (_jobObservation.Option == 3 && _jobObservation.Anomaly.IsNullOrEmpty())
            {
                Snackbar.Clear();
                Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                Snackbar.Add($"Write down the anomaly first", Severity.Warning);
                return;
            }


            startHour = DateTime.Now.TimeOfDay;
            endHour = new TimeSpan(00, 00, 00);

            if (CultureInfo.CurrentCulture.Name == "en-US")
            {

                var formatedStartDate = _jobObservation.StartDate;
                var formatedEndDate = _jobObservation.EndDate;

                var EnglishStartDate = formatedStartDate?.Month.ToString() + "/" + formatedStartDate?.Day.ToString() + "/" + formatedStartDate?.Year.ToString();
                var EnglishEndDate = formatedEndDate?.Month.ToString() + "/" + formatedEndDate?.Day.ToString() + "/" + formatedEndDate?.Year.ToString();
                _jobObservation.StartDate = DateTime.ParseExact(EnglishStartDate, "M/d/yyyy", CultureInfo.InvariantCulture);
                _jobObservation.EndDate = DateTime.ParseExact(EnglishEndDate, "M/d/yyyy", CultureInfo.InvariantCulture);


                hour1 = _jobObservation.StartDate?.ToShortDateString() + $" {startHour?.ToString("hh\\:mm\\:ss")}";
                hour2 = _jobObservation.EndDate?.ToShortDateString() + $" {endHour?.ToString("hh\\:mm\\:ss")}";


                if (DateTime.TryParseExact(hour1, $"M/d/yyyy HH:mm:ss", null, DateTimeStyles.None, out newDate1))
                {
                    Console.WriteLine(newDate1);
                }
                else
                {
                    Snackbar.Clear();
                    Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                    Snackbar.Add($"Error in Date Start", Severity.Error);
                    Console.WriteLine("Unable to parse '{0}'", hour1);
                }

                if (DateTime.TryParseExact(hour2, $"M/d/yyyy HH:mm:ss", null, DateTimeStyles.None, out newDate2))
                {
                    Console.WriteLine(newDate2);
                }
                else
                {
                    Snackbar.Clear();
                    Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                    Snackbar.Add($"Error in Date End", Severity.Error);
                    Console.WriteLine("Unable to parse '{0}'", hour2);
                }

                _jobObservation.StartDate = newDate1;
                _jobObservation.EndDate = newDate2;
                _jobObservation.Status = 1;


                var result = await JobObservationService.UpdateJobObservation(_jobObservation, LoggedUser.ObjectId);

                if (result)
                {

                    Snackbar.Clear();
                    Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                    Snackbar.Add($"Job Observation {_jobObservation.JobObservationId} Planned", Severity.Info);
                    //NavigationManager.NavigateTo("/jobobservationschedule", true);
                    IsVisible = false; // Cambiar el valor a false
                    await IsVisibleChanged.InvokeAsync(false);
                }
                else
                    await JSRuntime.InvokeVoidAsync("alert", "Update failed!"); // Alert


            }
            else
            {

                hour1 = _jobObservation.StartDate?.ToShortDateString() + $" {startHour?.ToString("hh\\:mm\\:ss")}";
                hour2 = _jobObservation.EndDate?.ToShortDateString() + $" {endHour?.ToString("hh\\:mm\\:ss")}";

                if (DateTime.TryParseExact(hour1, $"d/M/yyyy HH:mm:ss", null, DateTimeStyles.None, out newDate1))
                {
                    Console.WriteLine(newDate1);
                }
                else
                {
                    Snackbar.Clear();
                    Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                    Snackbar.Add($"Error in Date Start", Severity.Error);
                    Console.WriteLine("Unable to parse '{0}'", hour1);
                }


                if (DateTime.TryParseExact(hour2, $"d/M/yyyy HH:mm:ss", null, DateTimeStyles.None, out newDate2))
                {
                    Console.WriteLine(newDate2);
                }
                else
                {
                    Snackbar.Clear();
                    Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                    Snackbar.Add($"Error in Date End", Severity.Error);
                    Console.WriteLine("Unable to parse '{0}'", hour2);
                }

                _jobObservation.StartDate = newDate1;
                _jobObservation.EndDate = newDate2;
                _jobObservation.Status = 1;


                var result = await JobObservationService.UpdateJobObservation(_jobObservation, LoggedUser.ObjectId);

                if (result)
                {

                    Snackbar.Clear();
                    Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                    Snackbar.Add($"Job Observation {_jobObservation.JobObservationId} Planned", Severity.Info);
                    //NavigationManager.NavigateTo("/jobobservationschedule", true);
                    IsVisible = false; // Cambiar el valor a false
                    await IsVisibleChanged.InvokeAsync(false);
                }
                else
                    await JSRuntime.InvokeVoidAsync("alert", "Update failed!"); // Alert
            }
        }

        void history()
        {
            NavigationManager.NavigateTo($"jobobservation/history/{JobObservationId}");
        }

       

        private async void CloseModalFiles()
        {
            CodePathModalDisplay = false;

            StateHasChanged();

        }
        private async Task<AsyncVoidMethodBuilder> OpenDialogCodePath(SOSCodePath itemselected, MudTabPanel panelSelect)
        {
            searchCodeString = itemselected.Code;
            ShowLoading = true;
            CodePathModalDisplay = true;
            HoeFilesInFolder = new CDMS_HOE_Archives();
            StateHasChanged();

            try
            {
                CodePathDialogDisplay = itemselected;

                HOErute = itemselected.HOE;
                if (itemselected.HOE != "")
                {
                    Console.WriteLine($"hoe {itemselected.HOE}");
                    HoeFilesInFolder = await CDMSServices.GetFilesHOE(itemselected.HOE);
                    if (HoeFilesInFolder == null)
                        folderErrorHOE = true;
                    else
                    {
                        AuxHoeFilesInFolder = ObjectCloner.ObjectCloner.DeepClone(HoeFilesInFolder);

                        folderErrorHOE = false;
                    }
                }

                folderErrorGOS = true;
                GOSrute = itemselected.GOS;

                if (itemselected.GOS != "")
                {

                    Console.WriteLine($"gos {GOSrute}");


                    GosFilesInFolder = await CDMSServices.GetFilesGOS(GOSrute);
                    if (GosFilesInFolder == null)
                    {
                        folderErrorGOS = true;
                    }
                    else
                    {
                        folderErrorGOS = false;
                        AuxGosFilesInFolder = ObjectCloner.ObjectCloner.DeepClone(GosFilesInFolder);
                    }

                }

                folderErrorCCP = true;
                CCPrute = itemselected.CCP;
                if (itemselected.CCP != "")
                {

                    Console.WriteLine($"CCP {CCPrute}");

                    CcpFilesInFolder = new CDMS_CCP_Archives();
                    CcpFilesInFolder = await CDMSServices.GetFilesCCP(CCPrute);
                    if (CcpFilesInFolder == null)
                        folderErrorCCP = true;
                    else
                    {
                        AuxCcpFilesInFolder = ObjectCloner.ObjectCloner.DeepClone(CcpFilesInFolder);
                        folderErrorCCP = false;

                    }
                }


                //Common Directions
                folderErrorGOSCD = true;
                if (itemselected.CommonDirectionGOS != "")
                {

                    GOSruteCD = itemselected.CommonDirectionGOS;

                    Console.WriteLine($"gos cd {GOSruteCD}");

                    GosFilesInFolderCD = new CDMS_GOS_Archives();

                    GosFilesInFolderCD = await CDMSServices.GetFilesGOS(GOSruteCD);
                    if (GosFilesInFolderCD == null)
                    {
                        folderErrorGOSCD = true;
                    }
                    else
                    {
                        folderErrorGOSCD = false;
                        AuxGosFilesInFolderCD = ObjectCloner.ObjectCloner.DeepClone(GosFilesInFolderCD);
                    }

                }

                folderErrorCCPCD = true;
                if (itemselected.CommonDirectionCCP != "")
                {
                    CCPruteCD = itemselected.CommonDirectionCCP;
                    Console.WriteLine($"Ccp cd {CCPruteCD}");

                    CcpFilesInFolderCD = new CDMS_CCP_Archives();
                    CcpFilesInFolderCD = await CDMSServices.GetFilesCCP(CCPruteCD);
                    if (CcpFilesInFolderCD == null)
                        folderErrorCCPCD = true;
                    else
                    {
                        folderErrorCCPCD = false;
                        AuxCcpFilesInFolderCD = ObjectCloner.ObjectCloner.DeepClone(CcpFilesInFolderCD);
                    }

                }

                if (itemselected.CommonDirectionHOE != "")
                {


                    folderErrorHOECD = true;

                    HOEruteCD = itemselected.CommonDirectionHOE;
                    Console.WriteLine($"hoe cd {HOEruteCD}");
                    HoeFilesInFolderCD = new CDMS_HOE_Archives();
                    HoeFilesInFolderCD = await CDMSServices.GetFilesHOE(HOEruteCD);
                    if (HoeFilesInFolderCD == null)
                        folderErrorHOECD = true;
                    else
                    {
                        AuxHoeFilesInFolderCD = ObjectCloner.ObjectCloner.DeepClone(HoeFilesInFolderCD);

                        folderErrorHOECD = false;
                    }
                }

                //EndCommon Directions




            }
            catch (Exception ex)
            {
                Console.WriteLine($"OpenDialogCodePath Error: {ex.Message}");
            }
            finally
            {
                await SearchFunction();
                ShowLoading = false;
                StateHasChanged();
            }

            return new AsyncVoidMethodBuilder();
        }
        private async Task<AsyncVoidMethodBuilder> SearchFunction()
        {
            Console.WriteLine($"SearchFunction - Start {DateTime.Now}");

            if (CodePathDialogDisplay != null)
            {
                try
                {
                    ShowLoading = true;
                    Console.WriteLine($"State Start {ShowLoading}");
                    StateHasChanged();

                    if (string.IsNullOrEmpty(searchCodeString))
                    {
                        if (CodePathDialogDisplay.HOE != "")
                            HoeFilesInFolder = ObjectCloner.ObjectCloner.DeepClone(AuxHoeFilesInFolder);

                        if (CodePathDialogDisplay.GOS != "")
                            GosFilesInFolder = ObjectCloner.ObjectCloner.DeepClone(AuxGosFilesInFolder);

                        if (CodePathDialogDisplay.CCP != "")
                            CcpFilesInFolder = ObjectCloner.ObjectCloner.DeepClone(AuxCcpFilesInFolder);

                        if (CodePathDialogDisplay.CommonDirectionHOE != "")
                            HoeFilesInFolderCD = ObjectCloner.ObjectCloner.DeepClone(AuxHoeFilesInFolderCD);

                        if (CodePathDialogDisplay.CommonDirectionGOS != "")
                            GosFilesInFolderCD = ObjectCloner.ObjectCloner.DeepClone(AuxGosFilesInFolderCD);

                        if (CodePathDialogDisplay.CommonDirectionCCP != "")
                            CcpFilesInFolderCD = ObjectCloner.ObjectCloner.DeepClone(AuxCcpFilesInFolderCD);
                    }
                    else
                    {
                        if (CodePathDialogDisplay.HOE != "")
                        {
                            HoeFilesInFolder = ObjectCloner.ObjectCloner.DeepClone(AuxHoeFilesInFolder);
                            HoeFilesInFolder.operation = HoeFilesInFolder.operation.Where(x => x.Nombre.ToLower().Contains(searchCodeString.ToLower(), StringComparison.InvariantCultureIgnoreCase)).ToList();
                        }

                        if (CodePathDialogDisplay.GOS != "")
                        {
                            GosFilesInFolder = ObjectCloner.ObjectCloner.DeepClone(AuxGosFilesInFolder);
                            GosFilesInFolder.operation = AuxGosFilesInFolder.operation.Where(x => x.Nombre.ToLower().Contains(searchCodeString.ToLower(), StringComparison.InvariantCultureIgnoreCase)).ToList();
                        }

                        if (CodePathDialogDisplay.CCP != "")
                        {
                            CcpFilesInFolder = ObjectCloner.ObjectCloner.DeepClone(AuxCcpFilesInFolder);
                            CcpFilesInFolder.operation = CcpFilesInFolder.operation.Where(x => x.Nombre.ToLower().Contains(searchCodeString.ToLower(), StringComparison.InvariantCultureIgnoreCase)).ToList();
                        }

                        if (CodePathDialogDisplay.CommonDirectionHOE != "")
                        {
                            HoeFilesInFolderCD = ObjectCloner.ObjectCloner.DeepClone(AuxHoeFilesInFolderCD);
                            HoeFilesInFolderCD.operation = HoeFilesInFolderCD.operation.Where(x => x.Nombre.ToLower().Contains(searchCodeString.ToLower(), StringComparison.InvariantCultureIgnoreCase)).ToList();
                        }

                        if (CodePathDialogDisplay.CommonDirectionGOS != "")
                        {
                            GosFilesInFolderCD = ObjectCloner.ObjectCloner.DeepClone(AuxGosFilesInFolderCD);
                            GosFilesInFolderCD.operation = GosFilesInFolderCD.operation.Where(x => x.Nombre.ToLower().Contains(searchCodeString.ToLower(), StringComparison.InvariantCultureIgnoreCase)).ToList();
                        }

                        if (CodePathDialogDisplay.CommonDirectionCCP != "")
                        {
                            CcpFilesInFolderCD = ObjectCloner.ObjectCloner.DeepClone(AuxCcpFilesInFolderCD);
                            CcpFilesInFolderCD.operation = CcpFilesInFolderCD.operation.Where(x => x.Nombre.ToLower().Contains(searchCodeString.ToLower(), StringComparison.InvariantCultureIgnoreCase)).ToList();
                        }

                    }



                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error Filter: {ex.Message}");
                }
                finally
                {
                    ShowLoading = false;
                    StateHasChanged();
                }
            }
            else
            {
                Console.WriteLine($"Error Filter es nullo");
            }

            Console.WriteLine($"SearchFunction - End {DateTime.Now}");
            Console.WriteLine($"State End {ShowLoading}");
            //// if text is null or empty, show complete list
            //if (string.IsNullOrEmpty(searchString))
            //    return GosFilesInFolder.operation;

            //return GosFilesInFolder.operation.Where(x => x.Nombre.ToLower().Contains(searchString.ToLower(), StringComparison.InvariantCultureIgnoreCase)).ToList();
            return new AsyncVoidMethodBuilder();
        }


        private async Task DownloadFileFromURL(string urlroute, string namefile)
        {
            var fileName = namefile;
            var fileURL = urlroute;
            await JS.InvokeVoidAsync("triggerFileDownload", fileName, fileURL);
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
        }
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

        }

        TreeItemData nodoEncontrado { get; set; }
        CDMS_CCP_Directory CCPFolders { get; set; } = new CDMS_CCP_Directory();

        TreeItemData rootNodeCCP { get; set; } = new TreeItemData();
        TreeItemData SelectedNodeCCP { get; set; }
        private async Task<AsyncVoidMethodBuilder> CCPFolderByDirectory(string CCPrute)
        {

            try
            {
                ShowLoading = true;

                if (CCPrute != "")
                {
                    Console.WriteLine($"CCP {CCPrute}");

                    CcpFilesInFolder = new CDMS_CCP_Archives();
                    CcpFilesInFolder = await CDMSServices.GetFilesCCP(CCPrute);
                    if (CcpFilesInFolder == null)
                        folderErrorCCP = true;
                    else
                    {
                        AuxCcpFilesInFolder = ObjectCloner.ObjectCloner.DeepClone(CcpFilesInFolder);
                        folderErrorCCP = false;

                    }
                }



            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error CCPFolderByDirectory: {ex.Message}");
            }
            finally
            {
                ShowLoading = false;
                StateHasChanged();
            }

            return new AsyncVoidMethodBuilder();

        }

    }
}
