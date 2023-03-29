using DocumentFormat.OpenXml.Vml.Spreadsheet;
using Microsoft.JSInterop;
using MudBlazor;

namespace SupervisorMobility.Client.Pages.JobObservationPage
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
        string[] cicles = new string[5];

        private bool CcpDialog = false;
        private bool HoeDialog = false;
        private bool GosDialog = false;
        private bool searchAssychart = false;

        private CDMS_CCP_Document? CcpFilesInFolder;
        private CDMS_HOE_Document? HoeFilesInFolder;
        private CDMS_GOS_Document? GosFilesInFolder;

        private bool folderError = false;
        private string messageErrorFolders;

        public string placeholder = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, " +
          "sed do eiusmod tempor incididuntut labore et dolore magna aliqua. Ut enim ad minim " +
          "veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo coe velit esse cillum";

        //Operator user
        public List<User> users = new();
        public List<User> operatorUsers = new();

        protected async override Task OnInitializedAsync()
        {

            _jobObservation.Supervisor = new();
            _jobObservation.Operator = new();

            glosary = await GlosaryService.GetGlosary();
            _glosaryInfo = glosary.ToDictionary(x => x.Name, x => x);

            _lupJobObservations = await JobObservationService.GetJobObservationWithLup(JobObservationId);
            _jobObservation = await JobObservationService.GetJobObservationById(JobObservationId);
            _products = await ProductService.GetProducts();
            var prod = _jobObservation.Models.Split('|');
            models[0] = Int32.Parse(prod[0]);
            models[1] = Int32.Parse(prod[1]);
            models[2] = Int32.Parse(prod[2]);
            models[3] = Int32.Parse(prod[3]);
            models[4] = Int32.Parse(prod[4]);
            cicles = _jobObservation.Cicles.Split('|');

            startHour = _jobObservation.DateStart?.TimeOfDay;
            endHour = _jobObservation.DateEnd?.TimeOfDay;
            Console.WriteLine(startHour);

            if(_jobObservation.PlantId != 0)
            {
                if (_jobObservation.AreaId != 0)
                {
                    if (_jobObservation.DistributionId != 0)
                    {
                        if (_jobObservation.DistributionId != 0)
                        {

                            _assychart = await AssychartServices.GetAssyChartAdvance(_jobObservation.PlantId, _jobObservation.AreaId, _jobObservation.DistributionId, _jobObservation.OperationId);
                            if (_assychart == null)
                                messageErrorFolders = "The folders with the information provided were not located.";
                            else
                                searchAssychart = true;

                        }
                        else
                        {
                            messageErrorFolders = "Job Observation does not contain a valid operation";
                            Console.WriteLine("missing plant");
                        }
                    }
                    else
                    {
                        messageErrorFolders = "Job Observation does not contain a valid distribution";
                        Console.WriteLine("missing plant");
                    }
                }
                else
                {
                    messageErrorFolders = "Job Observation does not contain a valid area";
                    Console.WriteLine("missing plant");
                }
            }
            else
            {
                messageErrorFolders = "Job Observation does not contain a valid plant";
                Console.WriteLine("missing plant");
            }

        }

        void Closed(MudChip chip)
        {
            // react to chip closed
        }




        private string HOErute = "";
        private string CCPrute = "";
        private string GOSrute = "";
        private async void OpenDialogGOS(string ruta)
        {
            GOSrute = ruta;
            GosDialog = true;
            folderError = false;

            Console.WriteLine($"gos {ruta}");

            GosFilesInFolder = new CDMS_GOS_Document();

            GosFilesInFolder = await CDMSServices.GetFilesGOS(ruta);
            if (GosFilesInFolder == null)
                folderError = true;


            StateHasChanged();
        }
        void CloseGos() => GosDialog = false;

        private async void OpenDialogCcp(string ruta)
        {
            CCPrute = ruta;
            CcpDialog = true;
            folderError = false;
            Console.WriteLine($"Cpc {ruta}");

            CcpFilesInFolder = new CDMS_CCP_Document();
            CcpFilesInFolder = await CDMSServices.GetFilesCCP(ruta);
            if (CcpFilesInFolder == null)
                folderError = true;

            StateHasChanged();
        }
        void CloseCcp() => CcpDialog = false;

        private async void OpenDialogHoe(string ruta)
        {
            HOErute = ruta;
            HoeDialog = true;
            Console.WriteLine($"hoe {ruta}");

            folderError = false;
            HoeFilesInFolder = new CDMS_HOE_Document();
            HoeFilesInFolder = await CDMSServices.GetFilesHOE(ruta);
            if (HoeFilesInFolder == null)
                folderError = true;

            StateHasChanged();
        }
        void CloseHoe() => HoeDialog = false;


        private async Task DownloadFileFromURL(string urlroute, string namefile)
        {
            var fileName = namefile;
            var fileURL = urlroute;
            await JS.InvokeVoidAsync("triggerFileDownload", fileName, fileURL);
        }
    }
}
