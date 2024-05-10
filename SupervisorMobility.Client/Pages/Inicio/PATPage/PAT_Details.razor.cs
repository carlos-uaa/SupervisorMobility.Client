using Microsoft.JSInterop;
using MudBlazor;
using SupervisorMobility.Client.Data.Entities;
using System.Globalization;

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
            //_operations = await OperationsServices.GetOperations(_pat.PlantId, _pat.AreaId, _pat.DistributionId);
            //_UserOfArea = await UsersServices.GetUsersWhitCollections();


            foreach (var op in _distributions)
            {
                foreach (var usr in _UserOfArea)
                {
                    // Obtener los registros correspondientes a la operación y usuario actual
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
            foreach(var op in _distributions)
            {
                if(ILU_Matrix.TryGetValue((op.DistributionId, auxILU_UseId), out var context))
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
            foreach(var usr in _UserOfArea)
            {
                if(ILU_Matrix.TryGetValue((auxILU_OpId, usr.UserId), out var context))
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
            ShowTable = false;
            AddILUVisibleDialog = false;

            _newIlu.AcquisitionDate = DateTime.Now;
            var result = await ILUServices.AddRegisterForUser(_newIlu, (int)_newIlu.OperatorId);
            if (result != null)
            {
                await PrepareDataTable();

                Console.WriteLine("Se añadio");
                //aqui un mensaje bonito
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

            _pat!.Status = 2;
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


    }//end class pat details


}//end namespace