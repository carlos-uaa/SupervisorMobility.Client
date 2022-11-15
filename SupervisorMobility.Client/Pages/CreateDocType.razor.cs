using MudBlazor;

namespace SupervisorMobility.Client.Pages
{
    public partial class CreateDocType
    {
        private List<BreadcrumbItem> _links = new List<BreadcrumbItem>
        {
            new BreadcrumbItem("Home", href: "#"),
            new BreadcrumbItem("Configuration", href: "/configuration"),
            new BreadcrumbItem("Document Types", href: "/documenttypes"),
            new BreadcrumbItem("New doc type", href: "", disabled: true)
        };

        DocumentType _documentType = new();

        async void CreateDocTypeAsync()
        {
            NavigationManager.NavigateTo($"documenttypes");
        }
    }
}
