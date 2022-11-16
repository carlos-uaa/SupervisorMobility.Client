using Microsoft.JSInterop;

namespace SupervisorMobility.Client.Shared
{
    public partial class DeleteSupportDocumentTypeButton
    {
        [CascadingParameter]
        public int SupportDocumentTypeId { get; set; }

        async void DeleteSupportDocumentType()
        {
            bool confirm = await JSRuntime.InvokeAsync<bool>("confirm", $"Are you sure you want to delete this support document type?");

            if (confirm)
            {
                await SupportDocumentTypeService.DeleteSupportDocumentType(SupportDocumentTypeId);
            }
        }
    }
}
