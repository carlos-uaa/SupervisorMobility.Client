using global::System;
using global::System.Collections.Generic;
using global::System.Linq;
using global::System.Threading.Tasks;
using global::Microsoft.AspNetCore.Components;
using System.Net.Http;
using Microsoft.JSInterop;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.Web.Virtualization;
using Microsoft.AspNetCore.Components.WebAssembly.Http;
using SupervisorMobility.Client;
using SupervisorMobility.Client.Shared;
using SupervisorMobility.Client.Services;
using SupervisorMobility.Client.Data.Resources;
using Microsoft.Extensions.Localization;
using BlazorCameraStreamer;
using System.Net.Http.Json;
using AutoMapper;
using MudBlazor;
using MudBlazor.Utilities;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Runtime.CompilerServices;
using SupervisorMobility.Client.Data.Entities;
using SupervisorMobility.Client.Services.BreadcrumsService;

namespace SupervisorMobility.Client.Pages.Inicio.SOSProgramPage
{
    public partial class SOS_Details

    {
        [Parameter]
        public int sosId { get; set; }

        [Inject] private IDialogService DialogService { get; set; }
        [Inject]
        private IBreadcrumbService BreadcrumbService { get; set; }
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
        public List<Operation> _All_Operations { get; set; } = new();
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

        private class SOSRegUserOperationRelationship
        {
            public bool StateUpdate { get; set; }
            public bool Exist { get; set; }
            public SOSRegUserOperation Register { get; set; }
        }

        private class SosJobCount
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
        int diasSeparate = 1;
        int OptionRandom = 0;
        DateTime Startday = new();
        DateTime FirstdayYear = DateTime.Now;
        DateTime LastdayYear = DateTime.Now;
        int JobsPorDia = 1;

        private DialogOptions dialogOptions = new() { CloseOnEscapeKey = true, MaxWidth = MaxWidth.Large, FullWidth = true, DisableBackdropClick = true, CloseButton = true };
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
        public int totalProgrammed;
             

        public bool CodePathModalDisplay { get; set; } = false;
        private string searchCodeString = "";
        bool ShowLoading = true;
        private IList<string> _sourceMsgLoading = new List<string>();
        private IList<Color> _Colors = new List<Color>() { Color.Default, Color.Primary, Color.Secondary, Color.Success, Color.Info, Color.Default, Color.Primary, Color.Secondary, Color.Success, Color.Info };

      

        private List<User> _Users = new();
        private List<User> _UsersSV_Copy = new();
        bool FilterOperation = false;

        private bool isButtonDisabled = false;
        private bool SVSinCharge = false;

        //sv panel
        private bool visibleSVDialog = false;
        private bool visibleSuggestSVDialog = false;
        private int selectedSVPanel = 0;
        private int selectedSOPPanel = 0;


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
                _sos_plan = await SOSServices.GetSOSById(sosId, true);
                _distributions = await DistributionServices.GetDistributionsWithCollections((int)_sos_plan.PlantId, (int)_sos_plan.AreaId);

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

                foreach (var item in _distributions)
                {
                    _All_Operations.AddRange(item.Operations);
                }
                _Users = await UsersServices.GetUsersByUserTypeInPlantAndArea(_sos_plan.PlantId, _sos_plan.AreaId, 3, true, false);
                _UsersSV_Copy = ObjectCloner.ObjectCloner.DeepClone(_Users);
                _assyCharts = await AssyChartsServices.GetAssyChartsByArea((int)_sos_plan.PlantId, (int)_sos_plan.AreaId);
                await PrepareDataTable();
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
            totalProgrammed = _All_SOSJobobservation.Where(j => j.Status == 7 && j.StartDate?.Month == _yearMonth?.Month && j.StartDate?.Year == _yearMonth?.Year).Count();


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

        private void OnFirstDateChanged(DateTime? dt)
        {
            if (dt.HasValue)
            {
                Startday = new DateTime(dt.Value.Year, dt.Value.Month, dt.Value.Day);
            }

        }

        private void OnDateChanged(DateTime? value)
        {
            ShowLoading = true;

            _yearMonth = value;
            daysInMonth = DateTime.DaysInMonth(_yearMonth.Value.Year, _yearMonth.Value.Month);


            month = $"{_yearMonth?.ToString("MMMM")}";
            year = $"{_yearMonth?.ToString("yyyy")}";
            int monthIndex = DateTime.ParseExact(month, "MMMM", System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat).Month;
            int yearIndex = DateTime.ParseExact(year, "yyyy", System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat).Year;
        
            totalProgrammed = _All_SOSJobobservation.Where(j => j.Status == 7 && j.StartDate?.Month == _yearMonth?.Month && j.StartDate?.Year == _yearMonth?.Year).Count();

            ShowLoading = false;
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
                    tmpSuggdist.distribution.ShowDetails = true;
                }
            }
            else
            {
                tmpdist.ShowDetails = false;
            }
            StateHasChanged();
            ShowLoading = true;
            StateHasChanged();

            _yearMonth = _yearMonth?.AddMonths(-1);
            daysInMonth = DateTime.DaysInMonth(_yearMonth.Value.Year, _yearMonth.Value.Month);

            month = $"{_yearMonth?.ToString("MMMM")}";
            year = $"{_yearMonth?.ToString("yyyy")}";
            int monthIndex = DateTime.ParseExact(month, "MMMM", System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat).Month;
            int yearIndex = DateTime.ParseExact(year, "yyyy", System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat).Year;
           
            totalProgrammed = _All_SOSJobobservation.Where(j => j.Status == 7 && j.StartDate?.Month == _yearMonth?.Month && j.StartDate?.Year == _yearMonth?.Year).Count();
            ShowLoading = false;

            if (tmpdist != null)
            {
                tmpdist.ShowDetails = true;
            }
            if (tmpSuggdist != null)
            {
                tmpSuggdist.distribution.ShowDetails = true;
            }
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
                    tmpSuggdist.distribution.ShowDetails = true;
                }
            }
            else
            {
                    tmpdist.ShowDetails = false;
            }
            StateHasChanged();


            ShowLoading = true;
            StateHasChanged();
            _yearMonth = _yearMonth?.AddMonths(1);
            daysInMonth = DateTime.DaysInMonth(_yearMonth.Value.Year, _yearMonth.Value.Month);

            month = $"{_yearMonth?.ToString("MMMM")}";
            year = $"{_yearMonth?.ToString("yyyy")}";
            int monthIndex = DateTime.ParseExact(month, "MMMM", System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat).Month;
            int yearIndex = DateTime.ParseExact(year, "yyyy", System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat).Year;
           
            totalProgrammed = _All_SOSJobobservation.Where(j => j.Status == 7 && j.StartDate?.Month == _yearMonth?.Month && j.StartDate?.Year == _yearMonth?.Year).Count();
            StateHasChanged();
            ShowLoading = false;

            if (tmpdist != null)
            {
                tmpdist.ShowDetails = true;
            }
            if (tmpSuggdist != null)
            {
                tmpSuggdist.distribution.ShowDetails = true;
            }
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

        private async Task PrepareSuggestDataTable()
        {

            Suggested_Registers_Matrix?.Clear();

            foreach (var dist in _distributions)
            {
                for (int i = 1; i < 13; i++)
                {
                    int currentindex = i;
                    var matchingRegisters = _All_Suggested_SOSJobobservation?
                       .Where(r => r.DistributionId == dist.DistributionId && r.StartDate.Value.Month == currentindex)
                       .ToList();

                    Suggested_Registers_Matrix.Add((dist.DistributionId, currentindex), matchingRegisters);
                }
            }

        }

        private async Task PrepareDataTable()
        {
            _All_SOSJobobservation?.Clear();
            SOS_Registers_Matrix?.Clear();
            _SosRegisters?.Clear();
            _AnotherJobs?.Clear();
            OperationsInDistributionCount?.Clear();

            //Actualizacion local y evitar peticiones... cuando quede chida la pagina
            _SosRegisters = await SOSServices.GetSOSRegisters(_sos_plan.SOSid);
            _AnotherJobs = _mapper.Map<List<JobObservationNulls>>(await JobObsServices.GetAllJobObservations(year: (int)_sos_plan.AplicationYear, SOSAnualId: _sos_plan.SOSid));


            foreach (var OpItem in _All_Operations)
            {
                for (int i = 1; i < 13; i++)
                {
                    int currentindex = i;
                    var matchingRegisters = _SosRegisters?
                       .Where(r => r.OperationId == OpItem.OperationId && r.Year <= _sos_plan.AplicationYear && r.Month == currentindex)
                       .ToList();

                    SOS_Registers_Matrix.Add((OpItem.OperationId, currentindex), matchingRegisters);
                }
            }

            foreach (var item in _SosRegisters)
            {
                _All_SOSJobobservation?.Add(item.JobObservation);
            }

            if (SOS_Registers_UserOperationRelationship?.Count == 0)
            {
                Console.WriteLine($"First Time: Create SOS_Registers_UserOperationRelationship");
                _SosRegistersrUserOperation = await SOSServices.GetSOSRegUserOperation(_sos_plan.SOSid);

                foreach (var item in _SosRegistersrUserOperation)
                {
                    SOSRegUserOperationRelationship regAux = new();
                    regAux.Register = item;
                    regAux.StateUpdate = false;
                    regAux.Exist = true;
                    SOS_Registers_UserOperationRelationship.Add((int)item.OperationId, regAux);
                }

                foreach (var op in _All_Operations)
                {
                    if (!SOS_Registers_UserOperationRelationship.TryGetValue(op.OperationId, out var context))
                    {
                        //si no existre se crea de manera artificial
                        SOSRegUserOperationRelationship regAux = new();
                        regAux.Register = new();
                        regAux.Exist = false;
                        regAux.StateUpdate = false;
                        SOS_Registers_UserOperationRelationship.Add(op.OperationId, regAux);
                    }
                }

            }
            foreach (var reg in _SosRegisters)
            {
                foreach (var dist in _distributions)
                {
                    if (dist.Operations.Any(o => o.OperationId == reg.OperationId))
                    {
                        if (OperationsInDistributionCount.ContainsKey(dist.DistributionId))
                        {
                            OperationsInDistributionCount[dist.DistributionId].TotalSosJobs++;
                            switch (reg.JobObservation.Status)
                            {
                                case 1:
                                    OperationsInDistributionCount[dist.DistributionId].sosplanned++;
                                    break;
                                case 2:
                                    OperationsInDistributionCount[dist.DistributionId].sosinProgress++;
                                    break;
                                case 3:
                                    OperationsInDistributionCount[dist.DistributionId].soslate++;
                                    break;
                                case 4:
                                    OperationsInDistributionCount[dist.DistributionId].sosunderReview++;
                                    break;
                                case 5:
                                    OperationsInDistributionCount[dist.DistributionId].sosrejected++;
                                    break;
                                case 6:
                                    OperationsInDistributionCount[dist.DistributionId].sosfinished++;
                                    break;
                                case 7:
                                    OperationsInDistributionCount[dist.DistributionId].sosprogramed++;
                                    break;
                            }

                        }
                        else
                        {
                            SosJobCount newReg = new SosJobCount();

                            newReg.TotalJobs = dist.Operations.Count();

                            newReg.TotalSosJobs++;
                            //1 @Localizer["planned"]
                            //2 @Localizer["inProgress"]
                            //3 @Localizer["late"]
                            //4 @Localizer["underReview"]
                            //5 @Localizer["rejected"]
                            //6 @Localizer["finished"]
                            switch (reg.JobObservation.Status)
                            {
                                case 1:
                                    newReg.sosplanned++;
                                    break;
                                case 2:
                                    newReg.sosinProgress++;
                                    break;
                                case 3:
                                    newReg.soslate++;
                                    break;
                                case 4:
                                    newReg.sosunderReview++;
                                    break;
                                case 5:
                                    newReg.sosrejected++;
                                    break;
                                case 6:
                                    newReg.sosfinished++;
                                    break;

                                case 7:
                                    newReg.sosprogramed++;
                                    break;
                            }

                            OperationsInDistributionCount.Add(dist.DistributionId, newReg);
                        }
                        break;
                    }
                }

            }

            foreach (var reg in _AnotherJobs)
            {
                foreach (var dist in _distributions)
                {
                    if (dist.Operations.Any(o => o.OperationId == reg.OperationId))
                    {
                        if (OperationsInDistributionCount.ContainsKey(dist.DistributionId))
                        {
                            OperationsInDistributionCount[dist.DistributionId].TotalOutSosJobs++;
                            switch (reg.Status)
                            {
                                case 1:
                                    OperationsInDistributionCount[dist.DistributionId].planned++;
                                    break;
                                case 2:
                                    OperationsInDistributionCount[dist.DistributionId].inProgress++;
                                    break;
                                case 3:
                                    OperationsInDistributionCount[dist.DistributionId].late++;
                                    break;
                                case 4:
                                    OperationsInDistributionCount[dist.DistributionId].underReview++;
                                    break;
                                case 5:
                                    OperationsInDistributionCount[dist.DistributionId].rejected++;
                                    break;
                                case 6:
                                    OperationsInDistributionCount[dist.DistributionId].finished++;
                                    break;

                                case 7:
                                    OperationsInDistributionCount[dist.DistributionId].programed++;
                                    break;
                            }

                        }
                        else
                        {
                            SosJobCount newReg = new SosJobCount();

                            newReg.TotalJobs = dist.Operations.Count();

                            newReg.TotalOutSosJobs++;
                            //1 @Localizer["planned"]
                            //2 @Localizer["inProgress"]
                            //3 @Localizer["late"]
                            //4 @Localizer["underReview"]
                            //5 @Localizer["rejected"]
                            //6 @Localizer["finished"]
                            switch (reg.Status)
                            {
                                case 1:
                                    newReg.planned++;
                                    break;
                                case 2:
                                    newReg.inProgress++;
                                    break;
                                case 3:
                                    newReg.late++;
                                    break;
                                case 4:
                                    newReg.underReview++;
                                    break;
                                case 5:
                                    newReg.rejected++;
                                    break;
                                case 6:
                                    newReg.finished++;
                                    break;

                                case 7:
                                    newReg.programed++;
                                    break;
                            }

                            OperationsInDistributionCount.Add(dist.DistributionId, newReg);
                        }
                        break;
                    }
                }

            }

            //_Subordinates = await UsersServices.GetSubordinates(_sos_plan.Supervisor.UserId);

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
                    _NewJobObservation.OperationId = OperationId;
                    _NewJobObservation.DistributionId = DistributionId;

                    _NewJobObservation.SupervisorId = (int)context.Register.SupervisorId;

                    _NewJobObservation.Option = 2;
                    _NewJobObservation.Type = 3;
                    _NewJobObservation.Status = 7;
                    _NewJobObservation.SectionIds = jobCategoryStructureIds;
                    _NewJobObservation.IsActive = true;


                    DateTime parsedDate = new DateTime((int)_sos_plan.AplicationYear, month, 1);
                    parsedDate = await FindNextAvailableDate(parsedDate, false);
                    _NewJobObservation.StartDate = parsedDate;




                    var result = await SOSServices.CreateSOSRegister(_sos_plan.SOSid, month, (int)_sos_plan.AplicationYear, _NewJobObservation);
                    if (result != null)
                    {
                        disableBtnCreateSos = false;

                        ShowTable = false;
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

                    Console.WriteLine($"La clave '{OperationId}' no existe en el diccionario.");

                    //msg de crear Supervisor
                }

            }
            else
            {
                bool? result = await DialogService.ShowMessageBox(
                "Atencion",
                (MarkupString)$"Antes de proceder con la creación de Un Job Observation, por favor define un supervisor responsable.",
                yesText: "Entendio!");

                Console.WriteLine($"La clave '{OperationId}' no existe en el diccionario.");

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
                    _NewJobObservation.OperationId = OperationId;
                    _NewJobObservation.DistributionId = DistributionId;

                    _NewJobObservation.SupervisorId = (int)context.Register.SupervisorId;

                    _NewJobObservation.Option = 2;
                    _NewJobObservation.Type = 3;
                    _NewJobObservation.Status = 7;
                    _NewJobObservation.SectionIds = jobCategoryStructureIds;
                    _NewJobObservation.IsActive = true;


                    DateTime parsedDate = new DateTime((int)_sos_plan.AplicationYear, _yearMonth.Value.Month, day);
                    parsedDate = await FindNextAvailableDate(parsedDate, false);
                    _NewJobObservation.StartDate = parsedDate;




                    var result = await SOSServices.CreateSOSRegister(_sos_plan.SOSid, parsedDate.Month, (int)_sos_plan.AplicationYear, _NewJobObservation);
                    if (result != null)
                    {
                        disableBtnCreateSos = false;

                        ShowTable = false;
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

                    Console.WriteLine($"La clave '{OperationId}' no existe en el diccionario.");

                    //msg de crear Supervisor
                }

            }
            else
            {
                bool? result = await DialogService.ShowMessageBox(
                "Atencion",
                (MarkupString)$"Antes de proceder con la creación de Un Job Observation, por favor define un supervisor responsable.",
                yesText: "Entendio!");

                Console.WriteLine($"La clave '{OperationId}' no existe en el diccionario.");

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
                _NewJobObservation.OperationId = 0;
            }

            StateHasChanged();
        }

        private bool CcpDialog = false;
        private bool HoeDialog = false;
        private bool GosDialog = false;
        private bool searchAssychart = false;

        private CDMS_CCP_Archives? CcpFilesInFolder;
        private CDMS_HOE_Archives? HoeFilesInFolder;
        private CDMS_GOS_Archives? GosFilesInFolder;

        private bool folderError = false;
        private string messageErrorFolders;


        private string HOErute = "";
        private string CCPrute = "";
        private string GOSrute = "";


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

        private bool visible = false;
        private int jobId;
        private void OpenDialog2(int id)
        {
            jobId = id;
            visible = true;
        }
        void Close() => visible = false;

        void JobObservationUpdate(int jobObservationId)
        {
            NavigationManager.NavigateTo($"jobobservation/updatejobobservation/{jobObservationId}");
        }


        //Programmed Job observation Modal (SOS)
        private bool visible2 = false;
        private int jobId2;

        private void OpenDialog3(int id)
        {
            jobId2 = id;
            visible2 = true;

        }

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
                    Console.WriteLine(newDate1);
                }
                else
                    Console.WriteLine("Unable to parse {es-ES} '{0}'", hour1);

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
            await PrepareDataTable();
            visible2 = newValue;
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

            SuggestionMode = MonthlyView = false;

            if (tmpdist != null)
            {
                tmpdist.ShowDetails = true;
            }
            if (tmpSuggdist != null)
            {
                tmpSuggdist.distribution.ShowDetails = true;
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
                tmpdist.ShowDetails = true;
            }
            if (tmpSuggdist != null)
            {
                tmpSuggdist.distribution.ShowDetails = true;
            }

            StateHasChanged();
        }
        private void ShowBtnPress(int nr)
        {
            Distribution? tmpdist = _distributions.FirstOrDefault(f => f.DistributionId == nr);

            if (tmpdist != null)
            {
                Distribution? openDist = _distributions.Find(dist => dist.ShowDetails && dist.DistributionId != nr);

                if(openDist != null)
                {
                    openDist.ShowDetails = false;
                }

                tmpdist.ShowDetails = !tmpdist.ShowDetails;

                // Actualizar el estado
                StateHasChanged();
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

                        var result = await SOSServices.CreateSOSRegUserOperation(_sos_plan.SOSid, context.Register.Supervisor.UserId, op);
                        if (result != null)
                        {
                            ShowTable = false;
                            StateHasChanged();
                            context.Register = result;
                            context.Register.Supervisor = _Users.Find(u => u.UserId == context.Register.SupervisorId);
                            context.Exist = true;
                            context.StateUpdate = false;
                            await PrepareDataTable();

                            Snackbar.Clear();
                            Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                            Snackbar.Add($"SOS Supervisor Assigned", Severity.Info);


                            StateHasChanged();
                        }






                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error In Create SOSRegUserOperationRelationship Function: {ex.Message} ");

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

                                var option = await DialogService.ShowMessageBox(
                                $"{Localizer1["SOS_Title_AssignSV"]}",
                                (MarkupString)$"{Localizer1["SOS_Body_AssignSV"]}",
                                yesText: $"{Localizer1["SOS_Yes_AssignSV"]}!", noText: $"{Localizer1["SOS_No_AssignSV"]} SSV", cancelText: $"{Localizer1["SOS_Null_AssignSV"]}!");

                                Console.WriteLine($"Option : {option}");
                                //mensaje de opocion
                                switch (option)
                                {
                                    case true:
                                        //Todos los reguistros Unicamente en la misma distribucion
                                        var result1 = await SOSServices.UpdateSOSRegUserOperation(context.Register, 1);
                                        if (result1 != null)
                                        {
                                            ShowTable = false;
                                            StateHasChanged();
                                            context.Register = result1;
                                            context.Register.Supervisor = _Users.ToList().Find(u => u.UserId == context.Register.SupervisorId);
                                            context.Exist = true;
                                            context.StateUpdate = false;
                                            await PrepareDataTable();

                                            Snackbar.Clear();
                                            Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                                            Snackbar.Add($"All Distribution Supervisor Assigned Changed", Severity.Info);


                                            StateHasChanged();
                                        }
                                        break;

                                    case false:
                                        //Todos los reguistros en la SOS
                                        var result2 = await SOSServices.UpdateSOSRegUserOperation(context.Register, 2);
                                        if (result2 != null)
                                        {
                                            ShowTable = false;
                                            StateHasChanged();
                                            context.Register = result2;
                                            context.Register.Supervisor = _Users.ToList().Find(u => u.UserId == context.Register.SupervisorId);
                                            context.Exist = true;
                                            context.StateUpdate = false;
                                            await PrepareDataTable();

                                            Snackbar.Clear();
                                            Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                                            Snackbar.Add($"All SOS Supervisor Assigned Changed", Severity.Info);


                                            StateHasChanged();
                                        }
                                        break;

                                    case null:
                                        //Todos los reguistros en la operacion ROW
                                        var result3 = await SOSServices.UpdateSOSRegUserOperation(context.Register, 3);
                                        if (result3 != null)
                                        {
                                            ShowTable = false;
                                            StateHasChanged();
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
                                    Console.WriteLine($"First Time: Create SOS_Registers_UserOperationRelationship");
                                    _SosRegistersrUserOperation = await SOSServices.GetSOSRegUserOperation(_sos_plan.SOSid);

                                    foreach (var item in _SosRegistersrUserOperation)
                                    {
                                        SOSRegUserOperationRelationship regAux = new();
                                        regAux.Register = item;
                                        regAux.StateUpdate = false;
                                        regAux.Exist = true;
                                        SOS_Registers_UserOperationRelationship.Add((int)item.OperationId, regAux);
                                    }

                                    foreach (var itemOp in _All_Operations)
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
                            catch (Exception ex)
                            {
                                Console.WriteLine($"Error In assign Supervisor ID: {ex.Message} ");

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
            //Distribution tmpPerson = _distributions.First(f => f.DistributionId == nr);
            //tmpPerson.ShowDetails = !tmpPerson.ShowDetails;
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

        private async Task<DateTime> FindNextAvailableDate(DateTime startAvailabeDate, bool isSuggest, int id_SV = 0)
        {
            int yearToCheck = startAvailabeDate.Year;
            int initialDayOfYear = startAvailabeDate.DayOfYear;

            while (startAvailabeDate.Year == yearToCheck || startAvailabeDate.DayOfYear < initialDayOfYear)
            {
                if (isSuggest)
                {
                    if (!(await IsDateSuggestAlreadyUsed(startAvailabeDate, id_SV)) && !(await IsWeekend(startAvailabeDate)))
                    {
                        return startAvailabeDate;
                    }
                }
                else
                {
                    if (!(await IsDateAlreadyUsed(startAvailabeDate)) && !(await IsWeekend(startAvailabeDate)))
                    {
                        return startAvailabeDate;
                    }
                }

                startAvailabeDate = startAvailabeDate.AddDays(diasSeparate);

                if (startAvailabeDate > DateTime.MaxValue)
                {
                    throw new InvalidOperationException("No se pudo encontrar una fecha disponible en el año actual.");
                }
            }



            return startAvailabeDate.AddDays(-1);
        }

        private async Task<bool> IsDateAlreadyUsed(DateTime dateToCheck)
        {
            int count = _All_SOSJobobservation.Count(o => o.PlannedStartDate == dateToCheck.Date);
            return count > 1;
        }

        private async Task<bool> IsDateSuggestAlreadyUsed(DateTime dateToCheck, int id_SV)
        {
            if (id_SV != 0)
            {
                int count = _All_Suggested_SOSJobobservation.Count(o => o.PlannedStartDate == dateToCheck.Date && o.SupervisorId == id_SV);
                return count >= JobsPorDia;
            }

            return true;

        }

        private async Task<bool> IsWeekend(DateTime dateToCheck)
        {
            return dateToCheck.DayOfWeek == DayOfWeek.Saturday || dateToCheck.DayOfWeek == DayOfWeek.Sunday;
        }

        bool enableCreateSuggestion = false;
        bool SuggestionMode = false;

        private async Task CreateSuggestion()
        {
            //notificacion de es necesaria la distribucion 
            if (Startday.Date == DateTime.Now.AddDays(-1).Date && Startday.Year < _sos_plan.AplicationYear)
            {
                bool? resultDay = await DialogService.ShowMessageBox(
                "Warning",
                "Select Day To Start!",
                yesText: "OK!");
                StateHasChanged();
            }


            if (!Dist_Manager.Any(i => i.isSelected))
            {
                bool? resultDist = await DialogService.ShowMessageBox(
                "Warning",
                "Select Distribution First!",
                yesText: "OK!");
                StateHasChanged();
            }
            if (SV_Manager.Count == 0)
            {
                bool? resultSv = await DialogService.ShowMessageBox(
                            "Warning",
                            "SV is Neccesary!",
                            yesText: "OK!");
                StateHasChanged();
            }


            if (Dist_Manager.Any(i => i.isSelected) && SV_Manager.Count() != 0 && Startday.Date != DateTime.Now.AddDays(-1).Date)
            {

                enableCreateSuggestion = true;
                base.StateHasChanged();
                _yearMonth = Startday;
                //bool view dialog reorder userds
                SVSinCharge = false;
                isButtonDisabled = true;
                ShowLoading = true;
                StateHasChanged();

                //logica del año anterior
                //Supervisores encargados, seleccionar? usar todos/
                //
                Suggested_SOS_Registers_UserOperationRelationship?.Clear();
                try
                {
                    TimeSpan? startHour = new TimeSpan(00, 00, 00);

                    base.StateHasChanged();
                    //Console.WriteLine($"Toal Count {_All_Operations?.Count}");
                    Console.WriteLine($"TotalDist Count {selected_distribution.Operations?.Count}");

                    if (_All_Suggested_SOSJobobservation.Count == 0)
                    {
                        foreach (var item in Dist_Manager)
                        {

                            if (item.isSelected)
                            {
                                foreach (var op in item.distribution.Operations)
                                {
                                    //check if exist
                                    if (!_All_SOSJobobservation.Any(j => j.OperationId == op.OperationId))
                                    {

                                        JobObservationNulls _newSuggestion = new();

                                        int supervisorIndex = _All_Operations.IndexOf(op) % SV_Manager.Count;

                                        // Obtener el SupervisorId de SV_Manager usando el índice calculado
                                        int supervisorId = SV_Manager[supervisorIndex].UserId;

                                        // Asignar el SupervisorId a _newSuggestion
                                        _newSuggestion.SupervisorId = supervisorId;


                                        SOSRegUserOperationRelationship regAux = new();
                                        regAux.Register = new();
                                        regAux.Register.SOSReviewProgramid = _sos_plan.SOSid;
                                        regAux.Register.OperationId = op.OperationId;
                                        regAux.Register.SupervisorId = supervisorId;
                                        regAux.Register.Supervisor = SV_Manager.Find(u => u.UserId == supervisorId);


                                        regAux.Exist = false;
                                        regAux.StateUpdate = true;
                                        Suggested_SOS_Registers_UserOperationRelationship.Add(op.OperationId, regAux);


                                        _newSuggestion.PlantId = (int)_sos_plan.PlantId;
                                        _newSuggestion.Plant = _sos_plan.Plant;
                                        _newSuggestion.AreaId = (int)_sos_plan.AreaId;
                                        _newSuggestion.Area = _sos_plan.Area;

                                        _newSuggestion.Distribution = _distributions.Find(d => d.Operations.Any(o => o.OperationId == op.OperationId));
                                        _newSuggestion.DistributionId = _newSuggestion.Distribution.DistributionId;

                                        _newSuggestion.Operation = op;
                                        _newSuggestion.OperationId = op.OperationId;
                                        _newSuggestion.Option = 2;
                                        _newSuggestion.Type = 3;
                                        _newSuggestion.Status = 7;
                                        _newSuggestion.SectionIds = jobCategoryStructureIds;

                                        _newSuggestion.IsActive = true;

                                        DateTime parsedDate = Startday;
                                        parsedDate = await FindNextAvailableDate(parsedDate, true, supervisorId);

                                        _newSuggestion.StartDate = parsedDate;
                                        _newSuggestion.PlannedStartDate = parsedDate;


                                        _All_Suggested_SOSJobobservation.Add(_newSuggestion);
                                    }

                                }

                            }

                        }
                        //var OperationsInDist = _All_Operations.Where(o => o.id)


                    }

                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error CreateSuggestion: {ex.Message}");

                }
                finally
                {
                    ShowLoading = false;
                    isButtonDisabled = false;
                    base.StateHasChanged();
                }


                await PrepareSuggestDataTable();
                Console.WriteLine($" Final First Sugg Generation");
                enableCreateSuggestion = false;

            }

        }

        private async Task CreateNewSuggestion()
        {
            //notificacion de es necesaria la distribucion 
            if (Startday.Date == DateTime.Now.AddDays(-1).Date)
            {
                bool? resultDay = await DialogService.ShowMessageBox(
                "Warning",
                "Select Day To Start!",
                yesText: "OK!");
                StateHasChanged();
            }


            if (!Dist_Manager.Any(i => i.isSelected))
            {
                bool? resultDist = await DialogService.ShowMessageBox(
                "Warning",
                "Select Distribution First!",
                yesText: "OK!");
                StateHasChanged();
            }

            if (SV_Manager.Count == 0)
            {
                bool? resultSv = await DialogService.ShowMessageBox(
                            "Warning",
                            "SV is Neccesary!",
                            yesText: "OK!");
                StateHasChanged();
            }


            if (Dist_Manager.Any(i => i.isSelected) && SV_Manager.Count() != 0 && Startday.Date != DateTime.Now.AddDays(-1).Date)
            {

                isButtonDisabled = true;
                ShowLoading = true;
                ShowTable = false;
                base.StateHasChanged();
                StateHasChanged();

                _All_Suggested_SOSJobobservation?.Clear();
                Suggested_SOS_Registers_UserOperationRelationship?.Clear();

                var OperationIterator = selected_distribution.Operations;

                Random random = new Random();

                switch (OptionRandom)
                {
                    case 0:
                        _All_Operations?.Clear();
                        foreach (var item in Dist_Manager.Where(item => item.isSelected).Select(item => item.distribution))
                        {
                            _All_Operations.AddRange(item.Operations);
                        }
                        break;
                    case 1:
                        _All_Operations?.Clear();
                        foreach (var item in Dist_Manager.Where(item => item.isSelected).Select(item => item.distribution))
                        {
                            _All_Operations.AddRange(item.Operations);
                        }
                        _All_Operations = _All_Operations.OrderBy(x => random.Next()).ToList();
                        //OperationIterator = OperationIterator.OrderBy(x => random.Next()).ToList();

                        break;
                    case 2:
                        var _distributionsRandom = Dist_Manager.Where(item => item.isSelected).Select(item => item.distribution).OrderBy(x => random.Next()).ToList();
                        _All_Operations?.Clear();
                        foreach (var item in _distributionsRandom)
                        {
                            _All_Operations.AddRange(item.Operations);
                        }
                        break;
                }

                TimeSpan? startHour = new TimeSpan(00, 00, 00);

                try
                {
                    base.StateHasChanged();
                    //Console.WriteLine($"Toal Count {_All_Operations?.Count}");
                    Console.WriteLine($"Total Dist Count {OperationIterator?.Count}");

                    if (_All_Suggested_SOSJobobservation.Count == 0)
                    {
                        //foreach (var op in _All_Operations)
                        foreach (var op in _All_Operations)
                        {
                            if (!_All_SOSJobobservation.Any(j => j.OperationId == op.OperationId))
                            {

                                JobObservationNulls _newSuggestion = new();

                                int supervisorIndex = _All_Operations.IndexOf(op) % SV_Manager.Count;

                                // Obtener el SupervisorId de SV_Manager usando el índice calculado
                                int supervisorId = SV_Manager[supervisorIndex].UserId;

                                // Asignar el SupervisorId a _newSuggestion
                                _newSuggestion.SupervisorId = supervisorId;


                                SOSRegUserOperationRelationship regAux = new();
                                regAux.Register = new();
                                regAux.Register.SOSReviewProgramid = _sos_plan.SOSid;
                                regAux.Register.OperationId = op.OperationId;
                                regAux.Register.SupervisorId = supervisorId;
                                regAux.Register.Supervisor = SV_Manager.Find(u => u.UserId == supervisorId);


                                regAux.Exist = false;
                                regAux.StateUpdate = true;
                                Suggested_SOS_Registers_UserOperationRelationship.Add(op.OperationId, regAux);



                                _newSuggestion.PlantId = (int)_sos_plan.PlantId;
                                _newSuggestion.Plant = _sos_plan.Plant;
                                _newSuggestion.AreaId = (int)_sos_plan.AreaId;
                                _newSuggestion.Area = _sos_plan.Area;

                                _newSuggestion.Distribution = _distributions.Find(d => d.Operations.Any(o => o.OperationId == op.OperationId));
                                _newSuggestion.DistributionId = _newSuggestion.Distribution.DistributionId;

                                _newSuggestion.Operation = op;
                                _newSuggestion.OperationId = op.OperationId;


                                _newSuggestion.Option = 2;
                                _newSuggestion.Type = 3;
                                _newSuggestion.Status = 7;
                                _newSuggestion.SectionIds = jobCategoryStructureIds;
                                _newSuggestion.IsActive = true;

                                DateTime parsedDate = Startday;
                                parsedDate = await FindNextAvailableDate(parsedDate, true, supervisorId);

                                _newSuggestion.StartDate = parsedDate;
                                _newSuggestion.PlannedStartDate = parsedDate;



                                _All_Suggested_SOSJobobservation.Add(_newSuggestion);
                            }
                        }
                    }

                    await PrepareSuggestDataTable();

                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error CreateSuggestion: {ex.Message}");

                }
                finally
                {

                    ShowLoading = false;
                    ShowTable = true;
                    isButtonDisabled = false;
                    base.StateHasChanged();
                }

                Console.WriteLine($" Final New Sugg Generation");
                bool? resultDay = await DialogService.ShowMessageBox(
               "Info!",
               "New suggestion created!",
               yesText: "OK!");
                StateHasChanged();
            }
            ShowLoading = false;
            ShowTable = true;
            isButtonDisabled = false;
            base.StateHasChanged();

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
                var SearchJob = _All_Suggested_SOSJobobservation.Find(j => j.OperationId == op);
                SearchJob.SupervisorId = context.Register.Supervisor.UserId;

                Console.WriteLine($"Update SV");
            }
        }

        private async Task<AsyncVoidMethodBuilder> ApplySuggest()
        {
            Distribution? tmpdist = _distributions?.Find(d => d.ShowDetails == true);
            DistSelect? tmpSuggdist = null;

            if (tmpdist == null)
            {
                Dist_Manager.ForEach(d => d.distribution.ShowDetails = false);
            }
            else
            {
                _distributions.ForEach(d => d.ShowDetails = false);
            }

            isButtonDisabled = true;
            ShowLoading = true;
            base.StateHasChanged();
            StateHasChanged();


            var result = await SOSServices.ApplyMassiveSuggest(_sos_plan.SOSid, _All_Suggested_SOSJobobservation, Dist_Manager.Where(item => item.isSelected).ToList());
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
            Console.Write(CheckItem.distribution.Code);
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

        void CloseSuggestSVPanelDialog() => visibleSuggestSVDialog = false;


    }//end sos class


}