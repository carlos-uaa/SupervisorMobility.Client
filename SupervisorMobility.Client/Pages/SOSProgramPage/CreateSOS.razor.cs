using Microsoft.JSInterop;
using MudBlazor;
using SupervisorMobility.Client.Data.Entities;
using SupervisorMobility.Client.Pages.Configuration.PlantPage;

namespace SupervisorMobility.Client.Pages.SOSProgramPage
{
    public partial class CreateSOS
    {
        // Breadcrumb links
        private List<BreadcrumbItem> _links;
        // Objects
        SOSReviewProgram _sosReview = new();
        List<Plant> _plants { get; set; } = new();
        List<Area> _areas = new();

        List<User> _allSSVs = new();
        List<User> _SSVs = new();

        public int ssvAId;
        public int ssvBId;
        public int ssvCId;

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

                        _allSSVs = await UsersService.GetUserByTypeAndCollection(2);

                    }
                    else if (user.UserType == 2)
                    {


                    }
                    else if(user.UserType == 3)
                    {

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
            
            ssvAId = 0;
            ssvBId = 0;
            ssvCId = 0;
            _sosReview.AreaId = 0;

            _areas = await AreaServices.GetAreas(_sosReview.PlantId);

            StateHasChanged();
        }

        private async void ShowSSV()
        {

            _SSVs.Clear();
            ssvAId = 0;
            ssvBId = 0;
            ssvCId = 0;

            foreach (User ssv in _allSSVs)
            {
                if (ssv.PlantId == _sosReview.PlantId && ssv.Areas?.ToList().FindIndex(a => a.AreaId == _sosReview.AreaId) != -1)
                {
                    _SSVs.Add(ssv);
                }
            }

            StateHasChanged();




        }

        private void ShowSupervisors()
        {
            _SSVs.Clear();

            foreach (User ssv in _allSSVs)
            {
                if (ssv.PlantId == _sosReview.PlantId && ssv.Areas?.ToList().FindIndex(a => a.AreaId == _sosReview.AreaId) != -1)
                {
                    _SSVs.Add(ssv);
                }
            }


            StateHasChanged();
        }

        // Create Pat
        async void CreateSOSReviewAsync()
        {
            if (user.UserType == 1)
            {
                _sosReview.UserAid = ssvAId;
                _sosReview.UserBid = ssvBId;
                _sosReview.UserCid = ssvCId;
            }

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
