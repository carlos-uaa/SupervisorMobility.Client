using Microsoft.JSInterop;
using MudBlazor;

namespace SupervisorMobility.Client.Pages
{
    public partial class DocumentTypes
    {
        private List<BreadcrumbItem> _links = new List<BreadcrumbItem>
        {
            new BreadcrumbItem("Home", href: "#"),
            new BreadcrumbItem("Configuration", href: "/configuration"),
            new BreadcrumbItem("Support Document Types", href: "", disabled: true)
        };

        public List<SupportDocumentType> _supportDocumentTypes { get; set; } = new();

        SupportDocumentType _supportDocumentType = new();

        protected async override Task OnInitializedAsync()
        {
            _supportDocumentTypes = await SupportDocumentTypeService.GetSupportDocumentTypes();
        }

        void CreateSupportDocType()
        {
            NavigationManager.NavigateTo($"documenttypes/createsupportdoctype");
        }

        void EditSupportDocType(int supportDocumentTypeId)
        {
            NavigationManager.NavigateTo($"documenttypes/updatesupportdoctype/{supportDocumentTypeId}");
        }

        async Task DeleteSupportDocumentType(int supportDocumentTypeId)
        {
            bool confirm = await JSRuntime.InvokeAsync<bool>("confirm", $"Are you sure you want to delete this support document type?");

            if (confirm)
            {
                _supportDocumentTypes.RemoveAll(supportDocumentType => supportDocumentType.SupportDocumentTypeId == supportDocumentTypeId);
                await SupportDocumentTypeService.DeleteSupportDocumentType(supportDocumentTypeId);
            }
        }
    }
}
