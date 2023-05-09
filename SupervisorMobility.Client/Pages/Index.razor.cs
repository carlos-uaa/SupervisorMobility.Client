using Blazorise.Extensions;
using MudBlazor;
using System.Text.RegularExpressions;

namespace SupervisorMobility.Client.Pages
{
    public partial class Index
    {

        private List<BreadcrumbItem> _links = new List<BreadcrumbItem>
        {
            new BreadcrumbItem("Home", href: "#", disabled: true)
        };

        //private List<BreadcrumbItem> _login = new List<BreadcrumbItem>
        //{
        //    new BreadcrumbItem("Login", href: "#", disabled: true)
        //};

        //public bool logged = false;

        //private bool _processing = false;
        //public string username;
        //public string password;


        //public UserLogin _userLogin = new();

        //async Task ProcessSomething()
        //{
        //    _processing = true;
        //    await Task.Delay(2000);
        //    _processing = false;
        //}
        //protected async override Task OnInitializedAsync()
        //{
            
        
        //}
        //public double? Amount { get; set; }
        //public int? Weight { get; set; }
        //public string Password { get; set; } = "superstrong123";

        //bool isShow;
        //InputType PasswordInput = InputType.Password;
        //string PasswordInputIcon = Icons.Material.Filled.VisibilityOff;

        //void ButtonTestclick()
        //{
        //    if(isShow)
        //    {
        //        isShow = false;
        //        PasswordInputIcon = Icons.Material.Filled.VisibilityOff;
        //        PasswordInput = InputType.Password;
        //    }
        //    else
        //    {
        //        isShow = true;
        //        PasswordInputIcon = Icons.Material.Filled.Visibility;
        //        PasswordInput = InputType.Text;
        //    }
        //}


        //private async Task Login()
        //{

        //    _processing = true;
        //    var result = await UserLoginService.LoginAD(_userLogin.Username, _userLogin.Password);


        //    if(result != null)
        //    {
        //        Snackbar.Clear();
        //        Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
        //        Snackbar.Add($"Welcome Back {result.displayName}", Severity.Info);
        //        logged = true;
        //    }
        //    else
        //    {
        //        Snackbar.Clear();
        //        Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
        //        Snackbar.Add($"Error with username / password does not match", Severity.Error);
        //    }
        //    _processing = false;

        //}

        //public void ShowText()
        //{

        //}

        ////Delete Job observation
        //private bool visibleText = false;
        //private void OpenDeleteDialog()
        //{
        //    visibleText = true;
        //}
        //void CloseModal() => visibleText = false;
        //private DialogOptions dialogTextOptions = new() { CloseOnEscapeKey = true, MaxWidth = MaxWidth.ExtraSmall, FullWidth = true, Position = DialogPosition.TopCenter };

    }
}
