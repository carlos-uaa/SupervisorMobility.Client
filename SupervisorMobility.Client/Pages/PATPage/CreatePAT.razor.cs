using Microsoft.JSInterop;
using MudBlazor;
using SupervisorMobility.Client.Data.Entities;

namespace SupervisorMobility.Client.Pages.PATPage
{
    public partial class CreatePAT
    {
        // Breadcrumb links
        private List<BreadcrumbItem> _links = new List<BreadcrumbItem>
        {
            new BreadcrumbItem("Home", href: "#"),
            new BreadcrumbItem("PATs", href: "/PAT"),
            new BreadcrumbItem("New PAT", href: "", disabled: true)
        };

        // Objects
        PAT _pat = new();
        List<Plant> _plants { get; set; } = new();
        List<Product> _products { get; set; } = new();
        List<Area> _areas = new();
        List<Distribution> _distributions = new();

        List<User> _allUsers = new();
        List<User> _SSVs = new();
        List<User> _supervisors = new();
        public int ssvId;

        public string SupervisorName = string.Empty;

        //User
        private string json = string.Empty;
        public User user = new();
        public bool logged = false;


        // Initialization
        protected async override Task OnInitializedAsync()
        {

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
                
                _plants = await PlantServices.GetPlants();
                _pat.AplicationDate = DateTime.Now;
                _pat.CreationDate = DateTime.Now;

                if (user != null)
                {
                    if (user.UserType == 1)
                    {
                        _pat.PlantId = 0;
                        _pat.AreaId = 0;

                        _allUsers = await UsersService.GetUsersWhitCollections();

                    }
                    else if (user.UserType == 2)
                    {

                    }
                    else
                    {
                        _pat.PlantId = (int)user.PlantId;
                        _areas = await AreaServices.GetAreas(_pat.PlantId); 
                        
                        _pat.AreaId = (int)user.AreaId;
                        _distributions = await DistributionService.GetDistributionsWithCollections(_pat.PlantId, _pat.AreaId);


                        _pat.SupervisorId = user.UserId;

                        if(user.Superior != null)
                        {
                            _pat.SSVresponsibleID = user.SuperiorId;
                            SupervisorName = user.Superior.Name;
                        }
                        else
                        {
                            SupervisorName = "This user does not have a Superior";
                        }


                    }
                }
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
            _SSVs.Clear();
            ssvId = 0;
            _pat.AreaId = 0;
            _pat.DistributionId = 0;
            _areas = await AreaServices.GetAreas(_pat.PlantId);

            foreach (User usr in _allUsers)
            {
                if (usr.UserType == 2 && usr.PlantId == _pat.PlantId)
                {
                    _SSVs.Add(usr);
                }

            }

        }

        private async void ShowDistributions()
        {

            _pat.DistributionId = 0;
            _distributions = await DistributionService.GetDistributionsWithCollections(_pat.PlantId, _pat.AreaId);
            StateHasChanged();
        }

        private void ShowSupervisors()
        {
            _supervisors.Clear();
            _pat.SupervisorId = 0;

            foreach(User sv in _allUsers)
            {
                if(sv.SuperiorId == ssvId)
                _supervisors.Add(sv);
            }
            StateHasChanged();
        }

        // Create Pat
        async void CreatePatAsync()
        {
            if(user.UserType == 1)
            {
                _pat.SSVresponsibleID = ssvId;
            }

            _pat.Status = 1;

            var result = await PATsServices.CreatePat(_pat);
            if(result != null)
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
    }
}
