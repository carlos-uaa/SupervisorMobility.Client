using BlazorCameraStreamer;
using Blazorise.Extensions;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.JSInterop;
using MudBlazor;
using SupervisorMobility.Client.Data.Entities;
using SupervisorMobility.Client.Pages.Configuration.ProductPage;
using SupervisorMobility.Client.Services.BreadcrumsService;
using SupervisorMobility.Client.Services.UserService;
using System;
using System.Globalization;
using System.Text.RegularExpressions;
using static MudBlazor.CategoryTypes;

namespace SupervisorMobility.Client.Pages
{
    public partial class Index
    {
        [Inject]
        private IBreadcrumbService BreadcrumbService { get; set; }
        private List<BreadcrumbItem> _links;

        private List<BreadcrumbItem> _login;

        public bool logged = false;

        private bool _processing = false;
        public string username;
        public string password;

        //User
        public int LoginTry { get; set; } = 0;
        public bool userInSystem = true;
        private string json = string.Empty;
        public User user = new();

        public UserLogin _userLogin = new();

        async Task ProcessSomething()
        {
            _processing = true;
            await Task.Delay(2000);
            _processing = false;
        }

        public double? Amount { get; set; }
        public int? Weight { get; set; }
        public string Password { get; set; } = "superstrong123";

        bool isShow;
        InputType PasswordInput = InputType.Password;
        string PasswordInputIcon = Icons.Material.Filled.VisibilityOff;

        bool Dev_env { get; set; }

        protected async override Task OnInitializedAsync()
        {

            Dev_env = Environment.IsDevelopment();
            _login = new List<BreadcrumbItem>
            {
                new BreadcrumbItem(text: Localizer["login"], href: "/", disabled: true)
            };

            _links = new List<BreadcrumbItem>
            {
                new BreadcrumbItem(text: Localizer["home"], href: "/", disabled: true)
            };

            BreadcrumbService.UpdateBreadcrumbs(_links);

            logged = await HasPropertyAsync();
            if (logged)
            {

                await GetUserAsync();
                GlobalData.LoggedUser = user.Name;
                StateHasChanged();
            }
        }

        private async Task<bool> HasPropertyAsync() => await js.InvokeAsync<bool>("localStorage.hasOwnProperty", "user");

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
                json = await js.InvokeAsync<string>("localStorage.getItem", "user");
                user = JsonSerializer.Deserialize<User>(json) ?? new();

            }
            return hasProperty;
        }


        void CreateJobObservation()
        {
            if (CultureInfo.CurrentCulture.Name == "en-US")
            {
                var date = DateTime.ParseExact(DateTime.Now.ToShortDateString(), "M/d/yyyy", CultureInfo.InvariantCulture);

                var formatedDate = date;

                var EnglishDate = formatedDate.Day.ToString() + "/" + formatedDate.Month.ToString() + "/" + formatedDate.Year.ToString();

                var dateString = EnglishDate.Replace("/", "-");
                NavigationManager.NavigateTo($"jobobservation/createjobobservation/{dateString}");
            }
            else
            {
                var date = DateTime.ParseExact(DateTime.Now.ToShortDateString(), "d/M/yyyy", CultureInfo.InvariantCulture);
                var dateString = date.ToShortDateString().Replace("/", "-");
                NavigationManager.NavigateTo($"jobobservation/createjobobservation/{dateString}");
            }
        }

        void CreateNewJobObservation()
        {
            if (CultureInfo.CurrentCulture.Name == "en-US")
            {
                var date = DateTime.ParseExact(DateTime.Now.ToShortDateString(), "M/d/yyyy", CultureInfo.InvariantCulture);

                var formatedDate = date;

                var EnglishDate = formatedDate.Day.ToString() + "/" + formatedDate.Month.ToString() + "/" + formatedDate.Year.ToString();

                var dateString = EnglishDate.Replace("/", "-");
                NavigationManager.NavigateTo($"jobobservation/createnewjobobservation/{dateString}");
            }
            else
            {
                var date = DateTime.ParseExact(DateTime.Now.ToShortDateString(), "d/M/yyyy", CultureInfo.InvariantCulture);
                var dateString = date.ToShortDateString().Replace("/", "-");
                NavigationManager.NavigateTo($"jobobservation/createnewjobobservation/{dateString}");
            }
        }

        void PlanJobObservation()
        {
            if (CultureInfo.CurrentCulture.Name == "en-US")
            {
                var date = DateTime.ParseExact(DateTime.Now.ToShortDateString(), "M/d/yyyy", CultureInfo.InvariantCulture);
                var formatedDate = date;

                var EnglishDate = formatedDate.Day.ToString() + "/" + formatedDate.Month.ToString() + "/" + formatedDate.Year.ToString();

                var dateString = EnglishDate.Replace("/", "-");
                NavigationManager.NavigateTo($"jobobservation/planjobobservation/{dateString}");
            }
            else
            {
                var date = DateTime.ParseExact(DateTime.Now.ToShortDateString(), "d/M/yyyy", CultureInfo.InvariantCulture);
                var dateString = date.ToShortDateString().Replace("/", "-");
                NavigationManager.NavigateTo($"jobobservation/planjobobservation/{dateString}");
            }
        }

        void ButtonTestclick()
        {
            if (isShow)
            {
                isShow = false;
                PasswordInputIcon = Icons.Material.Filled.VisibilityOff;
                PasswordInput = InputType.Password;
            }
            else
            {
                isShow = true;
                PasswordInputIcon = Icons.Material.Filled.Visibility;
                PasswordInput = InputType.Text;
            }
        }


        private async Task Login()
        {

            _processing = true;
            var result = await UserLoginService.LoginAD(_userLogin.Username, _userLogin.Password);


            if (result != null)
            {
                user = await UsersService.GetUserByObjectIdWithCollections(result.userPrincipalName);

                if (user != null)
                {

                    GlobalData.LoggedUser = user.Name;

                    Snackbar.Clear();
                    Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                    Snackbar.Add($"Welcome Back {user.Name}", Severity.Info);
                    logged = true;

                    json = JsonSerializer.Serialize<User>(user);
                    await js.InvokeVoidAsync("localStorage.setItem", "user", json);
                    NavigationManager.NavigateTo($"/", true);
                    StateHasChanged();
                }
                else
                {
                    LoginTry++;
                    userInSystem = false;
                }
            }
            else
            {

                LoginTry++;
                Snackbar.Clear();
                Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                Snackbar.Add(Localizer["errorWithUsernamePasswordDoesNotMatch"], Severity.Error);
            }
            _processing = false;

            if (LoginTry == 2 && !userInSystem)
            {
                UserNotFound newUser = new();
                newUser.Name = result.displayName;
                newUser.ObjectId = result.userPrincipalName;
                newUser.IsActive = true;

                var response = await UsersService.CreateUnregisteredUser(newUser);
                if (response != null)
                {
                    Snackbar.Clear();
                    Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                    Snackbar.Add(Localizer["unregisteredUserAddedToDatabaseForAdmin"], Severity.Info);
                }
                else
                    await js.InvokeVoidAsync("alert", "Error, on create user please call your admin!"); // Alert

                Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                Snackbar.Add(Localizer["userNotFoundPleaseContactYourAdmin"], Severity.Warning);

                //notificacion de que se mando al administrador del sistema
            }
            else
            {
                //error del usuario en contraseña o usuario

            }
        }

        public void ShowText()
        {

        }

        //Delete Job observation
        private bool visibleText = false;
        private void OpenDeleteDialog()
        {
            visibleText = true;
        }
        void CloseModal() => visibleText = false;
        private DialogOptions dialogTextOptions = new() { CloseOnEscapeKey = true, MaxWidth = MaxWidth.ExtraSmall, FullWidth = true, Position = DialogPosition.TopCenter };


    }
}
