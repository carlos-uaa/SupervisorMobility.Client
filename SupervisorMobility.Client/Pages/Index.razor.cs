using Blazorise.Extensions;
using Microsoft.JSInterop;
using MudBlazor;
using SupervisorMobility.Client.Data.Entities;
using SupervisorMobility.Client.Services.UserService;
using System;
using System.Text.RegularExpressions;

namespace SupervisorMobility.Client.Pages
{
    public partial class Index
    {

        private List<BreadcrumbItem> _links = new List<BreadcrumbItem>
        {
            new BreadcrumbItem("Home", href: "#", disabled: true)
        };

        private List<BreadcrumbItem> _login = new List<BreadcrumbItem>
        {
            new BreadcrumbItem("Login", href: "#", disabled: true)
        };

        public bool logged = false;

        private bool _processing = false;
        public string username;
        public string password;

        //User
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


        protected async override Task OnInitializedAsync()
        {
            logged = await HasPropertyAsync();
            if (logged)
            {

                await GetUserAsync();
                GlobalData.LoggedUser = user.Name;
                StateHasChanged();
            }
        }

        private async Task<bool> HasPropertyAsync()
            => await js.InvokeAsync<bool>("localStorage.hasOwnProperty", "user");

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

                if(user != null)
                {

                    GlobalData.LoggedUser = user.Name;
                    Snackbar.Clear();
                    Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                    Snackbar.Add($"Welcome Back {user.Name}", Severity.Info);
                    logged = true;

                    json = JsonSerializer.Serialize<User>(user);
                    await js.InvokeVoidAsync("localStorage.setItem", "user", json);

                    StateHasChanged();

                }
                else
                {

                    UserNotFound newUser =new();
                    newUser.Name = result.displayName;
                    newUser.ObjectId = result.userPrincipalName;
                    newUser.IsActive = true;

                    var response = await UsersService.CreateUnregisteredUser(newUser);
                    if (response != null)
                    {
                        Snackbar.Clear();
                        Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                        Snackbar.Add($"Unregistered user added to database for admin.", Severity.Info);
                    }
                    else
                        await js.InvokeVoidAsync("alert", "Error, on create user please call your admin!"); // Alert

                    Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                    Snackbar.Add($"User not found, please contact your administrator", Severity.Warning);
                }
            }
            else
            {
                Snackbar.Clear();
                Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                Snackbar.Add($"Error with username / password does not match", Severity.Error);
            }
            _processing = false;

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
