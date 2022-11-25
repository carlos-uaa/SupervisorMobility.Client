using MudBlazor;

namespace SupervisorMobility.Client.Pages
{
    public partial class CreateSupportDocumentType
    {
        // Breadcrumb links
        private List<BreadcrumbItem> _links = new List<BreadcrumbItem>
        {
            new BreadcrumbItem("Home", href: "#"),
            new BreadcrumbItem("Configuration", href: "/configuration"),
            new BreadcrumbItem("Support Document Types", href: "/documenttypes"),
            new BreadcrumbItem("Create Support Document Types", href: "", disabled: true)
        };

        // Objects
        SupportDocumentType _supportDocumentType = new();

        // Create support document type
        async void CreateSupportDocumentTypeAsync()
        {
            var result = await SupportDocumentTypeService.CreateSupportDocumentType(_supportDocumentType);
            NavigationManager.NavigateTo($"documenttypes");
        }
    }
}
