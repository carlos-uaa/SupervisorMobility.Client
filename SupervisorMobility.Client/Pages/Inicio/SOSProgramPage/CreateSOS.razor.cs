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

        //User
        private string json = string.Empty;
        public User user = new();
        public bool logged = false;


        // Initialization
        protected async override Task OnInitializedAsync()
        {
            _links = new List<BreadcrumbItem>
        {
            new BreadcrumbItem(text: Localizer["home"], href: "#"),
            new BreadcrumbItem(text: Localizer["sosProgram"], href: "/sosProgram"),
            new BreadcrumbItem(text: Localizer["createSOS"], href: "", disabled: true)
        };


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
                    else if(user.UserType == 3)
                    {
                        _sosReview.PlantId = (int)user.PlantId;

                        _areas = await AreaServices.GetAreas(_sosReview.PlantId);

                        _sosReview.AreaId = (int)user.AreaId;

                        _sosReview.Supervisors?.Add(user);

                    }
                }
            }
            StateHasChanged();

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
          

            if (selectedSupervisorOfList != new User() )
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
            _Supervisors.Clear();

            foreach (User sv in _allSupervisors)
            {
                if (sv.PlantId == _sosReview.PlantId && sv.AreaId == _sosReview.AreaId)
                {
                    _Supervisors.Add(sv);
                }
            }

            supervisorId = 0;
            _sosReview.AreaId = 0;

            _areas = await AreaServices.GetAreas(_sosReview.PlantId);

            StateHasChanged();
        }

        private async void ShowSupervisors()
        {

            _Supervisors.Clear();
            supervisorId = 0;

            foreach (User sv in _allSupervisors)
            {
                if (sv.PlantId == _sosReview.PlantId && sv.AreaId == _sosReview.AreaId)
                {
                    _Supervisors.Add(sv);
                }
            }

            _Supervisors.Remove(user);
            StateHasChanged();
        }

        private void DeleteSupervisorList(User selection)
        {
            _sosReview.Supervisors?.Remove(selection);
            _Supervisors.Add(selection);
            StateHasChanged();
        }

        // Create Sos
        async void CreateSOSReviewAsync()
        {
           

            _sosReview.Status = 1;
            _sosReview.IsActive = true;

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
        }
    }
}
