using Microsoft.JSInterop;
using MudBlazor;

namespace SupervisorMobility.Client.Pages.Configuration.DocumentTypePage
{
    public partial class DocumentTypes
    {
        // Breadcrumb links
        private List<BreadcrumbItem> _links;

        // Objects
        public List<SupportDocumentType> _supportDocumentTypes { get; set; } = new();

        SupportDocumentType _supportDocumentType = new();

        // Initialization
        protected async override Task OnInitializedAsync()
        {
            _supportDocumentTypes = await SupportDocumentTypeService.GetSupportDocumentTypes();

            _links = new List<BreadcrumbItem>
            {
                new BreadcrumbItem(text: Localizer["home"], href: "/"),
                new BreadcrumbItem(text: Localizer["configuration"], href: "/configuration"),
                new BreadcrumbItem(text: Localizer["SDTTitle"], href: "", disabled: true)
            };
        }

        // Create support document type
        void CreateSupportDocType()
        {
            NavigationManager.NavigateTo($"documenttypes/createsupportdoctype");
        }

        // Delete support document type
        async Task DeleteSupportDocumentType(int supportDocumentTypeId)
        {
            _supportDocumentTypes.RemoveAll(supportDocumentType => supportDocumentType.SupportDocumentTypeId == supportDocumentTypeId);
            await SupportDocumentTypeService.DeleteSupportDocumentType(supportDocumentTypeId);

            visibleDelete = false;
        }

        // Update support document type
        void UpdateSupportDocType(int supportDocumentTypeId)
        {
            NavigationManager.NavigateTo($"documenttypes/updatesupportdoctype/{supportDocumentTypeId}");
        }

        private string searchString = "";

        private bool FilterFunc(SupportDocumentType element)
        {
            if (string.IsNullOrWhiteSpace(searchString))
                return true;
            if (element.SupportDocumentTypeId.ToString().Contains(searchString, StringComparison.OrdinalIgnoreCase))
                return true;
            if (element.Code.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                return true;
            if (element.Description.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                return true;
            if ($"{element.SupportDocumentTypeId} {element.Code} {element.Description}".Contains(searchString))
                return true;
            return false;
        }


        //Delete Document Type
        private bool visibleDelete = false;
        public int deleteDocumentTypeId = 0;
        private void OpenDeleteDialog(int deleteId)
        {
            deleteDocumentTypeId = deleteId;
            visibleDelete = true;
        }
        void CloseDeleteModal() => visibleDelete = false;
        private DialogOptions dialogDeleteOptions = new() { CloseOnEscapeKey = true, MaxWidth = MaxWidth.ExtraSmall, FullWidth = true, Position = DialogPosition.TopCenter, DisableBackdropClick = true, CloseButton = true };

    }
}
