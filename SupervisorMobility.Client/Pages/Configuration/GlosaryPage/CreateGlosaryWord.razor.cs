using MudBlazor;
using SupervisorMobility.Client.Data.Entities;

namespace SupervisorMobility.Client.Pages.Configuration.GlosaryPage
{
    public partial class CreateGlosaryWord
    {
        // Breadcrumb links
        private List<BreadcrumbItem> _links;

        // Objects
        Glosary _glosaryWord = new();
        protected async override Task OnInitializedAsync()
        {
            _links = new List<BreadcrumbItem>{
            new BreadcrumbItem(text: Localizer["home"], href: "/", disabled:false),
            new BreadcrumbItem(text: Localizer["configuration"], href: "/configuration", disabled:false),
            new BreadcrumbItem(text: Localizer["GlosaryTitle"], href: "/glosary", disabled: false),
            new BreadcrumbItem(text: Localizer["GlosaryNewWord"], href: "", disabled: true)
            };
          
        }
        
        async void CreateGlosaryWordAsync()
        {
            _glosaryWord.IsActive = true;
            var result = await GlosaryService.CreateGlosaryWord(_glosaryWord);
            NavigationManager.NavigateTo($"glosary");
        }
    }
}
