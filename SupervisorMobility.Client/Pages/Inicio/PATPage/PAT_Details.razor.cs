using Microsoft.JSInterop;
using MudBlazor;
using SupervisorMobility.Client.Data.Entities;
using System.Globalization;
using System.Linq;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Mvc;
using DocumentFormat.OpenXml.InkML;
using SupervisorMobility.Client.Pages.Inicio.HCIPage.Components;

namespace SupervisorMobility.Client.Pages.Inicio.PATPage
{
    public partial class PAT_Details
    {
        [Parameter]
        public int patID { get; set; }

        private PAT? _pat { get; set; } = new();
        private List<Distribution> _distributions { get; set; } = new();
        private List<User> _UserOfMonth { get; set; } = new();
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
        public int userHciId = 0;
        private bool UserHasHci = false;

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
        private List<int> _visibleSubordinateIds;
        bool Dev_env { get; set; }

        protected async override Task OnInitializedAsync()
        {
            Dev_env = Environment.IsDevelopment();

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
                _LevelsILU = await ILUServices.GetLevelsILU();


                await PrepareDataTable();
                StateHasChanged();
            }

            if ((int)_pat.AplicationYear > DateTime.Now.Year)
            {
                LastdayYear = new DateTime((int)_pat.AplicationYear, 12, 31);
                _yearMonth = new DateTime((int)_pat.AplicationYear, 1, 1);
                FirstdayYear = new DateTime((int)_pat.AplicationYear, 1, 1);
                date = new DateTime((int)_pat.AplicationYear, 1, 1);
            }
            else
            {
                LastdayYear = new DateTime((int)_pat.AplicationYear, 12, 31);
                _yearMonth = new DateTime((int)_pat.AplicationYear, DateTime.Now.Month, DateTime.Now.Day);
                FirstdayYear = new DateTime((int)_pat.AplicationYear, DateTime.Now.Month, DateTime.Now.Day);
                date = new DateTime((int)_pat.AplicationYear, DateTime.Now.Month, DateTime.Now.Day).AddMonths(-1);
            }
            StateHasChanged();

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
            _distributions?.Clear();
            _UserOfArea?.Clear();

            _distributions = await DistributionsServices.GetDistributions(_pat.PlantId, _pat.AreaId);

            foreach (User sv in _pat.Supervisors)
            {
                _UserOfArea.AddRange(await UsersServices.GetSubordinates(sv.UserId));
                _UserOfArea.Insert(0, sv);
            }

            var newSubordinates = _UserOfArea
              .Where(user => !_pat.PatSubordinates.Any(ps => ps.UserId == user.UserId))
                  .ToList();

            foreach (var user in newSubordinates)
            {
                _pat.PatSubordinates.Add(new PatSubordinate
                {
                    PatId = _pat.PATid,
                    UserId = user.UserId,
                    PatSubordinateDates = new List<PatSubordinateDates>
                    {
                        new PatSubordinateDates
                        {
                            StartDate = DateTime.Now,
                            EndDate = null
                        }
                    }
                });
            }


            foreach (var patSubordinate in _pat.PatSubordinates)
            {
                var endDate = patSubordinate.PatSubordinateDates?.LastOrDefault()?.EndDate;

                if (!_UserOfArea.Any(user => user.UserId == patSubordinate.UserId) && endDate == null)
                {
                    patSubordinate.PatSubordinateDates?.Add(new PatSubordinateDates
                    {
                        EndDate = DateTime.Now
                    });
                }
                else if (!_UserOfArea.Any(user => user.UserId == patSubordinate.UserId) && endDate != null)
                {
                    _UserOfArea.Add(await UsersServices.GetUserAndCollection(patSubordinate.UserId));
                }
            }
            StateHasChanged();

           


            foreach (var op in _distributions)
            {
                foreach (var usr in _UserOfArea)
                {
                    // Obtener los registros correspondientes a la operaci�n y usuario actual
                    var matchingRegisters = usr.ILURegisers?
                        .Where(r => r.DistributionId == op.DistributionId && r.OperatorId == usr.UserId && int.Parse(r.AcquisitionDate?.ToString("yyyy")) <= _pat.AplicationYear)
                        .OrderByDescending(r => r.AcquisitionDate)
                        .ToList();
                    
                    if(matchingRegisters?.Count() > 0)
                    {
                        AllRegistersOfPat?.AddRange(matchingRegisters.ToList());
                    }

                    ILU_Matrix?.Add((op.DistributionId, usr.UserId), matchingRegisters);
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

            SetHistoricalAbility();
            LoadHistoricalAbility();
            
            ShowTable = true;
            StateHasChanged();
            await JSRuntime.InvokeVoidAsync("blazorUtils.setDynamicLeft");

        }

        public void SetHistoricalAbility()
        {
            try
            {
                var result = new List<Dictionary<string, Dictionary<string, double>>>();

                for (int i = 0; i < monthsNames.Count; i++)
                {

                    double or_o = CalculateDistributionPercentageMonthAcumulted(i + 1) ?? 0.0;
                    double or_p = CalculateUserPercentageMonthAcumulted(i + 1) ?? 0.0;

                    // Si es null o 0, intenta calcularlo
                    if (or_o == null || or_o == 0.0)
                    {
                        or_o = monthsDistributionPercentage[i] ?? (CalculateDistributionPercentageMonthAcumulted(i + 1) ?? 0.0);
                    }


                    if (or_p == null || or_p == 0.0)
                    {
                        or_p = monthsUsersPercentage[i] ?? (CalculateUserPercentageMonthAcumulted(i + 1) ?? 0.0);
                    }

                    var monthData = new Dictionary<string, double>
            {
                { "OR_O", or_o },
                { "OR_P", or_p }
            };

                    result.Add(new Dictionary<string, Dictionary<string, double>>
            {
                { monthsNames[i], monthData }
            });
                    Console.WriteLine($"Month {i}: {monthsNames[i]}, OR_O: {or_o}, OR_P: {or_p}");
                }

                _pat.HistoricalAbility = JsonSerializer.Serialize(result, new JsonSerializerOptions { WriteIndented = true });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error generando el JSON: {ex.Message}");
            }
        }
        void LoadHistoricalAbility()
        {
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

                        monthsDistributionPercentage[i] = monthData.ContainsKey("OR_O") && monthData["OR_O"] != 0.0 ? monthData["OR_O"] : (CalculateDistributionPercentageMonth(i + 1) ?? null);
                        monthsUsersPercentage[i] = monthData.ContainsKey("OR_P") && monthData["OR_P"] != 0.0 ? monthData["OR_P"] : (CalculateUserPercentageMonth(i + 1) ?? null);
                    }
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error preparando los datos: {ex.Message}");
            }
        }


        private async void OpenHistoryILU(int ID_User)
        {
            UserHasHci = false;
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

            if(_UserOfArea.Where( u=> u.UserId == auxILU_UseId && u.HciId != null && u.HciId != 0).Any()){
                userHciId = (int)_UserOfArea.Find(u => u.UserId == ID_User).HciId;
                UserHasHci = true;
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

        private DialogOptions dialogOptions = new() { CloseOnEscapeKey = true, MaxWidth = MaxWidth.Large, FullWidth = true, DisableBackdropClick = true, CloseButton = true };


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
                else if (hasLowLevel && !meetsKnowledge || !hasLowLevel && !meetsKnowledge && sum > 0 || !meetsKnowledge)
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

            int index = 0;

            foreach (var usr in _UserOfArea)
            {
                var role = _pat.PatUserRoles?.ElementAtOrDefault(index)?.Role;
                var isSaveLeaderS = _pat.SaveLeader == "S";
                var isSaveLeaderC = _pat.SaveLeader == "C";
                var isRoleRelevant = role == null || role == OperatorRole.Lider || role == OperatorRole.CA;

                if(!((role == null && isSaveLeaderS) || (isRoleRelevant && isSaveLeaderC)))
                {
                    index++;
                    continue;
                }

                var sum = CountHighLevel(ILU_Matrix, _distributions, usr.UserId);
                var hasLowLevel = HasLowLevel(ILU_Matrix, _distributions, usr.UserId);
                var meetsKnowledge = sum >= _pat.KnowledgePercentage;

                if (meetsKnowledge)
                {
                    countDistO++;
                }
                else if (hasLowLevel && !meetsKnowledge || !hasLowLevel && !meetsKnowledge && sum > 0 || !meetsKnowledge)
                {
                    countDistX++;
                }
                index++;

            }

            return (countDistO + countDistO) > 0 ? (double)countDistO / (countDistO + countDistX) * 100 : null;
        }

        private async void DownloadExcel()
        {
            if (_pat.KnowledgePercentage != null || _pat.KnowledgePercentage != 0) 
            {
                if (MonthlyView)
                {
                    //aqui funcion par exportar al mes
                    await Exportation.ExportMonthlyPATToExcel(_pat.PATid, _yearMonth.Value.Month);

                }
                else
                {
                    await Exportation.ExportYearlyPATToExcel(_pat.PATid);
                }
            }
            else
            {
                Snackbar.Add($"First fill the rotation target", Severity.Warning);
            }
        }


        // Zoom
        private bool IsZoomed = false;
        private string dynamicStyle => $"overflow-x: auto; height: {viewHeigh}vh;";

        public int viewHeigh = 82;
        private async Task ToggleZoom()
        {
            IsZoomed = !IsZoomed;
            viewHeigh = IsZoomed ? 105 : 82;
            var zoomLevel = IsZoomed ? "0.75" : "1.0";
            await JSRuntime.InvokeVoidAsync("setZoom", zoomLevel);
        }

        #region Calendario
        //Montly
        bool AllHistory = false;
        bool MonthlyView = false;
        DateTime? _yearMonth;
        public DateTime? date;
        DateTime FirstdayYear = DateTime.Now;
        DateTime LastdayYear = DateTime.Now;
        private string month;
        private string year;
        private async void MontlyTab()
        {
            FilterUserMonth();

            StateHasChanged();

            MonthlyView = true;

            StateHasChanged();
        }
        private async void OnDateChanged(DateTime? value)
        {
            

            _yearMonth = value;
            
            month = $"{_yearMonth?.ToString("MMMM")}";
            year = $"{_yearMonth?.ToString("yyyy")}";
            int monthIndex = DateTime.ParseExact(month, "MMMM", System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat).Month;
            int yearIndex = DateTime.ParseExact(year, "yyyy", System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat).Year;
            
            Console.WriteLine($"_visibleSubordinateIds: {_visibleSubordinateIds.Count()}");
            Console.WriteLine($"Before Month: {month}, _Users: {_UserOfArea.Count()}, _Mounth: {_UserOfMonth.Count()}");
            await Task.Run(() => FilterUserMonth());
            await Task.Run(() => _UserOfMonth = _UserOfArea.Where(u => _visibleSubordinateIds.Contains(u.UserId)).ToList());
            Console.WriteLine($"_visibleSubordinateIds: {_visibleSubordinateIds.Count()}");
            Console.WriteLine($"Selected Month: {month}, _Users: {_UserOfArea.Count()}, _Mounth: {_UserOfMonth.Count()}");
            StateHasChanged();
        }

        public async Task LastMonth()
        {
            _yearMonth = _yearMonth?.AddMonths(-1);
           
            month = $"{_yearMonth?.ToString("MMMM")}";
            year = $"{_yearMonth?.ToString("yyyy")}";
            int monthIndex = DateTime.ParseExact(month, "MMMM", System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat).Month;
            int yearIndex = DateTime.ParseExact(year, "yyyy", System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat).Year;


            StateHasChanged();
        }

        public async Task NextMonth()
        {
            _yearMonth = _yearMonth?.AddMonths(1);

            month = $"{_yearMonth?.ToString("MMMM")}";
            year = $"{_yearMonth?.ToString("yyyy")}";
            int monthIndex = DateTime.ParseExact(month, "MMMM", System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat).Month;
            int yearIndex = DateTime.ParseExact(year, "yyyy", System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat).Year;

            StateHasChanged();
        }
        #endregion
        private void FilterUserMonth()
        {
           _visibleSubordinateIds = _pat.PatSubordinates
                                               .Where(ps =>
                                                   ps.PatSubordinateDates != null && ps.PatSubordinateDates.Any(d =>
                                                       d.StartDate.Month <= _yearMonth.Value.Date.Month &&
                                                       (d.EndDate == null || d.EndDate.Value.Month >= _yearMonth.Value.Date.Month)
                                                   )
                                               )
                                               .Select(ps => ps.UserId)
                                               .ToList();
        }

        private void FilterUserByMonth(int IndexMonth)
        {
            _visibleSubordinateIds = _pat.PatSubordinates
                                                .Where(ps =>
                                                    ps.PatSubordinateDates != null && ps.PatSubordinateDates.Any(d =>
                                                        d.StartDate.Month <= IndexMonth &&
                                                        (d.EndDate == null || d.EndDate.Value.Month >= IndexMonth)
                                                    )
                                                )
                                                .Select(ps => ps.UserId)
                                                .ToList();
        }



        #region HCI

        public List<HCIILU> userExpertise { get; set; } = new();
        public HCI _hci = new();

        private async void UpdateHci()
        {
            //foreach (PatSubordinate patSubordinate in _pat.PatSubordinates)
            //{
            //    userExpertise = new();
            //    List<ILURegister> allRegistersOperationsInUser = new();

            //    foreach (var op in _distributions)
            //    {
            //        if (ILU_Matrix.TryGetValue((op.DistributionId, patSubordinate.UserId), out var context))
            //        {
            //            var latestContext = context?.OrderByDescending(c => c.AcquisitionDate);

            //            if (latestContext?.Count() > 0)
            //            {
            //                allRegistersOperationsInUser.AddRange(latestContext);
            //            }
            //        }
            //    }
            //    Console.WriteLine("User: " + patSubordinate.UserId);
            //    foreach(var context in allRegistersOperationsInUser)
            //    {
            //        userExpertise.Add(new HCIILU
            //        {
            //            Start = context.AcquisitionDate,
            //            Description = context.DistributionId.ToString(),
            //            level = _LevelsILU.Find(u => u.ILULevelId == context.ILULevelId).ILULevelCode,
            //            RegisterILURegisterid = context.ILURegisterid
            //        });

            //        Console.WriteLine(context.AcquisitionDate?.ToString("dd/MM/yyyy"));
            //        Console.WriteLine(context.DistributionId);
            //        Console.WriteLine(_LevelsILU.Find(u => u.ILULevelId == context.ILULevelId).ILULevelCode);
            //    }

            //    _hci = await HCIServices.GetHCI(patSubordinate.UserId);
            //    _hci.ILUs = userExpertise;

            //    if (await HCIServices.UpdateHCI(_hci))
            //    {
            //        Snackbar.Add("Updated succesfully", Severity.Success);
            //    }
            //    else
            //    {
            //        Snackbar.Add("Error", Severity.Error);
            //    }
            //    Console.WriteLine("--------");
            //}
        }

        #endregion


        int JobObservationId { get; set; } = 0;
        bool visibleJobDetails { get; set; } = false;
        private async void OpenJobDetailsDialog(int? jobId = 0)
        {
            if (jobId.HasValue && jobId.Value != 0)
            {
                JobObservationId = jobId.Value;
                visibleJobDetails = true;
            }
        }

        void EditJobObservation(int jobObservationId)
        {
            NavigationManager.NavigateTo($"jobobservation/updatejobobservation/{jobObservationId}");
        }
        void Close() => visibleJobDetails = false;


        private double? CalculateDistributionPercentageMonth(int MonthIndex)
        {
            if (_pat.KnowledgePercentage == null || !_distributions.Any() || !_UserOfArea.Any())
                return null;


            countDistO = 0;
            countDistX = 0;

            int index = 0;

            foreach (var usr in _UserOfArea)
            {
                var role = _pat.PatUserRoles?.ElementAtOrDefault(index)?.Role;
                var isSaveLeaderS = _pat.SaveLeader == "S";
                var isSaveLeaderC = _pat.SaveLeader == "C";
                var isRoleRelevant = role == null || role == OperatorRole.Lider || role == OperatorRole.CA;

                if (!((role == null && isSaveLeaderS) || (isRoleRelevant && isSaveLeaderC)))
                {
                    index++;
                    continue;
                }

                // Filtrar solo los registros del mes correspondiente
                int sum = 0;
                bool hasLowLevel = false;

                foreach (var op in _distributions)
                {
                    if (ILU_Matrix.TryGetValue((op.DistributionId, usr.UserId), out var context) && context != null)
                    {
                        // Tomar el registro más reciente del mes correspondiente
                        var record = context
                            .Where(r => r.AcquisitionDate.HasValue && r.AcquisitionDate.Value.Month == MonthIndex)
                            .OrderByDescending(r => r.AcquisitionDate)
                            .FirstOrDefault();

                        if (record != null)
                        {
                            if (record.ILULevelId != 0 && record.ILULevelId > 5)
                                sum++;
                            if (record.ILULevelId != 0 && record.ILULevelId < 5)
                                hasLowLevel = true;
                        }
                    }
                }

                var meetsKnowledge = sum >= _pat.KnowledgePercentage;

                if (meetsKnowledge)
                {
                    countDistO++;
                }
                else if ((hasLowLevel && !meetsKnowledge) || (!hasLowLevel && !meetsKnowledge && sum > 0) || !meetsKnowledge)
                {
                    countDistX++;
                }
                index++;
            }

            return (countDistO + countDistX) > 0 ? (double)countDistO / (countDistO + countDistX) * 100 : null;
        }

        private double? CalculateUserPercentageMonth(int MonthIndex)
        {
            if (_pat.KnowledgePercentage == null || !_distributions.Any() || !_UserOfArea.Any())
                return null;

            countUserO = 0;
            countUserX = 0;

            foreach (var op in _distributions)
            {
                int sum = 0;
                bool hasLowLevel = false;

                foreach (var usr in _UserOfArea)
                {
                    if (ILU_Matrix.TryGetValue((op.DistributionId, usr.UserId), out var context) && context != null)
                    {
                        // Tomar el registro más reciente del mes correspondiente
                        var record = context
                            .Where(r => r.AcquisitionDate.HasValue && r.AcquisitionDate.Value.Month == MonthIndex)
                            .OrderByDescending(r => r.AcquisitionDate)
                            .FirstOrDefault();

                        if (record != null)
                        {
                            if (record.ILULevelId != 0 && record.ILULevelId > 5)
                                sum++;
                            if (record.ILULevelId != 0 && record.ILULevelId < 5)
                                hasLowLevel = true;
                        }
                    }
                }

                var meetsKnowledge = sum >= _pat.KnowledgePercentage;

                if (meetsKnowledge)
                {
                    countUserO++;
                }
                else if (hasLowLevel && !meetsKnowledge || !hasLowLevel && !meetsKnowledge && sum > 0 || !meetsKnowledge)
                {
                    countUserX++;
                }
            }

            return (countUserO + countUserX) > 0 ? (double)countUserO / (countUserO + countUserX) * 100 : null;
        }

       

        private double? CalculateDistributionPercentageMonthAcumulted(int MonthIndex)
        {
            if (_pat.KnowledgePercentage == null || !_distributions.Any() || !_UserOfArea.Any() || (MonthIndex > DateTime.Today.Month && _pat.AplicationYear == DateTime.Today.Year))
                return null;

            // Filtrar usuarios activos en el mes
            FilterUserByMonth(MonthIndex);
            var activeUserIds = _visibleSubordinateIds ?? new List<int>();
            var activeUsers = _UserOfArea.Where(u => activeUserIds.Contains(u.UserId)).ToList();

            countDistO = 0;
            countDistX = 0;

            int index = 0;

            foreach (var usr in activeUsers)
            {
               var role = _pat.PatUserRoles?.FirstOrDefault(r => r.UserId == usr.UserId)?.Role;

                var isSaveLeaderC = _pat.SaveLeader == "C";
                var isRoleRelevant = role == OperatorRole.Lider || role == OperatorRole.CA;

                if (isRoleRelevant && isSaveLeaderC)
                {
                    index++;
                    continue;
                }

                int sum = 0;
                bool hasLowLevel = false;

                foreach (var op in _distributions)
                {
                    if (ILU_Matrix.TryGetValue((op.DistributionId, usr.UserId), out var context) && context != null)
                    {
                        var record = context
                            .Where(r => r.AcquisitionDate.HasValue && r.AcquisitionDate.Value.Month <= MonthIndex)
                            .OrderByDescending(r => r.AcquisitionDate)
                            .FirstOrDefault();

                        if (record != null)
                        {
                            if (record.ILULevelId != 0 && record.ILULevelId > 5)
                                sum++;
                            if (record.ILULevelId != 0 && record.ILULevelId < 5)
                                hasLowLevel = true;
                        }
                    }
                }

                var meetsKnowledge = sum >= _pat.KnowledgePercentage;

                if (meetsKnowledge)
                {
                    countDistO++;
                }
                else if ((hasLowLevel && !meetsKnowledge) || (!hasLowLevel && !meetsKnowledge && sum > 0) || !meetsKnowledge)
                {
                    countDistX++;
                }
                index++;
            }

            return (countDistO + countDistX) > 0 ? (double)countDistO / (countDistO + countDistX) * 100 : null;
        }

        private double? CalculateUserPercentageMonthAcumulted(int MonthIndex)
        {
            if (_pat.KnowledgePercentage == null || !_distributions.Any() || !_UserOfArea.Any() || (MonthIndex > DateTime.Today.Month && _pat.AplicationYear == DateTime.Today.Year))
                return null;

            // Filtrar usuarios activos en el mes
            FilterUserByMonth(MonthIndex);
            var activeUserIds = _visibleSubordinateIds ?? new List<int>();
            var activeUsers = _UserOfArea.Where(u => activeUserIds.Contains(u.UserId)).ToList();

            countUserO = 0;
            countUserX = 0;

            foreach (var op in _distributions)
            {
                int sum = 0;
                bool hasLowLevel = false;

                foreach (var usr in activeUsers)
                {
                    var role = _pat.PatUserRoles?.FirstOrDefault(r => r.UserId == usr.UserId)?.Role;

                    var isSaveLeaderC = _pat.SaveLeader == "C";
                    var isRoleRelevant = role == OperatorRole.Lider || role == OperatorRole.CA;

                    if (isRoleRelevant && isSaveLeaderC)
                    {
                        continue;
                    }

                    if (ILU_Matrix.TryGetValue((op.DistributionId, usr.UserId), out var context) && context != null)
                    {
                        var record = context
                            .Where(r => r.AcquisitionDate.HasValue && r.AcquisitionDate.Value.Month <= MonthIndex)
                            .OrderByDescending(r => r.AcquisitionDate)
                            .FirstOrDefault();

                        if (record != null)
                        {
                            if (record.ILULevelId != 0 && record.ILULevelId > 5)
                                sum++;
                            if (record.ILULevelId != 0 && record.ILULevelId < 5)
                                hasLowLevel = true;
                        }
                    }
                }

                var meetsKnowledge = sum >= _pat.KnowledgePercentage;

                if (meetsKnowledge)
                {
                    countUserO++;
                }
                else if (hasLowLevel && !meetsKnowledge || !hasLowLevel && !meetsKnowledge && sum > 0 || !meetsKnowledge)
                {
                    countUserX++;
                }
            }

            return (countUserO + countUserX) > 0 ? (double)countUserO / (countUserO + countUserX) * 100 : null;
        }
    }//end class pat details


}//end namespace