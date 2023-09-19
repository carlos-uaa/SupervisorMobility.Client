using DocumentFormat.OpenXml.Bibliography;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using MudBlazor;
using System.Globalization;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;

namespace SupervisorMobility.Client.Pages.Inicio.LupPage
{
    public partial class LupUpdate
    {

        [Parameter]
        public int LupId { get; set; }

        private List<BreadcrumbItem> _links;

        public Lup _lup { get; set; } = new();
        public JobObservation jobObservation { get; set; } = new();

        //Justification Modal
        private bool visible = false;
        private void OpenCancelDialog()
        {
            visible = true;
        }
        void Close() => visible = false;
        private DialogOptions dialogOptions = new() { CloseOnEscapeKey = true, MaxWidth = MaxWidth.Small, FullWidth = true };


        protected async override Task OnInitializedAsync()
        {
             _links = new List<BreadcrumbItem>
            {
                new BreadcrumbItem(text: Localizer["home"], href: "#"),
                new BreadcrumbItem("LUP", href: "/lup"),
                new BreadcrumbItem(text: Localizer["update"] + " LUP", href: "/", disabled: true),
            };

            _lup = await LupServices.GetLupByIdWhitFile(LupId);
            jobObservation = await JobObservationService.GetJobObservationById(_lup.JobObservationId);
        }
        private async Task EditLup()
        {

            _lup.Status = 2;

            var result = await LupServices.UpdateLup(_lup);

            if (result)
            {
                Snackbar.Clear();
                Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                Snackbar.Add($"Lup {LupId} Updated", Severity.Info);
                NavigationManager.NavigateTo("/lup");
            }
            else
                await JSRuntime.InvokeVoidAsync("alert", "Update failed!"); // Alert
        }

        public async void CancelLup()
        {

            if(_lup.Justification == null || _lup.Justification.Length == 0)
            {
                Snackbar.Clear();
                Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                Snackbar.Add($"You need to add the justification", Severity.Error);
                return;
            }


            _lup.EndDate = DateTime.Now;
            _lup.Status = 4;

            var result = await LupServices.UpdateLup(_lup);

            if (result)
            {
                Snackbar.Clear();
                Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                Snackbar.Add($"Lup {LupId} Canceled", Severity.Info);
                NavigationManager.NavigateTo("/lup");
            }
            else
                await JSRuntime.InvokeVoidAsync("alert", "Update failed!"); // Alert
        }

        void GoBack()
        {
            NavigationManager.NavigateTo("/lup");
        }

        public async void FinishedLup()
        {

            _lup.EndDate = DateTime.Now;
            _lup.Status = 3;

            var result = await LupServices.UpdateLup(_lup);

            if (result)
            {
                Snackbar.Clear();
                Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                Snackbar.Add($"Lup {LupId} Finished", Severity.Info);
                NavigationManager.NavigateTo("/lup");
            }
            else
                await JSRuntime.InvokeVoidAsync("alert", "Update failed!"); // Alert

        }

        void GoToJobObservation(int jobObservationId)
        {
            NavigationManager.NavigateTo($"jobobservation/updatejobobservation/{jobObservationId}");
        }


        //Evidence
        private static string DefaultDragClass = "relative rounded-lg border-2 border-dashed pa-4 mt-4 mud-width-full mud-height-full z-10";
        private string DragClass = DefaultDragClass;

        private List<FileToDisplay> fileNames = new List<FileToDisplay>();
        private List<IBrowserFile> fileNames2 = new List<IBrowserFile>();


        private bool upload = true;

        private int maxAllowedFiles = 5;
        private long maxFileSize = long.MaxValue;

        private class FileToDisplay
        {
            public string name { get; set; }
            public string ftype { get; set; }
            public string message { get; set; }
        }

        private async void RemoveEvidence(int fileUploadId)
        {
            var response = await LupServices.RemoveEvidence(LupId, fileUploadId);
            Console.WriteLine(fileUploadId);
            if (response)
            {
                Snackbar.Clear();
                Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                Snackbar.Add($"Evidence removed", Severity.Info);
                _lup = await LupServices.GetLupByIdWhitFile(LupId);
                StateHasChanged();
            }
            else
            {
                Snackbar.Clear();
                Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                Snackbar.Add($"Failed to remove evidence", Severity.Error);
            }
        }


        private void OnInputFileChanged(InputFileChangeEventArgs e)
        {
            ClearDragClass();
            fileNames.Clear();
            fileNames2.Clear();
            foreach (var file in e.GetMultipleFiles(maxAllowedFiles))
            {
                fileNames2.Add(file);
                fileNames.Add(new FileToDisplay() { name = file.Name, ftype = file.ContentType });
            }
            Console.WriteLine($"{fileNames2.Count}");

            upload = false;

        }

        private async Task Clear()
        {
            upload = true;
            fileNames.Clear();
            fileNames2.Clear();

            ClearDragClass();
            await Task.Delay(100);
        }

        private async Task Upload()
        {
            //Upload the files here
            Snackbar.Configuration.PositionClass = Defaults.Classes.Position.TopCenter;
            //call function upload files
            Console.WriteLine($" en carga {fileNames2.Count}");


            foreach (var file in fileNames2)
            {
                using var content = new MultipartFormDataContent();
                var fileContent = new StreamContent(file.OpenReadStream(maxFileSize));
                fileContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);

                content.Add(
                         content: fileContent,
                         name: "\"file\"",
                         fileName: file.Name);

                var result = await FilesServices.UploadEvidences(content, LupId);
                if (result is not null)
                {
                    Snackbar.Configuration.MaxDisplayedSnackbars = 10;
                    Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                    Snackbar.Add($"{file.Name} Added to Lup {LupId}", Severity.Info);
                }
                else
                {
                    Snackbar.Clear();
                    Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                    Snackbar.Add($"Failed to upload Evidence to Lup", Severity.Error);
                    break;
                }
            }

            fileNames.Clear();
            fileNames2.Clear();
            _lup = await LupServices.GetLupByIdWhitFile(LupId);
            StateHasChanged();

            upload = true;

        }

        private void SetDragClass()
        {
            DragClass = $"{DefaultDragClass} mud-border-primary";
        }

        private void ClearDragClass()
        {
            DragClass = DefaultDragClass;
        }

        private async Task DownloadFile(int fileId, string filename)
        {
            await FilesServices.DownloadFileEvidence(fileId, filename);
        }

    }
}
