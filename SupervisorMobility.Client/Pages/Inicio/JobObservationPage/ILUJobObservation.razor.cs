using Blazorise.Extensions;
using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.Drawing;
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
    public partial class ILUJobObservation
    {
        [Parameter]
        public User LoggedUser { get; set; }

        [Parameter]
        public bool IsVisible { get; set; }

        [Parameter]
        public EventCallback<bool> IsVisibleChanged { get; set; }

        [Parameter]
        public string ProgrammedStartDate { get; set; }

        [Parameter]
        public int plant_id { get; set; }
        [Parameter]
        public int area_id { get; set; }
        [Parameter]
        public int distribution_id {  get; set; }
        [Parameter]
        public int supervisor_id { get; set; } 
        [Parameter]
        public int operator_id { get; set; }

        [Parameter]
        public string ILULevelAux { get; set; }

        public DateTime? AuxProgrammedDate { get; set; }

        public JobObservation _jobObservation { get; set; } = new();

        private DialogOptions dialogOptions = new() { CloseOnEscapeKey = true, MaxWidth = MaxWidth.Large, FullWidth = true };

        private AssyChart _assychart { get; set; } = new AssyChart();


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

        Plant _plant { get; set; } = new();
        Area _area = new();
        Distribution _distribution = new();
        List<Operation> _operations = new();

        User _supervisor = new();
        User _operator = new();

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
      
        //Glosary
        private List<Glosary> glosary = new();
        private Dictionary<string, Glosary> _glosaryInfo;

        public List<JobCategoryStructure> _checklistCategoriesAndQuestions { get; set; } = new();
        string jobCategoryStructureIds = "";

        public bool CodePathModalDisplay { get; set; } = false;
        private string searchCodeString = "";
        bool ShowLoading = true;

        SOSCodePath CodePathDialogDisplay { get; set; }
        private int auxILU_Level = 0;
        private List<ILULevel> _LevelsILU { get; set; } = new();


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

            glosary = await GlosaryService.GetGlosary();
            _glosaryInfo = glosary.ToDictionary(x => x.Name, x => x);

            _checklistCategoriesAndQuestions = await JobStructureCategoriesService.GetChecklistCategories(true);
            _LevelsILU = await ILUServices.GetLevelsILU();

            //optenemos categorias
            foreach (var category in _checklistCategoriesAndQuestions)
            {
                jobCategoryStructureIds += category.JobCategoryStructureId + "|";
            }

            if (!string.IsNullOrEmpty(jobCategoryStructureIds))
            {
                jobCategoryStructureIds = jobCategoryStructureIds.TrimEnd('|');
            }


            AuxProgrammedDate = DateTime.Now;

            _jobObservation.Supervisor = new();
            _jobObservation.Operator = new();

            _plant = await PlantServices.GetPlantById(plant_id);
            _area = await AreaServices.GetAreaById(plant_id, area_id);
            _distribution = await DistributionService.GetDistributionById(plant_id, area_id, distribution_id);
            _operations = _distribution.Operations;

            _supervisor = await UsersService.GetUserAndCollection(supervisor_id);
            _operator = _supervisor.Subordinates?.ToList().Find(u => u.UserId == operator_id);

            _jobObservation.Plant = _plant;
            _jobObservation.PlantId = _plant.PlantId;
            _jobObservation.Area = _area;
            _jobObservation.AreaId = _area.AreaId;
            _jobObservation.Distribution = _distribution;
            _jobObservation.DistributionId = _distribution.DistributionId;
            _jobObservation.Supervisor = _supervisor;
            _jobObservation.SupervisorId  = _supervisor.UserId;
            _jobObservation.Operator = _operator;
            _jobObservation.OperatorId = _operator.UserId;

            _jobObservation.Type = 4;
            _jobObservation.Option = 1;
            _jobObservation.Status = 7;
            _jobObservation.SectionIds = jobCategoryStructureIds;
            _jobObservation.IsActive = true;

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


            _jobObservation.StepsNumber = string.Join("|", _jobObservation.StepsNumber);
            _jobObservation.DoubleManagment = string.Join("|", _jobObservation.DoubleManagment);
            _jobObservation.Waiting = string.Join("|", _jobObservation.Waiting);
            _jobObservation.HOEStandardTimes = "0";
            _jobObservation.ProductIds = "0|0|0|0|0";
            _jobObservation.ProductSpecifications = "||||";


            Console.WriteLine(ILULevelAux);

            auxILU_Level = ILULevelAux switch
            {
                " " => 1,
                "I" => 4,
                "L" => 8,
                _ => 0 
            };
            _newIlu.ILULevelId = auxILU_Level;

            ShowLoading = false;
        }

      
        private void OnStartDateChanged(DateTime dt)
        {
            _jobObservation.StartDate = dt;
            _jobObservation.EndDate = dt;
        }
        private ILURegister _newIlu { get; set; } = new();

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


                var result = await JobObservationService.CreateJobObservationWithLup(_jobObservation);

                //var result = await JobObservationService.CreateJobObservation(_jobObservation);
                if (result != null)
                {
                    Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                    Snackbar.Add($"Job Observation Created", Severity.Info);

                    _jobObservation = result;
                    //_ = await GenerateChecklistAnswers();
                    //_ = await GenerateOperatorSignatureImage();
                    _newIlu.AcquisitionDate = newDate1;
                    _newIlu.DistributionId = _jobObservation.DistributionId;
                    _newIlu.OperatorId = _jobObservation.OperatorId;
                    _newIlu.isActive = true;
                    Console.WriteLine("Job Creada, se añade ");

                    var resultIlu = await ILUServices.AddRegisterForUser(_newIlu, _jobObservation.OperatorId);
                    if (resultIlu != null)
                    {
                        Console.WriteLine("IluCreado");
                        Snackbar.Add($"ILU Level Added", Severity.Success);
                    }

                    //ClearJOStorage();

                    NavigationManager.NavigateTo("/jobobservation");
                }
                else
                    await JSRuntime.InvokeVoidAsync("alert", "Error en los datos!"); // Alert


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


                var result = await JobObservationService.CreateJobObservationWithLup(_jobObservation);

                //var result = await JobObservationService.CreateJobObservation(_jobObservation);
                if (result != null)
                {
                    Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                    Snackbar.Add($"Job Observation Created", Severity.Info);

                    _jobObservation = result;
                    //_ = await GenerateChecklistAnswers();
                    //_ = await GenerateOperatorSignatureImage();

                    _newIlu.AcquisitionDate = newDate1;
                    _newIlu.DistributionId = _jobObservation.DistributionId;
                    _newIlu.OperatorId = _jobObservation.OperatorId;
                    _newIlu.isActive = true;
                        Console.WriteLine("Job Creada, se añade ");

                    var resultIlu = await ILUServices.AddRegisterForUser(_newIlu, _jobObservation.OperatorId);
                    if (resultIlu != null)
                    {
                        Console.WriteLine("IluCreado");
                        Snackbar.Add($"ILU Level Added", Severity.Success);
                    }

                    NavigationManager.NavigateTo("/jobobservation");
                }
                else
                    await JSRuntime.InvokeVoidAsync("alert", "Error en los datos!"); // Alert
            }
        }



        private async void CloseModalFiles()
        {
            CodePathModalDisplay = false;

            StateHasChanged();

        }

        int SOSCodePathId { get; set; } = 0;
        string SosPanelOpen { get; set; } = "";
        private async Task<AsyncVoidMethodBuilder> OpenDialogCodePath(SOSCodePath itemselected, int panelSelect)
        {

            ShowLoading = true;
            SOSCodePathId = itemselected.SOSCodePathId;
            switch (panelSelect)
            {
                case 1:
                    SosPanelOpen = "HOE";
                    break;

                case 2:
                    SosPanelOpen = "CCP";
                    break;

                case 3:
                    SosPanelOpen = "GOS";
                    break;

                case 4:
                    SosPanelOpen = "HOE_CD";
                    break;

                case 5:
                    SosPanelOpen = "CCP_CD";
                    break;

                case 6:
                    SosPanelOpen = "GOS_CD";
                    break;


            }

            CodePathModalDisplay = true;
            StateHasChanged();
            return new AsyncVoidMethodBuilder();
        }


        private void updateILULevel()
        {
            _newIlu.ILULevelId = auxILU_Level;
            Console.WriteLine(auxILU_Level);
        }

    }
}
