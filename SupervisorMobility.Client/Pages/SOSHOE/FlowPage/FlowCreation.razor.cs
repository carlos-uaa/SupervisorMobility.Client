using Blazor.Diagrams;
using Blazor.Diagrams.Core.Anchors;
using Blazor.Diagrams.Core.Controls.Default;
using Blazor.Diagrams.Core.Geometry;
using Blazor.Diagrams.Core.Models;
using Blazor.Diagrams.Core.Models.Base;
using Blazor.Diagrams.Core.PathGenerators;
using Blazor.Diagrams.Core.Positions;
using Blazor.Diagrams.Core.Routers;
using Blazor.Diagrams.Options;
using DocumentFormat.OpenXml.Drawing.Charts;
using DocumentFormat.OpenXml.EMMA;
using DocumentFormat.OpenXml.Presentation;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using SupervisorMobility.Client.Pages.SOSHOE.FlowPage.Widgets;
using System.Dynamic;

namespace SupervisorMobility.Client.Pages.SOSHOE.FlowPage
{
    public partial class FlowCreation
    {
        private BlazorDiagram Diagram { get; set; } = null!;
        private ElementReference container;

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

            Diagram = new BlazorDiagram(options);
            Diagram.RegisterComponent<TerminalNode, Widgets.Terminal>();
            Diagram.RegisterComponent<ResizeControl, Widgets.ResizeControlWidget>();

            var node1 = new NodeModel(new Point(50, 50));
            var node2 = new NodeModel(new Point(300, 50));
            var node3 = new NodeModel(new Point(300, 100));
            var node4 = new NodeModel(new Point(300, 200));

            node1.AddPort(PortAlignment.Right);
            node1.AddPort(PortAlignment.Left);

            node2.AddPort(PortAlignment.Right);
            node2.AddPort(PortAlignment.Left);

            node3.AddPort(PortAlignment.Top);
            node3.AddPort(PortAlignment.Bottom);

            node4.AddPort(PortAlignment.Top);
            node4.AddPort(PortAlignment.Bottom);

            Diagram.Nodes.Add(node1);
            Diagram.Controls.AddFor(node1).Add(new BoundaryControl());
            Diagram.Nodes.Add(node2);
            Diagram.Nodes.Add(node3);
            Diagram.Nodes.Add(node4);

            var link = new LinkModel(new SinglePortAnchor(node1.GetPort(PortAlignment.Right)), new SinglePortAnchor(node2.GetPort(PortAlignment.Left))) // Right of node1 to Left of node2
            {
                TargetMarker = LinkMarker.Arrow
            };
            Diagram.Links.Add(link);

            Diagram.Links.Added += OnLinkAdded;
            Diagram.Nodes.Added += OnNodeAdded;
            Diagram.PointerUp += (mdl, args) =>
            {
                Diagram.Refresh();
            };
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
                            var tgtAnchor = tgtAnchors.Anchors[position];

                            var newLink = new LinkModel(srcAnchor, tgtAnchor)
                            {
                                TargetMarker = LinkMarker.Arrow,
                                Router = new CustomRouter()
                            };

                            Diagram.Links.Remove(obj);
                            Diagram.Links.Add(newLink);

                            newLink.Refresh();

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

            var addedNode = new TerminalNode(84.7, 42.3, position: new Point(mouseX, mouseY))
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
    }
}