using DocumentFormat.OpenXml.Vml.Spreadsheet;
using Microsoft.JSInterop;
using MudBlazor;
using SupervisorMobility.Client.Data.Entities;
using SupervisorMobility.Client.Data.Entities.TreeStruct;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace SupervisorMobility.Client.Pages.Inicio.JobObservationPage
{
    public partial class JobObservationDetails
    {

        [Parameter]
        public int JobObservationId { get; set; }
        public JobObservation _jobObservation { get; set; } = new();
        List<Product> _products { get; set; } = new();
        public Lup lup { get; set; } = new();
        //Lup Modal
        private bool visible = false;
        private int lupId;

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

        public string hour1 { get; set; }
        public string hour2 { get; set; }
        TimeSpan? endHour { get; set; }
        TimeSpan? startHour { get; set; }

        public int plantId;
        public int areaId;
        public int distributionId;
        public int operationId;

        public DateTime? dateStart = DateTime.Today;
        public DateTime? dateEnd = DateTime.Today;

        public string observer { get; set; } = "Juan";
        public string operator1 { get; set; } = "Pedro";

        int[] models = new int[5];
        string[] cycles = new string[5];
        string[] HoeTimes = new string[5];

        private bool CcpDialog = false;
        private bool HoeDialog = false;
        private bool GosDialog = false;
        private bool searchAssychart = false;

        private bool folderError = false;
        private string messageErrorFolders;

        private List<SOSCodePath> listFilter = new();
        bool FilterOperation = false;

        //Operator user
        public List<User> users = new();
        public List<User> operatorUsers = new();


        //Edit Date
        TimeSpan? changeStartHour { get; set; }
        TimeSpan? changeEndHour { get; set; }

        string cycle1Color = "";
        string cycle2Color = "";
        string cycle3Color = "";
        string cycle4Color = "";
        string cycle5Color = "";

        public string[] questions = new string[5];

        public double taktTime { get; set; }
        public int kpiID = 0;
        public int auxErgonomicsLevel = 0;

        public List<JobCategoryStructure> _checklistCategoriesAndQuestions { get; set; } = new();
        public List<ChecklistAnswer> _checklistAnswers { get; set; } = new();
        private Dictionary<int, string> questionResponses = new Dictionary<int, string>();
        
        private Dictionary<int, ChecklistAnswer> questionAnswers = new Dictionary<int, ChecklistAnswer>();
        Dictionary<int, string> imageUrls = new Dictionary<int, string>();

        protected async override Task OnInitializedAsync()
        {

            _jobObservation.Supervisor = new();
            _jobObservation.Operator = new();

            glosary = await GlosaryService.GetGlosary();
            _glosaryInfo = glosary.ToDictionary(x => x.Name, x => x);

            _jobObservation = await JobObservationService.GetJobObservationById(JobObservationId, true, true, true, includeCkAnswers: true);
            //_jobObservation = await JobObservationService.GetJobObservationById(JobObservationId);
            _products = await ProductService.GetProducts();

            _checklistCategoriesAndQuestions = await ChecklistService.GetChecklistCategories(true);
            _checklistAnswers = await ChecklistAnswerServices.GetAllChecklistAnswersByJobObservationId(JobObservationId);


            foreach (var category in _checklistCategoriesAndQuestions)
            {
                foreach (var question in category.ChecklistQuestions)
                {
                    if (_jobObservation.ChecklistAnswers.Any(cka => cka.QuestionID == question.QuestionID))
                    {
                        var item = _jobObservation.ChecklistAnswers.ToList().Find(cka => cka.QuestionID == question.QuestionID);
                        if (item.Evidences.Count > 0)
                        {
                            item.Show = true;
                            foreach (var evidence in item.Evidences)
                            {
                                var imageUrl = await FilesServices.ShowImageEvidence(evidence.FileUploadId);
                                imageUrls[evidence.FileUploadId] = imageUrl;

                            }
                        }
                       
                    }
                    else
                    {
                        ChecklistAnswer newChAnswer = new();
                        newChAnswer.JobObservationId = _jobObservation.JobObservationId;
                        newChAnswer.QuestionID = question.QuestionID;
                        newChAnswer.Prompt = question.Prompt;
                        questionAnswers.Add(question.QuestionID, newChAnswer);
                    }
                }
            }


            if (_jobObservation.KpiId != null)
            {
                kpiID = (int)_jobObservation.KpiId;
            }

            if (_jobObservation.TaktTime == null)
            {
                taktTime = 0.0;
            }
            else
            {
                taktTime = double.Parse(_jobObservation.TaktTime, CultureInfo.InvariantCulture);
            }

            if (_jobObservation.Questions != null)
            {
                var quets = _jobObservation.Questions.Split('|');
                for (int i = 0; i < 5; i++)
                {
                    questions[i] = quets[i];
                }
            }
            else
            {
                for (int i = 0; i < 5; i++)
                {
                    questions[i] = null;
                }
            }


            if (_jobObservation.HOEStandardTimes != null)
            {
                HoeTimes = _jobObservation.HOEStandardTimes.Replace(',', '.').Split('|');
            }
            else
            {
                HoeTimes[0] = "";
                HoeTimes[1] = "";
                HoeTimes[2] = "";
                HoeTimes[3] = "";
                HoeTimes[4] = "";
            }
            if (_jobObservation.ModelsSpecification != null)
            {
                var prod = _jobObservation.ModelsSpecification.Split('|');
                models[0] = Int32.Parse(prod[0]);
                models[1] = Int32.Parse(prod[1]);
                models[2] = Int32.Parse(prod[2]);
                models[3] = Int32.Parse(prod[3]);
                models[4] = Int32.Parse(prod[4]);

            }
            else
            {
                models[0] = 0;
                models[1] = 0;
                models[2] = 0;
                models[3] = 0;
                models[4] = 0;
            }

            if (_jobObservation.Cycles != null)
            {
                cycles = _jobObservation.Cycles.Replace(',', '.').Split('|');
            }
            else
            {
                cycles[0] = "";
                cycles[1] = "";
                cycles[2] = "";
                cycles[3] = "";
                cycles[4] = "";
            }

            for (int i = 0; i < HoeTimes.Length; i++)
            {
                if (double.TryParse(cycles[i], out double cycleValue2) && double.Parse(HoeTimes[i]) != 0.0)
                {
                    double lowerBound = double.Parse(HoeTimes[i]) * 0.95; // Valor mínimo permitido (95% de HoeTimes)
                    double upperBound = double.Parse(HoeTimes[i]) * 1.05; // Valor máximo permitido (105% de HoeTimes)

                    if (cycleValue2 >= lowerBound && cycleValue2 <= upperBound)
                    {
                        switch (i)
                        {
                            case 0: cycle1Color = "green"; break;
                            case 1: cycle2Color = "green"; break;
                            case 2: cycle3Color = "green"; break;
                            case 3: cycle4Color = "green"; break;
                            case 4: cycle5Color = "green"; break;
                        }
                    }
                    else if (cycleValue2 < double.Parse(HoeTimes[i]))
                    {
                        switch (i)
                        {
                            case 0: cycle1Color = "yellow"; break;
                            case 1: cycle2Color = "yellow"; break;
                            case 2: cycle3Color = "yellow"; break;
                            case 3: cycle4Color = "yellow"; break;
                            case 4: cycle5Color = "yellow"; break;
                        }
                    }
                    else
                    {
                        switch (i)
                        {
                            case 0: cycle1Color = "red"; break;
                            case 1: cycle2Color = "red"; break;
                            case 2: cycle3Color = "red"; break;
                            case 3: cycle4Color = "red"; break;
                            case 4: cycle5Color = "red"; break;
                        }
                    }
                }
            }

            StateHasChanged();

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
                rootNodeCCP = TreeServices.ConstruirArbolCCP(CCPFolders.operation);
            }
            else
            {
                folderCCPError = true;
            }


            startHour = _jobObservation.StartDate?.TimeOfDay;
            endHour = _jobObservation.EndDate?.TimeOfDay;

            changeStartHour = _jobObservation.PlannedStartDate?.TimeOfDay;
            changeEndHour = _jobObservation.PlannedEndDate?.TimeOfDay;

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
                                _assychart = await AssychartServices.GetAssyChartJobObservation(_jobObservation.PlantId, _jobObservation.AreaId, _jobObservation.DistributionId);
                                if (_assychart == null)
                                {

                                    messageErrorFolders = Localizer["theFoldersWithTheInformationWereNotLocated"];
                                }
                                else
                                {
                                    if (_assychart.ErgonomicsLevel != null)
                                    {
                                        auxErgonomicsLevel = (int)_assychart.ErgonomicsLevel;
                                    }

                                    searchAssychart = true;
                                }
                            }
                            catch (Exception ex)
                            {
                                messageErrorFolders = Localizer["theFoldersWithTheInformationWereNotLocated"];
                            }

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

            if (searchAssychart && _assychart.RoutesProductsAssyChart?.Count > 0)
            {
                listFilter = _assychart.RoutesProductsAssyChart.Where(r => r.Code.ToLower().Contains(_jobObservation.Operation.Code.ToLower(), StringComparison.OrdinalIgnoreCase)).ToList();
                FilterOperation = true;
            }

        }

        void Closed(MudChip chip)
        {
            // react to chip closed
        }

        void history()
        {
            NavigationManager.NavigateTo($"jobobservation/history/{JobObservationId}");
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


        private async Task<AsyncVoidMethodBuilder> OpenDialogCodePath(SOSCodePath itemselected, MudTabPanel panelSelect)
        {
            searchCodeString = itemselected.Code;
            ShowLoading = true;
            base.StateHasChanged();
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

                    nodoEncontrado = TreeServices.FindNodeByPath(rootNodeCCP, CCPrute);

                    if (nodoEncontrado != null)
                    {
                        // El nodo fue encontrado, puedes trabajar con él aquí
                        // Por ejemplo, imprimir su nombre
                        Console.WriteLine("Nombre del nodo encontrado: " + nodoEncontrado.Nombre);
                    }
                    else
                    {
                        // El nodo no fue encontrado
                        Console.WriteLine("La ruta no se encontró en el árbol.");
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

        private async void CloseModalFiles()
        {
            CodePathModalDisplay = false;

            StateHasChanged();

        }

        bool ShowLoading = true;
        private IList<string> _sourceMsgLoading = new List<string>();
        private IList<Color> _Colors = new List<Color>() { Color.Default, Color.Primary, Color.Secondary, Color.Success, Color.Info, Color.Default, Color.Primary, Color.Secondary, Color.Success, Color.Info };

        private string searchCodeString = "";

        //Files Path
        private CDMS_CCP_Archives? CcpFilesInFolder;
        private CDMS_HOE_Archives? HoeFilesInFolder;
        private CDMS_GOS_Archives? GosFilesInFolder;
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
        SOSCodePath CodePathDialogDisplay { get; set; }
        /// <summary>
        /// //
        /// </summary>
        TreeItemData rootNodeCCP { get; set; } = new TreeItemData();
        CDMS_CCP_Directory CCPFolders { get; set; } = new CDMS_CCP_Directory();

        TreeItemData SelectedNodeCCP { get; set; }
        TreeItemData nodoEncontrado { get; set; }
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

        private async Task DownloadFile(int fileId, string filename)
        {
            await FilesServices.DownloadFileEvidence(fileId, filename);
        }
        //Show Photo
        private DialogOptions dialogPhotoOptions = new() { CloseOnEscapeKey = true, MaxWidth = MaxWidth.Medium, FullWidth = true, CloseButton = true };

        private bool visiblePhoto = false;


        private int photoIndex = 0;
        ChecklistAnswer SelectedAnswer { get; set; }
        private void OpenPhotoDialog(int index, ChecklistAnswer item)
        {
            SelectedAnswer = item;
            photoIndex = index;
            visiblePhoto = true;
        }
    }
}
