using MudBlazor;
using SupervisorMobility.Client.Data.Entities.SOS_Process;
using SupervisorMobility.Client.Pages.Inicio.HCIPage.Components;
using SupervisorMobility.Client.Services.HCIService;

namespace SupervisorMobility.Client.Pages.SOSHOE.SOSHOECollection.GeneralComponents
{
    public partial class GenerateComponent
    {
        [Inject]
        private IDialogService DialogService { get; set; }

        [CascadingParameter] MudDialogInstance MudDialog { get; set; }
        [Parameter]
        public int selectedIndexPageGenerate { get; set; }
        [Parameter]
        public SOSHub _sosHub { get; set; }
        [Parameter]
        public User user { get; set; }
        [Parameter]
        public int SOSHubId { get; set; }

        bool Dev_env { get; set; }

        public PAT _pat { get; set; } = new();
        public int ssvId { get; set; }

        public SOSAnalysis _sosAnalysis { get; set; } = new SOSAnalysis();
        public SOSDistribution _sosDistribution { get; set; } = new SOSDistribution();
        public SOSFlow _sosFlow { get; set; } = new SOSFlow();
        public SOSSequence _sosSequence { get; set; } = new SOSSequence();
        public SOSCombination _sosCombination { get; set; } = new SOSCombination();
        public HCI _hci { get; set; } = new HCI();
        private List<User> _Users;
        private User _SelectUser;


        public SOSSynopticTableofOperatingRequirements _SOSSynopticRequirements { get; set; } = new SOSSynopticTableofOperatingRequirements();

        public SOSSynopticTableofControlPoints _sosControlPoints { get; set; } = new SOSSynopticTableofControlPoints();

        public int ApproverAnalysisId { get; set; }
        public int ReviewerAnalysisId { get; set; }
        public int cycleId { get; set; }
        public bool ReviewerHYSCombinationExist { get; set; } = false;
        public int ReviewerHYSDocCombinationId { get; set; } = 0;
        [Parameter]
        public List<User> _supervisors { get; set; }
        public int ApproverCombinationId { get; set; } = 0;
        public int ReviewerCombinationId { get; set; } = 0;

        public int plantId { get; set; }
        public int areaId { get; set; }
        public int distributionId { get; set; }
        public int departmentId { get; set; }
        public int stationId { get; set; }

        [Parameter]
        public List<Plant> _plants { get; set; }

        [Parameter]
        public List<Area> _areas { get; set; }

        [Parameter]
        public List<Distribution> _distributions { get; set; }

        [Parameter]
        public List<Department> _departments { get; set; }

        [Parameter]
        public List<Station> _stations { get; set; }

        int ApproverDistributionId = 0;
        int ReviewerDistributionId = 0;

        int ApproverFlowId = 0;
        int ReviewerFlowId = 0;
        int ReviewerHYSDocFlowId = 0;
        bool ReviewerHYSFlowExist = false;

        int ApproverSequenceId = 0;
        int ReviewerSequenceId = 0;

        int OwnerSynopticRequirementsId = 0;
        int ApproverSynopticRequirementsId = 0;
        int ReviewerSynopticRequirementsId = 0;


        int OwnerControlPointsId = 0;
        int ApproverControlPointsId = 0;
        int ReviewerControlPointsId = 0;

        public int supervisorOwnerId { get; set; } = 0;
        public int supervisorEditorId { get; set; } = 0;
        public int OperatorTurn1 { get; set; } = 0;
        public int OperatorTurn2 { get; set; } = 0;
        public int OperatorTurn3 { get; set; } = 0;
        public int SupervisorTurn1 { get; set; } = 0;
        public int SupervisorTurn2 { get; set; } = 0;
        public int SupervisorTurn3 { get; set; } = 0;

        public Dictionary<int, List<User>> _operators { get; set; } = new();

        private DialogOptions dialogPagesOptions = new() { CloseOnEscapeKey = true, MaxWidth = MaxWidth.Large, FullWidth = true };

        public int loading;
        public bool loaded = false;

        protected async override Task OnInitializedAsync()
        {
            if (_plants == null || !_plants.Any())
            {
                _plants = await PlantServices.GetPlants();
                _plants = _plants.OrderBy(p => p.Description).ToList();
            }

            loading += 10;

            if (_departments == null || !_plants.Any())
            {
                _departments = await DepartmentServices.GetDepartments();
                _departments = _departments.OrderBy(d => d.Description).ToList();
            }

            loading += 10;

            if (_stations == null || !_stations.Any())
            {
                _stations = await StationServices.GetStations();
                _stations = _stations.OrderBy(s => s.Description).ToList();
            }

            loading += 10;

            if (_sosHub == null)
            {
                _sosHub = await SOSHubServices.GetSOSHub(SOSHubId, true, true, true, true, true, true, true, true, includeModel: true, includePeople: true, includeDocuments: true, includeCollections: true, includePats: true);
            }

            loading += 10;

            if (_sosHub.PlantId != null)
            {
                plantId = (int)_sosHub.PlantId;
                areaId = _sosHub.AreaId ?? areaId;
                distributionId = _sosHub.DistributionId ?? distributionId;
            }

            loading += 10;

            if (_areas == null || !_areas.Any())
            {
                switch (user.UserType)
                {
                    case 1:
                    case 3:
                        if (plantId != new int())
                        {
                            _areas = await AreaServices.GetAreas(plantId);
                            _areas = _areas.OrderBy(a => a.Description).ToList();
                        }
                        break;
                    case 2:
                        _areas = user.Areas.ToList();
                        break;
                }
            }

            loading += 10;

            if (_supervisors == null || !_supervisors.Any())
            {
                _supervisors = new List<User>();
                switch (user.UserType)
                {
                    case 1:
                        _supervisors = await UsersService.GetUsersByUserTypeInPlant(plantId, 3, false, false);
                        _supervisors = _supervisors.OrderBy(s => s.Name).ToList();
                        break;
                    case 2:
                        foreach (var sv in user.Subordinates.ToList())
                        {
                            _supervisors?.Add(sv);
                        }
                        break;
                    case 3:
                        _supervisors?.Add(user);
                        break;
                }
            }

            loading += 10;

            if (_distributions == null || !_distributions.Any())
            {
                if (plantId != 0 && areaId != 0)
                {
                    _distributions = await DistributionServices.GetDistributionsWithCollections(plantId, areaId);
                    _distributions = _distributions.OrderBy(d => d.Description).ToList();
                }
            }

            loading += 10;

            stationId = _sosHub.StationId ?? stationId;
            departmentId = _sosHub.DepartmentId ?? departmentId;


            cycleId = _sosHub.TrainingTime ?? 0;

            loading += 10;

            switch (selectedIndexPageGenerate)
            {
                case 0:
                    if (_sosHub.PATs.Count > 0 && _sosHub.PATs?.Last().Status != 6)
                    {
                        _pat = _sosHub.PATs.Last();
                        //_pat.PlantId = (int)_sosHub.PlantId;
                        //_pat.AreaId = (int)_sosHub.AreaId;

                        _pat.Plant = _plants.Find(p => p.PlantId == _pat.PlantId);
                        _pat.Area = _areas.Find(a => a.AreaId == _pat.AreaId);

                    }
                    else
                    {

                        _pat = new PAT();
                        _pat.PlantId = (int)_sosHub.PlantId;
                        _pat.AreaId = (int)_sosHub.AreaId;

                        _pat.Plant = _plants.Find(p => p.PlantId == _sosHub.PlantId);
                        _pat.Area = _areas.Find(a => a.AreaId == _sosHub.AreaId);
                    }
                    loading += 10;

                    break;
                case 1:
                    if (_sosHub.SOSAnalysis.Count > 0)
                    {
                        _sosAnalysis = _sosHub.SOSAnalysis.FirstOrDefault();
                        if (_sosAnalysis.SOSAnalysisId != 0 && _sosAnalysis.AnalysisLogbooks.Count > 0)
                        {
                            ApproverAnalysisId = (int)(_sosAnalysis.AnalysisLogbooks.Last().Status != 2 ? _sosAnalysis.AnalysisLogbooks.Last().ApproverId : 0);
                            ReviewerAnalysisId = (int)(_sosAnalysis.AnalysisLogbooks.Last().Status != 2 ? _sosAnalysis.AnalysisLogbooks.Last().ReviewerId : 0);
                        }

                        if (_sosAnalysis.AnalysisLogbooks.Count == 0)
                        {
                            _sosAnalysis.AnalysisLogbooks.Add(new SOSAnalysisLogbook());
                        }
                    }
                    loading += 10;
                    break;
                case 2:

                    if (_sosHub.SOSCombination.Count > 0)
                    {
                        _sosCombination = _sosHub.SOSCombination.FirstOrDefault() ?? new SOSCombination();
                        if (_sosCombination.SOSCombinationId != 0 && _sosCombination.CombinationLogbooks.Count > 0)
                        {
                            //reviewer = SV = Editor  //Approver = SSV = owner 

                            //ApproverDocCombinationId = (int)(_sosCombination.ApproverId ?? 0);
                            //ReviewerDocCombinationId = (int)(_sosCombination.ReviewerId ?? 0);
                            ReviewerHYSDocCombinationId = (int)(_sosCombination.ReviewerHSId ?? 0);

                            ApproverCombinationId = (int)(_sosCombination.CombinationLogbooks.Last().Status != 2 ? _sosCombination.CombinationLogbooks.Last().ApproverId : 0);
                            ReviewerCombinationId = (int)(_sosCombination.CombinationLogbooks.Last().Status != 2 ? _sosCombination.CombinationLogbooks.Last().ReviewerId : 0);
                        }

                        if (_sosCombination.CombinationLogbooks.Count == 0)
                        {
                            _sosCombination.CombinationLogbooks.Add(new SOSCombinationLogbook());
                        }

                        loading += 5;

                        if (_sosCombination.Turns?.Count >= 1)
                        {
                            OperatorTurn1 = (int)(_sosCombination.Turns.ElementAt(0).OperatorId ?? 0);
                            SupervisorTurn1 = (int)(_sosCombination.Turns.ElementAt(0).SupervisorId ?? 0);
                        }

                        if (_sosCombination.Turns?.Count >= 2)
                        {
                            OperatorTurn2 = (int)(_sosCombination.Turns.ElementAt(1).OperatorId ?? 0);
                            SupervisorTurn2 = (int)(_sosCombination.Turns.ElementAt(1).SupervisorId ?? 0);
                        }

                        if (_sosCombination.Turns?.Count >= 3)
                        {
                            OperatorTurn3 = (int)(_sosCombination.Turns.ElementAt(2).OperatorId ?? 0);
                            SupervisorTurn3 = (int)(_sosCombination.Turns.ElementAt(2).SupervisorId ?? 0);
                        }

                        loading += 5;
                    }
                    else { loading += 10; }
                    break;

                case 3:
                    if (_sosHub.SOSDistribution.Count > 0)
                    {
                        _sosDistribution = _sosHub.SOSDistribution.FirstOrDefault();
                        if (_sosDistribution.SOSDistributionId != 0 && _sosDistribution.DistributionLogbooks.Count > 0)
                        {
                            //reviewer = SV = Editor  //Approver = SSV = owner 
                            //ApproverDocDistributionId = (int)(_sosDistribution.ApproverId ?? 0);
                            //reviewer = SV = Editor
                            //ReviewerDocDistributionId = (int)(_sosDistribution.ReviewerId ?? 0);

                            ApproverDistributionId = (int)(_sosDistribution.DistributionLogbooks.Last().Status != 2 ? _sosDistribution.DistributionLogbooks.Last().ApproverId : 0);
                            ReviewerDistributionId = (int)(_sosDistribution.DistributionLogbooks.Last().Status != 2 ? _sosDistribution.DistributionLogbooks.Last().ReviewerId : 0);
                        }

                        if (_sosDistribution.DistributionLogbooks.Count == 0)
                        {
                            _sosDistribution.DistributionLogbooks.Add(new SOSDistributionLogbook());
                        }

                        loading += 3;

                        if (_sosDistribution.Turns?.Count >= 1)
                        {
                            OperatorTurn1 = (int)(_sosDistribution.Turns.ElementAt(0).OperatorId ?? 0);
                            SupervisorTurn1 = (int)(_sosDistribution.Turns.ElementAt(0).SupervisorId ?? 0);
                        }

                        if (_sosDistribution.Turns?.Count >= 2)
                        {
                            OperatorTurn2 = (int)(_sosDistribution.Turns.ElementAt(1).OperatorId ?? 0);
                            SupervisorTurn2 = (int)(_sosDistribution.Turns.ElementAt(1).SupervisorId ?? 0);
                        }

                        if (_sosDistribution.Turns?.Count >= 3)
                        {
                            OperatorTurn3 = (int)(_sosDistribution.Turns.ElementAt(2).OperatorId ?? 0);
                            SupervisorTurn3 = (int)(_sosDistribution.Turns.ElementAt(2).SupervisorId ?? 0);
                        }

                        loading += 2;

                        AvailableAnalyses = await SOSAnalysisServices.GetAllSOSAnalysisByDistribution((int)_sosHub.DistributionId);
                        Console.WriteLine($"Analisis: {AvailableAnalyses.Count()}");
                        AvailableSequences = await SOSSequenceServices.GetAllSOSSequenceByDistribution((int)_sosHub.DistributionId);
                        Console.WriteLine($"Sequencias: {AvailableSequences.Count()}");
                        //lo redirigimos a la vista dado que ya hay datos
                        selectedIndexPageGenerate = 33;

                        loading += 5;
                    }
                    else
                    {
                        //preparamos los datos
                        AvailableSoshubs = await SOSHubServices.GetAllSOSHub();
                        AvailableAnalyses = await SOSAnalysisServices.GetAllSOSAnalysisByDistribution((int)(_sosHub.DistributionId ?? 0));
                        AvailableSequences = await SOSSequenceServices.GetAllSOSSequenceByDistribution((int)(_sosHub.DistributionId ?? 0));

                        loading += 10;
                    }
                    break;
                case 4:
                    if (_sosHub.SOSFlow.Count > 0)
                    {
                        _sosFlow = _sosHub.SOSFlow.FirstOrDefault();
                        if (_sosFlow.SOSFlowId != 0 && _sosFlow.FlowLogbooks.Count > 0)
                        {
                            ApproverFlowId = (int)(_sosFlow.FlowLogbooks.Last().Status != 2 ? _sosFlow.FlowLogbooks.Last().ApproverId : 0);
                            ReviewerFlowId = (int)(_sosFlow.FlowLogbooks.Last().Status != 2 ? _sosFlow.FlowLogbooks.Last().ReviewerId : 0);
                        }

                        if (_sosFlow.FlowLogbooks.Count == 0)
                        {
                            _sosFlow.FlowLogbooks.Add(new SOSFlowLogbook());
                        }
                    }
                    loading += 10;
                    break;
                case 5:
                    if (_sosHub.SOSSequence.Count > 0)
                    {
                        _sosSequence = _sosHub.SOSSequence.FirstOrDefault();
                        if (_sosSequence.SOSSequenceId != 0 && _sosSequence.SequenceLogbooks.Count > 0)
                        {
                            ApproverSequenceId = (int)(_sosSequence.SequenceLogbooks.Last().Status != 2 ? _sosSequence.SequenceLogbooks.Last().ApproverId : 0);
                            ReviewerSequenceId = (int)(_sosSequence.SequenceLogbooks.Last().Status != 2 ? _sosSequence.SequenceLogbooks.Last().ReviewerId : 0);
                        }

                        if (_sosSequence.SequenceLogbooks.Count == 0)
                        {
                            _sosSequence.SequenceLogbooks.Add(new SOSSequenceLogbook());
                        }
                    }
                    loading += 10;
                    break;
                case 6:
                    if (_sosHub.SOSSynopticOperatingRequirements.Count > 0)
                    {
                        _SOSSynopticRequirements = _sosHub.SOSSynopticOperatingRequirements.FirstOrDefault();
                        if (_SOSSynopticRequirements.SOSSynopticTableofOperatingRequirementsId != 0 && _SOSSynopticRequirements.SynopticRequirementsLogbooks.Count > 0)
                        {
                            ApproverDistributionId = (int)(_SOSSynopticRequirements.SynopticRequirementsLogbooks.Last().Status != 2 ? _SOSSynopticRequirements.SynopticRequirementsLogbooks.Last().ApproverId : 0);
                            ReviewerDistributionId = (int)(_SOSSynopticRequirements.SynopticRequirementsLogbooks.Last().Status != 2 ? _SOSSynopticRequirements.SynopticRequirementsLogbooks.Last().ReviewerId : 0);
                        }

                        if (_SOSSynopticRequirements.SynopticRequirementsLogbooks.Count == 0)
                        {
                            _SOSSynopticRequirements.SynopticRequirementsLogbooks.Add(new SOSSynopticRequirementsLogbook());
                        }

                        loading += 3;



                        loading += 2;

                        AvailableSoshubs = await SOSHubServices.GetAllSOSHub();
                        AvailableSoshubs = AvailableSoshubs.Where(s => s.DistributionId == _sosHub.DistributionId).ToList();
                        AvailableSoshubs.RemoveAll(s => s.SOSHubId == _sosHub.SOSHubId);

                        AvailableAnalyses = await SOSAnalysisServices.GetAllSOSAnalysisByDistribution((int)_sosHub.DistributionId);
                        Console.WriteLine($"Analisis: {AvailableAnalyses.Count()}");
                        AvailableSequences = await SOSSequenceServices.GetAllSOSSequenceByDistribution((int)_sosHub.DistributionId);
                        Console.WriteLine($"Sequencias: {AvailableSequences.Count()}");
                        //lo redirigimos a la vista dado que ya hay datos
                        selectedIndexPageGenerate = 66;

                        loading += 5;
                    }
                    else
                    {
                        //preparamos los datos
                        var FilterSOSHubs = (await SOSHubServices.GetAllSOSHub(includeSOSDistribution: true)).Where(s => s.DistributionId == _sosHub.DistributionId && s.SOSHubId != _sosHub.SOSHubId).ToList();
                        AvailableSoshubs = CleanSOSHubs(FilterSOSHubs);
                        AvailableAnalyses = await SOSAnalysisServices.GetAllSOSAnalysisByDistribution((int)(_sosHub.DistributionId ?? 0));
                        Console.WriteLine($"Analisis: {AvailableAnalyses.Count()}");
                        AvailableSequences = await SOSSequenceServices.GetAllSOSSequenceByDistribution((int)(_sosHub.DistributionId ?? 0));
                        Console.WriteLine($"Sequencias: {AvailableSequences.Count()}");

                        _SOSSynopticRequirements.CreatedAt = DateTime.Now;

                        loading += 10;
                    }
                    break;
                case 7:
                    if (_sosHub.SOSSynopticControlPoints.Count > 0)
                    {
                        _sosControlPoints = _sosHub.SOSSynopticControlPoints.FirstOrDefault();
                        if (_sosControlPoints.SOSSynopticTableofControlPointsId != 0 && _sosControlPoints.SynopticPointsLogbooks.Count > 0)
                        {
                            ApproverDistributionId = (int)(_sosControlPoints.SynopticPointsLogbooks.Last().Status != 2 ? _sosControlPoints.SynopticPointsLogbooks.Last().ApproverId : 0);
                        }

                        if (_sosControlPoints.SynopticPointsLogbooks.Count == 0)
                        {
                            _sosControlPoints.SynopticPointsLogbooks.Add(new SOSSynopticPointsLogbook());
                        }

                        loading += 3;



                        loading += 2;

                        AvailableAnalyses = await SOSAnalysisServices.GetAllSOSAnalysisByDistribution((int)_sosHub.DistributionId);
                        Console.WriteLine($"Analisis: {AvailableAnalyses.Count()}");
                        AvailableSequences = await SOSSequenceServices.GetAllSOSSequenceByDistribution((int)_sosHub.DistributionId);
                        Console.WriteLine($"Sequencias: {AvailableSequences.Count()}");
                        //lo redirigimos a la vista dado que ya hay datos
                        selectedIndexPageGenerate = 66;

                        loading += 5;
                    }
                    else
                    {
                        //preparamos los datos
                        AvailableAnalyses = await SOSAnalysisServices.GetAllSOSAnalysisByDistribution((int)_sosHub.DistributionId);
                        Console.WriteLine($"Analisis: {AvailableAnalyses.Count()}");
                        AvailableSequences = await SOSSequenceServices.GetAllSOSSequenceByDistribution((int)_sosHub.DistributionId);
                        Console.WriteLine($"Sequencias: {AvailableSequences.Count()}");

                        _sosControlPoints.CreatedAt = DateTime.Now;

                        loading += 10;
                    }
                    break;

                case 9:
                    if (_sosHub.HciId != null && _sosHub.HciId != 0)
                    {
                        _hci = await HCIsServices.GetHCI((int)_sosHub.HciId);

                        loading += 3;



                        loading += 2;



                        loading += 5;
                    }
                    else
                    {
                        _Users = await HCIsServices.GetUsersWithoutHCI();


                        loading += 10;
                    }
                    break;

            }
            loaded = true;
            StateHasChanged();
        }

        public static int GetCycleId(string trainingTime)
        {
            string cycleIdString = trainingTime.Split(' ').First();

            if (int.TryParse(cycleIdString, out int cycleId))
            {
                return cycleId;
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// Cleans the SOSHubs by keeping only the matching SOSDistribution per hub.
        /// </summary>
        /// <param name="allFilterSosHubs">List of SOSHubs to clean.</param>
        private List<SOSHub> CleanSOSHubs(List<SOSHub> allFilterSosHubs)
        {
            foreach (var hub in allFilterSosHubs)
            {
                var matchedDistribution = hub.SOSDistribution?.FirstOrDefault(s => s.SOSHubId == hub.SOSHubId);
                hub.SOSDistribution = matchedDistribution != null ? new List<SOSDistribution> { matchedDistribution } : new List<SOSDistribution>();
            }

            return allFilterSosHubs.Where(h => h.SOSDistribution?.Any() == true).ToList();
        }

        async void CreatePatAsync()
        {

            Console.WriteLine(JsonSerializer.Serialize(_pat));

            if (_pat.PATid == 0)
            {

                _pat.Status = 1;
                _pat.AplicationYear = _pat.AplicationDate.Value.Year;
            }

            var result = await SOSHubServices.GeneratePat(_sosHub.SOSHubId, _pat);
            if (result != null)
            {
                Snackbar.Clear();
                Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                Snackbar.Add($"New PAT Created", Severity.Info);
                NavigationManager.NavigateTo($"/PAT/{result}");

            }
            else
            {
                Snackbar.Clear();
                Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                Snackbar.Add($"Error in Pat", Severity.Error);
            }

        }

        SOSAnalysisLogbook loganalysis { get; set; } = new SOSAnalysisLogbook();
        public async void GenerateAnalysis()
        {

            if (_sosHub.SOSAnalysis.Count > 0)
            {
                //REVISION
                if (ReviewerAnalysisId == 0 || ApproverAnalysisId == 0)
                {
                    bool? result = await DialogService.ShowMessageBox(
                       "Warning",
                       ApproverAnalysisId == 0 ? "Es necesario el aprobador" : "Es necesario seleccionar el editor (elaboro)!",
                       yesText: "Ok!");
                    var state = result == null ? "Canceled" : "Deleted!";
                    StateHasChanged();
                    return;
                }

                _sosAnalysis = _sosHub.SOSAnalysis.First();

                if (_sosAnalysis.AnalysisLogbooks.First().SOSAnalysisLogbookId == 0)
                {
                    _sosAnalysis.AnalysisLogbooks.Clear();
                }

                loganalysis.NoRevision = _sosAnalysis.AnalysisLogbooks?.Count();
                loganalysis.ApproverId = ApproverAnalysisId;
                loganalysis.ReviewerId = ReviewerAnalysisId;
                loganalysis.Date = System.DateTime.Now;
                loganalysis.Status = 1;
                loganalysis.IsActive = true;
                if (_sosAnalysis.AnalysisLogbooks == null)
                {
                    _sosAnalysis.AnalysisLogbooks = new List<SOSAnalysisLogbook>();
                }
                _sosAnalysis.AnalysisLogbooks.Add(loganalysis);

                var Gen_sosAnalysis = await SOSHubServices.GenerateAnalysis(SOSHubId, _sosAnalysis);

                Snackbar.Clear();
                Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                if (Gen_sosAnalysis != 0)
                {
                    Snackbar.Add($"{Localizer["_sosAnalysisGeneratedSucces"]}", Severity.Info);
                    NavigationManager.NavigateTo($"/soshoe/Analysis/Details/{_sosAnalysis.SOSAnalysisId}");
                    _sosAnalysis = new SOSAnalysis();
                    //Pregutar si quiere ver el analisis generado
                }
                else
                {
                    Snackbar.Add($"{Localizer["Fail_sosAnalysisGeneratedSucces"]}", Severity.Error);
                }

                StateHasChanged();

            }
            else
            {
                if (ReviewerAnalysisId == 0 || ApproverAnalysisId == 0)
                {
                    bool? result = await DialogService.ShowMessageBox(
                       "Warning",
                       string.IsNullOrEmpty(_sosAnalysis.OperationName) ? "Es necesario el aproador" : "Es necesario seleccionar el editor (elaboro)!",
                       yesText: "Ok!");
                    var state = result == null ? "Canceled" : "Deleted!";
                    StateHasChanged();
                }
                else if (string.IsNullOrEmpty(_sosAnalysis.ProcessName) || string.IsNullOrEmpty(_sosAnalysis.InternalControlNumber))
                {
                    bool? result = await DialogService.ShowMessageBox(
                      "Warning",
                      string.IsNullOrEmpty(_sosAnalysis.ProcessName) ? "Es necesario el nombre de Proceso" : "Es necesario el nombre del proceso!",
                      yesText: "Ok!");
                    var state = result == null ? "Canceled" : "Deleted!";
                    StateHasChanged();
                }
                else if (string.IsNullOrEmpty(_sosAnalysis.OperationName))
                {
                    bool? result = await DialogService.ShowMessageBox(
                      "Warning",
                       "Es necesario el nombre de operacion!",
                      yesText: "Ok!");
                    var state = result == null ? "Canceled" : "Deleted!";
                    StateHasChanged();
                }
                else
                {
                    loganalysis.NoRevision = 0;
                    loganalysis.ReviewerId = ReviewerAnalysisId;
                    loganalysis.ApproverId = ApproverAnalysisId;
                    loganalysis.Date = System.DateTime.Now;
                    loganalysis.Status = 1;
                    loganalysis.IsActive = true;
                    if (_sosAnalysis.AnalysisLogbooks == null)
                    {
                        _sosAnalysis.AnalysisLogbooks = new List<SOSAnalysisLogbook>();
                    }
                    _sosAnalysis.AnalysisLogbooks.Add(loganalysis);

                    if (_sosAnalysis.Times == null)
                    {
                        _sosAnalysis.Times = new List<SOSTime>();
                    }

                    foreach (Section section in _sosHub.Sections)
                    {
                        SOSTime newitem = new SOSTime();

                        newitem.SectionId = section.SectionId;
                        newitem.IsActive = true;
                        newitem.Time = "";

                        _sosAnalysis.Times.Add(newitem);
                    }


                    var Gen_sosAnalysis = await SOSHubServices.GenerateAnalysis(SOSHubId, _sosAnalysis);

                    Snackbar.Clear();
                    Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                    if (Gen_sosAnalysis != 0)
                    {
                        Snackbar.Add($"{Localizer["_sosAnalysisGeneratedSucces"]}", Severity.Info);
                        NavigationManager.NavigateTo($"/soshoe/Analysis/Details/{Gen_sosAnalysis}");
                        _sosAnalysis = new SOSAnalysis();
                        //Pregutar si quiere ver el analisis generado
                    }
                    else
                    {
                        Snackbar.Add($"{Localizer["Fail_sosAnalysisGeneratedSucces"]}", Severity.Error);
                    }

                    StateHasChanged();
                }
            }


        }

        SOSCombinationLogbook logCombination { get; set; } = new SOSCombinationLogbook();
        public async void GenerateCombination()
        {

            if (_sosHub.SOSCombination.Count > 0)
            {
                //REVISION
                if (ReviewerCombinationId == 0 || ApproverCombinationId == 0)
                {
                    bool? result = await DialogService.ShowMessageBox(
                       "Warning",
                       ApproverCombinationId == 0 ? "Es necesario el aprobador" : "Es necesario seleccionar el editor (elaboro)!",
                       yesText: "Ok!");
                    var state = result == null ? "Canceled" : "Deleted!";
                    StateHasChanged();
                    return;
                }

                _sosCombination = _sosHub.SOSCombination.First();

                if (_sosCombination.CombinationLogbooks.First().SOSCombinationLogbookId == 0)
                {
                    _sosCombination.CombinationLogbooks.Clear();
                }

                logCombination.NoRevision = _sosCombination.CombinationLogbooks?.Count();
                logCombination.ApproverId = ApproverCombinationId;
                logCombination.ReviewerId = ReviewerCombinationId;
                logCombination.Date = System.DateTime.Now;
                logCombination.Status = 1;
                logCombination.IsActive = true;
                if (_sosCombination.CombinationLogbooks == null)
                {
                    _sosCombination.CombinationLogbooks = new List<SOSCombinationLogbook>();
                }


                _sosCombination.CombinationLogbooks.Add(logCombination);

                var Gen_sosCombination = await SOSHubServices.GenerateCombination(SOSHubId, _sosCombination);

                Snackbar.Clear();
                Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                if (Gen_sosCombination != 0)
                {
                    Snackbar.Add($"{Localizer["_sosCombinationGeneratedSucces"]}", Severity.Info);
                    NavigationManager.NavigateTo($"/soshoe/Combination/Details/{_sosCombination.SOSCombinationId}");
                    _sosCombination = new SOSCombination();
                    //Pregutar si quiere ver el analisis generado
                }
                else
                {
                    Snackbar.Add($"{Localizer["Fail_sosCombinationGeneratedSucces"]}", Severity.Error);
                }

                StateHasChanged();

            }
            else
            {
                if (ReviewerCombinationId == 0 || ApproverCombinationId == 0)
                {
                    bool? result = await DialogService.ShowMessageBox(
                       "Warning",
                       string.IsNullOrEmpty(_sosCombination.OperationName) ? "Es necesario el aproador" : "Es necesario seleccionar el editor (elaboro)!",
                       yesText: "Ok!");
                    var state = result == null ? "Canceled" : "Deleted!";
                    StateHasChanged();
                }
                else if (string.IsNullOrEmpty(_sosCombination.ProcessName) || string.IsNullOrEmpty(_sosCombination.InternalControlNumber))
                {
                    bool? result = await DialogService.ShowMessageBox(
                      "Warning",
                      string.IsNullOrEmpty(_sosCombination.ProcessName) ? "Es necesario el nombre de Proceso" : "Es necesario el nombre del proceso!",
                      yesText: "Ok!");
                    var state = result == null ? "Canceled" : "Deleted!";
                    StateHasChanged();
                }
                else if (string.IsNullOrEmpty(_sosCombination.OperationName))
                {
                    bool? result = await DialogService.ShowMessageBox(
                      "Warning",
                       "Es necesario el nombre de operacion!",
                      yesText: "Ok!");
                    var state = result == null ? "Canceled" : "Deleted!";
                    StateHasChanged();
                }
                else
                {
                    logCombination.NoRevision = 0;
                    logCombination.ReviewerId = ReviewerCombinationId;
                    logCombination.ApproverId = ApproverCombinationId;
                    logCombination.Date = System.DateTime.Now;
                    logCombination.Status = 1;
                    logCombination.IsActive = true;
                    if (_sosCombination.CombinationLogbooks == null)
                    {
                        _sosCombination.CombinationLogbooks = new List<SOSCombinationLogbook>();
                    }

                    _sosCombination.CombinationLogbooks.Add(logCombination);

                    _sosCombination.ReviewerHSId = ReviewerHYSDocCombinationId;


                    if (SupervisorTurn1 != 0 && OperatorTurn1 != 0)
                    {
                        _sosCombination.Turns.ElementAt(0).SupervisorId = SupervisorTurn1;
                        _sosCombination.Turns.ElementAt(0).OperatorId = OperatorTurn1;
                    }

                    if (SupervisorTurn2 != 0 && OperatorTurn2 != 0)
                    {
                        _sosCombination.Turns.ElementAt(1).SupervisorId = SupervisorTurn2;
                        _sosCombination.Turns.ElementAt(1).OperatorId = OperatorTurn2;
                    }

                    if (SupervisorTurn3 != 0 && OperatorTurn3 != 0)
                    {
                        _sosCombination.Turns.ElementAt(2).SupervisorId = SupervisorTurn3;
                        _sosCombination.Turns.ElementAt(2).OperatorId = OperatorTurn3;
                    }

                    var Gen_sosCombination = await SOSHubServices.GenerateCombination(SOSHubId, _sosCombination);

                    Snackbar.Clear();
                    Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                    if (Gen_sosCombination != 0)
                    {
                        Snackbar.Add($"{Localizer["_sosCombinationGeneratedSucces"]}", Severity.Info);
                        NavigationManager.NavigateTo($"/soshoe/Combination/Details/{Gen_sosCombination}");
                        _sosCombination = new SOSCombination();
                        //Pregutar si quiere ver el analisis generado
                    }
                    else
                    {
                        Snackbar.Add($"{Localizer["Fail_sosCombinationGeneratedSucces"]}", Severity.Error);
                    }

                    StateHasChanged();
                }
            }


        }

        SOSDistributionLogbook logDistribution { get; set; } = new SOSDistributionLogbook();
        public async void GenerateDistribution()
        {

            if (_sosHub.SOSDistribution.Count > 0)
            {
                //REVISION
                if (ReviewerDistributionId == 0 || ApproverDistributionId == 0)
                {
                    bool? result = await DialogService.ShowMessageBox(
                       "Warning",
                       ApproverDistributionId == 0 ? "Es necesario el aprobador" : "Es necesario seleccionar el editor (elaboro)!",
                       yesText: "Ok!");
                    var state = result == null ? "Canceled" : "Deleted!";
                    StateHasChanged();
                    return;
                }

                var copySequences = _sosDistribution.Sequences;
                var copyAnalyses = _sosDistribution.Analyses;

                _sosDistribution = _sosHub.SOSDistribution.First();

                if (_sosDistribution.DistributionLogbooks.First().SOSDistributionLogbookId == 0)
                {
                    _sosDistribution.DistributionLogbooks.Clear();
                }

                logDistribution.NoRevision = _sosDistribution.DistributionLogbooks?.Count();
                logDistribution.ApproverId = ApproverDistributionId;
                logDistribution.ReviewerId = ReviewerDistributionId;
                logDistribution.Date = System.DateTime.Now;
                logDistribution.Status = 1;
                logDistribution.IsActive = true;
                if (_sosDistribution.DistributionLogbooks == null)
                {
                    _sosDistribution.DistributionLogbooks = new List<SOSDistributionLogbook>();
                }

                _sosDistribution.DistributionLogbooks.Add(logDistribution);



                var Gen_sosDistribution = await SOSHubServices.GenerateDistribution(SOSHubId, _sosDistribution);

                Snackbar.Clear();
                Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                if (Gen_sosDistribution != 0)
                {
                    Snackbar.Add($"{Localizer["_sosDistributionGeneratedSucces"]}", Severity.Info);
                    NavigationManager.NavigateTo($"/soshoe/Distribution/Details/{_sosDistribution.SOSDistributionId}");
                    _sosDistribution = new SOSDistribution();
                    //Pregutar si quiere ver el analisis generado
                }
                else
                {
                    Snackbar.Add($"{Localizer["Fail_sosDistributionGeneratedSucces"]}", Severity.Error);
                }

                StateHasChanged();

            }
            else
            {
                if (ReviewerDistributionId == 0 || ApproverDistributionId == 0)
                {
                    bool? result = await DialogService.ShowMessageBox(
                       "Warning",
                       string.IsNullOrEmpty(_sosDistribution.OperationName) ? "Es necesario el aproador" : "Es necesario seleccionar el editor (elaboro)!",
                       yesText: "Ok!");
                    var state = result == null ? "Canceled" : "Deleted!";
                    StateHasChanged();
                }
                else if (string.IsNullOrEmpty(_sosDistribution.ProcessName) || string.IsNullOrEmpty(_sosDistribution.InternalControlNumber))
                {
                    bool? result = await DialogService.ShowMessageBox(
                      "Warning",
                      string.IsNullOrEmpty(_sosDistribution.ProcessName) ? "Es necesario el nombre de Proceso" : "Es necesario el nombre del proceso!",
                      yesText: "Ok!");
                    var state = result == null ? "Canceled" : "Deleted!";
                    StateHasChanged();
                }
                else if (string.IsNullOrEmpty(_sosDistribution.OperationName))
                {
                    bool? result = await DialogService.ShowMessageBox(
                      "Warning",
                       "Es necesario el nombre de operacion!",
                      yesText: "Ok!");
                    var state = result == null ? "Canceled" : "Deleted!";
                    StateHasChanged();
                }
                else
                {
                    //reviewer = SV = Editor  //Approver = SSV = owner 

                    //_sosDistribution.ReviewerId = ReviewerDistributionId;
                    //_sosDistribution.ApproverId = ApproverDistributionId;

                    logDistribution.NoRevision = 0;
                    logDistribution.ReviewerId = ReviewerDistributionId;
                    logDistribution.ApproverId = ApproverDistributionId;
                    logDistribution.Date = System.DateTime.Now;
                    logDistribution.Status = 1;
                    logDistribution.IsActive = true;
                    if (_sosDistribution.DistributionLogbooks == null)
                    {
                        _sosDistribution.DistributionLogbooks = new List<SOSDistributionLogbook>();
                    }

                    _sosDistribution.DistributionLogbooks.Add(logDistribution);

                    if (SupervisorTurn1 != 0 && OperatorTurn1 != 0)
                    {
                        _sosDistribution.Turns.ElementAt(0).SupervisorId = SupervisorTurn1;
                        _sosDistribution.Turns.ElementAt(0).OperatorId = OperatorTurn1;
                    }

                    if (SupervisorTurn2 != 0 && OperatorTurn2 != 0)
                    {
                        _sosDistribution.Turns.ElementAt(1).SupervisorId = SupervisorTurn2;
                        _sosDistribution.Turns.ElementAt(1).OperatorId = OperatorTurn2;
                    }

                    if (SupervisorTurn3 != 0 && OperatorTurn3 != 0)
                    {
                        _sosDistribution.Turns.ElementAt(2).SupervisorId = SupervisorTurn3;
                        _sosDistribution.Turns.ElementAt(2).OperatorId = OperatorTurn3;
                    }

                    if (_sosDistribution.SOSDistributionOperationSequence == null)
                    {
                        _sosDistribution.SOSDistributionOperationSequence = new List<SOSDistributionOperationSequence>();
                    }

                    //foreach (Section section in _sosHub.Sections)
                    //{
                    //    SOSTime newitem = new SOSTime();

                    //    newitem.SectionId = section.SectionId;
                    //    newitem.IsActive = true;
                    //    newitem.Time = "";

                    //    _sosDistribution.Times.Add(newitem);
                    //}

                    var Gen_sosDistribution = await SOSHubServices.GenerateDistribution(SOSHubId, _sosDistribution);

                    Snackbar.Clear();
                    Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                    if (Gen_sosDistribution != 0)
                    {
                        Snackbar.Add($"{Localizer["_sosDistributionGeneratedSucces"]}", Severity.Info);
                        NavigationManager.NavigateTo($"/soshoe/Distribution/Details/{Gen_sosDistribution}");
                        _sosDistribution = new SOSDistribution();
                        //Pregutar si quiere ver el analisis generado
                    }
                    else
                    {
                        Snackbar.Add($"{Localizer["Fail_sosDistributionGeneratedSucces"]}", Severity.Error);
                    }

                    StateHasChanged();
                }
            }


        }

        SOSFlowLogbook logFlow { get; set; } = new SOSFlowLogbook();
        public async void GenerateFlow()
        {

            if (_sosHub.SOSFlow.Count > 0)
            {
                //REVISION
                if (ReviewerFlowId == 0 || ApproverFlowId == 0)
                {
                    bool? result = await DialogService.ShowMessageBox(
                       "Warning",
                       ApproverFlowId == 0 ? "Es necesario el aprobador" : "Es necesario seleccionar el editor (elaboro)!",
                       yesText: "Ok!");
                    var state = result == null ? "Canceled" : "Deleted!";
                    StateHasChanged();
                    return;
                }

                _sosFlow = _sosHub.SOSFlow.First();

                if (_sosFlow.FlowLogbooks.First().SOSFlowLogbookId == 0)
                {
                    _sosFlow.FlowLogbooks.Clear();
                }

                logFlow.NoRevision = _sosFlow.FlowLogbooks?.Count();
                logFlow.ApproverId = ApproverFlowId;
                logFlow.ReviewerId = ReviewerFlowId;
                logFlow.Date = System.DateTime.Now;
                logFlow.Status = 1;
                logFlow.IsActive = true;
                if (_sosFlow.FlowLogbooks == null)
                {
                    _sosFlow.FlowLogbooks = new List<SOSFlowLogbook>();
                }

                _sosFlow.FlowLogbooks.Add(logFlow);

                var Gen_sosFlow = await SOSHubServices.GenerateFlow(SOSHubId, _sosFlow);

                Snackbar.Clear();
                Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                if (Gen_sosFlow != 0)
                {
                    Snackbar.Add($"{Localizer["_sosFlowGeneratedSucces"]}", Severity.Info);
                    NavigationManager.NavigateTo($"/soshoe/Flow/Details/{_sosFlow.SOSFlowId}");
                    _sosFlow = new SOSFlow();
                    //Pregutar si quiere ver el analisis generado
                }
                else
                {
                    Snackbar.Add($"{Localizer["Fail_sosFlowGeneratedSucces"]}", Severity.Error);
                }

                StateHasChanged();

            }
            else
            {
                if (ReviewerFlowId == 0 || ApproverFlowId == 0)
                {
                    bool? result = await DialogService.ShowMessageBox(
                       "Warning",
                       string.IsNullOrEmpty(_sosFlow.OperationName) ? "Es necesario el aproador" : "Es necesario seleccionar el editor (elaboro)!",
                       yesText: "Ok!");
                    var state = result == null ? "Canceled" : "Deleted!";
                    StateHasChanged();
                }
                else if (string.IsNullOrEmpty(_sosFlow.ProcessName) || string.IsNullOrEmpty(_sosFlow.InternalControlNumber))
                {
                    bool? result = await DialogService.ShowMessageBox(
                      "Warning",
                      string.IsNullOrEmpty(_sosFlow.ProcessName) ? "Es necesario el nombre de Proceso" : "Es necesario el nombre del proceso!",
                      yesText: "Ok!");
                    var state = result == null ? "Canceled" : "Deleted!";
                    StateHasChanged();
                }
                else if (string.IsNullOrEmpty(_sosFlow.OperationName))
                {
                    bool? result = await DialogService.ShowMessageBox(
                      "Warning",
                       "Es necesario el nombre de operacion!",
                      yesText: "Ok!");
                    var state = result == null ? "Canceled" : "Deleted!";
                    StateHasChanged();
                }
                else if (string.IsNullOrEmpty(_sosFlow.TargetTime))
                {
                    bool? result = await DialogService.ShowMessageBox(
                      "Warning",
                       "Es necesario el tiempo objetivo!",
                      yesText: "Ok!");
                    var state = result == null ? "Canceled" : "Deleted!";
                    StateHasChanged();
                }
                else if (_sosFlow.CreatedAt == null)
                {
                    bool? result = await DialogService.ShowMessageBox(
                      "Warning",
                       "Es necesaria una fecha!",
                      yesText: "Ok!");
                    var state = result == null ? "Canceled" : "Deleted!";
                    StateHasChanged();
                }
                else
                {
                    logFlow.NoRevision = 0;
                    logFlow.ReviewerId = ReviewerFlowId;
                    logFlow.ApproverId = ApproverFlowId;
                    logFlow.Date = System.DateTime.Now;
                    logFlow.Status = 1;
                    logFlow.IsActive = true;
                    if (_sosFlow.FlowLogbooks == null)
                    {
                        _sosFlow.FlowLogbooks = new List<SOSFlowLogbook>();
                    }
                    _sosFlow.FlowLogbooks.Add(logFlow);

                    _sosFlow.ReviewerHSId = ReviewerHYSDocCombinationId;

                    var Gen_sosFlow = await SOSHubServices.GenerateFlow(SOSHubId, _sosFlow);

                    Snackbar.Clear();
                    Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                    if (Gen_sosFlow != 0)
                    {
                        Snackbar.Add($"{Localizer["_sosFlowGeneratedSucces"]}", Severity.Info);
                        NavigationManager.NavigateTo($"/soshoe/Flow/Details/{Gen_sosFlow}");
                        _sosFlow = new SOSFlow();
                        //Pregutar si quiere ver el analisis generado
                    }
                    else
                    {
                        Snackbar.Add($"{Localizer["Fail_sosFlowGeneratedSucces"]}", Severity.Error);
                    }

                    StateHasChanged();
                }
            }


        }

        SOSSequenceLogbook logSequence { get; set; } = new SOSSequenceLogbook();
        public async void GenerateSequence()
        {

            if (_sosHub.SOSSequence.Count > 0)
            {
                //REVISION
                if (ReviewerSequenceId == 0 || ApproverSequenceId == 0)
                {
                    bool? result = await DialogService.ShowMessageBox(
                       "Warning",
                       ApproverSequenceId == 0 ? "Es necesario el aprobador" : "Es necesario seleccionar el editor (elaboro)!",
                       yesText: "Ok!");
                    var state = result == null ? "Canceled" : "Deleted!";
                    StateHasChanged();
                    return;
                }

                _sosSequence = _sosHub.SOSSequence.First();

                if (_sosSequence.SequenceLogbooks.First().SOSSequenceLogbookId == 0)
                {
                    _sosSequence.SequenceLogbooks.Clear();
                }

                logSequence.NoRevision = _sosSequence.SequenceLogbooks?.Count();
                logSequence.ApproverId = ApproverSequenceId;
                logSequence.ReviewerId = ReviewerSequenceId;
                logSequence.Date = System.DateTime.Now;
                logSequence.Status = 1;
                logSequence.IsActive = true;
                if (_sosSequence.SequenceLogbooks == null)
                {
                    _sosSequence.SequenceLogbooks = new List<SOSSequenceLogbook>();
                }

                _sosSequence.SequenceLogbooks.Add(logSequence);

                var Gen_sosSequence = await SOSHubServices.GenerateSequence(SOSHubId, _sosSequence);

                Snackbar.Clear();
                Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                if (Gen_sosSequence != 0)
                {
                    Snackbar.Add($"{Localizer["_sosSequenceGeneratedSucces"]}", Severity.Info);
                    NavigationManager.NavigateTo($"/soshoe/Sequence/Details/{_sosSequence.SOSSequenceId}");
                    _sosSequence = new SOSSequence();
                    //Pregutar si quiere ver el analisis generado
                }
                else
                {
                    Snackbar.Add($"{Localizer["Fail_sosSequenceGeneratedSucces"]}", Severity.Error);
                }

                StateHasChanged();

            }
            else
            {
                if (ReviewerSequenceId == 0 || ApproverSequenceId == 0)
                {
                    bool? result = await DialogService.ShowMessageBox(
                       "Warning",
                       string.IsNullOrEmpty(_sosSequence.OperationName) ? "Es necesario el aproador" : "Es necesario seleccionar el editor (elaboro)!",
                       yesText: "Ok!");
                    var state = result == null ? "Canceled" : "Deleted!";
                    StateHasChanged();
                }
                else if (string.IsNullOrEmpty(_sosSequence.ProcessName) || string.IsNullOrEmpty(_sosSequence.InternalControlNumber))
                {
                    bool? result = await DialogService.ShowMessageBox(
                      "Warning",
                      string.IsNullOrEmpty(_sosSequence.ProcessName) ? "Es necesario el nombre de Proceso" : "Es necesario el nombre del proceso!",
                      yesText: "Ok!");
                    var state = result == null ? "Canceled" : "Deleted!";
                    StateHasChanged();
                }
                else if (string.IsNullOrEmpty(_sosSequence.OperationName))
                {
                    bool? result = await DialogService.ShowMessageBox(
                      "Warning",
                       "Es necesario el nombre de operacion!",
                      yesText: "Ok!");
                    var state = result == null ? "Canceled" : "Deleted!";
                    StateHasChanged();
                }
                else
                {
                    logSequence.NoRevision = 0;
                    logSequence.ApproverId = ApproverSequenceId;
                    logSequence.ReviewerId = ReviewerSequenceId;
                    logSequence.Date = System.DateTime.Now;
                    logSequence.Status = 1;
                    logSequence.IsActive = true;
                    if (_sosSequence.SequenceLogbooks == null)
                    {
                        _sosSequence.SequenceLogbooks = new List<SOSSequenceLogbook>();
                    }
                    _sosSequence.SequenceLogbooks.Add(logSequence);

                    if (_sosSequence.Times == null)
                    {
                        _sosSequence.Times = new List<SOSTime>();
                    }

                    foreach (Section section in _sosHub.Sections)
                    {

                        foreach (Analysis analysis in section.Analyses)
                        {
                            SOSTime newitem = new SOSTime();

                            newitem.SectionId = section.SectionId;
                            newitem.AnalysisId = analysis.AnalysisId;
                            newitem.IsActive = true;
                            newitem.Time = "";

                            _sosSequence.Times.Add(newitem);
                        }
                    }


                    var Gen_sosSequence = await SOSHubServices.GenerateSequence(SOSHubId, _sosSequence);

                    Snackbar.Clear();
                    Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                    if (Gen_sosSequence != 0)
                    {
                        Snackbar.Add($"{Localizer["_sosSequenceGeneratedSucces"]}", Severity.Info);
                        NavigationManager.NavigateTo($"/soshoe/Sequence/Details/{Gen_sosSequence}");
                        _sosSequence = new SOSSequence();
                        //Pregutar si quiere ver el analisis generado
                    }
                    else
                    {
                        Snackbar.Add($"{Localizer["Fail_sosSequenceGeneratedSucces"]}", Severity.Error);
                    }

                    StateHasChanged();
                }
            }


        }

        #region GenerateSynopticRequirements

        SOSSynopticRequirementsLogbook logSynopticRequirements { get; set; } = new SOSSynopticRequirementsLogbook();
        public async void GenerateSynopticRequirements()
        {

            if (_sosHub.SOSSynopticOperatingRequirements.Count > 0)
            {
                //REVISION
                if (ReviewerSynopticRequirementsId == 0 || ApproverSynopticRequirementsId == 0)
                {
                    bool? result = await DialogService.ShowMessageBox(
                       "Warning",
                       ApproverSynopticRequirementsId == 0 ? "Es necesario el aprobador" : "Es necesario seleccionar el editor (elaboro)!",
                       yesText: "Ok!");
                    var state = result == null ? "Canceled" : "Deleted!";
                    StateHasChanged();
                    return;
                }

                var copySequences = _SOSSynopticRequirements.Sequences;
                var copyAnalyses = _SOSSynopticRequirements.Analyses;

                _SOSSynopticRequirements = _sosHub.SOSSynopticOperatingRequirements.First();

                if (_SOSSynopticRequirements.SynopticRequirementsLogbooks.First().SOSSynopticRequirementsLogbookId == 0)
                {
                    _SOSSynopticRequirements.SynopticRequirementsLogbooks.Clear();
                }

                logSynopticRequirements.NoRevision = _SOSSynopticRequirements.SynopticRequirementsLogbooks?.Count();
                logSynopticRequirements.ApproverId = ApproverSynopticRequirementsId;
                logSynopticRequirements.ReviewerId = ReviewerSynopticRequirementsId;
                logSynopticRequirements.Date = System.DateTime.Now;
                logSynopticRequirements.Status = 1;
                logSynopticRequirements.IsActive = true;
                if (_SOSSynopticRequirements.SynopticRequirementsLogbooks == null)
                {
                    _SOSSynopticRequirements.SynopticRequirementsLogbooks = new List<SOSSynopticRequirementsLogbook>();
                }

                _SOSSynopticRequirements.SynopticRequirementsLogbooks.Add(logSynopticRequirements);



                var Gen_SOSSynopticRequirements = await SOSHubServices.GenerateSynopticRequirements(SOSHubId, _SOSSynopticRequirements);

                Snackbar.Clear();
                Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                if (Gen_SOSSynopticRequirements != 0)
                {
                    Snackbar.Add($"{Localizer["_SOSSynopticRequirementsGeneratedSucces"]}", Severity.Info);
                    NavigationManager.NavigateTo($"/soshoe/SynopticRequirements/Details/{_SOSSynopticRequirements.SOSSynopticTableofOperatingRequirementsId}");
                    _SOSSynopticRequirements = new SOSSynopticTableofOperatingRequirements();
                    //Pregutar si quiere ver el analisis generado
                }
                else
                {
                    Snackbar.Add($"{Localizer["Fail_SOSSynopticRequirementsGeneratedSucces"]}", Severity.Error);
                }

                StateHasChanged();

            }
            else
            {
                if (string.IsNullOrEmpty(_SOSSynopticRequirements.ProcessName) || OwnerSynopticRequirementsId == 0)
                {
                    bool? result = await DialogService.ShowMessageBox(
                      "Warning",
                       OwnerSynopticRequirementsId == 0 ? "Es necesario el elaborador" : "Es necesario el nombre de processo!",
                      yesText: "Ok!");
                    var state = result == null ? "Canceled" : "Deleted!";
                    StateHasChanged();
                }
                else if (ApproverSynopticRequirementsId == 0 || ReviewerSynopticRequirementsId == 0)
                {
                    bool? result = await DialogService.ShowMessageBox(
                      "Warning",
                      ApproverSynopticRequirementsId == 0 ? "Es necesario el aprovador" : "Es necesario seleccionar el editor!",
                      yesText: "Ok!");
                    var state = result == null ? "Canceled" : "Deleted!";
                    StateHasChanged();
                }
                else
                {
                    _SOSSynopticRequirements.SOSHubId = SOSHubId;

                    _SOSSynopticRequirements.ReviewerId = ReviewerSynopticRequirementsId;
                    _SOSSynopticRequirements.ApproverId = ApproverSynopticRequirementsId;
                    _SOSSynopticRequirements.CreatorId = OwnerSynopticRequirementsId;

                    logSynopticRequirements.NoRevision = 0;
                    logSynopticRequirements.ReviewerId = ReviewerSynopticRequirementsId;
                    logSynopticRequirements.ApproverId = ApproverSynopticRequirementsId;
                    logSynopticRequirements.Date = System.DateTime.Now;
                    logSynopticRequirements.Status = 1;
                    logSynopticRequirements.IsActive = true;
                    if (_SOSSynopticRequirements.SynopticRequirementsLogbooks == null)
                    {
                        _SOSSynopticRequirements.SynopticRequirementsLogbooks = new List<SOSSynopticRequirementsLogbook>();
                    }

                    _SOSSynopticRequirements.SynopticRequirementsLogbooks.Add(logSynopticRequirements);


                    if (_SOSSynopticRequirements.SOSSynopticRequirementsOperationSequence == null)
                    {
                        _SOSSynopticRequirements.SOSSynopticRequirementsOperationSequence = new List<SOSSynopticRequirementsOperationSequence>();
                    }



                    var Gen_SOSSynopticRequirements = await SOSHubServices.GenerateSynopticRequirements(SOSHubId, _SOSSynopticRequirements);

                    Snackbar.Clear();
                    Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                    if (Gen_SOSSynopticRequirements != 0)
                    {
                        Snackbar.Add($"{Localizer["_SOSSynopticRequirementsGeneratedSucces"]}", Severity.Info);
                        NavigationManager.NavigateTo($"/soshoe/SynopticRequirements/Details/{Gen_SOSSynopticRequirements}");
                        _SOSSynopticRequirements = new SOSSynopticTableofOperatingRequirements();
                        //Pregutar si quiere ver el analisis generado
                    }
                    else
                    {
                        Snackbar.Add($"{Localizer["Fail_SOSSynopticRequirementsGeneratedSucces"]}", Severity.Error);
                    }

                    StateHasChanged();
                }
            }

        }

        #endregion


        #region GenerateSynopticControlPoints

        SOSSynopticPointsLogbook logGenerateSynopticControlPoints { get; set; } = new SOSSynopticPointsLogbook();
        public async void GenerateSynopticControlPoints()
        {

            if (_sosHub.SOSSynopticControlPoints.Count > 0)
            {
                //REVISION
                if (ApproverControlPointsId == 0)
                {
                    bool? result = await DialogService.ShowMessageBox(
                       "Warning",
                       ApproverControlPointsId == 0 ? "Es necesario el aprobador" : "Es necesario seleccionar el editor (elaboro)!",
                       yesText: "Ok!");
                    var state = result == null ? "Canceled" : "Deleted!";
                    StateHasChanged();
                    return;
                }

                var copySequences = _sosControlPoints.Sequences;
                var copyAnalyses = _sosControlPoints.Analyses;

                _sosControlPoints = _sosHub.SOSSynopticControlPoints.First();

                if (_sosControlPoints.SynopticPointsLogbooks.First().SOSSynopticPointsLogbookId == 0)
                {
                    _sosControlPoints.SynopticPointsLogbooks.Clear();
                }

                logGenerateSynopticControlPoints.NoRevision = _sosControlPoints.SynopticPointsLogbooks?.Count();
                logGenerateSynopticControlPoints.ApproverId = ApproverControlPointsId;
                logGenerateSynopticControlPoints.Date = System.DateTime.Now;
                logGenerateSynopticControlPoints.Status = 1;
                logGenerateSynopticControlPoints.IsActive = true;
                if (_sosControlPoints.SynopticPointsLogbooks == null)
                {
                    _sosControlPoints.SynopticPointsLogbooks = new List<SOSSynopticPointsLogbook>();
                }

                _sosControlPoints.SynopticPointsLogbooks.Add(logGenerateSynopticControlPoints);



                var Gen_SOSSynopticRequirements = await SOSHubServices.GenerateSynopticControlPoints(SOSHubId, _sosControlPoints);

                Snackbar.Clear();
                Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                if (Gen_SOSSynopticRequirements != 0)
                {
                    Snackbar.Add($"{Localizer["_SOSSynopticControlPointsGeneratedSuccess"]}", Severity.Info);
                    NavigationManager.NavigateTo($"/soshoe/SynopticControlPoints/Details/{_sosControlPoints.SOSSynopticTableofControlPointsId}");
                    _sosControlPoints = new SOSSynopticTableofControlPoints();
                    //Pregutar si quiere ver el analisis generado
                }
                else
                {
                    Snackbar.Add($"{Localizer["Fail_SOSSynopticControlPointsGeneratedSuccess"]}", Severity.Error);
                }

                StateHasChanged();

            }
            else
            {
                if (string.IsNullOrEmpty(_sosControlPoints.ProcessName) || OwnerControlPointsId == 0)
                {
                    bool? result = await DialogService.ShowMessageBox(
                      "Warning",
                       OwnerControlPointsId == 0 ? "Es necesario el elaborador" : "Es necesario el nombre de processo!",
                      yesText: "Ok!");
                    var state = result == null ? "Canceled" : "Deleted!";
                    StateHasChanged();
                }
                else if (ApproverControlPointsId == 0 || ReviewerControlPointsId == 0)
                {
                    bool? result = await DialogService.ShowMessageBox(
                      "Warning",
                      ApproverControlPointsId == 0 ? "Es necesario el aprovador" : "Es necesario seleccionar el editor!",
                      yesText: "Ok!");
                    var state = result == null ? "Canceled" : "Deleted!";
                    StateHasChanged();
                }
                else
                {
                    _sosControlPoints.SOSHubId = SOSHubId;
                    _sosControlPoints.ReviewerId = ReviewerControlPointsId;
                    _sosControlPoints.ApproverId = ApproverControlPointsId;
                    _sosControlPoints.CreatorId = OwnerControlPointsId;

                    logGenerateSynopticControlPoints.NoRevision = 0;
                    logGenerateSynopticControlPoints.ApproverId = ApproverControlPointsId;
                    logGenerateSynopticControlPoints.Date = System.DateTime.Now;
                    logGenerateSynopticControlPoints.Status = 1;
                    logGenerateSynopticControlPoints.IsActive = true;
                    if (_sosControlPoints.SynopticPointsLogbooks == null)
                    {
                        _sosControlPoints.SynopticPointsLogbooks = new List<SOSSynopticPointsLogbook>();
                    }

                    _sosControlPoints.SynopticPointsLogbooks.Add(logGenerateSynopticControlPoints);


                    if (_sosControlPoints.SOSSynopticRequirementsOperationSequence == null)
                    {
                        _sosControlPoints.SOSSynopticRequirementsOperationSequence = new List<SOSSynopticRequirementsOperationSequence>();
                    }



                    var Gen_sosControlPoints = await SOSHubServices.GenerateSynopticControlPoints(SOSHubId, _sosControlPoints);

                    Snackbar.Clear();
                    Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                    if (Gen_sosControlPoints != 0)
                    {
                        Snackbar.Add($"{Localizer["_SOSSynopticControlPointsGeneratedSuccess"]}", Severity.Info);
                        NavigationManager.NavigateTo($"/soshoe/SynopticControlPoints/Details/{Gen_sosControlPoints}");
                        _sosControlPoints = new SOSSynopticTableofControlPoints();
                        //Pregutar si quiere ver el analisis generado
                    }
                    else
                    {
                        Snackbar.Add($"{Localizer["Fail_SOSSynopticControlPointsGeneratedSuccess"]}", Severity.Error);
                    }

                    StateHasChanged();
                }
            }


        }

        #endregion

        #region HCI

        private async void GenerateHci()
        {

            _hci.User = await UsersService.GetUserAndCollection(_SelectUser.UserId);
            _hci.UserId = _SelectUser.UserId;

            _hci.SOSHubId = _sosHub.SOSHubId;


            if (await HCIsServices.CreateHCI(_hci))
            {
                Snackbar.Add("Created succesfully", Severity.Success);
                var UpdatedUser = await UsersService.GetUser(_SelectUser.UserId);
                NavigationManager.NavigateTo($"/HCI/Details/{UpdatedUser.HciId}");
            }
            else
            {
                Snackbar.Add("Error", Severity.Error);
            }
        }

        #endregion

        List<SOSHub> AvailableSoshubs = new();
        List<SOSAnalysis> AvailableAnalyses = new();
        List<SOSSequence> AvailableSequences = new();

        private string searchAnalysis = "";
        private IEnumerable<SOSAnalysis> FilteredAnalysis =>
            AvailableAnalyses.Where(op =>
                string.IsNullOrEmpty(searchAnalysis) ||
                (op.InternalControlNumber?.Contains(searchAnalysis, StringComparison.OrdinalIgnoreCase) ?? false) ||
                (op.ProcessName?.Contains(searchAnalysis, StringComparison.OrdinalIgnoreCase) ?? false) ||
                (op.OperationName?.Contains(searchAnalysis, StringComparison.OrdinalIgnoreCase) ?? false));

        private string searchSosHub = "";
        private IEnumerable<SOSHub> FilteredSosHubs =>
            AvailableSoshubs.Where(op =>
                string.IsNullOrEmpty(searchSosHub) ||
                (op.Folio?.Contains(searchSosHub, StringComparison.OrdinalIgnoreCase) ?? false) ||
                (op.ProcessSheet?.Contains(searchSosHub, StringComparison.OrdinalIgnoreCase) ?? false) ||
                (op.OtherInformation?.Contains(searchSosHub, StringComparison.OrdinalIgnoreCase) ?? false));


        private string searchSequence = "";
        private IEnumerable<SOSSequence> FilteredSequences =>
            AvailableSequences.Where(op =>
                string.IsNullOrEmpty(searchSequence) ||
                (op.InternalControlNumber?.Contains(searchSequence, StringComparison.OrdinalIgnoreCase) ?? false) ||
                (op.ProcessName?.Contains(searchSequence, StringComparison.OrdinalIgnoreCase) ?? false) ||
                (op.OperationName?.Contains(searchSequence, StringComparison.OrdinalIgnoreCase) ?? false));

        private Task<IEnumerable<int>> SearchApproverOwners(string searchString)
        {
            IEnumerable<int> result;

            if (string.IsNullOrEmpty(searchString))
            {
                result = _sosHub.ApproverOwners?.Select(x => x.UserId);
            }
            else
            {
                result = _sosHub.ApproverOwners?
                    .Where(x => x.Name.Contains(searchString, StringComparison.InvariantCultureIgnoreCase))
                    .Select(x => x.UserId);
            }

            return Task.FromResult(result);
        }

        private Task<IEnumerable<int>> SearchReviewerEditors(string searchString)
        {
            IEnumerable<int> result;

            if (string.IsNullOrEmpty(searchString))
            {
                result = _sosHub.ReviewerEditors?.Select(x => x.UserId);
            }
            else
            {
                result = _sosHub.ReviewerEditors?
                    .Where(x => x.Name.Contains(searchString, StringComparison.InvariantCultureIgnoreCase))
                    .Select(x => x.UserId);
            }

            return Task.FromResult(result);
        }

        private Task<IEnumerable<int>> SearchSupervisors(string searchString)
        {
            IEnumerable<int> result;

            if (string.IsNullOrEmpty(searchString))
            {
                result = _supervisors.Select(x => x.UserId);
            }
            else
            {
                result = _supervisors
                    .Where(x => x.Name.Contains(searchString, StringComparison.InvariantCultureIgnoreCase))
                    .Select(x => x.UserId);
            }

            return Task.FromResult(result);
        }

        private void AddCombinationTurn()
        {
            if (_sosCombination.Turns == null)
            {
                _sosCombination.Turns = new List<Turn>();
            }

            string[] numbers = new string[] { "ero", "ndo", "ero", "rto" };
            Turn toCreate = new Turn();
            toCreate.TurnType = (_sosCombination.Turns.Count + 1).ToString() + numbers[(int)_sosCombination.Turns.Count()];
            _sosCombination.Turns?.Add(toCreate);
        }

        private Task<IEnumerable<int>> SearchOperator(string searchString, int turn)
        {
            IEnumerable<int> result;

            if (string.IsNullOrEmpty(searchString))
            {
                result = _operators[turn].Select(x => x.UserId);
            }
            else
            {
                result = _operators[turn]
                    .Where(x => x.Name.Contains(searchString, StringComparison.InvariantCultureIgnoreCase))
                    .Select(x => x.UserId);
            }

            return Task.FromResult(result);
        }

        private async void ShowOperators(int value, int TurnId)
        {

            switch (TurnId)
            {
                case 0:
                    SupervisorTurn1 = value;
                    if (value == 0)
                    {
                        OperatorTurn1 = 0;
                    }
                    break;
                case 1:
                    SupervisorTurn2 = value;
                    if (value == 0)
                    {
                        OperatorTurn2 = 0;
                    }
                    break;
                case 2:
                    SupervisorTurn3 = value;
                    if (value == 0)
                    {
                        OperatorTurn3 = 0;
                    }
                    break;
            }


            if (user.UserType == 1)
            {
                List<User> _oper = await UsersService.GetUsersByUserTypeInPlantAndArea(plantId, areaId, 4, false, false);
                _oper = _oper.OrderBy(s => s.Name).ToList();

                if (_operators.ContainsKey(TurnId))
                {
                    _operators[TurnId] = _oper;
                }
                else
                {
                    _operators.Add(TurnId, _oper);
                }

            }
            else if (user.UserType == 2)
            {

                List<User> _oper = new();
                switch (TurnId)
                {
                    case 0:
                        _oper = await UsersService.GetSubordinates(SupervisorTurn1, false);
                        break;
                    case 1:
                        _oper = await UsersService.GetSubordinates(SupervisorTurn2, false);
                        break;
                    case 2:
                        _oper = await UsersService.GetSubordinates(SupervisorTurn3, false);
                        break;
                }

                _oper = _oper.OrderBy(s => s.Name).ToList();

                if (_operators.ContainsKey(TurnId))
                {
                    _operators[TurnId] = _oper;
                }
                else
                {
                    _operators.Add(TurnId, _oper);
                }
            }

            StateHasChanged();
        }

        private void AddDistributionTurn()
        {
            if (_sosDistribution.Turns == null)
            {
                _sosDistribution.Turns = new List<Turn>();
            }

            string[] numbers = new string[] { "ero", "ndo", "ero", "rto" };
            Turn toCreate = new Turn();
            toCreate.TurnType = (_sosDistribution.Turns.Count + 1).ToString() + numbers[(int)_sosDistribution.Turns.Count()];
            _sosDistribution.Turns?.Add(toCreate);
        }

        public void Details<T>(int id) where T : class
        {
            if (typeof(T) == typeof(SOSAnalysis))
            {
                NavigationManager.NavigateTo($"/soshoe/Analysis/Details/{id}");
            }
            else if (typeof(T) == typeof(SOSCombination))
            {
                NavigationManager.NavigateTo($"/soshoe/Combination/Details/{id}");
            }
            else if (typeof(T) == typeof(SOSDistribution))
            {
                NavigationManager.NavigateTo($"/soshoe/Distribution/Details/{id}");
            }
            else if (typeof(T) == typeof(SOSFlow))
            {
                NavigationManager.NavigateTo($"/soshoe/Flow/Details/{id}");
            }
            else if (typeof(T) == typeof(SOSSequence))
            {
                NavigationManager.NavigateTo($"/soshoe/Sequence/Details/{id}");
            }
            else if (typeof(T) == typeof(PAT))
            {
                NavigationManager.NavigateTo($"/PAT/{id}");
            }
            else if (typeof(T) == typeof(SOSSynopticTableofOperatingRequirements))
            {
                NavigationManager.NavigateTo($"/soshoe/SynopticRequirements/Details/{id}");
            }
            else if (typeof(T) == typeof(SOSSynopticTableofControlPoints))
            {
                NavigationManager.NavigateTo($"/soshoe/SynopticControlPoints/Details/{id}");
            }
            else if (typeof(T) == typeof(HCI))
            {
                NavigationManager.NavigateTo($"/HCI/Details/{id}");
            }

            // A�adir m�s casos seg�n sea necesario
        }

        public void ReturnToGenerateindex()
        {
            MudDialog.Close(DialogResult.Ok("Reopen"));
        }



        private async Task<IEnumerable<User>> SearchSV(string value)
        {

            var reviewerIds = _sosHub.ReviewerEditors?.Select(r => r.UserId).ToHashSet() ?? new HashSet<int>();
            if (string.IsNullOrWhiteSpace(value))
            {
                var orderedUsers = _Users
                        .OrderByDescending(u => u.SuperiorId.HasValue && reviewerIds.Contains(u.SuperiorId.Value))
                        .ToList();

                return orderedUsers;
            }

            value = value.Trim().ToLowerInvariant();

            await Task.Delay(150);

            return _Users.OrderByDescending(u => u.SuperiorId.HasValue && reviewerIds.Contains(u.SuperiorId.Value)).Where(x =>
                (!string.IsNullOrEmpty(x.Name) && x.Name.Contains(value, StringComparison.OrdinalIgnoreCase)) ||
                (x.Payroll.HasValue && x.Payroll.Value.ToString().Contains(value, StringComparison.OrdinalIgnoreCase)) ||
                (!string.IsNullOrEmpty(x.Email) && x.Email.Contains(value, StringComparison.OrdinalIgnoreCase)) ||
                (!string.IsNullOrEmpty(x.Management) && x.Management.Contains(value, StringComparison.OrdinalIgnoreCase)) ||
                (!string.IsNullOrEmpty(x.Process) && x.Process.Contains(value, StringComparison.OrdinalIgnoreCase)) ||
                (x.Plant != null && (
                    (!string.IsNullOrEmpty(x.Plant.Description) && x.Plant.Description.Contains(value, StringComparison.OrdinalIgnoreCase)) ||
                    (!string.IsNullOrEmpty(x.Plant.Code) && x.Plant.Code.Contains(value, StringComparison.OrdinalIgnoreCase))
                )) ||
                (x.Area != null && (
                    (!string.IsNullOrEmpty(x.Area.Description) && x.Area.Description.Contains(value, StringComparison.OrdinalIgnoreCase)) ||
                    (!string.IsNullOrEmpty(x.Area.Code) && x.Area.Code.Contains(value, StringComparison.OrdinalIgnoreCase))
                )) ||
                (x.Group != null && (
                    (!string.IsNullOrEmpty(x.Group.Description) && x.Group.Description.Contains(value, StringComparison.OrdinalIgnoreCase)) ||
                    (!string.IsNullOrEmpty(x.Group.Code) && x.Group.Code.Contains(value, StringComparison.OrdinalIgnoreCase))
                )) ||
                x.UserId.ToString().Contains(value, StringComparison.OrdinalIgnoreCase)
            );
        }


        public void ValidateNextStepDistribution()
        {
            //if ((_sosDistribution.Sequences == null || _sosDistribution.Sequences.Count() <= 0) && (_sosDistribution.Analyses == null || _sosDistribution.Analyses.Count() <= 0))
            //    Snackbar.Add("You need to select a Sequence or an Analysis to continue with the Next Step", Severity.Warning);
            //else
                selectedIndexPageGenerate = 33;
        }
    }
}