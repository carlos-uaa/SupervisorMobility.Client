using Blazor.Diagrams.Core;
using Blazor.Diagrams.Core.Anchors;
using Blazor.Diagrams.Core.Geometry;
using Blazor.Diagrams.Core.Models;
using Blazor.Diagrams.Core.Models.Base;
using Blazor.Diagrams.Core.Positions;
using Blazor.Diagrams.Core.Routers;
using DocumentFormat.OpenXml.Bibliography;
using SupervisorMobility.Client.Data.Entities.IS;
using System.Reflection;
using static MudBlazor.CategoryTypes;

namespace SupervisorMobility.Client.Pages.SOSHOE.FlowPage.Widgets
{

    public class CustomLinkModel : LinkModel
    {
        public double[] StrokeDashArray { get; set; }

        public CustomLinkModel(Anchor source, Anchor target) : base(source, target)
        {
            // Inicializar con un patrón de línea punteada
            StrokeDashArray = new double[] { 5, 5 };
        }
    }

    public class DashedCustomRouter : NormalRouter
    {
        private readonly Router _fallbackRouter;

        private double _shapeMargin;

        private double _globalMargin;
        public DashedCustomRouter(double shapeMargin = 10.0, double globalMargin = 50.0, Router? fallbackRouter = null)
        {
            _shapeMargin = shapeMargin;
            _globalMargin = globalMargin;
            _fallbackRouter = fallbackRouter ?? new NormalRouter();
        }

        public override Point[] GetRoute(Diagram diagram, BaseLinkModel link)
        {
            if (link.Source == null || link.Target == null)
                return _fallbackRouter.GetRoute(diagram, link); // No routing possible if ports are not set
            if (link.Source is DynamicAnchor originAnchor)
            {
                if (link.Target is DynamicAnchor targetAnchor)
                {
                    var originNode = originAnchor.Node;
                    var targetNode = targetAnchor.Node;

                    var originPosition = originAnchor.Providers[0] as BoundsBasedPositionProvider;
                    var targetPosition = targetAnchor.Providers[0] as BoundsBasedPositionProvider;

                    PortAlignment originAlignment = GetPortAlignment(originPosition.X, originPosition.Y);
                    PortAlignment targetAlignment = GetPortAlignment(targetPosition.X, targetPosition.Y);

                    var points = new List<Point>();

                    // Get the dynamic positions of the source and target ports
                    var sourcePoint = link.Source.GetPosition(link);
                    var targetPoint = link.Target.GetPosition(link);

                    // Add the starting point
                    points.Add(sourcePoint);

                    // Check the relative positions of the source and target
                    CalculateOrthogonalPath(points, sourcePoint, targetPoint, originAlignment, targetAlignment, originNode as ICmpAnchors, targetNode as ICmpAnchors);


                    return points.ToArray();
                }
            }
            return _fallbackRouter.GetRoute(diagram, link);
        }

        private PortAlignment GetPortAlignment(double x, double y)
        {
            if (x == 0)
            {
                return PortAlignment.Left;
            }
            else if (x <= 0.5)
            {
                if (y == 0) return PortAlignment.Top;
                else return PortAlignment.Bottom;
            }
            else
            {
                return PortAlignment.Right;
            }
        }

        (Point, Point) GetBoundCoord(PortAlignment Alignment, ICmpAnchors NodeAnchors, Point ComparePoint)
        {
            const double PortOffset = 10;
            return Alignment switch
            {
                PortAlignment.Top => (new Point(ComparePoint.X - (NodeAnchors.Width / 2), ComparePoint.Y), new Point(ComparePoint.X + (NodeAnchors.Width / 2), ComparePoint.Y + NodeAnchors.Height)),
                PortAlignment.Bottom => (new Point(ComparePoint.X - (NodeAnchors.Width / 2), ComparePoint.Y - NodeAnchors.Height), new Point(ComparePoint.X + (NodeAnchors.Width / 2), ComparePoint.Y)),
                PortAlignment.Left => (new Point(ComparePoint.X, ComparePoint.Y - (NodeAnchors.Height / 2)), new Point(ComparePoint.X + NodeAnchors.Width, ComparePoint.Y + (NodeAnchors.Height / 2))),
                PortAlignment.Right => (new Point(ComparePoint.X - NodeAnchors.Width, ComparePoint.Y - (NodeAnchors.Height / 2)), new Point(ComparePoint.X, ComparePoint.Y + (NodeAnchors.Height / 2))),
                _ => (new Point(0, 0), new Point(0, 0))
            };
        }

        private void CalculateOrthogonalPath(List<Point> points, Point source, Point target
            , PortAlignment srcAlignment, PortAlignment tgtAlignment, ICmpAnchors src, ICmpAnchors tgt)
        {
            // Get boundary coordinates

            (Point sourceTopLeft, Point sourceBottomRight) = GetBoundCoord(srcAlignment, src, source);
            (Point targetTopLeft, Point targetBottomRight) = GetBoundCoord(tgtAlignment, tgt, target);

            const int offset = 20;

            Point first = new(0, 0);
            double sourceX = 0, sourceY = 0, targetX, targetY;

            switch (srcAlignment)
            {
                case PortAlignment.Left:
                    first = new Point(source.X - offset, source.Y);
                    sourceX = source.X - offset; sourceY = source.Y;
                    break;
                case PortAlignment.Right:
                    first = new Point(source.X + offset, source.Y);
                    sourceX = source.X + offset; sourceY = source.Y;
                    break;
                case PortAlignment.Top:
                    first = new Point(source.X, source.Y - offset);
                    sourceX = source.X; sourceY = source.Y - offset;
                    break;
                case PortAlignment.Bottom:
                    first = new Point(source.X, source.Y + offset);
                    sourceX = source.X; sourceY = source.Y + offset;
                    break;
            }

            //points.Add(current);

            (targetX, targetY) = tgtAlignment switch
            {
                PortAlignment.Left => (target.X - offset, target.Y),
                PortAlignment.Right => (target.X + offset, target.Y),
                PortAlignment.Top => (target.X, target.Y - offset),
                PortAlignment.Bottom => (target.X, target.Y + offset),
                _ => (0, 0)
            };

            var middleX = (sourceX + targetX) / 2;
            var middleY = (sourceY + targetY) / 2;

            double tempX, tempY;

            if (srcAlignment == PortAlignment.Left || srcAlignment == PortAlignment.Right)
            {
                if (srcAlignment == tgtAlignment)
                {
                    if ((tgtAlignment == PortAlignment.Right && sourceX > targetX)
                        || (tgtAlignment == PortAlignment.Left && sourceX < targetX))
                    {
                        points.Add(first);
                        if (CheckIfInBounds(targetTopLeft, targetBottomRight, middleY, true))
                        {
                            tempY = CheckIfInBounds(targetTopLeft, targetBottomRight, sourceBottomRight.Y + offset, true) ?
                                targetBottomRight.Y + offset : sourceBottomRight.Y + offset;
                            points.Add(new Point(sourceX, tempY));
                            points.Add(new Point(targetX, tempY));
                            points.Add(new Point(targetX, targetY));
                        }
                        else
                        {
                            points.Add(new Point(sourceX, targetY));
                        }
                    }
                    else
                    {
                        if (CheckIfInBounds(targetTopLeft, targetBottomRight, middleY, true))
                        {
                            tempY = CheckIfInBounds(targetTopLeft, targetBottomRight, sourceBottomRight.Y + offset, true) ?
                                targetBottomRight.Y + offset : sourceBottomRight.Y + offset;
                            points.Add(first);
                            points.Add(new Point(sourceX, tempY));
                            points.Add(new Point(targetX, tempY));
                            points.Add(new Point(targetX, targetY));
                        }
                        else
                        {
                            points.Add(new Point(targetX, first.Y));
                            points.Add(new Point(targetX, targetY));
                        }
                    }
                }
                else
                {
                    bool flag = false;
                    switch (tgtAlignment)
                    {
                        case PortAlignment.Left: flag = sourceX > targetX; goto case PortAlignment.BottomLeft;
                        case PortAlignment.Right: flag = sourceX < targetX; goto case PortAlignment.BottomLeft;
                        case PortAlignment.BottomLeft:
                            points.Add(first);
                            if (flag)
                            {
                                if (CheckIfInBounds(targetTopLeft, targetBottomRight, middleY, true))
                                {
                                    tempY = CheckIfInBounds(targetTopLeft, targetBottomRight, sourceBottomRight.Y + offset, true) ?
                                        targetBottomRight.Y + offset : sourceBottomRight.Y + offset;
                                    points.Add(new Point(sourceX, tempY));
                                    points.Add(new Point(targetX, tempY));
                                }
                                else
                                {
                                    points.Add(new Point(sourceX, middleY));
                                    points.Add(new Point(targetX, middleY));
                                }
                                points.Add(new Point(targetX, targetY));
                            }
                            else
                            {
                                points.Add(new Point(middleX, sourceY));
                                points.Add(new Point(middleX, targetY));
                            }
                            break;
                        case PortAlignment.Top: flag = sourceY > targetY; goto case PortAlignment.TopLeft;
                        case PortAlignment.Bottom: flag = sourceY < targetY; goto case PortAlignment.TopLeft;
                        case PortAlignment.TopLeft:
                            points.Add(first);
                            if (flag)
                            {
                                if (CheckIfInBounds(targetTopLeft, targetBottomRight, middleX, false))
                                {
                                    if (srcAlignment == PortAlignment.Right)
                                    {
                                        tempX = CheckIfInBounds(targetTopLeft, targetBottomRight, sourceBottomRight.X + offset, false) ?
                                            targetBottomRight.X + offset : sourceBottomRight.X + offset;
                                    }
                                    else
                                    {
                                        tempX = CheckIfInBounds(targetTopLeft, targetBottomRight, sourceTopLeft.X - offset, false) ?
                                            targetTopLeft.X - offset : targetTopLeft.X - offset;
                                    }
                                    points.Add(new Point(tempX, sourceY));
                                    points.Add(new Point(tempX, targetY));
                                }
                                else
                                {
                                    if (CheckIfInBounds(sourceTopLeft, sourceBottomRight, middleY, true))
                                    {
                                        if (tgtAlignment == PortAlignment.Top)
                                        {
                                            tempY = CheckIfInBounds(targetTopLeft, targetBottomRight, sourceTopLeft.Y - offset, true) ?
                                            targetTopLeft.Y - offset : sourceTopLeft.Y - offset;
                                        }
                                        else
                                        {
                                            tempY = CheckIfInBounds(targetTopLeft, targetBottomRight, sourceBottomRight.Y + offset, true) ?
                                            targetBottomRight.Y + offset : sourceBottomRight.Y + offset;
                                        }
                                        points.Add(new Point(sourceX, tempY));
                                        points.Add(new Point(targetX, tempY));
                                    }
                                    else
                                    {
                                        points.Add(new Point(sourceX, targetY));
                                        points.Add(new Point(middleX, targetY));
                                    }
                                }
                                points.Add(new Point(targetX, targetY));
                            }
                            else
                            {
                                if ((srcAlignment == PortAlignment.Right && sourceX > targetX)
                                    || (srcAlignment == PortAlignment.Left && sourceX < targetX))
                                {
                                    if (CheckIfInBounds(sourceTopLeft, sourceBottomRight, middleY, true))
                                    {
                                        if (tgtAlignment == PortAlignment.Top)
                                        {
                                            tempY = CheckIfInBounds(targetTopLeft, targetBottomRight, sourceTopLeft.Y - offset, true) ?
                                            targetTopLeft.Y - offset : sourceTopLeft.Y - offset;
                                        }
                                        else
                                        {
                                            tempY = CheckIfInBounds(targetTopLeft, targetBottomRight, sourceBottomRight.Y + offset, true) ?
                                            targetBottomRight.Y + offset : sourceBottomRight.Y + offset;
                                        }
                                        points.Add(new Point(sourceX, tempY));
                                        points.Add(new Point(targetX, tempY));
                                    }
                                    else
                                    {
                                        points.Add(new Point(sourceX, targetY));
                                        points.Add(new Point(targetX, targetY));
                                    }
                                }
                                else
                                {
                                    points.Add(new Point(targetX, sourceY));
                                }
                            }
                            break;
                    }
                }
            }
            else //Top and Bottom
            {
                if (srcAlignment == tgtAlignment)
                {
                    if ((tgtAlignment == PortAlignment.Top && sourceY < targetY)
                        || (tgtAlignment == PortAlignment.Bottom && sourceY > targetY))
                    {
                        points.Add(first);
                        if (CheckIfInBounds(targetTopLeft, targetBottomRight, sourceX, false))
                        {
                            if (sourceX < targetX)
                            {
                                tempX = CheckIfInBounds(targetTopLeft, targetBottomRight, sourceBottomRight.X + offset, false) ?
                                targetBottomRight.X + offset : sourceBottomRight.X + offset;
                            }
                            else
                            {
                                tempX = CheckIfInBounds(targetTopLeft, targetBottomRight, sourceTopLeft.X - offset, false) ?
                                targetTopLeft.X - offset : sourceTopLeft.X - offset;
                            }

                            points.Add(new Point(tempX, sourceY));
                            points.Add(new Point(tempX, targetY));
                            points.Add(new Point(targetX, targetY));
                        }
                        else
                        {
                            points.Add(new Point(targetX, sourceY));
                        }
                    }
                    else
                    {
                        points.Add(first);
                        if (CheckIfInBounds(targetTopLeft, targetBottomRight, middleX, false))
                        {
                            if (sourceX >= targetX)
                            {
                                tempX = CheckIfInBounds(targetTopLeft, targetBottomRight, sourceBottomRight.X + offset, false) ?
                                targetBottomRight.X + offset : sourceBottomRight.X + offset;
                            }
                            else
                            {
                                tempX = CheckIfInBounds(targetTopLeft, targetBottomRight, sourceTopLeft.X - offset, false) ?
                                targetTopLeft.X - offset : sourceTopLeft.X - offset;
                            }
                            points.Add(new Point(tempX, sourceY));
                            points.Add(new Point(tempX, targetY));
                        }
                        else
                        {
                            points.Add(new Point(sourceX, targetY));
                        }
                        points.Add(new Point(targetX, targetY));
                    }
                }
                else
                {
                    bool flag = false;
                    switch (tgtAlignment)
                    {
                        case PortAlignment.Left: flag = sourceX > targetX; goto case PortAlignment.BottomLeft;
                        case PortAlignment.Right: flag = sourceX < targetX; goto case PortAlignment.BottomLeft;
                        case PortAlignment.BottomLeft:
                            points.Add(first);
                            if (flag)
                            {
                                if (CheckIfInBounds(targetTopLeft, targetBottomRight, middleY, true))
                                {
                                    if (srcAlignment == PortAlignment.Top)
                                        tempY = CheckIfInBounds(targetTopLeft, targetBottomRight, sourceTopLeft.Y - offset, true) ?
                                            targetTopLeft.Y - offset : sourceTopLeft.Y - offset;
                                    else
                                        tempY = CheckIfInBounds(targetTopLeft, targetBottomRight, sourceBottomRight.Y + offset, true) ?
                                            targetBottomRight.Y + offset : sourceBottomRight.Y + offset;
                                    points.Add(new Point(sourceX, tempY));
                                    points.Add(new Point(targetX, tempY));
                                }
                                else
                                {
                                    if (CheckIfInBounds(sourceTopLeft, sourceBottomRight, middleX, false))
                                    {
                                        if (tgtAlignment == PortAlignment.Right)
                                        {
                                            tempX = CheckIfInBounds(targetTopLeft, targetBottomRight, sourceBottomRight.X, false) ?
                                                targetBottomRight.X + offset : sourceBottomRight.X + offset;
                                        }
                                        else
                                        {
                                            tempX = CheckIfInBounds(targetTopLeft, targetBottomRight, sourceTopLeft.X, false) ?
                                                targetTopLeft.X - offset : sourceTopLeft.X - offset;
                                        }
                                        points.Add(new Point(tempX, sourceY));
                                        points.Add(new Point(tempX, targetY));
                                    }
                                    else
                                    {
                                        points.Add(new Point(targetX, sourceY));
                                    }
                                }
                                points.Add(new Point(targetX, targetY));
                            }
                            else
                            {
                                if ((srcAlignment == PortAlignment.Top && sourceY < targetY)
                                    || (srcAlignment == PortAlignment.Bottom && sourceY > targetY))
                                {
                                    if (CheckIfInBounds(sourceTopLeft, sourceBottomRight, middleX, false))
                                    {
                                        if (tgtAlignment == PortAlignment.Right)
                                        {
                                            tempX = CheckIfInBounds(targetTopLeft, targetBottomRight, sourceTopLeft.X - offset - 10, false) ?
                                                (CheckIfInBounds(targetTopLeft, targetBottomRight, sourceBottomRight.X + offset, false) ?
                                                    targetBottomRight.X + offset : sourceBottomRight.X + offset) : sourceTopLeft.X - offset;
                                        }
                                        else
                                        {
                                            tempX = CheckIfInBounds(targetTopLeft, targetBottomRight, sourceBottomRight.X + offset + 10, false) ?
                                                (CheckIfInBounds(targetTopLeft, targetBottomRight, sourceTopLeft.X - offset, false) ?
                                                    targetTopLeft.X - offset : sourceTopLeft.X - offset) : sourceBottomRight.X + offset;
                                        }
                                        points.Add(new Point(tempX, sourceY));
                                        points.Add(new Point(tempX, targetY));
                                    }
                                    else
                                    {
                                        points.Add(new Point(middleX, sourceY));
                                        points.Add(new Point(middleX, targetY));
                                    }
                                }
                                else
                                {
                                    points.Add(new Point(sourceX, targetY));
                                }
                            }
                            break;
                        case PortAlignment.Top: flag = sourceY > targetY; goto case PortAlignment.TopLeft;
                        case PortAlignment.Bottom: flag = sourceY < targetY; goto case PortAlignment.TopLeft;
                        case PortAlignment.TopLeft:
                            points.Add(first);
                            if (flag)
                            {
                                if (CheckIfInBounds(targetTopLeft, targetBottomRight, middleX, false))
                                {
                                    if (sourceX >= targetX)
                                    {
                                        tempX = CheckIfInBounds(targetTopLeft, targetBottomRight, sourceBottomRight.X + offset, false) ?
                                            targetBottomRight.X + offset : sourceBottomRight.X + offset;
                                    }
                                    else
                                    {
                                        tempX = CheckIfInBounds(targetTopLeft, targetBottomRight, sourceTopLeft.X - offset, false) ?
                                            targetTopLeft.X - offset : targetTopLeft.X - offset;
                                    }
                                    points.Add(new Point(tempX, sourceY));
                                    points.Add(new Point(tempX, targetY));
                                }
                                else
                                {
                                    points.Add(new Point(middleX, sourceY));
                                    points.Add(new Point(middleX, targetY));
                                }
                                points.Add(new Point(targetX, targetY));
                            }
                            else
                            {
                                points.Add(first);
                                points.Add(new Point(sourceX, middleY));
                                points.Add(new Point(targetX, middleY));
                            }
                            break;
                    }
                }
            }
        }

     

        bool CheckIfInBounds(Point TopLeftBound, Point BottomRightBound, double value, bool height)
        {
            return height ? value <= BottomRightBound.Y && value >= TopLeftBound.Y : value <= BottomRightBound.X && value >= TopLeftBound.X;
        }
    }
}
