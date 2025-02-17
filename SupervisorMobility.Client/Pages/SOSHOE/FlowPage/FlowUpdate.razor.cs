using MudBlazor;
using System.Text.RegularExpressions;
using System.Text;
using System.Globalization;
using Microsoft.JSInterop;
using SupervisorMobility.Client.Data.Entities.SOS_Process;
using Blazor.Diagrams;
using Blazor.Diagrams.Core.PathGenerators;
using Blazor.Diagrams.Core.Routers;
using Blazor.Diagrams.Options;
using SupervisorMobility.Client.Pages.SOSHOE.FlowPage.Widgets;
using Blazor.Diagrams.Core.Anchors;
using Blazor.Diagrams.Core.Models.Base;
using Blazor.Diagrams.Core.Models;
using Microsoft.AspNetCore.Components.Web;
using Newtonsoft.Json;
using Blazor.Diagrams.Core.Geometry;

namespace SupervisorMobility.Client.Pages.SOSHOE.FlowPage
{
    public partial class FlowUpdate
    {
        [Parameter]
        public int? FlowId { get; set; }

        SOSFlow _sosFlow { get; set; } = new();


        public bool UpdateButton = false;


        //Commentaries and Logbok
        private DialogOptions dialogCommentariesOptions = new() { CloseOnEscapeKey = true, MaxWidth = MaxWidth.Medium, FullWidth = true, CloseButton = true };
        private DialogOptions dialogLogbookOptions = new() { CloseOnEscapeKey = true, MaxWidth = MaxWidth.Medium, FullWidth = true, CloseButton = true };

        private bool visibleCommentaries = false;
        private bool visibleLogbook = false;

        //Loading
        private IList<string> _sourceMsgLoading = new List<string>();
        private IList<MudBlazor.Color> _Colors = new List<MudBlazor.Color>() { MudBlazor.Color.Default, MudBlazor.Color.Primary, MudBlazor.Color.Secondary, MudBlazor.Color.Success, MudBlazor.Color.Info, MudBlazor.Color.Default, MudBlazor.Color.Primary, MudBlazor.Color.Secondary, MudBlazor.Color.Success, MudBlazor.Color.Info };
        public bool ShowLoading = true;
        private double totalTime;

        //User
        private string json = string.Empty;
        public User user = new();
        public bool logged = false;

        //Diagram 
        private BlazorDiagram Diagram { get; set; } = null!;
        private ElementReference container { get; set; }

        private string diagramJson { get; set; } = "";
        private string inputJson { get; set; } = "";





        protected async override Task OnInitializedAsync()
        {

            var options = new BlazorDiagramOptions
            {
                AllowMultiSelection = true,
                Zoom =
                {
                    Enabled = false,
                },
                Links =
                {
                    DefaultRouter = new OrthogonalRouter(),
                    DefaultPathGenerator = new StraightPathGenerator()
                },
                AllowPanning = false
            };

            if (Diagram == null)
            {

                Diagram = new BlazorDiagram(options);
                Diagram.RegisterComponent<TerminalNode, Widgets.Terminal>();
                Diagram.RegisterComponent<ProcessNode, Widgets.Process>();
                Diagram.RegisterComponent<ConnectionNode, Widgets.Connection>();
                Diagram.RegisterComponent<SupplementNode, Widgets.Supplement>();
                Diagram.RegisterComponent<DecisionNode, Widgets.Decision>();
                Diagram.RegisterComponent<DecisionRomboNode, Widgets.DecisionRombo>();

                Diagram.RegisterComponent<ResizeControl, Widgets.ResizeControlWidget>();



                Diagram.Links.Added += OnLinkAdded;
                Diagram.Nodes.Added += OnNodeAdded;
                Diagram.PointerUp += (mdl, args) =>
                {
                    Diagram.Refresh();
                };
            }

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

            logged = await HasPropertyAsync();

            if (!logged)
            {
                Snackbar.Clear();
                Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
                Snackbar.Add($"Error You have to log in", Severity.Error);
                NavigationManager.NavigateTo($"/");
            }
            else
            {
                _sosFlow = await SOSFlowServices.GetSOSFlow((int)FlowId, true, true, true, true, true);
            }

            ShowLoading = false;
            StateHasChanged();

            await JSRuntime.InvokeVoidAsync("window.preventDefaultDragOver", container);
            await JSRuntime.InvokeVoidAsync("window.addEventsToFigures");
        }

        #region User
        //Local storage user
        private async Task GetUserAsync()
        {
            if (!await TryGetAsync())
            {
                user = new();
            }
        }

        private async Task<bool> TryGetAsync()
        {
            bool hasProperty = await HasPropertyAsync();
            if (hasProperty)
            {
                json = await JSRuntime.InvokeAsync<string>("localStorage.getItem", "user");
                user = System.Text.Json.JsonSerializer.Deserialize<User>(json) ?? new();

            }
            return hasProperty;
        }

        private async Task<bool> HasPropertyAsync()
            => await JSRuntime.InvokeAsync<bool>("localStorage.hasOwnProperty", "user");


        #endregion



        public bool TryGetFlowLogbooksElementAtIndex(int index, out SOSFlowLogbook? item)
        {
            item = null;
            if (_sosFlow.FlowLogbooks == null || _sosFlow.FlowLogbooks.Count == 0)
            {
                return false;
            }

            int invertedIndex = _sosFlow.FlowLogbooks.Count - 1 - index;

            if (invertedIndex >= 0 && invertedIndex < _sosFlow.FlowLogbooks.Count)
            {
                item = _sosFlow.FlowLogbooks?.ElementAt(invertedIndex);
                return true;
            }

            return false;
        }

        public static string ReasonFormat(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return input;
            }

            if (!input.StartsWith("("))
            {
                input = "(" + input;
            }

            if (!input.EndsWith(")"))
            {
                input = input + ")";
            }

            return input;
        }

        private void OpenCommentariesDialog()
        {
            visibleCommentaries = true;
        }

        private void CloseCommentariesDialog()
        {
            visibleCommentaries = false;
        }

        private void OpenLogbookDialog()
        {
            visibleLogbook = true;

        }

        private void CloseLogbookDialog()
        {
            visibleLogbook = false;
        }

        private async void DownloadExcel()
        {
            //await Exportation.ExportFlowToExcel(FlowId.Value);
        }


        void HoeDetails(int HoeId)
        {
            NavigationManager.NavigateTo($"/soshoe/Hub/Details/{HoeId}");
        }
        void UpdateFlow(int FlowId)
        {
            NavigationManager.NavigateTo($"/soshoe/Flow/Update/{FlowId}");
        }


        private bool visibleApproveFlow = false;
        private void OpenApproveFlow()
        {
            visibleApproveFlow = true;
        }
        void CloseSign() => visibleApproveFlow = false;
        private DialogOptions dialogSignOptions = new() { CloseOnEscapeKey = true, MaxWidth = MaxWidth.ExtraSmall, FullWidth = true, DisableBackdropClick = true, CloseButton = true };


        public async Task ApproveFlow()
        {

            _sosFlow.FlowLogbooks.Last()!.Status = 2;

            await UpdateFlow();

            visibleApproveFlow = false;
        }

        private async Task UpdateFlow()
        {
            Snackbar.Clear();
            UpdateButton = true;


            //var resultSOS = await SOSHubServices.UpdateSOSHub(_sosAnalysis.SOSHub);

            //if (resultSOS != null)
            //{
            //    Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
            //    Snackbar.Add($"SOS Updated!", Severity.Info);
            //}
            //else
            //    await JSRuntime.InvokeVoidAsync("alert", "Error al actualizar!");

            var result = await SOSFlowServices.UpdateSOSFlow(_sosFlow);

            if (result != null)
            {
                Snackbar.Add($"Flow Updated!", Severity.Info);

                _sosFlow = result;

                NavigationManager.NavigateTo("/soshoe/Flow");
            }
            else
                await JSRuntime.InvokeVoidAsync("alert", "Error al actualizar!");
            UpdateButton = false;

        }

        //Edit
        //

        private bool _gridPoints;
        private bool _expandedDiagramFigures = true;

        public bool GridPoints
        {
            get => _gridPoints;
            set
            {
                _gridPoints = value;
                Diagram.Refresh();
            }
        }

        private void OnNodeAdded(NodeModel node)
        {
            node.Changed += (node) =>
            {
                if (node is NodeModel nodeM)
                {
                    nodeM.RefreshLinks();
                }
            };
        }

        private void OnLinkAdded(BaseLinkModel link)
        {
            if (link is LinkModel newLink && !newLink.IsAttached)
            {
                link.TargetAttached += OnAttached;
            }

        }

        private void OnAttached(BaseLinkModel obj)
        {
            if (obj.Source is SinglePortAnchor sourceA)
            {
                var srcPort = sourceA.Port;
                var srcNode = srcPort.Parent;

                int position = GetArrayPosition(srcPort.Alignment);

                if (srcNode != null && srcNode is ICmpAnchors srcAnchors)
                {
                    var srcAnchor = srcAnchors.Anchors[position];
                    if (obj.Target is SinglePortAnchor targetA)
                    {
                        var tgtPort = targetA.Port;
                        var tgtNode = tgtPort.Parent;

                        position = GetArrayPosition(tgtPort.Alignment);

                        if (tgtNode != null && tgtNode is ICmpAnchors tgtAnchors)
                        {
                            Diagram.Links.Remove(obj);

                            var tgtAnchor = tgtAnchors.Anchors[position];


                            // Verificar si el nodo fuente o destino es un SupplementNode
                            if (srcNode is SupplementNode || tgtNode is SupplementNode)
                            {
                                var link = new CustomLinkModel(srcAnchor, tgtAnchor);

                                link.Router = new DashedCustomRouter();
                                Diagram.Links.Add(link);
                                link.Refresh();

                            }
                            else
                            {

                                var newLink = new LinkModel(srcAnchor, tgtAnchor)
                                {
                                    TargetMarker = LinkMarker.Arrow,
                                };
                                newLink.Router = new CustomRouter();

                                Diagram.Links.Add(newLink);

                                newLink.Refresh();

                            }

                            Diagram.Refresh();
                        }
                    }
                    else if (obj.Target is PositionAnchor targetAnc)
                    {
                        obj.SetSource(srcAnchor);
                        obj.TargetMarker = LinkMarker.Arrow;
                    }
                }
            }
        }

        // Calculate X position for the circle based on alignment
        private double GetX(PortAlignment alignment) => alignment switch
        {
            PortAlignment.Left => 0,
            PortAlignment.Right => 1,
            _ => 0.5
        };

        // Calculate Y position for the circle based on alignment
        private double GetY(PortAlignment alignment) => alignment switch
        {
            PortAlignment.Top => 0,
            PortAlignment.Bottom => 1,
            _ => 0.5
        };

        private int GetArrayPosition(PortAlignment alignment)
        {
            return alignment switch
            {
                PortAlignment.Left => (int)Positions.Left,
                PortAlignment.Top => (int)Positions.Top,
                PortAlignment.Right => (int)Positions.Right,
                PortAlignment.Bottom => (int)Positions.Bottom,
                _ => 0
            };
        }

        private async void OnDrop(DragEventArgs e)
        {

            var containerOffset = await JSRuntime.InvokeAsync<Offset>("getOffset", container);
            var mouseX = e.ClientX - containerOffset.Left - 42.35;
            var mouseY = e.ClientY - containerOffset.Top - 21.15;


            var figureType = await JSRuntime.InvokeAsync<string>("window.getDraggedFigureType");

            Console.WriteLine("Figure Type: " + figureType);

            switch (figureType)
            {
                case "Terminal":


                    var addedTerminal = new TerminalNode(84.7, 42.3, position: new Point(mouseX, mouseY))
                    {
                        Title = "",
                    };

                    var firstNodeTerminal = Diagram.Nodes.Add(addedTerminal);
                    firstNodeTerminal.AddPort(PortAlignment.Left);
                    firstNodeTerminal.AddPort(PortAlignment.Top);
                    firstNodeTerminal.AddPort(PortAlignment.Right);
                    firstNodeTerminal.AddPort(PortAlignment.Bottom);

                    Diagram.Controls.AddFor(firstNodeTerminal).Add(new ResizeControl());
                    Diagram.SelectModel(firstNodeTerminal, true);
                    break;

                case "Proceso":

                    var addedNode = new ProcessNode(84.7, 42.3, position: new Point(mouseX, mouseY))
                    {
                        Title = "",
                    };

                    var firstNode = Diagram.Nodes.Add(addedNode);
                    firstNode.AddPort(PortAlignment.Left);
                    firstNode.AddPort(PortAlignment.Top);
                    firstNode.AddPort(PortAlignment.Right);
                    firstNode.AddPort(PortAlignment.Bottom);

                    Diagram.Controls.AddFor(firstNode).Add(new ResizeControl());
                    Diagram.SelectModel(firstNode, true);

                    break;


                case "Decision":

                    var addedNodeDecision = new DecisionNode(84.7, 42.3, position: new Point(mouseX, mouseY))
                    {
                        Title = "",
                    };

                    var DecisionNode = Diagram.Nodes.Add(addedNodeDecision);
                    DecisionNode.AddPort(PortAlignment.Left);
                    DecisionNode.AddPort(PortAlignment.Top);
                    DecisionNode.AddPort(PortAlignment.Right);
                    DecisionNode.AddPort(PortAlignment.Bottom);

                    Diagram.Controls.AddFor(DecisionNode).Add(new ResizeControl());
                    Diagram.SelectModel(DecisionNode, true);

                    break;
                case "DecisionRombo":

                    var addedNodeDecisionRombo = new DecisionRomboNode(84.7, 42.3, position: new Point(mouseX, mouseY))
                    {
                        Title = "",
                    };

                    var DecisionRomboNode = Diagram.Nodes.Add(addedNodeDecisionRombo);
                    DecisionRomboNode.AddPort(PortAlignment.Left);
                    DecisionRomboNode.AddPort(PortAlignment.Top);
                    DecisionRomboNode.AddPort(PortAlignment.Right);
                    DecisionRomboNode.AddPort(PortAlignment.Bottom);

                    Diagram.Controls.AddFor(DecisionRomboNode).Add(new ResizeControl());
                    Diagram.SelectModel(DecisionRomboNode, true);

                    break;


                case "Conexion":

                    var addedNodeConnection = new ConnectionNode(84.7, 42.3, position: new Point(mouseX, mouseY))
                    {
                        Title = "",
                    };

                    var NodeConnection = Diagram.Nodes.Add(addedNodeConnection);
                    NodeConnection.AddPort(PortAlignment.Left);
                    NodeConnection.AddPort(PortAlignment.Top);
                    NodeConnection.AddPort(PortAlignment.Right);
                    NodeConnection.AddPort(PortAlignment.Bottom);

                    Diagram.Controls.AddFor(NodeConnection).Add(new ResizeControl());
                    Diagram.SelectModel(NodeConnection, true);

                    break;


                case "Suplemento":

                    var addedNodeSuplement = new SupplementNode(84.7, 42.3, position: new Point(mouseX, mouseY))
                    {
                        Title = "",
                    };

                    var NodeSuplement = Diagram.Nodes.Add(addedNodeSuplement);
                    NodeSuplement.AddPort(PortAlignment.Left);
                    NodeSuplement.AddPort(PortAlignment.Top);
                    NodeSuplement.AddPort(PortAlignment.Right);
                    NodeSuplement.AddPort(PortAlignment.Bottom);

                    Diagram.Controls.AddFor(NodeSuplement).Add(new ResizeControl());
                    Diagram.SelectModel(NodeSuplement, true);

                    break;
            }

            //Diagram.SelectModel(firstNode, true);
        }


        #region Data structures



        private class Offset
        {
            public double Left { get; set; }
            public double Top { get; set; }
        }

        enum Positions
        {
            Left,
            Top,
            Right,
            Bottom
        }
        #endregion

        public class DiagramData
        {
            public List<NodeData> Nodes { get; set; } = new();
            public List<LinkData> Links { get; set; } = new();
        }

        public class NodeData
        {
            public string Id { get; set; }
            public PositionData Position { get; set; }
            public string Type { get; set; } // Para restaurar el tipo de nodo
        }

        public class PositionData
        {
            public double X { get; set; }
            public double Y { get; set; }
        }

        public class LinkData
        {
            public string Id { get; set; }
            public string SourceNodeId { get; set; }
            public string TargetNodeId { get; set; }
        }


        private DiagramData ConvertDiagramToData()
        {
            var data = new DiagramData
            {

            };

            return data;
        }

        private void ConvertDataToDiagram(DiagramData data)
        {
            Diagram.Nodes.Clear();
            Diagram.Links.Clear();



            StateHasChanged();
        }


        private void SaveDiagramAsJson()
        {
            var data = ConvertDiagramToData();
            diagramJson = JsonConvert.SerializeObject(data);
            Console.WriteLine("Diagram JSON: " + diagramJson);
        }

        private void LoadDiagramFromJson()
        {
            if (!string.IsNullOrEmpty(inputJson))
            {
                var data = JsonConvert.DeserializeObject<DiagramData>(inputJson);
                ConvertDataToDiagram(data);
                StateHasChanged();
            }
        }

        String SelectedFigure = "";
        private void SelectDataFigure(string DataFigure)
        {
            SelectedFigure = DataFigure;
            Console.WriteLine(SelectedFigure);
        }

        private async void ChangedExpandedDiagram()
        {
            _expandedDiagramFigures = !_expandedDiagramFigures;
            await JSRuntime.InvokeVoidAsync("window.preventDefaultDragOver", container);
            await JSRuntime.InvokeVoidAsync("window.addEventsToFigures");
        }
    }
}