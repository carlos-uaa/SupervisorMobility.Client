using Blazor.Diagrams.Core.Models;
using Blazor.Diagrams;
using Microsoft.AspNetCore.Components.Web;
using SupervisorMobility.Client.Pages.SOSHOE.FlowPage.Components.Nodes;
using SupervisorMobility.Client.Pages.SOSHOE.FlowPage.Components.Widgets;
using Blazorise;


namespace SupervisorMobility.Client.Pages.SOSHOE.FlowPage
{
    public partial class FlowDetailsRenew
    {

        private readonly BlazorDiagram _blazorDiagram = new BlazorDiagram();
      
        private int? _draggedType;

        //Loading
        private IList<string> _sourceMsgLoading = new List<string>();
        private IList<MudBlazor.Color> _Colors = new List<MudBlazor.Color>() { MudBlazor.Color.Default, MudBlazor.Color.Primary, MudBlazor.Color.Secondary, MudBlazor.Color.Success, MudBlazor.Color.Info, MudBlazor.Color.Default, MudBlazor.Color.Primary, MudBlazor.Color.Secondary, MudBlazor.Color.Success, MudBlazor.Color.Info };
        public bool ShowLoading = true;

        protected async override Task OnInitializedAsync()
        {
            base.OnInitialized();

            _blazorDiagram.RegisterComponent<TerminalNode, TerminalWidget>();
          

          

            _sourceMsgLoading.Add($"{Localizer1["Loading1"]}");
            _sourceMsgLoading.Add($"{Localizer1["Loading2"]}");
            _sourceMsgLoading.Add($"{Localizer1["Loading3"]}");
            _sourceMsgLoading.Add($"{Localizer1["Loading4"]}");
            _sourceMsgLoading.Add($"{Localizer1["Loading5"]}");
            _sourceMsgLoading.Add($"{Localizer1["Loading6"]}");
            _sourceMsgLoading.Add($"{Localizer1["Loading7"]}");
            _sourceMsgLoading.Add($"{Localizer1["Loading8"]}");
            _sourceMsgLoading.Add($"{Localizer1["Loading9"]}");
            _sourceMsgLoading.Add($"{Localizer1["Loading10"]}");
            _sourceMsgLoading.Add($"{Localizer1["Loading11"]}");

         

            ShowLoading = false;
            StateHasChanged();

        }

        private bool _gridPoints;

        public bool GridPoints
        {
            get => _gridPoints;
            set
            {
                _gridPoints = value;
                _blazorDiagram.Refresh();
            }
        }

        private void OnDragStart(int key)
        {
            // Can also use transferData, but this is probably "faster"
            _draggedType = key;
        }

        private void OnDrop(DragEventArgs e)
        {
            if (_draggedType == null)
                return;

            // Verifica que el contenedor esté disponible
            if (_blazorDiagram.Container == null)
            {
                Console.WriteLine("Container is not available for dropping the node.");
                return;
            }

            var position = _blazorDiagram.GetRelativeMousePoint(e.ClientX, e.ClientY);
            var node = _draggedType == 0 ? new NodeModel(position) : new TerminalNode(position);
            node.AddPort(PortAlignment.Top);
            node.AddPort(PortAlignment.Bottom);
            _blazorDiagram.Nodes.Add(node);
            _draggedType = null;
        }

    }
}