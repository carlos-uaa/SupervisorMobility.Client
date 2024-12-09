using DocumentFormat.OpenXml.EMMA;
using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.Drawing;
using Microsoft.JSInterop;
using MudBlazor;
using SupervisorMobility.Client.Data.Entities;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;

namespace SupervisorMobility.Client.Pages.Inicio.PATPage
{
    public partial class PAT_Details
    {
        [Parameter]
        public int patID { get; set; }

        private PAT? _pat { get; set; } = new();
        private List<Distribution> _distributions { get; set; } = new();
        private List<User> _UserOfArea { get; set; } = new();
        private List<ILULevel> _LevelsILU { get; set; } = new();
        //private ILURegister[,] ILU_Matrix { get; set; } = new ILURegister[0,0];
        private Dictionary<(int, int), List<ILURegister>?> ILU_Matrix { get; set; } = new Dictionary<(int, int), List<ILURegister>?>();


        private Dictionary<int, bool> Distributions_Knolowed { get; set; } = new Dictionary<int, bool>();
        private Dictionary<int, bool> User_Knolowed { get; set; } = new Dictionary<int, bool>();

        bool ShowTable = false;



        //dialog
        bool AddILUVisibleDialog = false;
        bool ILUHistoryDialog = false;
        bool ILUHistoryOperationDialog = false;
        private ILURegister _newIlu { get; set; } = new();
        private List<ILURegister> AllRegistersOfPat { get; set; } = new();
        private int auxILU_Level = 0;
        private int auxILU_UseId = 0;
        private int auxILU_OpId = 0;

        private List<ILURegister> AllRegistersOperationsInUser { get; set; } = new();
        private List<ILURegister> AllRegistersUsersInOperation { get; set; } = new();

        private TableGroupDefinition<ILURegister> UserInOperations = new()
        {
            GroupName = "User",
            Indentation = false,
            Expandable = false,
            Selector = (e) => e.OperatorId
        };

        private TableGroupDefinition<ILURegister> OperationsInUser = new()
        {
            GroupName = "Operation",
            Indentation = false,
            Expandable = false,
            Selector = (e) => e.DistributionId
        };

        private List<BreadcrumbItem> _links;

        //User
        private string json = string.Empty;
        public User user = new();
        public bool logged = false;

        private IList<string> _sourceMsgLoading = new List<string>();
        private IList<Color> _Colors = new List<Color>() { Color.Default, Color.Primary, Color.Secondary, Color.Success, Color.Info, Color.Default, Color.Primary, Color.Secondary, Color.Success, Color.Info };

        //Create Job Variables

        private int distribution_id { get; set; }
        private int operator_id { get; set; }
        private string ProgrammedStartDate { get; set; }


        private string leader { get; set; } = "S";
        private int? peopleCount { get; set; }

        private List<string> monthsNames = new List<string>
        {
            "ENE", "FEB", "MAR", "ABR", "MAY", "JUN",
            "JUL", "AGO", "SEP", "OCT", "NOV", "DIC"
        };
        private double?[] monthsDistributionPercentage = new double?[12];
        private double?[] monthsUsersPercentage = new double?[12];


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

            _links = new List<BreadcrumbItem>
            {
                new BreadcrumbItem(text: Localizer["home"], href: "/"),
                new BreadcrumbItem("PAT", href: "/PAT"),
                new BreadcrumbItem(text: Localizer["PATDetails"], href: "", disabled: true),
            };

            BreadcrumbService.UpdateBreadcrumbs(_links);
            logged = await HasPropertyAsync();

            if (!logged)
            {
                Snackbar.Clear();
                Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                Snackbar.Add($"Error You have to log in", Severity.Error);
                NavigationManager.NavigateTo($"/");
            }
            else
            {
                await GetUserAsync();

                _pat = await PATsServices.getPat(patID);

                await PrepareDataTable();
                StateHasChanged();
            }


        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {

            //await JS.InvokeVoidAsync("blazorUtils.truncateText");

        }

        //Local storage user
        private async Task GetUserAsync()
        {
            if (!await TryGetAsync())
            {
                user = new();
            }
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




        private async Task PrepareDataTable()
        {
            AllRegistersOfPat?.Clear();
            ILU_Matrix?.Clear();
            Distributions_Knolowed?.Clear();
            User_Knolowed?.Clear();
            _LevelsILU?.Clear();
            _distributions?.Clear();
            _UserOfArea?.Clear();

            _LevelsILU = await ILUServices.GetLevelsILU();
            _distributions = await DistributionsServices.GetDistributions(_pat.PlantId, _pat.AreaId);
            _UserOfArea = await UsersServices.GetSubordinates((int)_pat.Supervisor.UserId);
            _UserOfArea.Insert(0, _pat.Supervisor);
            //_operations = await OperationsServices.GetOperations(_pat.PlantId, _pat.AreaId, _pat.DistributionId);
            //_UserOfArea = await UsersServices.GetUsersWhitCollections();

            try
            {
                if (!string.IsNullOrWhiteSpace(_pat.HistoricalAbility))
                {
                    var parsedData = JsonSerializer.Deserialize<List<Dictionary<string, Dictionary<string, double>>>>(_pat.HistoricalAbility);

                    if (parsedData == null || parsedData.Count != 12)
                        throw new InvalidOperationException("El JSON debe contener datos para exactamente 12 meses.");

                    for (int i = 0; i < parsedData.Count; i++)
                    {
                        var monthKey = parsedData[i].Keys.First();
                        var monthData = parsedData[i][monthKey];

                        monthsDistributionPercentage[i] = monthData.ContainsKey("OR_O") && monthData["OR_O"] != 0.0 ? monthData["OR_O"] : null;
                        monthsUsersPercentage[i] = monthData.ContainsKey("OR_P") && monthData["OR_P"] != 0.0 ? monthData["OR_P"] : null;
                    }
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error preparando los datos: {ex.Message}");
            }

            foreach (var op in _distributions)
            {
                foreach (var usr in _UserOfArea)
                {
                    // Obtener los registros correspondientes a la operaci�n y usuario actual
                    var matchingRegisters = usr.ILURegisers?
                        .Where(r => r.DistributionId == op.DistributionId && r.OperatorId == usr.UserId && int.Parse(r.AcquisitionDate?.ToString("yyyy")) <= _pat.AplicationYear)
                        .OrderByDescending(r => r.AcquisitionDate)
                        .ToList();

                    AllRegistersOfPat.AddRange(matchingRegisters?.ToList());
                    ILU_Matrix.Add((op.DistributionId, usr.UserId), matchingRegisters);
                    // Almacenar los registros en la
                    //ILU_Matrix[op.OperationId, usr.UserId] = matchingRegisters;

                }
            }

            if (_pat.PatDistributionComments is null)
            {
                _pat.PatDistributionComments = new List<PatDistributionComment>();
            }
            foreach (var op in _distributions)
            {
                if (AllRegistersOfPat.FindIndex(r => r.DistributionId == op.DistributionId) != -1)
                {
                    Distributions_Knolowed.Add(op.DistributionId, true);
                }
                else
                {
                    Distributions_Knolowed.Add(op.DistributionId, false);

                }

                if (!_pat.PatDistributionComments.Any(dc => dc.DistributionId == op.DistributionId))
                {
                    PatDistributionComment newPatDistributionComment = new PatDistributionComment
                    {
                        DistributionId = op.DistributionId,
                        IsActive = true,
                        PATId = patID,
                    };
                    _pat.PatDistributionComments.Add(newPatDistributionComment);
                }
            }

            if (_pat.PatUserRoles is null)
            {
                _pat.PatUserRoles = new List<PatUserRole>();
            }

            foreach (var usr in _UserOfArea)
            {
                if (AllRegistersOfPat.FindIndex(r => r.OperatorId == usr.UserId) != -1)
                {
                    User_Knolowed.Add(usr.UserId, true);
                }
                else
                {
                    User_Knolowed.Add(usr.UserId, false);

                }


                if (!_pat.PatUserRoles.Any(ur => ur.UserId == usr.UserId))
                {
                    PatUserRole newPatUserRole = new PatUserRole
                    {
                        UserId = usr.UserId,
                        IsActive = true,
                        PATId = patID,
                    };

                    _pat.PatUserRoles.Add(newPatUserRole);
                }
            }

            ShowTable = true;
            StateHasChanged();
            await JSRuntime.InvokeVoidAsync("blazorUtils.setDynamicLeft");

        }

        private void updateILULevel()
        {
            _newIlu.ILULevelId = auxILU_Level;
        }

        private async void OpenHistoryILU(int ID_User)
        {
            auxILU_UseId = ID_User;

            AllRegistersOperationsInUser?.Clear();
            foreach (var op in _distributions)
            {
                if (ILU_Matrix.TryGetValue((op.DistributionId, auxILU_UseId), out var context))
                {
                    var latestContext = context.OrderByDescending(c => c.AcquisitionDate);

                    if (latestContext?.Count() > 0)
                    {
                        AllRegistersOperationsInUser.AddRange(latestContext);
                    }
                }
            }
            ILUHistoryDialog = true;
            StateHasChanged();
        }
        private async void OpenHistoryILU_Operation(int Op_User)
        {
            auxILU_OpId = Op_User;

            AllRegistersUsersInOperation?.Clear();
            foreach (var usr in _UserOfArea)
            {
                if (ILU_Matrix.TryGetValue((auxILU_OpId, usr.UserId), out var context))
                {
                    var latestContext = context.OrderByDescending(c => c.AcquisitionDate);

                    if (latestContext?.Count() > 0)
                    {
                        AllRegistersUsersInOperation.AddRange(latestContext);
                    }
                }
            }
            ILUHistoryOperationDialog = true;
            StateHasChanged();
        }

        private async void CloseHistoryILU()
        {
            ILUHistoryDialog = false;
            StateHasChanged();
        }
        private async void CloseHistory_OperationILU()
        {
            ILUHistoryOperationDialog = false;
            StateHasChanged();
        }

        private async void OpenDialogAddILU(int ID_Operation, int ID_User)
        {
            _newIlu.DistributionId = ID_Operation;
            _newIlu.OperatorId = ID_User;
            _newIlu.isActive = true;
            auxILU_Level = 0;
            AddILUVisibleDialog = true;
            StateHasChanged();
        }

        private async void AddILUClose()
        {
            Snackbar.Clear();
            Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;



            _newIlu.AcquisitionDate = DateTime.Now;
            var filteredDistributions = _distributions.Where(dist => dist.DistributionId == _newIlu.DistributionId);

            bool hasLeadersInDistribution = false;
            var validLevelIds = new HashSet<int> { 3, 5, 7, 9, 10 };
            string leaderName = "";

            if (_newIlu?.ILULevelId is int iluLevelId && validLevelIds.Contains(iluLevelId))
            {

                foreach (var op in filteredDistributions)
                {
                    var leaders = _UserOfArea
                        .Where(usr => ILU_Matrix.TryGetValue((op.DistributionId, usr.UserId), out var context) && context.Any())
                        .Select(usr => new
                        {
                            User = usr,
                            LatestContext = ILU_Matrix[(op.DistributionId, usr.UserId)]
                                .OrderByDescending(c => c.AcquisitionDate)
                                .FirstOrDefault()
                        })
                        .Where(x => x.LatestContext != null)
                        .Select(x => new
                        {
                            x.User,
                            ILULevelNumber = _LevelsILU
                                .Find(u => u.ILULevelId == x.LatestContext.ILULevelId)?.ILULevelCode
                        })
                        .Where(x => x.ILULevelNumber != null && x.ILULevelNumber != "�" && x.User.UserId != _newIlu.OperatorId)
                        .ToList();

                    var firstLeader = leaders.FirstOrDefault(x =>
                                x.ILULevelNumber == "ILeader" ||
                                x.ILULevelNumber == "LTraineeLeader" ||
                                x.ILULevelNumber == "LLeader" ||
                                x.ILULevelNumber == "ULeaderTrainee" ||
                                x.ILULevelNumber == "ULeader");

                    if (firstLeader != null)
                    {
                        leaderName = firstLeader.User.Name;
                        hasLeadersInDistribution = true;
                        break;
                    }
                }
            }


            if (!hasLeadersInDistribution)
            {
                ShowTable = false;
                AddILUVisibleDialog = false;
                var result = await ILUServices.AddRegisterForUser(_newIlu, (int)_newIlu.OperatorId);
                if (result != null)
                {
                    _pat = await PATsServices.getPat(patID);
                    await PrepareDataTable();
                    Snackbar.Add($"ILU Level Added", Severity.Success);
                }

            }
            else
            {
                Snackbar.Add($"{leaderName} is already a Leader in this distribution!", Severity.Warning);
            }

        }

        private DialogOptions dialogOptions = new() { CloseOnEscapeKey = true, MaxWidth = MaxWidth.Large, FullWidth = true, DisableBackdropClick = true, CloseButton = true };

        bool CreateILUJob = false;
        void CreateJobObservation(int distributionId, int operatorId)
        {
            distribution_id = distributionId;
            operator_id = operatorId;

            if (CultureInfo.CurrentCulture.Name == "en-US")
            {
                var date = DateTime.ParseExact(DateTime.Now.ToShortDateString(), "M/d/yyyy", CultureInfo.InvariantCulture);
                var formatedDate = date;

                var EnglishDate = formatedDate.Day.ToString() + "/" + formatedDate.Month.ToString() + "/" + formatedDate.Year.ToString();
                var dateString = EnglishDate.Replace("/", "-");
                ProgrammedStartDate = dateString;

            }
            else
            {
                var date = DateTime.ParseExact(DateTime.Now.ToShortDateString(), "d/M/yyyy", CultureInfo.InvariantCulture);
                var dateString = date.ToShortDateString().Replace("/", "-");
                ProgrammedStartDate = dateString;

            }
            CreateILUJob = true;
        }
        void CloseILUJob() => CreateILUJob = false;

        //Finished Job observation
        private bool visibleSign = false;
        private void OpenSignComment()
        {
            visibleSign = true;
        }
        void CloseSign() => visibleSign = false;
        private DialogOptions dialogSignOptions = new() { CloseOnEscapeKey = true, MaxWidth = MaxWidth.ExtraSmall, FullWidth = true, DisableBackdropClick = true, CloseButton = true };

        public async Task ApprovePat()
        {

            _pat!.Status = 6;
            var result = await PATsServices.UpdatePat(_pat);

            if (result)
            {
                Snackbar.Clear();
                Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                Snackbar.Add($"PAT Approved succesfully!", Severity.Info);
                NavigationManager.NavigateTo($"PAT");
            }

            visibleSign = false;
        }


        // Distributions

        private bool HasLowLevel(IDictionary<(int DistributionId, int UserId), List<ILURegister>> iluMatrix, IEnumerable<Distribution> distributions, int userId)
        {
            return distributions.Any(op =>
            {
                if (iluMatrix.TryGetValue((op.DistributionId, userId), out var context))
                {
                    var distinctRecords = context?
                        .GroupBy(c => new { c.DistributionId, c.OperatorId })
                        .Select(g => g.OrderByDescending(c => c.AcquisitionDate).FirstOrDefault());

                    return distinctRecords?.Any(r => r?.ILULevelId != 0 && r?.ILULevelId < 5) ?? false;
                }
                return false;
            });
        }

        private int CountHighLevel(IDictionary<(int DistributionId, int UserId), List<ILURegister>> iluMatrix, IEnumerable<Distribution> distributions, int userId)
        {
            return distributions.Sum(op =>
            {
                if (iluMatrix.TryGetValue((op.DistributionId, userId), out var context))
                {
                    var distinctRecords = context?
                        .GroupBy(c => new { c.DistributionId, c.OperatorId })
                        .Select(g => g.OrderByDescending(c => c.AcquisitionDate).FirstOrDefault());

                    return distinctRecords?.Count(r => r?.ILULevelId != 0 && r?.ILULevelId > 5) ?? 0;
                }
                return 0;
            });
        }

        // Users

        private bool HasLowLevelForUsers(IDictionary<(int DistributionId, int UserId), List<ILURegister>> iluMatrix, IEnumerable<User> users, int distributionId)
        {
            return users.Any(usr =>
            {
                if (iluMatrix.TryGetValue((distributionId, usr.UserId), out var context))
                {
                    var distinctRecords = context?
                        .GroupBy(c => new { c.DistributionId, c.OperatorId })
                        .Select(g => g.OrderByDescending(c => c.AcquisitionDate).FirstOrDefault());

                    return distinctRecords?.Any(r => r?.ILULevelId != 0 && r?.ILULevelId < 5) ?? false;
                }
                return false;
            });
        }

        private int CountHighLevelForUsers(IDictionary<(int DistributionId, int UserId), List<ILURegister>> iluMatrix, IEnumerable<User> users, int distributionId)
        {
            return users.Sum(usr =>
            {
                if (iluMatrix.TryGetValue((distributionId, usr.UserId), out var context))
                {
                    var distinctRecords = context?
                        .GroupBy(c => new { c.DistributionId, c.OperatorId })
                        .Select(g => g.OrderByDescending(c => c.AcquisitionDate).FirstOrDefault());

                    return distinctRecords?.Count(r => r?.ILULevelId != 0 && r?.ILULevelId > 5) ?? 0;
                }
                return 0;
            });
        }

        public int countUserO { get; set; } = 0;
        public int countUserX { get; set; } = 0;
        public int countDistO { get; set; } = 0;
        public int countDistX { get; set; } = 0;

        private double? CalculateUserPercentage()
        {
            if (_pat.KnowledgePercentage == null || !_distributions.Any() || !_UserOfArea.Any())
                return null;

            countUserO = 0;
            countUserX = 0;

            foreach (var op in _distributions)
            {
                var sum = CountHighLevelForUsers(ILU_Matrix, _UserOfArea, op.DistributionId);
                var hasLowLevel = HasLowLevelForUsers(ILU_Matrix, _UserOfArea, op.DistributionId);
                var meetsKnowledge = sum >= _pat.KnowledgePercentage;

                if (meetsKnowledge)
                {
                    countUserO++;
                }
                else if (hasLowLevel && !meetsKnowledge || !hasLowLevel && !meetsKnowledge && sum > 0)
                {
                    countUserX++;
                }
            }

            return (countUserO + countUserX) > 0 ? (double)countUserO / (countUserO + countUserX) * 100 : null;
        }

        private double? CalculateDistributionPercentage()
        {
            if (_pat.KnowledgePercentage == null || !_distributions.Any() || !_UserOfArea.Any())
                return null;

            countDistO = 0;
            countDistX = 0;

            foreach (var usr in _UserOfArea)
            {
                var sum = CountHighLevel(ILU_Matrix, _distributions, usr.UserId);
                var hasLowLevel = HasLowLevel(ILU_Matrix, _distributions, usr.UserId);
                var meetsKnowledge = sum >= _pat.KnowledgePercentage;

                if (meetsKnowledge)
                {
                    countDistO++;
                }
                else if (hasLowLevel && !meetsKnowledge || !hasLowLevel && !meetsKnowledge && sum > 0)
                {
                    countDistX++;
                }
            }

            return (countDistO + countDistO) > 0 ? (double)countDistO / (countDistO + countDistX) * 100 : null;
        }

        private async void DownloadExcel()
        {
            if (_pat.KnowledgePercentage != null || _pat.KnowledgePercentage != 0) 
            {
                await Exportation.ExportYearlyPATToExcel(_pat.PATid);
            }
            else
            {
                Snackbar.Add($"First fill the rotation target", Severity.Warning);
            }
        }


        //In progress
        private async Task SaveProgressPat()
        {

            _pat.Status = 2;

            SetHistoricalAbility();
            Console.WriteLine(_pat.HistoricalAbility);

            var result = await PATsServices.UpdatePat(_pat);


            if (result)
            {
                Snackbar.Clear();
                Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                Snackbar.Add($"PAT {_pat.PATid} Updated", Severity.Info);
                NavigationManager.NavigateTo("/Pat");
            }
            else
                await JSRuntime.InvokeVoidAsync("alert", "Update failed!"); // Alert
        }


        public void SetHistoricalAbility()
        {
            try
            {
                var result = new List<Dictionary<string, Dictionary<string, double>>>();

                for (int i = 0; i < monthsNames.Count; i++)
                {
                    double or_o = monthsDistributionPercentage[i] ?? 0.0;
                    double or_p = monthsUsersPercentage[i] ?? 0.0;

                    var monthData = new Dictionary<string, double>
                    {
                        { "OR_O", or_o },
                        { "OR_P", or_p }
                    };

                    result.Add(new Dictionary<string, Dictionary<string, double>>
                    {
                        { monthsNames[i], monthData }
                    });
                }

                _pat.HistoricalAbility = JsonSerializer.Serialize(result, new JsonSerializerOptions { WriteIndented = true });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error generando el JSON: {ex.Message}");
            }
        }


    }//end class pat details


}//end namespace