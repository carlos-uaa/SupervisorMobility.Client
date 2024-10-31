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

        [Parameter]
        public bool IsSchedule { get; set; } = false;

        [Inject] private ISOSDataService SOSDataServices { get; set; }
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

        private bool searchAssychart = false;


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

            if (IsSchedule)
            {
                ProgrammedStartDate = ProgrammedStartDate.Replace("-", "/");
                AuxProgrammedDate = DateTime.ParseExact(ProgrammedStartDate, "d/M/yyyy", null);

            }
            else
            {

                if (DateTime.TryParse(ProgrammedStartDate, CultureInfo.InvariantCulture, DateTimeStyles.None, out var AuxDate))
                {
                    Console.WriteLine($"Primera Conversion Exitosa" + AuxDate);
                    AuxProgrammedDate = AuxDate;
                }
            }


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
                                _assychart = await AssychartServices.GetAssyChartAdvance(_jobObservation.PlantId, _jobObservation.AreaId, _jobObservation.DistributionId,(int) _jobObservation.Operations?.FirstOrDefault().OperationId);
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
                listFilter = _assychart.RoutesProductsAssyChart.Where(r => r.Code.ToLower().Contains(_jobObservation.Operations?.FirstOrDefault().Code.ToLower(), StringComparison.OrdinalIgnoreCase)).ToList();
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
            if (_jobObservation.Operations.Count() == 0)
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

                    //Aqui actualizamos en el servicio de sosdata
                    SOSDataServices.UpdateJobItem(_jobObservation);

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

                    //Aqui actualizamos en el servicio de sosdata
                    SOSDataServices.UpdateJobItem(_jobObservation);




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
        int SOSCodePathId { get; set; } = 0;

        private async Task<AsyncVoidMethodBuilder> OpenDialogCodePath(SOSCodePath itemselected, MudTabPanel panelSelect)
        {
            searchCodeString = itemselected.Code;
            ShowLoading = true;
            CodePathModalDisplay = true;
            StateHasChanged();

            SOSCodePathId = itemselected.SOSCodePathId;


            CodePathModalDisplay = true;
            StateHasChanged();

            return new AsyncVoidMethodBuilder();
        }
        

    }
}
