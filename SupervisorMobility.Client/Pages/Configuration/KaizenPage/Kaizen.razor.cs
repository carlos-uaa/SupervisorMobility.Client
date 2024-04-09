using Microsoft.JSInterop;
using MudBlazor;
using SupervisorMobility.Client.Data.Entities;

namespace SupervisorMobility.Client.Pages.Configuration.KaizenPage
{
    public partial class Kaizen
    {
        private List<BreadcrumbItem> _links;

        //User
        private string json = string.Empty;
        public User user = new();
        public bool logged = false;

        string kaizenNameText = "ELIMINACION DE MARCA DE SCRAP EN ZONA DE FR PLR RH P71A";
        List<Plant> _plants { get; set; } = new();
        List<Area> _areas = new();
        int plantId = 0;
        int areaId = 0;
        int kpiId = 0;
        DateTime startDate = DateTime.Now;

        string kpiNametext = "Tiempo de reparación";
        List<User> _seniorSupervisors { get; set; } = new();
        List<User> _allSSVs { get; set; } = new();
        List<User> _supervisors { get; set; } = new();
        List<User> _operators = new();
        public List<User> operatorUsers = new();
        int ssvId = 0;
        int supervisorId = 0;
        int operatorId = 0;


        string costText = "Ahorro de $133.47 por hora por reparación de paneles con marca, se reparan 4 paneles por hora, total de paneles 900.  Total de horas = (900 / 4) = 225 horas   Total de ahorro = (225 * 133.47) = $30.030.75";
        string laborText = "Costo por modificación de troquel $133.47 por hora, se usaron 18 horas, total: $2,402.46";
        string materialText = "N/A";
        string machineText = "N/A";
        string calculationFormText = "$30.030.75 - 2,402.46";
        string totalText = "$27,628.29";

        protected async override Task OnInitializedAsync()
        {
            _links = new List<BreadcrumbItem>
                {
                    new BreadcrumbItem(text: Localizer["home"], href: "/"),
                    new BreadcrumbItem(text: Localizer["configuration"], href: "/configuration"),
                    new BreadcrumbItem(text: Localizer["kaizen"], href: "", disabled: true)
                };

            BreadcrumbService.UpdateBreadcrumbs(_links);
            await GetUserAsync();
            logged = await HasPropertyAsync();
            if (!logged)
            {
                Snackbar.Clear();
                Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                Snackbar.Add($"Error You have to log in", Severity.Error);
                NavigationManager.NavigateTo($"/");
            }

            _plants = await PlantServices.GetPlants();
            _plants = _plants.OrderBy(p => p.Description).ToList();
        }

        //Local storage user
        private async Task GetUserAsync()
        {
            if (!await TryGetAsync())
                user = new();
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
            ssvId = 0;
            operatorId = 0;
            areaId = 0;
            _areas = await AreaServices.GetAreas(plantId);
            _areas = _areas.OrderBy(a => a.Description).ToList();
        }

        private async void ShowOperators()
        {

            if (user.UserType == 1 || user.UserType == 2)
            {
                _operators = await UsersService.GetSubordinates(supervisorId, false);
                _operators = _operators.OrderBy(o => o.Name).ToList();
            }
            operatorUsers = new();
            operatorId = 0;
            //operator User
            foreach (var operatorUser in _operators)
            {
                if (operatorUser.AreaId == areaId && operatorUser.SuperiorId == supervisorId)
                {
                    operatorUsers.Add(operatorUser);
                }
            }
            StateHasChanged();
        }

        private async void ShowSeniorSupervisors()
        {
            supervisorId = 0;
            ssvId = 0;
            operatorId = 0;
            _seniorSupervisors = new();
            _allSSVs = await UsersService.GetUsersByUserTypeInPlant(plantId, 2, true, false);
            _allSSVs = _allSSVs.OrderBy(s => s.Name).ToList();

            foreach (User ssv in _allSSVs)
            {
                if (ssv.Areas?.ToList().FindIndex(a => a.AreaId == areaId) != -1)
                {
                    _seniorSupervisors.Add(ssv);
                }
            }
            StateHasChanged();
        }

        private async void ShowSupervisors()
        {
            supervisorId = 0;
            operatorId = 0;
            _supervisors = await UsersService.GetUsersByUserTypeInPlantAndArea(plantId, areaId, 3, false, false);
            _supervisors = _supervisors.OrderBy(s => s.Name).ToList();
            StateHasChanged();
        }
    }
}