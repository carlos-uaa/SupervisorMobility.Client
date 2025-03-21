using Microsoft.JSInterop;
using MudBlazor;
using SupervisorMobility.Client.Data.Entities;
using SupervisorMobility.Client.Data.Entities.SOS_Process;
using SupervisorMobility.Client.Pages.Configuration.PlantPage;

namespace SupervisorMobility.Client.Pages.Inicio.PATPage
{
    public partial class CreatePAT
    {
        // Breadcrumb links
        private List<BreadcrumbItem> _links;
        // Objects
        [Parameter]
        public PAT _pat { get; set; } = new();

        [Parameter]
        public List<Plant> _plants { get; set; } = new();

        [Parameter]
        public List<Area> _areas { get; set; } = new();

        [Parameter]
        public List<User> _supervisors { get; set; } = new();

        [Parameter]
        public int Plant_Id { get; set; } = 0;
        [Parameter]
        public int Area_Id { get; set; } = 0;
        [Parameter]
        public int SOSHubId { get; set; } = 0;

        [Parameter]
        public bool is_Hoe { get; set; } = false;

        [Parameter]
        public EventCallback<DateTime?> OnAplicationDateChanged { get; set; }


        //User
        private string json = string.Empty;
        public User user = new();
        public bool logged = false;

        bool ShowLoading = true;
        private IList<string> _sourceMsgLoading = new List<string>();
        private IList<Color> _Colors = new List<Color>() { Color.Default, Color.Primary, Color.Secondary, Color.Success, Color.Info, Color.Default, Color.Primary, Color.Secondary, Color.Success, Color.Info };

        //Supervisors elements
        public bool cantCreate = true;
        private User selectedSupervisorOfList = null;
        private bool ActiveAddSubordinated = false;


        // Initialization
        protected async override Task OnInitializedAsync()
        {

            ShowLoading = true;

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

            if (!is_Hoe)
            {

                _links = new List<BreadcrumbItem>
                {
                    new BreadcrumbItem(text: Localizer["home"], href: "/"),
                    new BreadcrumbItem("PAT", href: "/PAT"),
                    new BreadcrumbItem(text: Localizer["new"] + " PAT", href: "", disabled: true)
                };
            }
            else
            {
                _links = new List<BreadcrumbItem>
                {
                    new BreadcrumbItem(text: Localizer["home"], href: "/soshoe"),
                    new BreadcrumbItem(text: Localizer["hoe"], href: "/soshoe/Hub"),
                    new BreadcrumbItem(text: Localizer["details"] + SOSHubId, href: "", disabled: true),
                    new BreadcrumbItem("PAT", href: "/PAT"),
                    new BreadcrumbItem(text: Localizer["new"] + " PAT", href: "", disabled: true)
                };
            }
            BreadcrumbService.UpdateBreadcrumbs(_links);

            logged = await HasPropertyAsync();
            if (!logged)
            {
                Snackbar.Clear();
                Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                Snackbar.Add(Localizer["errorYouHaveToLogIn"], Severity.Warning);
                NavigationManager.NavigateTo($"/");
            }
            else
            {
                await GetUserAsync();

                if (_plants is null || _plants.Count() == 0)
                {
                    _plants = await PlantServices.GetPlants();
                }

                _pat.AplicationDate = DateTime.Now;
                _pat.CreationDate = DateTime.Now;

                if (user != null)
                {
                    if (user.UserType == 1)
                    {
                        _pat.PlantId = Plant_Id;
                        _pat.AreaId = Area_Id;

                        //_allSSVs = await UsersService.GetUsersByType(2, true, false);
                        _supervisors = await UsersService.GetUsersByType(3, true, false);

                    }
                    else if (user.UserType == 3)
                    {
                        if (Plant_Id == 0)
                        {
                            _pat.PlantId = (int)user.PlantId;
                        }
                        else
                        {
                            _pat.PlantId = Plant_Id;
                        }

                        if (_areas is null || _areas.Count() == 0)
                        {
                            _areas = await AreaServices.GetAreas(_pat.PlantId);
                        }

                        if (Area_Id == 0)
                        {
                            _pat.AreaId = Area_Id;
                        }
                        else
                        {
                            _pat.AreaId = (int)user.AreaId;
                        }

                    }
                }
            }
            ShowLoading = false;
            StateHasChanged();
        }


        //Aplication Year
        private async Task HandleDateChanged(DateTime? args)
        {
            if (is_Hoe)
            {
                await OnAplicationDateChanged.InvokeAsync(args);
            }
            else
            {
                _pat.AplicationDate = args;
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



        private async void ShowAreas()
        {
            _supervisors.Clear();
            _pat.AreaId = 0;
            _areas = await AreaServices.GetAreas(_pat.PlantId);

        }


        private async void ShowSupervisors()
        {
            _supervisors.Clear();
            if (user.UserType == 1)
            {
                _supervisors = await UsersService.GetUsersByType(3, true, false);
            }
            else if (user.UserType <= 3)
            {
                _supervisors = await UsersService.GetUsersByUserTypeInPlantAndArea(_pat.PlantId, _pat.AreaId, 3, true, false);
            }
            StateHasChanged();
        }


        // Create Pat
        async void CreatePatAsync()
        {
            if (user.UserType == 1)
            {
                //_pat.SSVresponsibleID = ssvId;
            }

            _pat.CreationDate = DateTime.UtcNow;
            _pat.AplicationYear = _pat.AplicationDate.Value.Year;
            _pat.Status = 1;

            var result = await PATsServices.CreatePat(_pat);
            if (result != null)
            {
                Snackbar.Clear();
                Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                Snackbar.Add($"New PAT Created", Severity.Info);
                NavigationManager.NavigateTo($"pat");
            }
            else
            {
                Snackbar.Clear();
                Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                Snackbar.Add($"Error in Pat", Severity.Error);
            }
        }

        private async Task<IEnumerable<User>> SearchSV(string value)
        {
            // In real life use an asynchronous function for fetching data from an api.
            // await Task.Delay(1000);

            // if text is null or empty, show complete list
            if (string.IsNullOrEmpty(value))
                return _supervisors;

            return _supervisors.Where(x => x.Name.Contains(value, StringComparison.InvariantCultureIgnoreCase));
        }

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

        private void DeleteSupervisorList(User selection)
        {
            _pat.Supervisors?.Remove(selection);
            _supervisors.Add(selection);
            cantCreate = _pat.Supervisors.Count == 0;
            StateHasChanged();
        }

        private void AddSupervisor(User selection)
        {
            if (_pat.Supervisors == null)
            {
                _pat.Supervisors = new List<User>();
            }


            if (selectedSupervisorOfList != null && !_pat.Supervisors.Contains(selection))
            {
                _pat.Supervisors.Add(selection);
                _supervisors.Remove(selection);
                selectedSupervisorOfList = null;
                ActiveAddSubordinated = true;
            }

            cantCreate = _pat.Supervisors.Count == 0;
            StateHasChanged();
        }


    }
}