using Microsoft.JSInterop;
using MudBlazor;
using MudBlazor.Utilities;
using System.Globalization;
using System.Runtime.CompilerServices;
using SupervisorMobility.Client.Pages.Inicio.SOSProgramPage.Dialogs;
using SupervisorMobility.Client.Pages.Inicio.JobObservationPage.Modals;
using FuzzyString;

namespace SupervisorMobility.Client.Pages.Inicio.SOSProgramPage
{
    public partial class SOS_Details

    {
        [Parameter]
        public int sosId { get; set; }

        [Inject] private IDialogService DialogService { get; set; }
        [Inject] private IBreadcrumbService BreadcrumbService { get; set; }


        private List<BreadcrumbItem> _links;

        private List<Distribution> _distributions = new();
        private List<Distribution> _distributionsSuggest = new();

        public class DistSelect
        {
            public Distribution distribution { get; set; }
            public bool isSelected { get; set; } = false;
        }
        public List<DistSelect> Dist_Manager { get; set; } = new List<DistSelect>();

        private Distribution selected_distribution = new();
        private int selected_distId = 0;
        private List<AssyChart> _assyCharts = new();
        private SOSReviewProgram _sos_plan = new();
        private JobObservation _NewJobObservation = new();
        private SOSRegisterJobObservation _NewRegister = new();


        int SOSCodePathId { get; set; } = 0;
        string SosPanelOpen { get; set; } = "";

        private List<User> SV_Manager = new();


        private List<Operation> _Operations = new();

        //register jobs processed
        private Dictionary<(int, int), List<SOSRegisterJobObservation>?> SOS_Registers_Matrix { get; set; } = new Dictionary<(int, int), List<SOSRegisterJobObservation>?>();
        //relation User SV to Operation processed
        private Dictionary<int, SOSRegUserOperationRelationship?> SOS_Registers_UserOperationRelationship { get; set; } = new Dictionary<int, SOSRegUserOperationRelationship?>();

        //Resume Operations In Distribution
        private Dictionary<int, SosJobCount> OperationsInDistributionCount = new Dictionary<int, SosJobCount>();

        //suggest
        private Dictionary<int, SOSRegUserOperationRelationship?> Suggested_SOS_Registers_UserOperationRelationship { get; set; } = new Dictionary<int, SOSRegUserOperationRelationship?>();
        private Dictionary<(int, int), List<JobObservationNulls>?> Suggested_Registers_Matrix { get; set; } = new Dictionary<(int, int), List<JobObservationNulls>?>();


        //Register Jobs without processed
        private List<SOSRegisterJobObservation> _SosRegisters = new();
        //Relation User SV to Operation without processed
        public List<SOSRegUserOperation> _SosRegistersrUserOperation { get; set; } = new();

        //Data General
        public List<JobObservationNulls> _SOSJobobservation { get; set; } = new();
        public List<JobObservationNulls> _AnotherJobs { get; set; } = new();
        public List<JobObservationNulls> _All_SOSJobobservation { get; set; } = new();
        public List<JobObservationNulls> _All_Suggested_SOSJobobservation { get; set; } = new();

        public bool ProgrammSuggestion { get; set; } = false;
        public JobObservationNulls _Selected_Suggested_SOSJobobservatio_Null { get; set; } = new();
        public JobObservation _Selected_Suggested_SOSJobobservation { get; set; } = new();

        public class SOSRegUserOperationRelationship
        {
            public bool StateUpdate { get; set; }
            public bool Exist { get; set; }
            public SOSRegUserOperation Register { get; set; }
        }

        public class SosJobCount
        {
            public int TotalJobs { get; set; }

            public int TotalSosJobs { get; set; }

            public int TotalOutSosJobs { get; set; }

            public int sosplanned { get; set; }
            public int sosinProgress { get; set; }
            public int soslate { get; set; }
            public int sosunderReview { get; set; }
            public int sosrejected { get; set; }
            public int sosfinished { get; set; }
            public int sosprogramed { get; set; }

            public int planned { get; set; }
            public int inProgress { get; set; }
            public int late { get; set; }
            public int underReview { get; set; }
            public int rejected { get; set; }
            public int finished { get; set; }
            public int programed { get; set; }

            //Desplegable
            public bool isActive { get; set; }
        }



        private string[] labels { get; set; }


        //User
        private string json = string.Empty;
        public User user = new();
        public User? selectedUser = new();
        public bool logged = false;
        public bool ShowTable = false;


        bool AddOperation = false;
        bool disableBtnCreateSos = false;
        bool AddOperator = false;
        bool MonthlyView = false;
        bool ScheduleView = false;
        int diasSeparate = 1;
        int OptionRandom = 0;
        DateTime Startday = DateTime.Now;
        DateTime FirstdayYear = DateTime.Now;
        DateTime LastdayYear = DateTime.Now;
        int JobsPorDia = 1;

        private DialogOptions dialogOptions = new() { CloseOnEscapeKey = false, MaxWidth = MaxWidth.Large, FullWidth = true, DisableBackdropClick = true, CloseButton = false };
        private DialogOptions dialogSVOptions = new() { CloseOnEscapeKey = false, DisableBackdropClick = true, CloseButton = false };

        TextInfo textInfo = CultureInfo.CurrentCulture.TextInfo;
        SosJobCount opInDistDialog = new();
        bool ShowGraphicDonnut = false;

        //Calendario
        public DateTime? date;
        DateTime? _yearMonth;
        int daysInMonth;
        private string month;
        private string year;


        public int optionStatus { get; set; } = 0;


        public List<JobCategoryStructure> _checklistCategoriesAndQuestions { get; set; } = new();
        string jobCategoryStructureIds = "";

        public int idFilter;


        public bool CodePathModalDisplay { get; set; } = false;
        bool ShowLoading = true;
        private IList<string> _sourceMsgLoading = new List<string>();
        private IList<Color> _Colors = new List<Color>() { Color.Default, Color.Primary, Color.Secondary, Color.Success, Color.Info, Color.Default, Color.Primary, Color.Secondary, Color.Success, Color.Info };



        private List<User> _Users = new();
        private List<User> _UsersSV_Copy = new();

        private bool isButtonDisabled = false;
        private bool SVSinCharge = false;

        //sv panel
        private bool visibleSVDialog = false;
        private bool visibleSuggestSVDialog = false;
        private int selectedSVPanel = 0;
        private int selectedSOPPanel = 0;

        //schedule
        List<string> monthNames = new List<string>();
        List<string> days = new List<string>();
        List<Week> weeks = new List<Week>();
        DateTime startDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
        DateTime endDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddMonths(1).AddDays(-1);


        public bool loadingToolbar = true;
        public bool loadingData = true;
        public bool loadingSchedule = true;

        //Glosary
        private List<Glosary> glosary = new();
        private Dictionary<string, Glosary> _glosaryInfo;

        protected async override Task OnInitializedAsync()
        {
            ShowLoading = false;

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

            List<string> labelaux = new List<string>();

            labelaux.Add($"{Localizer["planned"]}");
            labelaux.Add($"{Localizer["inProgress"]}");
            labelaux.Add($"{Localizer["late"]}");
            labelaux.Add($"{Localizer["underReview"]}");
            labelaux.Add($"{Localizer["rejected"]}");
            labelaux.Add($"{Localizer["finished"]}");
            labelaux.Add($"{Localizer["programmed"]}");

            labels = labelaux.ToArray();

            glosary = await GlosaryService.GetGlosary();
            _glosaryInfo = glosary.ToDictionary(x => x.Name, x => x);

            logged = await HasPropertyAsync();
            if (!logged)
            {
                Snackbar.Clear();
                Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                Snackbar.Add(Localizer["errorYouHaveToLogIn"], Severity.Error);
                NavigationManager.NavigateTo($"/");
            }
            else
            {
                await GetUserAsync();
                _sos_plan = await SOSPlanReviewServices.GetSOSById(sosId, true, true, true);
                _distributions = await DistributionServices.GetDistributionsWithCollections((int)_sos_plan.PlantId, (int)_sos_plan.AreaId);

                //Dist_Manager Son las distribuciones disponibles para crear una sugerencia
                if (_sos_plan != null && _sos_plan.Suggestions != null)
                {
                    _distributionsSuggest = _distributions.Where(d => _sos_plan.Suggestions.Any(s => s.DistributionId == d.DistributionId && s.SuggestionApplied == false)).ToList();
                    foreach (var dist in _distributionsSuggest)
                    {
                        DistSelect item = new DistSelect();
                        item.distribution = new Distribution();
                        item.distribution = dist;
                        Dist_Manager.Add(item);
                    }
                }



                _Users = await UsersServices.GetUsersByUserTypeInPlantAndArea(_sos_plan.PlantId, _sos_plan.AreaId, 3, true, false);
                _UsersSV_Copy = ObjectCloner.ObjectCloner.DeepClone(_Users);
                _assyCharts = await AssyChartsServices.GetAssyChartsByArea((int)_sos_plan.PlantId, (int)_sos_plan.AreaId);

                StateHasChanged();
            }

            if (_sos_plan.AplicationYear > DateTime.Now.Year)
            {
                Startday = new DateTime((int)_sos_plan.AplicationYear, 1, 1);
            }
            else
            {
                Startday = DateTime.Now;
            }

            if ((int)_sos_plan.AplicationYear > DateTime.Now.Year)
            {
                LastdayYear = new DateTime((int)_sos_plan.AplicationYear, 12, 31);
                _yearMonth = new DateTime((int)_sos_plan.AplicationYear, 1, 1);
                FirstdayYear = new DateTime((int)_sos_plan.AplicationYear, 1, 1);
                date = new DateTime((int)_sos_plan.AplicationYear, 1, 1);
            }
            else
            {

                LastdayYear = new DateTime((int)_sos_plan.AplicationYear, 12, 31);
                _yearMonth = new DateTime((int)_sos_plan.AplicationYear, DateTime.Now.Month, DateTime.Now.Day);
                FirstdayYear = new DateTime((int)_sos_plan.AplicationYear, DateTime.Now.Month, DateTime.Now.Day);
                date = new DateTime((int)_sos_plan.AplicationYear, DateTime.Now.Month, DateTime.Now.Day).AddMonths(-1);
            }



            daysInMonth = DateTime.DaysInMonth(_yearMonth.Value.Year, _yearMonth.Value.Month);

            month = $"{_yearMonth?.ToString("MMMM")}";


            _checklistCategoriesAndQuestions = await JobStructureCategoriesService.GetChecklistCategories(true);

            //optenemos categorias
            foreach (var category in _checklistCategoriesAndQuestions)
            {
                jobCategoryStructureIds += category.JobCategoryStructureId + "|";
            }

            if (!string.IsNullOrEmpty(jobCategoryStructureIds))
            {
                jobCategoryStructureIds = jobCategoryStructureIds.TrimEnd('|');
            }

            _links = new List<BreadcrumbItem>
            {
                new BreadcrumbItem(text: Localizer["home"], href: "/"),
                new BreadcrumbItem(text: Localizer["sosProgram"], href: "/sosProgram"),
                new BreadcrumbItem(text: Localizer["sosDetails"],  href: $"sosDetails/{_sos_plan.SOSid}"),
                new BreadcrumbItem(text: $"SOS Anual Plan {_sos_plan.AplicationYear}", href: $"sosDetails/{_sos_plan.SOSid}"),
            };
            BreadcrumbService.UpdateBreadcrumbs(_links);

            monthNames = System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.MonthGenitiveNames.ToList();

            await SOSDataServices.SetSosJobObservation(sosId, _distributions, jobCategoryStructureIds);
            _AnotherJobs = SOSDataServices._AnotherJobs;
            OperationsInDistributionCount = SOSDataServices.OperationsInDistributionCount;

            SchedulinMode();
        }

        public async void SchedulinMode()
        {
            //Es para mostrar el control de cambio de mes
            MonthlyView = true;
            //saber que me encuentro en el calendario
            ScheduleView = true;
            await LoadSchedule();
        }
        private async Task LoadSchedule()
        {
            loadingData = true;
            StateHasChanged();
            //aqui manda a llamar al servicio y actualizamos los datos necesarios unicamente

            GenerateCalendarHead();
            GenerateCalendarBody();
            if (SuggestionMode)
            {
                await PrepareSuggestDataTable();
            }
            else
            {
                await PrepareDataTable();
            }

            loadingData = false;
            StateHasChanged();
        }
        private async void YearlyTab()
        {
            Distribution? tmpdist = _distributions?.Find(d => d.ShowDetails == true);

            DistSelect? tmpSuggdist = null;
            if (tmpdist == null)
            {
                tmpSuggdist = Dist_Manager?.Find(d => d.distribution.ShowDetails == true);
                Dist_Manager.ForEach(d => d.distribution.ShowDetails = false);
            }
            else
            {
                _distributions.ForEach(d => d.ShowDetails = false);
            }
            StateHasChanged();

            ScheduleView = MonthlyView = false;

            if (tmpdist != null)
            {
                await PrepareDataTable(tmpdist.DistributionId);
                tmpdist.ShowDetails = true;
            }
            else if (tmpSuggdist != null)
            {
                await PrepareSuggestDataTable(tmpSuggdist.distribution.DistributionId);
                tmpSuggdist.distribution.ShowDetails = true;
            }
            else if (!SuggestionMode)
            {
                //aqui manda a llamar al servicio y actualizamos los datos necesarios unicamente
                await PrepareDataTable();
            }
            else
            {
                await PrepareSuggestDataTable();
            }

            StateHasChanged();
        }

        private async void MontlyTab()
        {
            Distribution? tmpdist = _distributions?.Find(d => d.ShowDetails == true);

            DistSelect? tmpSuggdist = null;
            if (tmpdist == null)
            {
                tmpSuggdist = Dist_Manager?.Find(d => d.distribution.ShowDetails == true);
                Dist_Manager.ForEach(d => d.distribution.ShowDetails = false);
            }
            else
            {
                _distributions.ForEach(d => d.ShowDetails = false);
            }
            StateHasChanged();

            MonthlyView = true;
            SuggestionMode = false;

            if (tmpdist != null)
            {
                await PrepareDataTable(tmpdist.DistributionId);
                tmpdist.ShowDetails = true;
            }
            else if (tmpSuggdist != null)
            {
                await PrepareSuggestDataTable(tmpSuggdist.distribution.DistributionId);
                tmpSuggdist.distribution.ShowDetails = true;
            }
            else if (!SuggestionMode)
            {
                //aqui manda a llamar al servicio y actualizamos los datos necesarios unicamente
                await PrepareDataTable();
            }
            else
            {
                await PrepareSuggestDataTable();
            }

            StateHasChanged();
        }


        private async void SuggestYearlyTab()
        {
            DistSelect? tmpSuggdist = Dist_Manager?.Find(d => d.distribution.ShowDetails == true);

            if (tmpSuggdist != null)
            {
                Dist_Manager.ForEach(d => d.distribution.ShowDetails = false);
            }

            StateHasChanged();

            ScheduleView = MonthlyView = false;

            SuggestionMode = true;

            if (tmpSuggdist != null)
            {
                await PrepareSuggestDataTable(tmpSuggdist.distribution.DistributionId);
                tmpSuggdist.distribution.ShowDetails = true;
            }


            StateHasChanged();
        }

        private async void SuggestMontlyTab()
        {
            DistSelect? tmpSuggdist = Dist_Manager?.Find(d => d.distribution.ShowDetails == true);

            if (tmpSuggdist != null)
            {
                Dist_Manager.ForEach(d => d.distribution.ShowDetails = false);
            }

            StateHasChanged();

            MonthlyView = SuggestionMode = true;

            if (tmpSuggdist != null)
            {
                await PrepareSuggestDataTable(tmpSuggdist.distribution.DistributionId);
                tmpSuggdist.distribution.ShowDetails = true;
            }


            StateHasChanged();
        }

        private void OpenDialog4(string date)
        {
            searchString = "";
            date = date.Replace("/", "-");
            programmedStartDate = date;
            visible3 = true;
        }


        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (!ShowLoading)
            {
                await JS.InvokeVoidAsync("blazorUtilsSOS.setDynamicLeftSOS");
                await JS.InvokeVoidAsync("blazorUtilsTOPSOS.setDynamicTOP");
            }

            //await JS.InvokeVoidAsync("blazorUtils.truncateText");

        }


        private void GenerateCalendarHead()
        {
            if (startDate.DayOfWeek == DayOfWeek.Monday)
            {
                startDate = startDate.AddDays(-1);
            }
            switch (startDate.DayOfWeek)
            {
                case DayOfWeek.Monday: startDate = startDate.AddDays(-1); break;
                case DayOfWeek.Tuesday: startDate = startDate.AddDays(-2); break;
                case DayOfWeek.Wednesday: startDate = startDate.AddDays(-3); break;
                case DayOfWeek.Thursday: startDate = startDate.AddDays(-4); break;
                case DayOfWeek.Friday: startDate = startDate.AddDays(-5); break;
                case DayOfWeek.Saturday: startDate = startDate.AddDays(-6); break;


            }

            var day1 = new List<string>();
            for (var dt = startDate; dt <= endDate; dt = dt.AddDays(1))
            {
                day1.Add(dt.ToString("dddd"));
            }
            days = day1.Distinct().ToList();
        }
        private void GenerateCalendarBody()
        {
            weeks = new List<Week>();
            int flag = 0;
            Week week = new Week();
            List<DayEvent> dates = new List<DayEvent>();

            var totalDays = (int)(endDate - startDate).TotalDays;
            int countdays = 0;

            for (var dt = startDate; dt <= endDate; dt = dt.AddDays(1))
            {
                flag = flag + 1;
                dates.Add(new DayEvent()
                {
                    DateValue = dt.ToString("d/M/yyyy"),
                    DayName = dt.ToString("dd")
                });

                if (flag == 7)
                {
                    week = new Week();
                    week.Dates = dates;
                    weeks.Add(week);
                    dates = new List<DayEvent>();
                    flag = 0;
                }
                if (countdays == totalDays)
                {
                    week = new Week();
                    week.Dates = dates;
                    weeks.Add(week);
                    break;
                }
                countdays++;
            }
            loadingSchedule = false;

        }





        private void OnFirstDateChanged(DateTime? dt)
        {
            if (dt.HasValue)
            {
                Startday = new DateTime(dt.Value.Year, dt.Value.Month, dt.Value.Day);
            }

        }

        private async void OnDateChanged(DateTime? value)
        {
            Distribution? tmpdist = _distributions?.Find(d => d.ShowDetails == true);

            DistSelect? tmpSuggdist = null;
            if (tmpdist == null)
            {
                tmpSuggdist = Dist_Manager?.Find(d => d.distribution.ShowDetails == true);
                if (tmpSuggdist != null)
                {
                    tmpSuggdist.distribution.ShowDetails = false;
                }
            }
            else
            {
                tmpdist.ShowDetails = false;
            }
            ShowLoading = true;
            loadingData = true;
            StateHasChanged();

            _yearMonth = value;
            daysInMonth = DateTime.DaysInMonth(_yearMonth.Value.Year, _yearMonth.Value.Month);

            month = $"{_yearMonth?.ToString("MMMM")}";
            year = $"{_yearMonth?.ToString("yyyy")}";
            int monthIndex = DateTime.ParseExact(month, "MMMM", System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat).Month;
            int yearIndex = DateTime.ParseExact(year, "yyyy", System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat).Year;

            startDate = new DateTime(yearIndex, monthIndex, 1);
            endDate = new DateTime(yearIndex, monthIndex, 1).AddMonths(1).AddDays(-1);

            if (tmpdist != null)
            {
                await PrepareDataTable(tmpdist.DistributionId);
                tmpdist.ShowDetails = true;
            }
            else if (tmpSuggdist != null)
            {
                await PrepareSuggestDataTable(tmpSuggdist.distribution.DistributionId);
                tmpSuggdist.distribution.ShowDetails = true;
            }
            else if (!SuggestionMode)
            {
                //aqui manda a llamar al servicio y actualizamos los datos necesarios unicamente
                await PrepareDataTable();
            }
            else
            {
                await PrepareSuggestDataTable();
            }

            if (ScheduleView)
            {
                GenerateCalendarHead();
                GenerateCalendarBody();
                if (SuggestionMode)
                {
                    await PrepareSuggestDataTable();
                }
                else
                {
                    await PrepareDataTable();
                }
            }



            ShowLoading = false;
            loadingData = false;
            StateHasChanged();
        }

        public async Task LastMonth()
        {
            Distribution? tmpdist = _distributions?.Find(d => d.ShowDetails == true);

            DistSelect? tmpSuggdist = null;
            if (tmpdist == null)
            {
                tmpSuggdist = Dist_Manager?.Find(d => d.distribution.ShowDetails == true);
                if (tmpSuggdist != null)
                {
                    tmpSuggdist.distribution.ShowDetails = false;
                }
            }
            else
            {
                tmpdist.ShowDetails = false;
            }
            ShowLoading = true;
            loadingData = true;
            StateHasChanged();

            _yearMonth = _yearMonth?.AddMonths(-1);
            daysInMonth = DateTime.DaysInMonth(_yearMonth.Value.Year, _yearMonth.Value.Month);

            month = $"{_yearMonth?.ToString("MMMM")}";
            year = $"{_yearMonth?.ToString("yyyy")}";
            int monthIndex = DateTime.ParseExact(month, "MMMM", System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat).Month;
            int yearIndex = DateTime.ParseExact(year, "yyyy", System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat).Year;



            startDate = new DateTime(yearIndex, monthIndex, 1);
            endDate = new DateTime(yearIndex, monthIndex, 1).AddMonths(1).AddDays(-1);


            if (tmpdist != null)
            {
                await PrepareDataTable(tmpdist.DistributionId);
                tmpdist.ShowDetails = true;
            }
            else if (tmpSuggdist != null)
            {
                await PrepareSuggestDataTable(tmpSuggdist.distribution.DistributionId);
                tmpSuggdist.distribution.ShowDetails = true;
            }
            else if (!SuggestionMode)
            {
                //aqui manda a llamar al servicio y actualizamos los datos necesarios unicamente
                await PrepareDataTable();
            }
            else
            {
                await PrepareSuggestDataTable();
            }

            if (ScheduleView)
            {
                GenerateCalendarHead();
                GenerateCalendarBody();
            }
            ShowLoading = false;
            loadingData = false;
            StateHasChanged();
        }

        public async Task NextMonth()
        {
            Distribution? tmpdist = _distributions?.Find(d => d.ShowDetails == true);

            DistSelect? tmpSuggdist = null;
            if (tmpdist == null)
            {
                tmpSuggdist = Dist_Manager?.Find(d => d.distribution.ShowDetails == true);
                if (tmpSuggdist != null)
                {
                    tmpSuggdist.distribution.ShowDetails = false;
                }
            }
            else
            {
                tmpdist.ShowDetails = false;
            }

            loadingData = true;
            ShowLoading = true;

            StateHasChanged();
            _yearMonth = _yearMonth?.AddMonths(1);
            daysInMonth = DateTime.DaysInMonth(_yearMonth.Value.Year, _yearMonth.Value.Month);

            month = $"{_yearMonth?.ToString("MMMM")}";
            year = $"{_yearMonth?.ToString("yyyy")}";
            int monthIndex = DateTime.ParseExact(month, "MMMM", System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat).Month;
            int yearIndex = DateTime.ParseExact(year, "yyyy", System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat).Year;


            startDate = new DateTime(yearIndex, monthIndex, 1);
            endDate = new DateTime(yearIndex, monthIndex, 1).AddMonths(1).AddDays(-1);

            if (tmpdist != null)
            {
                await PrepareDataTable(tmpdist.DistributionId);
                tmpdist.ShowDetails = true;
            }
            else if (tmpSuggdist != null)
            {
                await PrepareSuggestDataTable(tmpSuggdist.distribution.DistributionId);
                tmpSuggdist.distribution.ShowDetails = true;
            }
            else if (!SuggestionMode)
            {
                //aqui manda a llamar al servicio y actualizamos los datos necesarios unicamente
                await PrepareDataTable();
            }
            else
            {
                await PrepareSuggestDataTable();
            }

            if (ScheduleView)
            {
                GenerateCalendarHead();
                GenerateCalendarBody();
            }

            ShowLoading = false;
            loadingData = false;
            StateHasChanged();
        }

        //Local storage user
        private async Task GetUserAsync()
        {
            if (!await TryGetAsync())
            {
                user = new();
            }
        }



        private async Task PrepareDataTable(int id_distribution = 0)
        {

            ShowTable = false;

            //await Task.Run(() =>
            //{

            //El sos register quiza pueda ser odmitido cuando se navega entre los meses, ya que no hay necesidad de consultarlo

            if (ScheduleView && id_distribution == 0)
            {
                //traer toda las cosas por mes
                _All_SOSJobobservation?.Clear();
                _All_SOSJobobservation = SOSDataServices.Get_AllSos_Month(_yearMonth.Value.Month);
            }
            else if (MonthlyView && id_distribution != 0)
            {
                //Vista Mensual
                //_All_SOSJobobservation = SOSDataServices.Get_AllSos_Month_Dist(_yearMonth.Value.Month, id_distribution);
                if (_All_SOSJobobservation.Any(j => j.DistributionId != id_distribution) || _All_SOSJobobservation?.Count == 0)
                {
                    _All_SOSJobobservation?.Clear();
                    SOS_Registers_UserOperationRelationship?.Clear();
                    _All_SOSJobobservation = SOSDataServices.Get_AllSos_Dist(id_distribution);
                    SOS_Registers_UserOperationRelationship = SOSDataServices.Get_SOS_Registers_UserOperationRelationship(id_distribution);
                }
                SOS_Registers_Matrix?.Clear();
                SOS_Registers_Matrix = SOSDataServices.Get_Registers_Matrix_Month(_yearMonth.Value.Month);
            }
            else if (!ScheduleView && !MonthlyView && id_distribution != 0)
            {
                //Vista Anual
                if (!MonthlyView || _All_SOSJobobservation.Any(j => j.DistributionId != id_distribution) || _All_SOSJobobservation?.Count == 0 || SOS_Registers_Matrix?.Count == 0)
                {
                    _All_SOSJobobservation?.Clear();
                    SOS_Registers_Matrix?.Clear();
                    SOS_Registers_UserOperationRelationship?.Clear();
                    _All_SOSJobobservation = SOSDataServices.Get_AllSos_Dist(id_distribution);
                    SOS_Registers_Matrix = SOSDataServices.Get_Registers_Matrix_Dist(id_distribution);
                    SOS_Registers_UserOperationRelationship = SOSDataServices.Get_SOS_Registers_UserOperationRelationship(id_distribution);
                }
            }


            ShowTable = true;
            StateHasChanged();
        }
        private async Task PrepareSuggestDataTable(int id_distribution = 0)
        {

            if (ScheduleView && id_distribution == 0)
            {
                //traer toda las cosas por mes
                _All_Suggested_SOSJobobservation?.Clear();
                _All_Suggested_SOSJobobservation = SOSDataServices.Get_Suggest_AllSos_Month(_yearMonth.Value.Month);
            }
            else if (MonthlyView && id_distribution != 0)
            {
                //Vista Mensual

                if (_All_Suggested_SOSJobobservation.Any(j => j.DistributionId != id_distribution) || _All_Suggested_SOSJobobservation?.Count == 0)
                {
                    _All_Suggested_SOSJobobservation?.Clear();
                    Suggested_SOS_Registers_UserOperationRelationship?.Clear();
                    _All_Suggested_SOSJobobservation = SOSDataServices.Get_Suggest_AllSos_Dist(id_distribution);
                    Suggested_SOS_Registers_UserOperationRelationship = SOSDataServices.Get_Suggested_SOS_Registers_UserOperationRelationship(id_distribution);
                }
                Suggested_Registers_Matrix?.Clear();
                Suggested_Registers_Matrix = SOSDataServices.Get_Suggest_Registers_Matrix_Month(_yearMonth.Value.Month);
            }
            else if (!ScheduleView && !MonthlyView && id_distribution != 0)
            {
                //Vista Anual
                if (!MonthlyView || _All_Suggested_SOSJobobservation.Any(j => j.DistributionId != id_distribution) || _All_Suggested_SOSJobobservation?.Count == 0 || Suggested_Registers_Matrix?.Count == 0)
                {
                    _All_Suggested_SOSJobobservation?.Clear();
                    Suggested_SOS_Registers_UserOperationRelationship?.Clear();
                    _All_Suggested_SOSJobobservation = SOSDataServices.Get_Suggest_AllSos_Dist(id_distribution);
                    Suggested_SOS_Registers_UserOperationRelationship = SOSDataServices.Get_Suggested_SOS_Registers_UserOperationRelationship(id_distribution);
                }
                Suggested_Registers_Matrix?.Clear();
                Suggested_Registers_Matrix = SOSDataServices.Get_Suggest_Registers_Matrix_Dist(id_distribution);
            }


            ShowTable = true;
            StateHasChanged();
        }
        private async Task<bool> TryGetAsync()
        {
            bool hasProperty = await HasPropertyAsync();
            if (hasProperty)
            {
                json = await JSRuntime.InvokeAsync<string>("localStorage.getItem", "user");
                user = JsonSerializer.Deserialize<User>(json) ?? new();

            }
            return hasProperty;
        }

        private async Task<bool> HasPropertyAsync()
            => await JSRuntime.InvokeAsync<bool>("localStorage.hasOwnProperty", "user");




        private async void CreateJobObservation(int month, int DistributionId, int OperationId)
        {
            disableBtnCreateSos = true;
            StateHasChanged();


            bool existeClave = SOS_Registers_UserOperationRelationship.ContainsKey(OperationId);

            if (SOS_Registers_UserOperationRelationship.TryGetValue(OperationId, out var context))
            {
                if (context.Exist)
                {

                    _NewJobObservation.PlantId = (int)_sos_plan.PlantId;
                    _NewJobObservation.AreaId = (int)_sos_plan.AreaId;


                    var operationsList = _NewJobObservation.Operations?.ToList() ?? new List<Operation>();
                    operationsList.Add(operationsList.First(o => o.OperationId == OperationId));
                    _NewJobObservation.Operations = operationsList;

                    _NewJobObservation.DistributionId = DistributionId;

                    _NewJobObservation.SupervisorId = (int)context.Register.SupervisorId;

                    _NewJobObservation.Option = 2;
                    _NewJobObservation.Type = 3;
                    _NewJobObservation.Status = 7;
                    _NewJobObservation.SectionIds = jobCategoryStructureIds;
                    _NewJobObservation.IsActive = true;


                    DateTime parsedDate = new DateTime((int)_sos_plan.AplicationYear, month, 1);
                    parsedDate = await SOSDataServices.FindNextAvailableDate(parsedDate, false);
                    _NewJobObservation.StartDate = parsedDate;




                    var result = await SOSPlanReviewServices.CreateSOSRegister(_sos_plan.SOSid, month, (int)_sos_plan.AplicationYear, _NewJobObservation);
                    if (result != null)
                    {
                        result.Operation = SOSDataServices._All_Operations.Find(o => o.OperationId == OperationId);
                        //result.JobObservation.Operation = result.Operation;

                        var resultoperationsList = result.JobObservation.Operations?.ToList() ?? new List<Operation>();
                        operationsList.Add(operationsList.First(o => o.OperationId == OperationId));
                        result.JobObservation.Operations = operationsList;


                        result.JobObservation.Distribution = SOSDataServices._distributions.Find(d => d.DistributionId == DistributionId);
                        result.JobObservation.JobObservationId = (int)result.JobObservationId;
                        SOSDataServices._All_SOSJobobservation.Add(result.JobObservation);

                        SOSDataServices._SosRegisters.Add(result);

                        var matchingRegisters = SOSDataServices._SosRegisters?
                       .Where(r => r.OperationId == OperationId && r.Year <= _sos_plan.AplicationYear && r.Month == month)
                       .ToList();

                        if (SOSDataServices.SOS_Registers_Matrix.TryGetValue((OperationId, month), out var contextFind))
                        {
                            if (contextFind != null)
                            {
                                contextFind = matchingRegisters;
                            }
                            else
                            {
                                SOS_Registers_Matrix.Add((OperationId, month), matchingRegisters);
                            }
                        }


                        disableBtnCreateSos = false;

                        ShowTable = false;
                        //La pagina se resetea a shedule, hay que acomodar las variables
                        MonthlyView = false;
                        ScheduleView = true;
                        StateHasChanged();


                        await PrepareDataTable();

                        Snackbar.Clear();
                        Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                        Snackbar.Add($"SOS Review Job Observation Created", Severity.Info);
                        StateHasChanged();
                    }
                }
                else
                {
                    bool? result = await DialogService.ShowMessageBox(
                    "Atencion",
                    (MarkupString)$"Antes de proceder con la creación de Un Job Observation, por favor define un supervisor responsable.",
                    yesText: "Entendio!");

                    //Console.WriteLine($"La clave '{OperationId}' no existe en el diccionario.");

                    //msg de crear Supervisor
                }

            }
            else
            {
                bool? result = await DialogService.ShowMessageBox(
                "Atencion",
                (MarkupString)$"Antes de proceder con la creación de Un Job Observation, por favor define un supervisor responsable.",
                yesText: "Entendio!");

                //Console.WriteLine($"La clave '{OperationId}' no existe en el diccionario.");

                //msg de crear Supervisor
            }



            disableBtnCreateSos = false;
            StateHasChanged();
        }

        private async void CreateJobObservationInMonth(int day, int DistributionId, int OperationId)
        {
            disableBtnCreateSos = true;
            StateHasChanged();


            bool existeClave = SOS_Registers_UserOperationRelationship.ContainsKey(OperationId);

            if (SOS_Registers_UserOperationRelationship.TryGetValue(OperationId, out var context))
            {
                if (context.Exist)
                {

                    _NewJobObservation.PlantId = (int)_sos_plan.PlantId;
                    _NewJobObservation.AreaId = (int)_sos_plan.AreaId;

                    var operationsList = _NewJobObservation.Operations?.ToList() ?? new List<Operation>();
                    operationsList.Add(operationsList.First(o => o.OperationId == OperationId));
                    _NewJobObservation.Operations = operationsList;

                    _NewJobObservation.DistributionId = DistributionId;

                    _NewJobObservation.SupervisorId = (int)context.Register.SupervisorId;

                    _NewJobObservation.Option = 2;
                    _NewJobObservation.Type = 3;
                    _NewJobObservation.Status = 7;
                    _NewJobObservation.SectionIds = jobCategoryStructureIds;
                    _NewJobObservation.IsActive = true;


                    DateTime parsedDate = new DateTime((int)_sos_plan.AplicationYear, _yearMonth.Value.Month, day);
                    parsedDate = await SOSDataServices.FindNextAvailableDate(parsedDate, false);
                    _NewJobObservation.StartDate = parsedDate;




                    SOSRegisterJobObservation? result = await SOSPlanReviewServices.CreateSOSRegister(_sos_plan.SOSid, parsedDate.Month, (int)_sos_plan.AplicationYear, _NewJobObservation);
                    if (result != null)
                    {
                        result.Operation = SOSDataServices._All_Operations.Find(o => o.OperationId == OperationId);

                        var operationsListres = result.JobObservation.Operations?.ToList() ?? new List<Operation>();
                        operationsListres.Add(operationsList.First(o => o.OperationId == OperationId));
                        result.JobObservation.Operations = operationsList;

                        result.JobObservation.Distribution = SOSDataServices._distributions.Find(d => d.DistributionId == DistributionId);
                        result.JobObservation.JobObservationId = (int)result.JobObservationId;
                        SOSDataServices._All_SOSJobobservation.Add(result.JobObservation);

                        SOSDataServices._SosRegisters.Add(result);

                        var matchingRegisters = SOSDataServices._SosRegisters?
                       .Where(r => r.OperationId == OperationId && r.Year <= _sos_plan.AplicationYear && r.Month == parsedDate.Month)
                       .ToList();

                        if (SOSDataServices.SOS_Registers_Matrix.TryGetValue((OperationId, parsedDate.Month), out var contextFind))
                        {
                            if (contextFind != null)
                            {
                                contextFind = matchingRegisters;
                            }
                            else
                            {
                                SOS_Registers_Matrix.Add((OperationId, parsedDate.Month), matchingRegisters);
                            }
                        }

                        disableBtnCreateSos = false;

                        ShowTable = false;
                        //La pagina se resetea a shedule, hay que acomodar las variables
                        MonthlyView = false;
                        ScheduleView = true;
                        StateHasChanged();


                        await PrepareDataTable();

                        Snackbar.Clear();
                        Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                        Snackbar.Add($"SOS Review Job Observation Created", Severity.Info);
                        StateHasChanged();
                    }
                }
                else
                {
                    if (user.UserType == 3)
                    {
                        var resultCreateSv = await SOSPlanReviewServices.CreateSOSRegUserOperation(_sos_plan.SOSid, user.UserId, OperationId);
                        if (resultCreateSv != null)
                        {
                            context.Register = resultCreateSv;
                            context.Register.Supervisor = _Users.Find(u => u.UserId == context.Register.SupervisorId);
                            context.Exist = true;
                            context.StateUpdate = false;

                            Snackbar.Clear();
                            Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                            Snackbar.Add($"SOS Supervisor Auto Assigned", Severity.Info);

                            StateHasChanged();
                        }

                        _NewJobObservation.PlantId = (int)_sos_plan.PlantId;
                        _NewJobObservation.AreaId = (int)_sos_plan.AreaId;

                        var operationsList = _NewJobObservation.Operations?.ToList() ?? new List<Operation>();
                        operationsList.Add(operationsList.First(o => o.OperationId == OperationId));
                        _NewJobObservation.Operations = operationsList;


                        _NewJobObservation.DistributionId = DistributionId;

                        _NewJobObservation.SupervisorId = (int)context.Register.SupervisorId;

                        _NewJobObservation.Option = 2;
                        _NewJobObservation.Type = 3;
                        _NewJobObservation.Status = 7;
                        _NewJobObservation.SectionIds = jobCategoryStructureIds;
                        _NewJobObservation.IsActive = true;


                        DateTime parsedDate = new DateTime((int)_sos_plan.AplicationYear, _yearMonth.Value.Month, day);
                        parsedDate = await SOSDataServices.FindNextAvailableDate(parsedDate, false);
                        _NewJobObservation.StartDate = parsedDate;




                        SOSRegisterJobObservation? result = await SOSPlanReviewServices.CreateSOSRegister(_sos_plan.SOSid, parsedDate.Month, (int)_sos_plan.AplicationYear, _NewJobObservation);
                        if (result != null)
                        {
                            result.Operation = SOSDataServices._All_Operations.Find(o => o.OperationId == OperationId);


                            var resultoperationsList = _NewJobObservation.Operations?.ToList() ?? new List<Operation>();
                            resultoperationsList.Add(operationsList.First(o => o.OperationId == OperationId));
                            result.JobObservation.Operations = operationsList;

                            result.JobObservation.Distribution = SOSDataServices._distributions.Find(d => d.DistributionId == DistributionId);
                            result.JobObservation.JobObservationId = (int)result.JobObservationId;
                            var jobToAdd = await JobObsServices.GetJobObservationById((int)result.JobObservationId);
                            var jobMapped = _mapper.Map<JobObservationNulls>(jobToAdd);
                            SOSDataServices._All_SOSJobobservation.Add(jobMapped);

                            SOSDataServices._SosRegisters.Add(result);

                            var matchingRegisters = SOSDataServices._SosRegisters?
                           .Where(r => r.OperationId == OperationId && r.Year <= _sos_plan.AplicationYear && r.Month == parsedDate.Month)
                           .ToList();

                            if (SOSDataServices.SOS_Registers_Matrix.TryGetValue((OperationId, parsedDate.Month), out var contextFind))
                            {
                                if (contextFind != null)
                                {
                                    contextFind = matchingRegisters;
                                }
                                else
                                {
                                    SOS_Registers_Matrix.Add((OperationId, parsedDate.Month), matchingRegisters);
                                }
                            }

                            disableBtnCreateSos = false;

                            ShowTable = false;
                            MonthlyView = false;
                            ScheduleView = true;


                            await PrepareDataTable();

                            Snackbar.Clear();
                            Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                            Snackbar.Add($"SOS Review Job Observation Created", Severity.Info);
                            StateHasChanged();
                        }


                    }
                    else
                    {
                        bool? result = await DialogService.ShowMessageBox(
                        "Atencion",
                        (MarkupString)$"Antes de proceder con la creación de Un Job Observation, por favor define un supervisor responsable.",
                        yesText: "Entendio!");

                        //Console.WriteLine($"La clave '{OperationId}' no existe en el diccionario.");

                        ////ssv o admin intentanto crear job
                        ///
                    }



                }

            }
            else
            {
                bool? result = await DialogService.ShowMessageBox(
                "Atencion",
                (MarkupString)$"SOS Register User-Operation Relationship Not Exist.",
                yesText: "ok!");

                //Console.WriteLine($"La clave '{OperationId}' no existe en el diccionario.");

                //msg de crear Supervisor
            }



            disableBtnCreateSos = false;
            StateHasChanged();
        }


        private async void SwitchOperators()
        {

            if (!AddOperator)
            {
                _NewJobObservation.OperatorId = 0;
            }

            StateHasChanged();
        }

        private async void SwitchOperations()
        {
            if (!AddOperation)
            {
                //_NewJobObservation.OperationId = 0;
            }

            StateHasChanged();
        }

        private bool visible = false;
        private int jobId;
        JobObservation _jobObservation = null;
        TimeSpan? endHour { get; set; }
        TimeSpan? startHour { get; set; }
        DateTime? plannedStartDate = new();
        DateTime? plannedEndDate = new();
        private async void OpenDialog2(int id)
        {
            jobId = id;
            _jobObservation = await JobObsServices.GetJobObservationById(jobId, true, true, true, includeCkAnswers: true);
            startHour = _jobObservation.StartDate?.TimeOfDay;
            endHour = _jobObservation.EndDate?.TimeOfDay;
            if (_jobObservation.PlannedStartDate != null)
            {
                plannedStartDate = _jobObservation.PlannedStartDate;
                plannedEndDate = _jobObservation.PlannedEndDate;
            }
            else
            {
                plannedStartDate = _jobObservation.StartDate;
                plannedEndDate = _jobObservation.EndDate;
            }

            visible = true;
        }
        void Close() => visible = false;

        IDialogReference dialogDate;
        private async void OpenCommentDialog()
        {
            var parameters = new DialogParameters { { "_jobObservation", _jobObservation }, { "ChangeDate", EventCallback.Factory.Create(this, ChangeDate) } };
            dialogDate = await DialogService.ShowAsync<ChangeDate_Dialog>("", parameters, dialogCommentOptions);
            await dialogDate.Result;
        }
        private DialogOptions dialogCommentOptions = new() { CloseOnEscapeKey = true, MaxWidth = MaxWidth.Small, FullWidth = true };
        public string hour1 { get; set; }
        public string hour2 { get; set; }

        DateTime newDate1;
        DateTime newDate2;
        public async Task ChangeDate()
        {
            if (_jobObservation.Justification == null || _jobObservation.Justification == "")
            {
                Snackbar.Clear();
                Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                Snackbar.Add(Localizer["AddComment"], Severity.Error);
                return;
            }

            if (CultureInfo.CurrentCulture.Name == "en-US")
            {
                var formatedStartDate = _jobObservation.StartDate;
                var formatedEndDate = _jobObservation.EndDate;

                var EnglishStartDate = formatedStartDate?.Month.ToString() + "/" + formatedStartDate?.Day.ToString() + "/" + formatedStartDate?.Year.ToString();
                var EnglishEndDate = formatedEndDate?.Month.ToString() + "/" + formatedEndDate?.Day.ToString() + "/" + formatedEndDate?.Year.ToString();
                _jobObservation.StartDate = DateTime.ParseExact(EnglishStartDate, "M/d/yyyy", CultureInfo.InvariantCulture);
                _jobObservation.EndDate = DateTime.ParseExact(EnglishEndDate, "M/d/yyyy", CultureInfo.InvariantCulture);

                hour1 = _jobObservation.StartDate?.ToShortDateString() + $" {startHour}";
                hour2 = _jobObservation.EndDate?.ToShortDateString() + $" {endHour}";

                if (DateTime.TryParseExact(hour1, $"M/d/yyyy HH:mm:ss", null, DateTimeStyles.None, out newDate1))
                {
                    //Console.WriteLine(newDate1);
                }
                else
                {
                    Snackbar.Clear();
                    Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                    Snackbar.Add($"Error in Date Start", Severity.Error);
                    //Console.WriteLine("Unable to parse '{0}'", hour1);
                }


                if (DateTime.TryParseExact(hour2, $"M/d/yyyy HH:mm:ss", null, DateTimeStyles.None, out newDate2))
                {
                    //Console.WriteLine(newDate2);
                }
                else
                {
                    Snackbar.Clear();
                    Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                    Snackbar.Add(Localizer["DateError"], Severity.Error);
                    //Console.WriteLine("Unable to parse '{0}'", hour2);
                }

                _jobObservation.StartDate = newDate1;
                _jobObservation.EndDate = newDate2;

                _jobObservation.PlannedStartDate = newDate1;
                _jobObservation.PlannedEndDate = newDate2;

                //Console.WriteLine(_jobObservation.StartDate);
                //Console.WriteLine(_jobObservation.EndDate);


                if (plannedStartDate == _jobObservation.StartDate && plannedEndDate == _jobObservation.EndDate)
                {
                    Snackbar.Clear();
                    Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                    Snackbar.Add(Localizer["YouNeedChangeDate"], Severity.Error);
                    return;
                }

                if (_jobObservation.Status == 3)
                {
                    _jobObservation.Status = 1;
                }

                var result = await JobObsServices.UpdateJobObservation(_jobObservation, user.ObjectId);

                if (result)
                {

                    Snackbar.Clear();
                    Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                    Snackbar.Add(Localizer["DateChangeInJob"] + $" {_jobObservation.JobObservationId}", Severity.Info);

                    visible = false;
                    dialogDate.Close();
                    StateHasChanged();
                    SOSDataServices.UpdateJobItem(_jobObservation);
                    OnDateChanged(_yearMonth);
                }
                else
                    await JSRuntime.InvokeVoidAsync("alert", "Update failed!"); // Alert
            }
            else
            {
                hour1 = _jobObservation.StartDate?.ToShortDateString() + $" {startHour}";
                hour2 = _jobObservation.EndDate?.ToShortDateString() + $" {endHour}";

                if (DateTime.TryParseExact(hour1, $"d/M/yyyy HH:mm:ss", null, DateTimeStyles.None, out newDate1))
                {
                    //Console.WriteLine(newDate1);
                }
                else
                {
                    Snackbar.Clear();
                    Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                    Snackbar.Add(Localizer["DateStartError"], Severity.Error);
                    //Console.WriteLine("Unable to parse '{0}'", hour1);
                }


                if (DateTime.TryParseExact(hour2, $"d/M/yyyy HH:mm:ss", null, DateTimeStyles.None, out newDate2))
                {
                    //Console.WriteLine(newDate2);
                }
                else
                {
                    Snackbar.Clear();
                    Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                    Snackbar.Add(Localizer["DateEndError"], Severity.Error);
                    //Console.WriteLine("Unable to parse '{0}'", hour2);
                }

                _jobObservation.StartDate = newDate1;
                _jobObservation.EndDate = newDate2;

                _jobObservation.PlannedStartDate = newDate1;
                _jobObservation.PlannedEndDate = newDate2;

                //Console.WriteLine(_jobObservation.StartDate);
                //Console.WriteLine(_jobObservation.EndDate);


                if (plannedStartDate == _jobObservation.StartDate && plannedEndDate == _jobObservation.EndDate)
                {
                    Snackbar.Clear();
                    Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                    Snackbar.Add(Localizer["YouNeedChangeDate"], Severity.Error);
                    return;
                }

                if (_jobObservation.Status == 3)
                {
                    _jobObservation.Status = 1;
                }

                var result = await JobObsServices.UpdateJobObservation(_jobObservation, user.ObjectId);


                if (result)
                {

                    Snackbar.Clear();
                    Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                    Snackbar.Add(Localizer["DateChangeInJob"] + $" {_jobObservation.JobObservationId}", Severity.Info);


                    visible = false;
                    dialogDate.Close();
                    StateHasChanged();
                    SOSDataServices.UpdateJobItem(_jobObservation);
                    OnDateChanged(_yearMonth);
                }
                else
                    await JSRuntime.InvokeVoidAsync("alert", "Update failed!"); // Alert
            }
        }

        void JobObservationUpdate(int jobObservationId)
        {
            NavigationManager.NavigateTo($"jobobservation/updatejobobservation/{jobObservationId}");
        }


        //Programmed Job observation Modal (SOS)
        private bool visible2 = false;
        private int jobId2;


        private void OpenDialog3(int id, DateTime date)
        {
            jobId2 = id;
            //verificar formato de envio
            string fechaComoString = date.ToString("MM-dd-yyyy");

            if (CultureInfo.CurrentCulture.Name == "en-US")
            {
                var formatedStartDate = date;

                var EnglishStartDate = formatedStartDate.Month.ToString() + "/" + formatedStartDate.Day.ToString() + "/" + formatedStartDate.Year.ToString();
                var formatedStartDate2 = DateTime.ParseExact(EnglishStartDate, "M/d/yyyy", CultureInfo.InvariantCulture);

                programmedStartDate = formatedStartDate2.ToString("MM-dd-yyyy");
            }
            else
            {
                var hour1 = date.ToShortDateString();

                if (DateTime.TryParseExact(hour1, $"d/M/yyyy", null, DateTimeStyles.None, out var newDate1))
                {
                    //Console.WriteLine(newDate1);
                }
                else
                    //Console.WriteLine("Unable to parse {es-ES} '{0}'", hour1);

                    programmedStartDate = newDate1.ToString("MM-dd-yyyy");
            }

            visible2 = true;
        }

        void Close2() => visible2 = false;
        void CloseGraphicDonnut()
        {
            opInDistDialog.isActive = false;
            ShowGraphicDonnut = false;
        }
        private void OpenGraphicDonnut(SosJobCount item)
        {
            opInDistDialog = item;
            opInDistDialog.isActive = true;
            ShowGraphicDonnut = true;
        }
        private async Task HandleVisibleChanged(bool newValue)
        {

            loadingData = true;
            ShowLoading = true;


            Distribution? tmpdist = _distributions?.Find(d => d.ShowDetails == true);

            DistSelect? tmpSuggdist = null;
            if (tmpdist == null)
            {
                tmpSuggdist = Dist_Manager?.Find(d => d.distribution.ShowDetails == true);
                if (tmpSuggdist != null)
                {
                    tmpSuggdist.distribution.ShowDetails = false;
                }
            }
            else
            {
                tmpdist.ShowDetails = false;
            }

            StateHasChanged();



            if (tmpdist != null)
            {
                await PrepareDataTable(tmpdist.DistributionId);
                tmpdist.ShowDetails = true;
            }
            else if (tmpSuggdist != null)
            {
                await PrepareSuggestDataTable(tmpSuggdist.distribution.DistributionId);
                tmpSuggdist.distribution.ShowDetails = true;
            }
            else if (!SuggestionMode)
            {
                //aqui manda a llamar al servicio y actualizamos los datos necesarios unicamente
                await PrepareDataTable();
            }
            else
            {
                await PrepareSuggestDataTable();
            }


            visible2 = newValue;
            ShowLoading = false;
            loadingData = false;
            StateHasChanged();

        }

        //Button Programmed Job observation Modal (SOS)
        private bool visible3 = false;

        public string programmedStartDate = "";
        private void OpenDialog4(int DistributionIndex, int MonthIndex, int Type)
        {
            searchString = "";
            _SOSJobobservation = new();

            if (SOS_Registers_Matrix.TryGetValue((DistributionIndex, MonthIndex), out var context))
            {
                if (context?.Count() > 0)
                {
                    foreach (SOSRegisterJobObservation jobobs in context)
                    {
                        if (Type == 1 && jobobs.JobObservation.Status == 7)
                        {
                            _SOSJobobservation.Add(jobobs.JobObservation);
                        }
                        else if (Type == 2 && jobobs.JobObservation.Status != 7)
                        {
                            _SOSJobobservation.Add(jobobs.JobObservation);
                        }
                    }
                }
            }

            visible3 = true;
        }
        void Close3() => visible3 = false;

        private string searchString = "";

        private bool FilterFunc(JobObservationNulls element)
        {
            if (string.IsNullOrWhiteSpace(searchString))
                return true;
            if (element.JobObservationId.ToString().Contains(searchString, StringComparison.OrdinalIgnoreCase))
                return true;
            if (element.Distribution.Description.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                return true;
            if (element.StartDate.ToString().Contains(searchString, StringComparison.OrdinalIgnoreCase))
                return true;
            if ($"{element.JobObservationId} {element.Distribution.Description} {element.StartDate?.ToString()}".Contains(searchString))
                return true;
            return false;
        }




        private async void CloseModalFiles()
        {
            CodePathModalDisplay = false;

            StateHasChanged();

        }

        private async Task<AsyncVoidMethodBuilder> OpenDialogCodePath(SOSCodePath itemselected, int panelSelect)
        {

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




        private bool visible5 = false;
        public List<JobObservationNulls> _DayJobObservations { get; set; } = new();

        private void OpenDialog5(string date)
        {
            searchString = "";
            var compare = DateTime.ParseExact(date, "d/M/yyyy", null);
            _DayJobObservations = _All_Suggested_SOSJobobservation.Where(j => Convert.ToDateTime(j.StartDate?.ToShortDateString()).Date <= Convert.ToDateTime(compare.ToShortDateString()).Date && Convert.ToDateTime(compare.ToShortDateString()).Date <= Convert.ToDateTime(j.EndDate?.ToShortDateString()).Date && j.Status != 7).ToList();
            visible5 = true;
        }
        void Close5() => visible5 = false;
        ////
        private IEnumerable<string> ValidateStartDay(int? startDay)
        {
            if (!startDay.HasValue)
            {
                yield return "Ingrese un número válido.";
                yield break;
            }

            if (startDay < 3 || startDay >= 25)
            {
                yield return "El día debe ser mayor o igual a 3 y menor a 25.";
            }
        }
        //

        private async void ShowBtnPress(int distribution_Id)
        {
            //Console.WriteLine($"Btn Press dist: {distribution_Id} - {DateTime.Now}");
            Distribution? tmpdist = _distributions.FirstOrDefault(f => f.DistributionId == distribution_Id);

            if (tmpdist != null)
            {
                Distribution? AnotherOpenDistExist = _distributions.Find(dist => dist.ShowDetails && dist.DistributionId != distribution_Id);

                if (AnotherOpenDistExist != null)
                {
                    AnotherOpenDistExist.ShowDetails = false;
                }

                tmpdist.ShowDetails = !tmpdist.ShowDetails;

                if (tmpdist.ShowDetails)
                {
                    if (SuggestionMode)
                    {
                        await PrepareSuggestDataTable(tmpdist.DistributionId);
                    }
                    else
                    {
                        await PrepareDataTable(tmpdist.DistributionId);
                    }
                }

            }
        }

        private async Task BtnSupervisorsEdit(int op)
        {
            CloseSVPanelDialog();

            if (SOS_Registers_UserOperationRelationship.TryGetValue(op, out var context))
            {
                context.StateUpdate = !context.StateUpdate;

                if (!context.Exist)
                {
                    try
                    {
                        //Validar verificaion

                        var result = await SOSPlanReviewServices.CreateSOSRegUserOperation(_sos_plan.SOSid, context.Register.Supervisor.UserId, op);
                        if (result != null)
                        {
                            context.Register = result;
                            context.Register.Supervisor = _Users.Find(u => u.UserId == context.Register.SupervisorId);
                            context.Exist = true;
                            context.StateUpdate = false;

                            ShowTable = false;
                            //La pagina se resetea a shedule, hay que acomodar las variables
                            MonthlyView = false;
                            ScheduleView = true;
                            StateHasChanged();

                            await PrepareDataTable();

                            Snackbar.Clear();
                            Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                            Snackbar.Add($"SOS Supervisor Assigned", Severity.Info);


                            StateHasChanged();
                        }

                    }
                    catch (Exception ex)
                    {
                        //Console.WriteLine($"Error In Create SOSRegUserOperationRelationship Function: {ex.Message} ");

                        bool? result = await DialogService.ShowMessageBox(
                               "Atencion",
                               (MarkupString)$"Por favor selecciona un supervisor responsable valido.",
                               yesText: "Entendio!");

                        context.StateUpdate = !context.StateUpdate;
                    }
                }
                else
                {
                    if (!context.StateUpdate)
                    {
                        if (context.Register.SupervisorId != context.Register.Supervisor?.UserId)
                        {
                            try
                            {
                                context.Register.SupervisorId = context.Register.Supervisor.UserId;

                                var dialogOptions = new DialogOptions { FullWidth = true, DisableBackdropClick = true };

                                var dialog = await DialogService.ShowAsync<AssignSVDialog>(Localizer1["SOS_Title_AssignSV"], dialogOptions);
                                var result = await dialog.Result;

                                if (!result.Canceled)
                                {
                                    bool? option = (bool?)result.Data;
                                    //Console.WriteLine($"Option : {option}");
                                    //mensaje de opocion
                                    switch (option)
                                    {
                                        case true:
                                            //Todos los reguistros Unicamente en la misma distribucion
                                            var result1 = await SOSPlanReviewServices.UpdateSOSRegUserOperation(context.Register, 1);
                                            if (result1 != null)
                                            {
                                                ShowTable = false;
                                                MonthlyView = false;
                                                ScheduleView = true;
                                                context.Register = result1;
                                                context.Register.Supervisor = _Users.ToList().Find(u => u.UserId == context.Register.SupervisorId);
                                                context.Exist = true;
                                                context.StateUpdate = false;
                                                StateHasChanged();
                                                await PrepareDataTable();

                                                Snackbar.Clear();
                                                Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                                                Snackbar.Add($"All Distribution Supervisor Assigned Changed", Severity.Info);

                                                NavigationManager.NavigateTo(NavigationManager.Uri, forceLoad: true);
                                                StateHasChanged();
                                            }
                                            break;

                                        case false:
                                            //Todos los reguistros en la SOS
                                            var result2 = await SOSPlanReviewServices.UpdateSOSRegUserOperation(context.Register, 2);
                                            if (result2 != null)
                                            {
                                                ShowTable = false;
                                                MonthlyView = false;
                                                ScheduleView = true;
                                                StateHasChanged();
                                                context.Register = result2;
                                                context.Register.Supervisor = _Users.ToList().Find(u => u.UserId == context.Register.SupervisorId);
                                                context.Exist = true;
                                                context.StateUpdate = false;
                                                await PrepareDataTable();

                                                Snackbar.Clear();
                                                Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                                                Snackbar.Add($"All SOS Supervisor Assigned Changed", Severity.Info);

                                                NavigationManager.NavigateTo(NavigationManager.Uri, forceLoad: true);

                                                StateHasChanged();
                                            }
                                            break;

                                        case null:
                                            //Todos los reguistros en la operacion ROW
                                            var result3 = await SOSPlanReviewServices.UpdateSOSRegUserOperation(context.Register, 3);
                                            if (result3 != null)
                                            {
                                                ShowTable = false;
                                                MonthlyView = false;
                                                ScheduleView = true; StateHasChanged();
                                                context.Register = result3;
                                                context.Register.Supervisor = _Users.ToList().Find(u => u.UserId == context.Register.SupervisorId);
                                                context.Exist = true;
                                                context.StateUpdate = false;
                                                await PrepareDataTable();

                                                Snackbar.Clear();
                                                Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                                                Snackbar.Add($"SOS Supervisor Per Row Assigned", Severity.Info);

                                                StateHasChanged();
                                            }
                                            break;
                                    }

                                    if (option != null)
                                    {
                                        SOS_Registers_UserOperationRelationship.Clear();
                                        OperationsInDistributionCount.Clear();
                                        //Console.WriteLine($"First Time: Create SOS_Registers_UserOperationRelationship");
                                        _SosRegistersrUserOperation = await SOSPlanReviewServices.GetSOSRegUserOperation(_sos_plan.SOSid);

                                        foreach (var item in _SosRegistersrUserOperation)
                                        {
                                            SOSRegUserOperationRelationship regAux = new();
                                            regAux.Register = item;
                                            regAux.StateUpdate = false;
                                            regAux.Exist = true;
                                            SOS_Registers_UserOperationRelationship.Add((int)item.OperationId, regAux);
                                        }

                                        foreach (var itemOp in SOSDataServices._All_Operations)
                                        {
                                            if (!SOS_Registers_UserOperationRelationship.TryGetValue(itemOp.OperationId, out var contextitemOp))
                                            {
                                                //si no existre se crea de manera artificial
                                                SOSRegUserOperationRelationship regAux = new();
                                                regAux.Register = new();
                                                regAux.Exist = false;
                                                regAux.StateUpdate = false;
                                                SOS_Registers_UserOperationRelationship.Add(itemOp.OperationId, regAux);
                                            }
                                        }


                                    }
                                }

                            }
                            catch (Exception ex)
                            {
                                //Console.WriteLine($"Error In assign Supervisor ID: {ex.Message} ");

                                bool? result = await DialogService.ShowMessageBox(
                                       "Assign Supervisor ID Error",
                                       (MarkupString)$"Call For Develoment.",
                                       yesText: "ok!");
                            }

                        }
                        else
                        {
                            Snackbar.Clear();
                            Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                            Snackbar.Add($"Ningun Cambio, Con exito!", Severity.Info);
                            StateHasChanged();

                        }

                    }
                }

            }

        }


        private async Task<IEnumerable<User>> SearchSV(string value)
        {
            // In real life use an asynchronous function for fetching data from an api.
            // await Task.Delay(1000);

            // if text is null or empty, show complete list
            if (string.IsNullOrEmpty(value))
                return _Users;

            return _Users.Where(x => x.Name.ToLower().Contains(value.ToLower(), StringComparison.InvariantCultureIgnoreCase));
        }

        private int ObtenerDiasLaburables()
        {
            DateTime fechaInicio = new DateTime(_yearMonth.Value.Year, _yearMonth.Value.Month, 1);
            int totalDiasLaborables = 0;
            int totalDiasEnMes = DateTime.DaysInMonth(fechaInicio.Year, fechaInicio.Month);

            for (int i = 1; i <= totalDiasEnMes; i++)
            {
                DateTime fechaActual = new DateTime(fechaInicio.Year, fechaInicio.Month, i);

                // Verificar si el día actual no es sábado ni domingo
                if (fechaActual.DayOfWeek != DayOfWeek.Saturday && fechaActual.DayOfWeek != DayOfWeek.Sunday)
                {
                    totalDiasLaborables++;
                }
            }

            return totalDiasLaborables;
        }



        bool enableCreateSuggestion = false;
        bool SuggestionMode = false;

        private void StartCreateSuggestion()
        {
            enableCreateSuggestion = true;
            _yearMonth = Startday;
            StateHasChanged();

            if (Startday.Date == DateTime.Now.AddDays(-1).Date && Startday.Year < _sos_plan.AplicationYear)
            {
                DialogService.ShowMessageBox("Warning", "Select Day To Start!", yesText: "OK!");
                enableCreateSuggestion = false;
                StateHasChanged();
                return;
            }

            if (!Dist_Manager.Any(i => i.isSelected))
            {
                DialogService.ShowMessageBox("Warning", "Select Distribution First!", yesText: "OK!");
                enableCreateSuggestion = false;
                StateHasChanged();
                return;
            }


            if (JobsPorDia == 0)
            {
                DialogService.ShowMessageBox("Warning", "Its Necessary at least one Job per Day !", yesText: "OK!");
                StateHasChanged();
                enableCreateSuggestion = false;
                return;
            }

            if (diasSeparate == 0)
            {
                DialogService.ShowMessageBox("Warning", "It is necessary to separate at least 1 day !", yesText: "OK!");
                StateHasChanged();
                enableCreateSuggestion = false;
                return;
            }

            if (SV_Manager.Count() == 0)
            {
                DialogService.ShowMessageBox("Warning", "SV is Necessary!", yesText: "OK!");
                enableCreateSuggestion = false;
                StateHasChanged();
                return;
            }

            Task.Run(async () =>
            {
                //ShowLoading = true;
                await CreateSuggestion();
            }
           );
        }

        private async Task CreateSuggestion()
        {

            if (Dist_Manager.Any(i => i.isSelected) && SV_Manager.Count() != 0 && Startday.Date != DateTime.Now.AddDays(-1).Date)
            {
                await SOSDataServices.SetSugestionJobObservation(_sos_plan, Dist_Manager, SV_Manager, diasSeparate, Startday, JobsPorDia);
                await PrepareSuggestDataTable();

                await DialogService.ShowMessageBox("Info!", "Suggestion created!", yesText: "OK!");
                //Console.WriteLine($" Final First Sugg Generation");

                //ShowLoading = false;
                enableCreateSuggestion = false;
                SVSinCharge = false;
                isButtonDisabled = false;
                StateHasChanged();
            }

        }

        private async void StartCreateNewSuggestion()
        {
            enableCreateSuggestion = true;
            _yearMonth = Startday;
            StateHasChanged();

            if (Startday.Date == DateTime.Now.AddDays(-1).Date)
            {
                DialogService.ShowMessageBox("Warning", "Select Day To Start!", yesText: "OK!");
                StateHasChanged();
                return;
            }

            if (!Dist_Manager.Any(i => i.isSelected))
            {
                DialogService.ShowMessageBox("Warning", "Select Distribution First!", yesText: "OK!");
                StateHasChanged();
                return;
            }

            if (JobsPorDia == 0)
            {
                DialogService.ShowMessageBox("Warning", "Its Necessary at least one Job per Day !", yesText: "OK!");
                StateHasChanged();
                return;
            }

            if (diasSeparate == 0)
            {
                DialogService.ShowMessageBox("Warning", "It is necessary to separate at least 1 day !", yesText: "OK!");
                StateHasChanged();
                return;
            }

            if (SV_Manager.Count == 0)
            {
                DialogService.ShowMessageBox("Warning", "SV is Necessary!", yesText: "OK!");
                StateHasChanged();
                return;
            }


            isButtonDisabled = true;
            ShowLoading = true;
            StateHasChanged();

            await Task.Run(async () =>
            {
                await CreateNewSuggestion();
            }
            );
        }

        private async Task CreateNewSuggestion()
        {
            DateTime StartdayUTC = Startday.ToUniversalTime(); // Para asegurarte de que siempre sea UTC


            await SOSDataServices.SetNewConfigSugestionJobObservation(_sos_plan, Dist_Manager, SV_Manager, diasSeparate, StartdayUTC, JobsPorDia, OptionRandom);


            await DialogService.ShowMessageBox("Info!", "New suggestion created!", yesText: "OK!");



            DistSelect? tmpSuggdist = null;

            tmpSuggdist = Dist_Manager?.Find(d => d.distribution.ShowDetails == true);
            Dist_Manager.ForEach(d => d.distribution.ShowDetails = false);

            if (tmpSuggdist != null)
            {
                await PrepareSuggestDataTable(tmpSuggdist.distribution.DistributionId);
                tmpSuggdist.distribution.ShowDetails = true;
            }
            else
            {
                await PrepareSuggestDataTable();
            }



            // Restaura el estado y muestra la tabla nuevamente
            ShowLoading = false;
            enableCreateSuggestion = false;
            ShowTable = true;
            isButtonDisabled = false;
            StateHasChanged();

        }

        private void OpenDialogSuggestSV()
        {
            if (SV_Manager.Count == 0)
                SVSinCharge = true;

            MonthlyView = true;
            SuggestionMode = true;
        }

        // Drag and drop category
        async void ItemUpdated(MudItemDropInfo<User> dropItem)
        {
            dropItem.Item.Container = dropItem.DropzoneIdentifier;
            var indexOffset = 0;
            int currentCategory;
            int newSequence;

            SV_Manager.UpdateOrder(dropItem, item => item.Sequence, indexOffset);

            newSequence = dropItem.IndexInZone + 1;

            dropItem.Item.Sequence = newSequence;


            SV_Manager = SV_Manager.OrderBy(u => u.Sequence).ToList();

        }



        private async Task<IEnumerable<User>> SearchSuggestSV(string value)
        {
            // In real life use an asynchronous function for fetching data from an api.
            // await Task.Delay(1000);

            // if text is null or empty, show complete list
            if (string.IsNullOrEmpty(value))
                return _UsersSV_Copy;

            return _UsersSV_Copy.Where(x => x.Name.Contains(value, StringComparison.InvariantCultureIgnoreCase));
        }

        private User selectedSupervisorOfList = null;
        private bool ActiveAddSubordinated = false;

        private async void OnSelectedSuperiorFunction(User element, int type)
        {

            selectedSupervisorOfList = element;


            if (selectedSupervisorOfList != new User())
            {
                ActiveAddSubordinated = false;
            }
            else
            {
                ActiveAddSubordinated = true;
            }

        }

        private async void AddSupervisor(User selection)
        {
            ShowTable = false;
            if (SV_Manager == null)
            {
                SV_Manager = new List<User>();
            }


            if (selectedSupervisorOfList != null && !SV_Manager.Contains(selection))
            {

                SV_Manager.Add(selection);

                _UsersSV_Copy.Remove(selection);

                selectedSupervisorOfList = null;
                ActiveAddSubordinated = true;
            }
            ShowTable = true;

            StateHasChanged();
        }

        private void DeleteSupervisorList(User selection)
        {
            SV_Manager.Remove(selection);
            _UsersSV_Copy.Add(selection);
            StateHasChanged();
        }

        private void UpdateSuggesSv(int op)
        {
            if (Suggested_SOS_Registers_UserOperationRelationship.TryGetValue(op, out var context))
            {
                context.StateUpdate = true;
                var SearchJob = _All_Suggested_SOSJobobservation.Find(j => j.Operations?.FirstOrDefault()?.OperationId == op);
                SearchJob.SupervisorId = context.Register.Supervisor.UserId;

                //Console.WriteLine($"Update SV");
                visibleSuggestSVDialog = false;
            }
        }

        private async Task<AsyncVoidMethodBuilder> ApplySuggest()
        {

            DistSelect? tmpSuggdist = Dist_Manager?.Find(d => d.distribution.ShowDetails == true);

            if (tmpSuggdist != null)
            {
                Dist_Manager.ForEach(d => d.distribution.ShowDetails = false);
            }

            isButtonDisabled = true;
            ShowLoading = true;
            base.StateHasChanged();
            StateHasChanged();

            //mandar absolutamente todas las existentes

            var result = await SOSPlanReviewServices.ApplyMassiveSuggest(_sos_plan.SOSid, SOSDataServices._All_Suggested_SOSJobobservation, Dist_Manager.Where(item => item.isSelected).ToList());
            if (result)
            {
                StateHasChanged();

                Snackbar.Clear();
                Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                Snackbar.Add($"Massive Creation Suggest Sucesfull", Severity.Info);

                _sos_plan.SuggestionApplied = true;

                NavigationManager.NavigateTo(NavigationManager.Uri, forceLoad: true);
                StateHasChanged();
            }
            else
            {

                Snackbar.Clear();
                Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                Snackbar.Add($"Error! Create Suggest", Severity.Info);

                StateHasChanged();
            }

            ShowLoading = false;
            SuggestionMode = false;
            return new AsyncVoidMethodBuilder();
        }

        public void UpdateSelectDistribution()
        {
            selected_distribution = _distributions?.Find(d => d.DistributionId == selected_distId);
        }

        public void UpdateMultiDistribution(DistSelect CheckItem)
        {
            CheckItem.isSelected = !CheckItem.isSelected;
            //Console.Write(CheckItem.distribution.Code);
            StateHasChanged();
        }

        public void SelectUnselect()
        {
            if (Dist_Manager.Any(i => i.isSelected))
            {
                foreach (var item in Dist_Manager)
                {
                    item.isSelected = false;
                }
            }
            else
            {
                foreach (var item in Dist_Manager)
                {
                    item.isSelected = true;
                }

            }
            StateHasChanged();
        }

        SOSRegUserOperationRelationship RegSelect = new();
        private void OpenSVPanelDialog(int panelid, SOSRegUserOperationRelationship itemselect, int op)
        {
            if (panelid != 2)
                selectedSVPanel = panelid;

            RegSelect = itemselect;
            selectedSOPPanel = op;

            if (panelid == 1)
            {
                RegSelect.StateUpdate = true;
            }
            visibleSVDialog = true;
        }

        void CloseSVPanelDialog() => visibleSVDialog = false;

        private void OpenSuggestSVPanelDialog(int panelid, SOSRegUserOperationRelationship itemselect, int op)
        {
            if (panelid != 2)
                selectedSVPanel = panelid;

            RegSelect = itemselect;
            selectedSOPPanel = op;

            if (panelid == 1)
            {
                RegSelect.StateUpdate = true;
            }
            visibleSuggestSVDialog = true;
        }

        private bool dense = false;
        private bool hover = false;
        private bool ronly = false;

        private void OpenToReprogramSuggestion(JobObservationNulls context)
        {
            _Selected_Suggested_SOSJobobservatio_Null = context;

            _mapper.Map(_Selected_Suggested_SOSJobobservatio_Null, _Selected_Suggested_SOSJobobservation);

            ProgrammSuggestion = true;

        }
        private async void CloseToReprogramSuggestion()
        {
            ShowLoading = true;
            int opid = (int)_Selected_Suggested_SOSJobobservatio_Null.Operations?.FirstOrDefault()?.OperationId;
            int distid = (int)_Selected_Suggested_SOSJobobservatio_Null.DistributionId;
            DateTime? oldDate = _Selected_Suggested_SOSJobobservatio_Null.StartDate;
            DateTime? newDate = _Selected_Suggested_SOSJobobservation.PlannedStartDate;

            if (Suggested_Registers_Matrix.TryGetValue((distid, oldDate.Value.Month), out var Suggestcontext))
            {
                var itemToMove = Suggestcontext.Find(j => j.StartDate.Value.Month == oldDate.Value.Month && j.Operations?.FirstOrDefault()?.OperationId == opid);

                if (itemToMove != null)
                {
                    // Remover el elemento encontrado de la lista actual (Suggestcontext)
                    Suggestcontext.Remove(itemToMove);

                    // Obtener la lista correspondiente al nuevo mes (newMonth)
                    if (Suggested_Registers_Matrix.TryGetValue((distid, newDate.Value.Month), out var targetContext))
                    {
                        // Agregar el elemento a la lista correspondiente al nuevo mes
                        targetContext.Add(itemToMove);

                        // Actualizar el diccionario con la nueva lista
                        Suggested_Registers_Matrix[(distid, newDate.Value.Month)] = targetContext;
                    }
                }
            }

            _mapper.Map(_Selected_Suggested_SOSJobobservation, _Selected_Suggested_SOSJobobservatio_Null);
            //actualizar la matrix unicamente
            if (ScheduleView)
            {
                GenerateCalendarHead();
                GenerateCalendarBody();
            }

            ShowLoading = false;
            ProgrammSuggestion = false;
            StateHasChanged();
        }

        private void OnStartDateChanged(DateTime dt)
        {
            _Selected_Suggested_SOSJobobservation.StartDate = dt;
            _Selected_Suggested_SOSJobobservation.PlannedStartDate = dt;
            _Selected_Suggested_SOSJobobservation.EndDate = dt;


        }

        void CloseSuggestSVPanelDialog() => visibleSuggestSVDialog = false;


    }//end sos class


}