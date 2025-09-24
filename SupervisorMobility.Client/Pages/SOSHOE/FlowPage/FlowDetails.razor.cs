using MudBlazor;
using Microsoft.JSInterop;
using Newtonsoft.Json;
using System.Runtime.CompilerServices;
using SupervisorMobility.Client.Pages.SOSHOE.FlowPage.Widgets;
using Blazor.Diagrams.Core.Models.Base;
using Blazor.Diagrams.Core.Models;
using Blazor.Diagrams.Core.Geometry;
using Blazor.Diagrams;
using Blazor.Diagrams.Core.PathGenerators;
using Blazor.Diagrams.Core.Routers;
using Blazor.Diagrams.Options;
using static System.Net.WebRequestMethods;
using DocumentFormat.OpenXml.Vml;

namespace SupervisorMobility.Client.Pages.SOSHOE.FlowPage
{
    public partial class FlowDetails
    {
        [Parameter]
        public int? FlowId { get; set; }

        SOSFlow _sosFlow { get; set; } = new();
        private List<SOSFlowLogbook> mostRecentLogs = new List<SOSFlowLogbook>();
        private int logCount = 0;
        private int totalLogbooks = 0;
        private int remainingLogs = 0;
        private List<int> logbookIds = new List<int>();

        int cycleId = 0;
        private List<string> capturedImages = new List<string>();


        //Show evidence
        private DialogOptions dialogEvidenceOptions = new() { CloseOnEscapeKey = true, MaxWidth = MaxWidth.Medium, FullWidth = true, CloseButton = true };

        private bool visibleEvidence = false;

        private int photoIndex = 0;


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

        private string[] additionalTimes = new string[] { "0", "0", "0", "0", "0" };
        private string[] cycleTimes = new string[] { "0", "0", "0", "0", "0" };
        private string[] applicationModels = new string[] { "", "", "", "", "" };

        //User
        private string json = string.Empty;
        public User user = new();
        public bool logged = false;

        //Duagram
        private BlazorDiagram Diagram { get; set; } = null!;
        private bool _gridPoints;

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
                Diagram.RegisterComponent<CommentaryNode, Widgets.Commentary>();


                Diagram.RegisterComponent<ResizeControl, Widgets.ResizeControlWidget>();

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
                _ = await LoadDiagramFromJson();

                if (_sosFlow.FlowLogbooks != null)
                {
                    mostRecentLogs = _sosFlow.FlowLogbooks
                        .OrderByDescending(log => log.SOSFlowLogbookId)
                        .Take(Math.Min(3, _sosFlow.FlowLogbooks.Count))
                        .OrderBy(log => log.SOSFlowLogbookId)
                        .ToList();

                    logCount = mostRecentLogs.Count;
                    totalLogbooks = _sosFlow.FlowLogbooks.Count;

                }

                remainingLogs = 3 - logCount;
                logbookIds = mostRecentLogs.Select(log => log.SOSFlowLogbookId).ToList();



                cycleId = _sosFlow.SOSHub?.TrainingTime ?? 0;

            }
            ShowLoading = false;
            StateHasChanged();
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

        private List<string> GetRevisionNumbers()
        {
            List<string> revisionNumbers = new List<string> { "", "", "" };

            if (totalLogbooks <= 3)
            {
                for (int i = 0; i < totalLogbooks; i++)
                {
                    if (i == 0)
                    {
                        revisionNumbers[0] = "N";
                    }
                    else
                    {
                        revisionNumbers[i] = (i).ToString();
                    }
                }
            }
            else
            {
                revisionNumbers[0] = (totalLogbooks - 3).ToString();
                revisionNumbers[1] = (totalLogbooks - 2).ToString();
                revisionNumbers[2] = (totalLogbooks - 1).ToString();
            }

            return revisionNumbers;
        }

        public static int GetCycleId(string trainingTime)
        {
            string cycleIdString = trainingTime.Split(' ').First();

            if (int.TryParse(cycleIdString, out int cycleId))
            {
                return cycleId;
            }
            else
            {
                return 0;
            }
        }


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

        private void OpenEvidenceDialog(int index)
        {
            photoIndex = index;
            visibleEvidence = true;

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
            // Capture image data as Base64
            var imageData = await JSRuntime.InvokeAsync<string>("captureSvgLayerAsImage");

            // Convert Base64 to byte array
            var imageBytes = Convert.FromBase64String(imageData.Split(',')[1]);

            // Create a multipart form data content
            var content = new MultipartFormDataContent
            {
                { new ByteArrayContent(imageBytes), "Diagrams", "element_image.png" }
            };

            // Send to API endpoint
            await Exportation.ExportFlowToExcel(FlowId.Value, content);
        }
        //For multiple files 
        /* Convert each Base64 image to byte array
        //for (int i = 0; i < imagesData.Length; i++)
        //{
        //    if (imagesData[i] != null)
        //    {
        //        var imageBytes = Convert.FromBase64String(imagesData[i].Split(',')[1]);
        //        content.Add(new ByteArrayContent(imageBytes), "files", $"image_{i + 1}.png");
        //    }
        }*/

        void HoeDetails(int HoeId)
        {
            NavigationManager.NavigateTo($"/soshoe/Hub/Details/{HoeId}");
        }
        void UpdateFlow(int FlowId)
        {
            NavigationManager.NavigateTo($"/soshoe/Flow/Update/{FlowId}");
        }

        private async Task<AsyncVoidMethodBuilder> LoadDiagramFromJson()
        {
            if (!string.IsNullOrEmpty(_sosFlow.Flow))
            {
                var data = JsonConvert.DeserializeObject<DiagramData>(_sosFlow.Flow);
                if (data != null)
                {
                    ConvertDataToDiagram(data);
                    StateHasChanged();
                }
            }
            return new AsyncVoidMethodBuilder();
        }

        private async Task<AsyncVoidMethodBuilder> ConvertDataToDiagram(DiagramData data)
        {
            Diagram.Nodes.Clear();
            Diagram.Links.Clear();

            var nodeDictionary = new Dictionary<string, List<NodeModel>>();

            foreach (var nodeData in data.Nodes)
            {

                NodeModel node = nodeData.Type switch
                {
                    nameof(TerminalNode) => new TerminalNode(nodeData.Size.Width, nodeData.Size.Height, new Point(nodeData.Position.X, nodeData.Position.Y)) { Title = nodeData.Title },
                    nameof(ProcessNode) => new ProcessNode(nodeData.Size.Width, nodeData.Size.Height, new Point(nodeData.Position.X, nodeData.Position.Y)) { Title = nodeData.Title },
                    nameof(ConnectionNode) => new ConnectionNode(nodeData.Size.Width, nodeData.Size.Height, new Point(nodeData.Position.X, nodeData.Position.Y)) { Title = nodeData.Title },
                    nameof(SupplementNode) => new SupplementNode(nodeData.Size.Width, nodeData.Size.Height, new Point(nodeData.Position.X, nodeData.Position.Y)) { Title = nodeData.Title },
                    nameof(DecisionNode) => new DecisionNode(nodeData.Size.Width, nodeData.Size.Height, new Point(nodeData.Position.X, nodeData.Position.Y)) { Title = nodeData.Title },
                    nameof(DecisionRomboNode) => new DecisionRomboNode(nodeData.Size.Width, nodeData.Size.Height, new Point(nodeData.Position.X, nodeData.Position.Y)) { Title = nodeData.Title },
                    nameof(CommentaryNode) => new CommentaryNode(nodeData.Size.Width, nodeData.Size.Height, new Point(nodeData.Position.X, nodeData.Position.Y)) { Title = nodeData.Title },
                    _ => throw new InvalidOperationException("Tipo de nodo desconocido")
                };

                var nodeModelToAdd = Diagram.Nodes.Add(node);

                if (nameof(CommentaryNode) != nodeData.Type)
                {
                    nodeModelToAdd.AddPort(PortAlignment.Left);
                    nodeModelToAdd.AddPort(PortAlignment.Top);
                    nodeModelToAdd.AddPort(PortAlignment.Right);
                    nodeModelToAdd.AddPort(PortAlignment.Bottom);
                }


                if (!nodeDictionary.ContainsKey(nodeData.Id))
                {
                    nodeDictionary[nodeData.Id] = new List<NodeModel>();
                }
                nodeDictionary[nodeData.Id].Add(nodeModelToAdd);

                Diagram.Refresh();

            }

            foreach (var linkData in data.Links)
            {
                if (nodeDictionary.TryGetValue(linkData.SourceNodeId, out var sourceNodes) &&
                    nodeDictionary.TryGetValue(linkData.TargetNodeId, out var targetNodes))
                {
                    foreach (var sourceNode in sourceNodes)
                    {
                        foreach (var targetNode in targetNodes)
                        {

                            var sourcePort = sourceNode.Ports.FirstOrDefault(p => p.Alignment.ToString() == linkData.SourcePort);
                            var targetPort = targetNode.Ports.FirstOrDefault(p => p.Alignment.ToString() == linkData.TargetPort);


                            var srcNode = sourcePort.Parent;

                            int position = GetArrayPosition(linkData.SourcePort);

                            if (srcNode != null && srcNode is ICmpAnchors srcAnchors)
                            {
                                var srcAnchor = srcAnchors.Anchors[position];

                                var tgtNode = targetPort.Parent;

                                position = GetArrayPosition(linkData.TargetPort);

                                if (tgtNode != null && tgtNode is ICmpAnchors tgtAnchors)
                                {
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
                                    link.Locked = true; 

                                    Diagram.Links.Add(link);
                                    link.Refresh();
                                    Diagram.Refresh();
                                }

                            }


                            Diagram.Refresh();
                        }
                    }


                }
            }

            foreach(NodeModel node in Diagram.Nodes)
            {
                foreach(var port in node.Ports)
                {
                    port.Locked = true;
                }
                node.Locked = true;
            }


            StateHasChanged();
            return new AsyncVoidMethodBuilder();
        }

        enum Positions
        {
            Left,
            Top,
            Right,
            Bottom
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
        public bool GridPoints
        {
            get => _gridPoints;
            set
            {
                _gridPoints = value;
                Diagram.Refresh();
            }
        }

    }
}