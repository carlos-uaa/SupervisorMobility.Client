using MudBlazor;

namespace SupervisorMobility.Client.Pages
{
    public partial class UpdateDocType
    {
        private List<BreadcrumbItem> _links = new List<BreadcrumbItem>
        {
            new BreadcrumbItem("Home", href: "#"),
            new BreadcrumbItem("Configuration", href: "/configuration"),
            new BreadcrumbItem("Document Types", href: "/documenttypes"),
            new BreadcrumbItem("Update doc type", href: "", disabled: true)
        };

        DocumentType _documentType = new();

        async void EditDocTypeAsync()
        {
            NavigationManager.NavigateTo($"documenttypes");
        }
    }
}
