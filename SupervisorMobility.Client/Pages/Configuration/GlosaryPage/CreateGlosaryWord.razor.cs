using MudBlazor;

namespace SupervisorMobility.Client.Pages.Configuration.GlosaryPage
{
    public partial class CreateGlosaryWord
    {
        // Breadcrumb links
        private List<BreadcrumbItem> _links = new List<BreadcrumbItem>
        {
            new BreadcrumbItem("Home", href: "#"),
            new BreadcrumbItem("Configuration", href: "/configuration"),
            new BreadcrumbItem("Glosary", href: "/glosary"),
            new BreadcrumbItem("New Word", href: "", disabled: true)
        };

        // Objects
        Glosary _glosaryWord = new();

        // Create group
        async void CreateGlosaryWordAsync()
        {
            var result = await GlosaryService.CreateGlosaryWord(_glosaryWord);
            NavigationManager.NavigateTo($"glosary");
        }
    }
}
