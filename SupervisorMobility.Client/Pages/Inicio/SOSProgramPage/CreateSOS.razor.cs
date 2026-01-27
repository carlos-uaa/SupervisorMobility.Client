using Microsoft.JSInterop;
using MudBlazor;
using SupervisorMobility.Client.Data.Entities;
using SupervisorMobility.Client.Pages.Configuration.PlantPage;

namespace SupervisorMobility.Client.Pages.Inicio.SOSProgramPage
{
    public partial class CreateSOS
    {
        // Breadcrumb links
        private List<BreadcrumbItem> _links;
        // Objects
        SOSReviewProgram _sosReview = new();
        List<Plant> _plants { get; set; } = new();
        List<Area> _areas = new();

        List<User> _allSupervisors = new();
        List<User> _Supervisors = new();

        public int supervisorId;

        public string SupervisorName = string.Empty;

        [Inject]
        private IDialogService DialogService { get; set; }

        //User
        private string json = string.Empty;
        public User user = new();
        public bool logged = false;

        public bool cantCreate = true;

        bool ShowLoading = true;
        private IList<string> _sourceMsgLoading = new List<string>();
        private IList<Color> _Colors = new List<Color>() { Color.Default, Color.Primary, Color.Secondary, Color.Success, Color.Info, Color.Default, Color.Primary, Color.Secondary, Color.Success, Color.Info };
        private DialogOptions dialogOptions = new() { CloseOnEscapeKey = true, MaxWidth = MaxWidth.Large, FullWidth = true, DisableBackdropClick = true, CloseButton = true };


        [Inject]
        private IBreadcrumbService BreadcrumbService { get; set; }


        // Initialization

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

            ShowLoading = true;

            _links = new List<BreadcrumbItem>
            {
                new BreadcrumbItem(text: Localizer["home"], href: "/"),
                new BreadcrumbItem(text: Localizer["sosProgram"], href: "/sosProgram"),
                new BreadcrumbItem(text: Localizer["createSOS"], href: "", disabled: true)
            };

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

                _plants = await PlantServices.GetPlants();
                //_sosReview.AplicationYear = DateTime.Now.Year;
                _sosReview.CreationDate = DateTime.Now;

                if (user != null)
                {
                    if (user.UserType == 1)
                    {
                        _sosReview.PlantId = 0;
                        _sosReview.AreaId = 0;

                        _allSupervisors = await UsersService.GetUsersByType(3, true, false);

                    }
                    else if (user.UserType == 2)
                    {
                        _sosReview.PlantId = (int)user.PlantId;

                        _areas = user.Areas.ToList();
                        if (_Supervisors == null)
                        {
                            _Supervisors = new List<User>();
                        }
                        _Supervisors?.ToList().AddRange(user.Subordinates);

                    }
                    else if (user.UserType == 3)
                    {
                        _sosReview.PlantId = (int)user.PlantId;

                        _areas.Add(user.Area);

                        _sosReview.AreaId = user.Areas != null && user.Areas.Count > 0 ? user.Areas.FirstOrDefault().AreaId : 0;
                        if (_sosReview.Supervisors == null)
                        {
                            _sosReview.Supervisors = new List<User>();
                        }
                        _allSupervisors = await UsersService.GetUsersByUserTypeInPlantAndArea(_sosReview.PlantId, _sosReview.AreaId, 3, true, false);
                        if (_Supervisors == null)
                        {
                            _Supervisors = new List<User>();
                        }
                        _Supervisors = _allSupervisors;
                        if (_Supervisors.Any(u => u.UserId == user.UserId))
                        {
                            int indexofRemove = _Supervisors.FindIndex(u => u.UserId == user.UserId);
                            if (indexofRemove > -1)
                            {
                                _Supervisors.RemoveAt(indexofRemove);
                            }

                        }
                        _sosReview.Supervisors?.Add(user);

                        cantCreate = _sosReview.Supervisors.Count == 0;

                    }
                }
            }

            StateHasChanged();
            ShowLoading = false;
        }
        private async Task<IEnumerable<User>> SearchSV(string value)
        {
            // In real life use an asynchronous function for fetching data from an api.
            // await Task.Delay(1000);

            // if text is null or empty, show complete list
            if (string.IsNullOrEmpty(value))
                return _Supervisors;

            return _Supervisors.Where(x => x.Name.Contains(value, StringComparison.InvariantCultureIgnoreCase));
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
        //Local storage user
        private async Task GetUserAsync()
        {
            if (!await TryGetAsync())
            {
                user = new();
            }
        }

        private void AddSupervisor(User selection)
        {
            if (_sosReview.Supervisors == null)
            {
                _sosReview.Supervisors = new List<User>();
            }


            if (selectedSupervisorOfList != null && !_sosReview.Supervisors.Contains(selection))
            {

                _sosReview.Supervisors.Add(selection);

                _Supervisors.Remove(selection);

                selectedSupervisorOfList = null;
                ActiveAddSubordinated = true;
            }

            cantCreate = _sosReview.Supervisors.Count == 0;

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



        private async void ShowAreas()
        {

            supervisorId = 0;
            _sosReview.AreaId = 0;

            if (user.UserType == 1)
                _areas = await AreaServices.GetAreas(_sosReview.PlantId);



            StateHasChanged();
        }

        private async void ShowSupervisors()
        {
            if (_sosReview.Supervisors == null)
            {
                _sosReview.Supervisors = new List<User>();
            }
            _Supervisors = new();
            _sosReview.Supervisors?.Clear();
            cantCreate = true;

            if (user.UserType == 1)
            {
                foreach (User sv in _allSupervisors)
                {
                    if (sv.PlantId == _sosReview.PlantId && sv.Areas.Any(a => a.AreaId == _sosReview.AreaId))
                    {
                        _Supervisors.Add(sv);
                    }
                }
            }
            else if (user.UserType == 2)
            {
                foreach (User sv in user.Subordinates)
                {
                    if (sv.PlantId == _sosReview.PlantId && sv.Areas.Any(a => a.AreaId == _sosReview.AreaId) && sv.UserType == 3)
                    {
                        _sosReview.Supervisors.Add(sv);
                    }
                }
            }
            else if (user.UserType == 3)
            {
                //_Supervisors = _allSupervisors;

                if (_Supervisors == null)
                {
                    _Supervisors = new List<User>();
                }
                _Supervisors.ToList().AddRange(_allSupervisors);
                if (_Supervisors.Any(u => u.UserId == user.UserId))
                {
                    int indexofRemove = _Supervisors.FindIndex(u => u.UserId == user.UserId);
                    if (indexofRemove > -1)
                    {
                        _Supervisors.RemoveAt(indexofRemove);
                    }

                }
                _sosReview.Supervisors?.Add(user);
            }


            cantCreate = _sosReview.Supervisors.Count == 0;

            StateHasChanged();
            base.StateHasChanged();
        }

        private void DeleteSupervisorList(User selection)
        {
            _sosReview.Supervisors?.Remove(selection);
            _Supervisors.Add(selection);
            cantCreate = _sosReview.Supervisors.Count == 0;
            StateHasChanged();

        }

        // Create Sos
        async void CreateSOSReviewAsync()
        {
            cantCreate = true;
            ShowLoading = true;
            _sosReview.Status = 1;
            _sosReview.IsActive = true;

            //var findExistSOS = await SOSReviewServices.FindSOS(_sosReview.PlantId, _sosReview.AreaId, _sosReview.CreationDate.Value.Year - 1 );
            //if (findExistSOS != null)
            //{
            //    bool? msgOption = await DialogService.ShowMessageBox("Warning",
            //      "Se encontro un plan del año anterior, quieres usarlo como base?!",
            //     yesText: "Si", cancelText: "No");
            //    var state = msgOption == null ? "No - Crear Nuevo" : "Si - Usar!";
            //    StateHasChanged();

            //    if (state == "No - Crear Nuevo")
            //    {
            //        if (_sosReview.Supervisors == null)
            //        {
            //            _sosReview.Supervisors = new List<User>();
            //        }

            //        switch (user.UserType)
            //        {
            //            case 2:
            //                //Si lo hace el SSV Añade a sus subordinados
            //                _sosReview.Supervisors.ToList().AddRange(user.Subordinates);
            //                break;
            //            case 3:
            //                //Si lo hace el SV se añadade a el
            //                _sosReview.Supervisors.Add(user);
            //                break;
            //        }


            //        var result = await SOSReviewServices.CreateSOSReview(_sosReview);
            //        if (result != null)
            //        {
            //            Snackbar.Clear();
            //            Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
            //            Snackbar.Add($"New SOS Created", Severity.Info);
            //            NavigationManager.NavigateTo($"sosProgram");
            //        }
            //        else
            //        {
            //            Snackbar.Clear();
            //            Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
            //            Snackbar.Add($"Error in SOS", Severity.Error);
            //        }

            //    }
            //    else
            //    {
            //        //usar el plan del año pasado
            //        Console.WriteLine("Se usa la del año pasado");
            //    }
            //}
            //else
            //{
                if (_sosReview.Supervisors == null)
                {
                    _sosReview.Supervisors = new List<User>();
                }

                switch (user.UserType)
                {
                    case 2:
                        //Si lo hace el SSV Añade a sus subordinados
                        _sosReview.Supervisors.ToList().AddRange(user.Subordinates);
                        break;
                    case 3:
                        //Si lo hace el SV se añadade a el
                        _sosReview.Supervisors.Add(user);
                        break;
                }


                var result = await SOSReviewServices.CreateSOSReview(_sosReview);
                if (result != null)
                {
                    Snackbar.Clear();
                    Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                    Snackbar.Add($"New SOS Created", Severity.Info);
                    NavigationManager.NavigateTo($"sosProgram");
                }
                else
                {
                    Snackbar.Clear();
                    Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                    Snackbar.Add($"Error in SOS", Severity.Error);
                }

            //}

        }
    }
}
