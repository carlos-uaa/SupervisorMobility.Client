using MudBlazor;
using SupervisorMobility.Client.Data.Entities;

namespace SupervisorMobility.Client.Pages.Inicio.LupPage
{
    public partial class LupDetails
    {

        [Parameter]
        public int LupId { get; set; }

        public Lup _lup { get; set; } = new();
        public JobObservation jobObservation { get; set; } = new();
        Dictionary<int, string> imageUrls = new Dictionary<int, string>();


        bool showLoading = true;
        private IList<string> _sourceMsgLoading = new List<string>();
        private IList<Color> _Colors = new List<Color>() { Color.Default, Color.Primary, Color.Secondary, Color.Success, Color.Info, Color.Default, Color.Primary, Color.Secondary, Color.Success, Color.Info };


        protected async override Task OnInitializedAsync()
        {
            _sourceMsgLoading = new List<string>();
            for (int i = 1; i <= 11; i++)
            {
                _sourceMsgLoading.Add($"{Localizer1["Loading" + i]}");
            }

            _lup = await LupServices.GetLupByIdWhitFile(LupId);
            jobObservation = await JobObservationService.GetJobObservationById(_lup.JobObservationId, true);

            foreach (var evidence in _lup.Evidences)
            {
                if (evidence.ContentType == "image/png")
                {
                    var imageUrl = await FilesServices.ShowImageEvidence(evidence.FileUploadId);
                    imageUrls[evidence.FileUploadId] = imageUrl;
                }
            }
            showLoading = false;
        }

        void GoToJobObservation(int jobObservationId)
        {
            NavigationManager.NavigateTo($"jobobservation/updatejobobservation/{jobObservationId}");
        }

        private async Task DownloadFile(int fileId, string filename)
        {
            await FilesServices.DownloadFileEvidence(fileId, filename);
        }

        //Show Photo
        private DialogOptions dialogPhotoOptions = new() { CloseOnEscapeKey = true, MaxWidth = MaxWidth.Medium, FullWidth = true, CloseButton = true };

        private bool visiblePhoto = false;

        private int photoIndex = 0;

        private void OpenPhotoDialog(int index)
        {
            photoIndex = index;
            visiblePhoto = true;

        }


        private void CloseChip() { }
    }
}
