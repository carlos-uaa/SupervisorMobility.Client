using Blazor.Diagrams;
using Blazor.Diagrams.Core.Anchors;
using Blazor.Diagrams.Core.Geometry;
using Blazor.Diagrams.Core.Models;
using Blazor.Diagrams.Core.Models.Base;
using Blazor.Diagrams.Core.PathGenerators;
using Blazor.Diagrams.Core.Routers;
using Blazor.Diagrams.Options;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using SupervisorMobility.Client.Pages.SOSHOE.FlowPage.Widgets;
using System.Reflection;

namespace SupervisorMobility.Client.Pages.SOSHOE.FlowPage
{
    public partial class FlowCreation
    {
        private BlazorDiagram Diagram { get; set; } = null!;
        private ElementReference container;
        private string diagramJson { get; set; } = "";
        private string inputJson { get; set; } = "";
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await JSRuntime.InvokeVoidAsync("window.preventDefaultDragOver", container);
                await JSRuntime.InvokeVoidAsync("window.addEventsToFigures");
            }
        }
        protected override void OnInitialized()
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


                            BaseLinkModel link = new LinkModel(srcAnchor, tgtAnchor);
                            if (srcNode is SupplementNode || tgtNode is SupplementNode)
                            {
                                link.Router = new DashedCustomRouter();
                            }
                            else
                            {
                                link.TargetMarker = LinkMarker.Arrow;
                                link.Router = new CustomRouter();
                            }
                            Diagram.Links.Add(link);
                            link.Refresh();
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


    }


}