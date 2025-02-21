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
using System.Threading;
using static SupervisorMobility.Client.Pages.SOSHOE.FlowPage.FlowCreation;
using Microsoft.AspNetCore.Components.Routing;
using DocumentFormat.OpenXml.Drawing.ChartDrawing;

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

        private void OnNodeAdded(NodeModel nodo)
        {
            nodo.Changed += (nodo) =>
            {
                if (nodo is NodeModel nodoM)
                {
                    nodoM.RefreshLinks();
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

        private int GetArrayPosition(string alignment)
        {
            return alignment switch
            {
                nameof(PortAlignment.Left) => (int)Positions.Left,
                nameof(PortAlignment.Top) => (int)Positions.Top,
                nameof(PortAlignment.Right) => (int)Positions.Right,
                nameof(PortAlignment.Bottom) => (int)Positions.Bottom,
                _ => 0
            };
        }

        private string GetPortAlignment(DynamicAnchor anchor, NodeModel node, BaseLinkModel link)
        {
            var anchorPosition = anchor.GetPosition(link, link.Route);
            var leftDistance = Math.Abs(anchorPosition.X - node.Position.X);
            var rightDistance = Math.Abs(anchorPosition.X - (node.Position.X + node.Size.Width));
            var topDistance = Math.Abs(anchorPosition.Y - node.Position.Y);
            var bottomDistance = Math.Abs(anchorPosition.Y - (node.Position.Y + node.Size.Height));

            var minDistance = new[] { leftDistance, rightDistance, topDistance, bottomDistance }.Min();

            if (minDistance == leftDistance) return PortAlignment.Left.ToString();
            if (minDistance == rightDistance) return PortAlignment.Right.ToString();
            if (minDistance == topDistance) return PortAlignment.Top.ToString();
            return PortAlignment.Bottom.ToString();
        }

        private async void OnDrop(DragEventArgs e)
        {
            var containerOffset = await JSRuntime.InvokeAsync<Offset>("getOffset", container);
            var mouseX = e.ClientX - containerOffset.Left - 42.35;
            var mouseY = e.ClientY - containerOffset.Top - 21.15;

            var figureType = await JSRuntime.InvokeAsync<string>("window.getDraggedFigureType");

            NodeModel addedNode = figureType switch
            {
                "Terminal" => new TerminalNode(84.7, 42.3, position: new Point(mouseX, mouseY)) { Title = "" },
                "Proceso" => new ProcessNode(84.7, 42.3, position: new Point(mouseX, mouseY)) { Title = "" },
                "Decision" => new DecisionNode(84.7, 42.3, position: new Point(mouseX, mouseY)) { Title = "" },
                "DecisionRombo" => new DecisionRomboNode(84.7, 42.3, position: new Point(mouseX, mouseY)) { Title = "" },
                "Conexion" => new ConnectionNode(84.7, 42.3, position: new Point(mouseX, mouseY)) { Title = "" },
                "Suplemento" => new SupplementNode(84.7, 42.3, position: new Point(mouseX, mouseY)) { Title = "" },
                _ => throw new InvalidOperationException("Tipo de figura desconocido")
            };

            var addedNodeModel = Diagram.Nodes.Add(addedNode);
            addedNodeModel.AddPort(PortAlignment.Left);
            addedNodeModel.AddPort(PortAlignment.Top);
            addedNodeModel.AddPort(PortAlignment.Right);
            addedNodeModel.AddPort(PortAlignment.Bottom);

            Diagram.Controls.AddFor(addedNodeModel).Add(new ResizeControl());
            Diagram.SelectModel(addedNodeModel, true);
        }

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
                if (data != null)
                {
                    ConvertDataToDiagram(data);
                    StateHasChanged();
                }
            }
        }


        private async void ChangedExpandedDiagram()
        {
            _expandedDiagramFigures = !_expandedDiagramFigures;
            await JSRuntime.InvokeVoidAsync("window.preventDefaultDragOver", container);
            await JSRuntime.InvokeVoidAsync("window.addEventsToFigures");
        }

        private DiagramData ConvertDiagramToData()
        {
            var data = new DiagramData
            {
                Nodes = Diagram.Nodes.Select(node => new NodeData
                {
                    Id = node.Id,
                    Position = new PositionData { X = node.Position.X, Y = node.Position.Y },
                    Size = new SizeData { Width = node.Size.Width-20, Height = node.Size.Height-20 },
                    Type = node.GetType().Name,
                    Title = node?.Title ?? string.Empty
                }).ToList(),

                Links = Diagram.Links.Select(link =>
                {
                    var sourceNode = link.Source.Model as NodeModel;
                    var targetNode = link.Target.Model as NodeModel;

                    var sourcePortAlignment = link.Source is DynamicAnchor source_Anchor ? GetPortAlignment(source_Anchor, sourceNode, link) : "No disponible";
                    var targetPortAlignment = link.Target is DynamicAnchor target_Anchor ? GetPortAlignment(target_Anchor, targetNode, link) : "No disponible";

                    return new LinkData
                    {
                        Id = link.Id,
                        SourceNodeId = sourceNode?.Id,
                        TargetNodeId = targetNode?.Id,
                        SourcePort = sourcePortAlignment,
                        TargetPort = targetPortAlignment
                    };
                }).ToList()
            };

            return data;
        }
        private void ConvertDataToDiagram(DiagramData data)
        {
            Diagram.Nodes.Clear();
            Diagram.Links.Clear();

            var nodeDictionary = new Dictionary<string, NodeModel>();

            foreach (var nodeData in data.Nodes)
            {

                /////////////////////////////////
                //NodeModel addedNode = figureType switch
                //{
                //    "Terminal" => new TerminalNode(84.7, 42.3, position: new Point(mouseX, mouseY)) { Title = "" },
                //    "Proceso" => new ProcessNode(84.7, 42.3, position: new Point(mouseX, mouseY)) { Title = "" },
                //    "Decision" => new DecisionNode(84.7, 42.3, position: new Point(mouseX, mouseY)) { Title = "" },
                //    "DecisionRombo" => new DecisionRomboNode(84.7, 42.3, position: new Point(mouseX, mouseY)) { Title = "" },
                //    "Conexion" => new ConnectionNode(84.7, 42.3, position: new Point(mouseX, mouseY)) { Title = "" },
                //    "Suplemento" => new SupplementNode(84.7, 42.3, position: new Point(mouseX, mouseY)) { Title = "" },
                //    _ => throw new InvalidOperationException("Tipo de figura desconocido")
                //};

                //var addedNodeModel = Diagram.Nodes.Add(addedNode);
                //addedNodeModel.AddPort(PortAlignment.Left);
                //addedNodeModel.AddPort(PortAlignment.Top);
                //addedNodeModel.AddPort(PortAlignment.Right);
                //addedNodeModel.AddPort(PortAlignment.Bottom);

                //Diagram.Controls.AddFor(addedNodeModel).Add(new ResizeControl());
                //Diagram.SelectModel(addedNodeModel, true);
                //////////////


                NodeModel node = nodeData.Type switch
                {
                    nameof(TerminalNode) => new TerminalNode(nodeData.Size.Width, nodeData.Size.Height, new Point(nodeData.Position.X, nodeData.Position.Y)) { Title = nodeData.Title },
                    nameof(ProcessNode) => new ProcessNode(nodeData.Size.Width, nodeData.Size.Height, new Point(nodeData.Position.X, nodeData.Position.Y)) { Title = nodeData.Title },
                    nameof(ConnectionNode) => new ConnectionNode(nodeData.Size.Width, nodeData.Size.Height, new Point(nodeData.Position.X, nodeData.Position.Y)) { Title = nodeData.Title },
                    nameof(SupplementNode) => new SupplementNode(nodeData.Size.Width, nodeData.Size.Height, new Point(nodeData.Position.X, nodeData.Position.Y)) { Title = nodeData.Title },
                    nameof(DecisionNode) => new DecisionNode(nodeData.Size.Width, nodeData.Size.Height, new Point(nodeData.Position.X, nodeData.Position.Y)) { Title = nodeData.Title },
                    nameof(DecisionRomboNode) => new DecisionRomboNode(nodeData.Size.Width, nodeData.Size.Height, new Point(nodeData.Position.X, nodeData.Position.Y)) { Title = nodeData.Title },
                    _ => throw new InvalidOperationException("Tipo de nodo desconocido")
                };

                var NodeModel = Diagram.Nodes.Add(node);
                NodeModel.AddPort(PortAlignment.Left);
                NodeModel.AddPort(PortAlignment.Top);
                NodeModel.AddPort(PortAlignment.Right);
                NodeModel.AddPort(PortAlignment.Bottom);

                Diagram.Controls.AddFor(NodeModel).Add(new ResizeControl());
                nodeDictionary[nodeData.Id] = NodeModel;
                //Diagram.Nodes.Add(node);
                Diagram.Refresh();

            }

            foreach (var linkData in data.Links)
            {
                if (nodeDictionary.TryGetValue(linkData.SourceNodeId, out var sourceNode) &&
                    nodeDictionary.TryGetValue(linkData.TargetNodeId, out var targetNode))
                {
                    var sourcePort = sourceNode.Ports.FirstOrDefault(p => p.Alignment.ToString() == linkData.SourcePort);
                    var targetPort = targetNode.Ports.FirstOrDefault(p => p.Alignment.ToString() == linkData.TargetPort);
                    
                    //Si intento obtener el anchor falla, como si el nodo fuera nullo
                    var srcAnchor = (sourceNode as ICmpAnchors).Anchors[GetArrayPosition(linkData.SourcePort)];
                    var tgtAnchor = (targetNode as ICmpAnchors).Anchors[GetArrayPosition(linkData.TargetPort)];


                    if (sourceNode is SupplementNode || targetNode is SupplementNode)
                    {
                        var link = new LinkModel(sourcePort, targetPort);

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

            StateHasChanged();
        }

        private void PrintDiagramContent()
        {
            Console.WriteLine("Nodos del diagrama:");
            foreach (var node in Diagram.Nodes)
            {
                Console.WriteLine($"Nodo ID: {node.Id}, Tipo: {node.GetType().Name}, Título: {node.Title}, Posición: ({node.Position.X}, {node.Position.Y}), Tamaño: ({node.Size.Width}, {node.Size.Height})");
                foreach (var port in node.Ports)
                {
                    Console.WriteLine($"   Puerto ID: {port.Id}, Alineación: {port.Alignment}");
                }
            }

            Console.WriteLine("Enlaces del diagrama:");
            foreach (var link in Diagram.Links)
            {
                var sourceNode = link.Source.Model as NodeModel;
                var targetNode = link.Target.Model as NodeModel;

                var sourceAnchor = link.Source as DynamicAnchor;
                var targetAnchor = link.Target as DynamicAnchor;

                var sourcePosition = sourceAnchor?.GetPosition(link, link.Route);
                var targetPosition = targetAnchor?.GetPosition(link, link.Route);

                var sourcePortAlignment = link.Source is DynamicAnchor source_Anchor ? GetPortAlignment(source_Anchor, sourceNode, link) : "No disponible";
                var targetPortAlignment = link.Target is DynamicAnchor target_Anchor ? GetPortAlignment(target_Anchor, targetNode, link) : "No disponible";

                Console.WriteLine($"Enlace ID: {link.Id}, Nodo Origen: {sourceNode?.Id}, Nodo Destino: {targetNode?.Id}, Posición Origen: ({sourcePosition?.X}, {sourcePosition?.Y}), Port Origen: {sourcePortAlignment} , Posición Destino: ({targetPosition?.X}, {targetPosition?.Y}), Port Destino: {targetPortAlignment}");
            }
        }

    }
}