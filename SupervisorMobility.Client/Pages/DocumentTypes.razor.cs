using MudBlazor;

namespace SupervisorMobility.Client.Pages
{
    public partial class DocumentTypes
    {
        private List<BreadcrumbItem> _links = new List<BreadcrumbItem>
        {
            new BreadcrumbItem("Home", href: "#"),
            new BreadcrumbItem("Configuration", href: "/configuration"),
            new BreadcrumbItem("Document Types", href: "", disabled: true)
        };

        List<DocumentType> _documentTypes = new List<DocumentType>
        {
            new DocumentType { DocTypeId = 1, Code = "HOE", Description = "Hoja de operación estándar" }
        };

        void CreateDocType()
        {
            NavigationManager.NavigateTo($"documenttypes/createdoctype");
        }

        void EditDocType()
        {
            NavigationManager.NavigateTo($"documenttypes/updatedoctype");
        }
    }
}
