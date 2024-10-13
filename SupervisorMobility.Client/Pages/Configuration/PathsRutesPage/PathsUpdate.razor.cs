using DocumentFormat.OpenXml.Bibliography;
using Microsoft.AspNetCore.Mvc.Internal;
using Microsoft.IdentityModel.Abstractions;
using Microsoft.JSInterop;
using MudBlazor;
using SupervisorMobility.Client.Data.Entities.TreeStruct;
using SupervisorMobility.Client.Pages.Inicio.SOSProgramPage.Dialogs;
using System.Diagnostics;
using System.Text.Json.Nodes;

namespace SupervisorMobility.Client.Pages.Configuration.PathsRutesPage
{
    public partial class PathsUpdate
    {
        [Parameter]
        public int PathId { get; set; }

        bool ShowLoading = true;
        bool Update = true;
        bool Find_all_tree = false;
        bool Find_Product = false;
        bool ShowMoreInfo = false;
        bool ShowSearchInfo = false;

        bool if_add_CD_CCP = false;
        bool if_add_CD_GOS = false;
        bool if_add_CD_HOE = false;


        private IList<string> _sourceMsgLoading = new List<string>();
        private IList<Color> _Colors = new List<Color>() { Color.Default, Color.Primary, Color.Secondary, Color.Success, Color.Info, Color.Default, Color.Primary, Color.Secondary, Color.Success, Color.Info };

        List<BreadcrumbItem> _links;

        private SOSCodePath SosCode = new SOSCodePath();

        TreeItemData Node_CCP { get; set; } = new TreeItemData();
        TreeItemData Node_GOS { get; set; } = new TreeItemData();
        TreeItemData Node_HOE { get; set; } = new TreeItemData();
        TreeItemData SelectedNodeCCP { get; set; }
        TreeItemData SelectedNodeGOS { get; set; }
        TreeItemData SelectedNodeHOE { get; set; }
        TreeItemData SelectedNodeCCPCD { get; set; }
        TreeItemData SelectedNodeGOSCD { get; set; }
        TreeItemData SelectedNodeHOECD { get; set; }


        bool isGosFolder = false;
        bool isCcpFolder = false;
        bool isHoeFolder = false;
        private CDMS_CCP_Archives? CcpFilesInFolder;
        private CDMS_HOE_Archives? HoeFilesInFolder;
        private CDMS_GOS_Archives? GosFilesInFolder;

        private bool folderCCPError = false;
        private bool folderHOEError = false;
        private bool folderGOSError = false;

        MudMessageBox HOEmbox { get; set; }
        MudMessageBox CCPmbox { get; set; }
        MudMessageBox GOSmbox { get; set; }

        public bool HOEModalDisplay { get; set; } = false;

        private DialogOptions dialogOptions = new() { CloseOnEscapeKey = false, MaxWidth = MaxWidth.ExtraExtraLarge, FullWidth = true, Position = DialogPosition.TopCenter, DisableBackdropClick = true, CloseButton = true };


        private List<Product> _products = new List<Product>();
        private List<Plant> _plants = new List<Plant>();
        private Dictionary<int, List<Area>> _areas = new Dictionary<int, List<Area>>();
        private Dictionary<int, Dictionary<int, List<Distribution>>> _distributions = new Dictionary<int, Dictionary<int, List<Distribution>>>();

        private Plant _plant = new Plant();
        private Area _area = new Area();
        private Distribution _distribution = new Distribution();
        private Product _product = new Product();

        int IndexPlant = -1;
        int IndexArea = -1;
        int IndexDistr = -1;
        int IndexProd = -1;

        CDMS_HOE_Directory HOEFolders { get; set; } = new CDMS_HOE_Directory();
        CDMS_GOS_Directory GOSFolders { get; set; } = new CDMS_GOS_Directory();
        CDMS_CCP_Directory CCPFolders { get; set; } = new CDMS_CCP_Directory();

        protected async override Task OnInitializedAsync()
        {

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
            _links = new List<BreadcrumbItem>
            {
                new BreadcrumbItem(text: Localizer["home"], href: "/"),
                new BreadcrumbItem(text: Localizer["configuration"], href: "/configuration"),
                new BreadcrumbItem(text: Localizer["PathsRoute"], href: "/PathsRoute"),
                new BreadcrumbItem(text: Localizer["PathsRouteCreate"], href: "/PathsRoute", disabled: true),
            };

            try
            {
                //SosCode = await TreeServices.getCodePath(PathId);
                SosCode = await AssyChartServices.GetCodePath(PathId);

                _products = await ProductsServices.GetProducts();

                _plants = await PlantsServices.GetPlants();
                foreach (var plant in _plants)
                {
                    var areas = await AreasServices.GetAreas(plant.PlantId);
                    _areas.Add(plant.PlantId, areas);

                    var areaDistributions = new Dictionary<int, List<Distribution>>();
                    foreach (var area in areas)
                    {
                        var distributions = await DistributionsServices.GetDistributions(plant.PlantId, area.AreaId);
                        areaDistributions.Add(area.AreaId, distributions);
                    }
                    _distributions.Add(plant.PlantId, areaDistributions);
                }

                try
                {
                    //hoe
                    try
                    {
                        HOEFolders = await CDMSServices.GetFoldersHOE();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error Get HOE Folder From CDMS");
                        Console.WriteLine(ex.Message);
                    }

                    if (HOEFolders != null)
                    {
                        Node_HOE = TreeServices.Make_Tree_HOE(HOEFolders.operation);
                    }

                    //gos
                    try
                    {
                        GOSFolders = await CDMSServices.GetFoldersGOS();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error Get GOS Folder From CDMS");
                        Console.WriteLine(ex.Message);
                    }

                    if (GOSFolders != null)
                    {
                        Node_GOS = TreeServices.Make_Tree_GOS(GOSFolders.operation);
                    }

                    //ccp
                    try
                    {
                        CCPFolders = await CDMSServices.GetFoldersCCP();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error Get CCP Folder From CCP");
                        Console.WriteLine(ex.Message);
                        Console.WriteLine(ex.Message);
                    }

                    if (CCPFolders != null)
                    {
                        Node_CCP = TreeServices.Make_Tree_CCP(CCPFolders.operation);
                    }

                    //Node_HOE = await TreeServices.getRootHOE();
                    //Node_GOS = await TreeServices.getRootGOS();
                    //Node_CCP = await TreeServices.getRootCCP();

                    if_add_CD_HOE = SosCode.CommonDirectionHOE != "";
                    if_add_CD_CCP = SosCode.CommonDirectionCCP != "";
                    if_add_CD_GOS = SosCode.CommonDirectionGOS != "";

                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error on OnInitializedAsync");
                    Console.WriteLine(ex);
                }
                finally
                {

                }

                _plant = SosCode.AssyChart.Plant;
                _area = SosCode.AssyChart.Area;
                _distribution = SosCode.AssyChart.Distribution;
                _product = SosCode.Product;

                IndexPlant = _plants.FindIndex(p => p.PlantId == SosCode.AssyChart?.PlantId);
                IndexArea = _areas.ElementAt(IndexPlant).Value.FindIndex(a => a.AreaId == SosCode.AssyChart?.AreaId);
                IndexDistr = _distributions.ElementAt(IndexPlant).Value.ElementAt(IndexArea).Value.FindIndex(d => d.DistributionId == SosCode.DistributionId);
                IndexProd = SosCode.ProductId ?? -1;

                Console.WriteLine($"p : {IndexPlant} a: {IndexArea} d: {IndexDistr}  pro: {IndexProd}");
            }
            catch (Exception ex)
            {

            }
            finally
            {
                ShowLoading = false;
                if (IndexPlant != -1 && IndexArea != -1 && IndexDistr != -1)
                    Find_all_tree = true;

                if (IndexPlant != -1 && IndexArea != -1 && IndexDistr != -1)
                    ShowMoreInfo = true;

                if (IndexProd != -1)
                    Find_Product = true;

                SelectedNodeHOE = TreeServices.FindNodeByPath(Node_HOE, SosCode.HOE);
                if (SelectedNodeHOE != null)
                {
                    Hoe = (SelectedNodeHOE, SosCode.HOE);
                }
                else
                {
                    Hoe = (SosCode.HOE, SosCode.HOE);
                }

                SelectedNodeGOS = TreeServices.FindNodeByPath(Node_GOS, SosCode.GOS);

                if (SelectedNodeHOE != null)
                {
                    Gos = (SelectedNodeGOS, SosCode.GOS);
                }
                else
                {
                    Gos = (SosCode.GOS, SosCode.GOS);
                }

                SelectedNodeCCP = TreeServices.FindNodeByPath(Node_CCP, SosCode.CCP);

                if (SelectedNodeHOE != null)
                {
                    Ccp = (SelectedNodeCCP, SosCode.CCP);
                }
                else
                {
                    Ccp = (SosCode.CCP, SosCode.CCP);
                }


                SelectedNodeHOECD = TreeServices.FindNodeByPath(Node_HOE, SosCode.CommonDirectionHOE);
                if (SelectedNodeHOE != null)
                {
                    HoeCD = (SelectedNodeHOECD, SosCode.CommonDirectionHOE);
                }
                else
                {
                    HoeCD = (SosCode.CommonDirectionHOE, SosCode.CommonDirectionHOE);
                }

                SelectedNodeGOSCD = TreeServices.FindNodeByPath(Node_GOS, SosCode.CommonDirectionGOS);

                if (SelectedNodeHOE != null)
                {
                    GosCD = (SelectedNodeGOSCD, SosCode.CommonDirectionGOS);
                }
                else
                {
                    GosCD = (SosCode.CommonDirectionGOS, SosCode.CommonDirectionGOS);
                }

                SelectedNodeCCPCD = TreeServices.FindNodeByPath(Node_CCP, SosCode.CommonDirectionCCP);

                if (SelectedNodeHOE != null)
                {
                    CcpCD = (SelectedNodeCCPCD, SosCode.CommonDirectionCCP);
                }
                else
                {
                    CcpCD = (SosCode.CommonDirectionCCP, SosCode.CommonDirectionCCP);
                }
            }

        }


        private async void UpdatePathAsync()
        {
            Update = false;

            SosCode.ProductId = _product.ProductId;
            SosCode.DistributionId = _distribution.DistributionId;

            var anyAssyChart = await AssyChartServices.GetAssyChartJobObservation(_plant.PlantId, _area.AreaId, _distribution.DistributionId);

            if (anyAssyChart != null)
            {
                Console.WriteLine("Se añade al assychartr");
                //se Invoca metodo que añade al assychart
                SosCode.AssyChardId = anyAssyChart.AssyChardId;


                var resultAdd = await AssyChartServices.UpdateCodePath(PathId, SosCode);

                if (resultAdd)
                {
                    await TreeServices.updateCodePath(SosCode);
                    NavigationManager.NavigateTo("/PathsRoute");
                }
                else
                    await JS.InvokeVoidAsync("alert", "Fail to updater CodePath, contact admin!"); // Alert
            }
            else
            {
                Console.WriteLine("Se Crea el assychart");

                AssyChart newAssychart = new();
                newAssychart.CreationDate = DateTime.Now;
                newAssychart.PlantId = _plant.PlantId;
                newAssychart.AreaId = _area.AreaId;
                newAssychart.DistributionId = _distribution.DistributionId;
                newAssychart.RoutesProductsAssyChart?.Add(SosCode);
                newAssychart.IsActive = true;
                //crear asychasr
                var result = await AssyChartServices.CreateAssyChart(newAssychart);

                if (result != null)
                    NavigationManager.NavigateTo("/PathsRoute");
                else
                    await JS.InvokeVoidAsync("alert", "Fail to create Assy Chart, contact admin!"); // Alert
                                                                                                    //se invoca crear assychart
            }


            Update = true;
        }

        public async Task<HashSet<TreeItemData>> LoadServerData(TreeItemData parentNode)
        {
            await Task.Delay(50);
            return parentNode.TreeItems;
        }

        void DeleteGOSRoute()
        {
            SosCode.GOS = "";
        }
        void DeleteCCPRoute()
        {
            SosCode.CCP = "";
        }
        void DeleteHOERoute()
        {
            SosCode.HOE = "";
            //if (IndexPlant != -1 && IndexArea != -1 && IndexDistr != -1)
            ShowMoreInfo = false;
        }
        void DeleteGOSRouteCD()
        {
            SosCode.CommonDirectionGOS = "";
        }
        void DeleteCCPRouteCD()
        {
            SosCode.CommonDirectionCCP = "";
        }
        void DeleteHOERouteCD()
        {
            SosCode.CommonDirectionHOE = "";

        }

        void UpdatePlant()
        {

            _plant = _plants.ElementAt(IndexPlant);
            IndexArea = -1;
            IndexDistr = -1;

            StateHasChanged();
        }

        void UpdateArea()
        {
            _area = _areas.ElementAt(IndexPlant).Value.ElementAt(IndexArea);
            IndexDistr = -1;
            StateHasChanged();
        }

        void UpdateDistribution()
        {
            _distribution = _distributions.ElementAt(IndexPlant).Value.ElementAt(IndexArea).Value.ElementAt(IndexDistr);

            if (IndexPlant != -1 && IndexArea != -1 && IndexDistr != -1)
                ShowMoreInfo = true;

            StateHasChanged();
        }
        void UpdateProduct()
        {
            _product = _products.Find(p => p.ProductId == IndexProd);
            if (_product != null && IndexProd != -1)
            {
                Find_Product = true;
            }
            StateHasChanged();
        }

        //aqui se trata de optener todos los elementos a partir de la ruta.
        async void ApplyHOE()
        {
            ShowMoreInfo = false;
            ShowSearchInfo = true;
            Find_all_tree = false;
            Find_Product = false;
            isHoeFolder = false;

            Debug.WriteLine($"Apply Hoe on change: {SosCode.HOE}");
            Console.WriteLine($"Apply Hoe on change: {SosCode.HOE}");


            try
            {
                try
                {
                    HoeFilesInFolder = await CDMSServices.GetFilesHOE(SosCode.HOE);

                    if (HoeFilesInFolder.message == "NO FILES IN DIRECTORY" || HoeFilesInFolder.message == "INCOMPLETE FIELDS FOR HOE in ⪢ ⪢ ⪢ ⪢ VALIDATE_PAD_HOE" || HoeFilesInFolder.message == "NO FILES OR DIRECTORIES")
                    {
                        isHoeFolder = false;
                        bool msgHOEBox = await OpenMessageHOE();
                        Find_Product = false;

                    }
                    else
                    {
                        isHoeFolder = true;

                        //Console.WriteLine($"HOE Path: {Path}");
                        //var StrOutSpace = Path.Replace(" ", "");

                        //if (StrOutSpace.Contains("/"))
                        //{
                        //    var StrPlant = StrOutSpace.Split("/").First().Split(".").Last();
                        //    Console.WriteLine($"HOE Plant: {StrPlant}");
                        //    var StrArea = StrOutSpace.Split("/").ElementAt(StrOutSpace.Split("/").Count() - 3).Split(".").Last();
                        //    Console.WriteLine($"HOE area: {StrArea}");
                        //    var StrDistribution = Path.Split("/").Last().Split(".").Last().Substring(1);
                        //    Console.WriteLine($"HOE distr: {StrDistribution}");
                        //    var StrProduct = StrOutSpace.Split("/").ElementAt(StrOutSpace.Split("/").Count() - 2).Split(".").Last();
                        //    Console.WriteLine($"HOE product: {StrProduct}");


                        //    if (_plants.Any(p => p.Code == StrPlant) == true)
                        //    {
                        //        IndexPlant = _plants.FindIndex(p => p.Code == StrPlant);
                        //        _plant = _plants.ElementAt(IndexPlant);
                        //        Console.WriteLine($"Plant exist:");
                        //        if (_areas.ElementAt(IndexPlant).Value.Any(p => p.Code == StrArea) == true)
                        //        {
                        //            IndexArea = _areas.ElementAt(IndexPlant).Value.FindIndex(p => p.Code == StrArea);
                        //            Console.WriteLine($"Area exist:");
                        //            _area = _areas.ElementAt(IndexPlant).Value.ElementAt(IndexArea);

                        //            if (_distributions.ElementAt(IndexPlant).Value.ElementAt(IndexArea).Value.Any(p => p.Description == StrDistribution) == true)
                        //            {
                        //                IndexDistr = _distributions.ElementAt(IndexPlant).Value.ElementAt(IndexArea).Value.FindIndex(p => p.Description == StrDistribution);
                        //                _distribution = _distributions.ElementAt(IndexPlant).Value.ElementAt(IndexArea).Value.ElementAt(IndexDistr);
                        //                Console.WriteLine($"Distribution exist:");
                        //                Find_all_tree = true;
                        //            }



                        //        }
                        //    }

                        //    if (_products.Any(p => p.Code == StrProduct) == true)
                        //    {
                        //        IndexProd = _products.FindIndex(p => p.Code == StrProduct);
                        //        _product = _products.ElementAt(IndexProd);
                        //        Find_Product = true;
                        //    }
                        //    else
                        //    {
                        //        Find_Product = false;
                        //    }
                        //}


                    }
                }
                catch (Exception ex)
                {
                    ShowMoreInfo = false;
                    Find_all_tree = false;
                    Find_Product = false;


                    bool msgHOEBox = await OpenMessageHOE();
                    Console.WriteLine(ex.ToString());
                }


            }
            catch (Exception ex)
            {

            }
            finally
            {
                if (IndexPlant != -1 && IndexArea != -1 && IndexDistr != -1)
                    Find_all_tree = true;

                ShowSearchInfo = false;

                if (isHoeFolder)
                    ShowMoreInfo = true;

                StateHasChanged();
            }



        }

        private void OpenHOEModal()
        {
            isGosFolder = false;
            isCcpFolder = false;
            isHoeFolder = false;

            HOEModalDisplay = true;
            StateHasChanged();

        }

        private async void CloseHOEModal()
        {
          

            if (isHoeFolder)
            {
                HOEModalDisplay = false;
            }

            //if (IndexPlant != -1 && IndexArea != -1 && IndexDistr != -1)
            //    Find_all_tree = true;

            //if (IndexProd != -1)
            //    Find_Product = true;

            StateHasChanged();
        }


        private async Task<bool> OpenMessageHOE()
        {
            bool? result = await HOEmbox.Show();

            return result == null ? false : true;
        }

        private void CloseFunctionHoeModal()
        {
            Find_all_tree = (IndexPlant != -1 && IndexArea != -1 && IndexDistr != -1);
            CloseHOEModal();
            StateHasChanged();
        }


        private void AddRemove_CD_HOE()
        {
            if_add_CD_HOE = !if_add_CD_HOE;
        }
        private void AddRemove_CD_GOS()
        {
            if_add_CD_GOS = !if_add_CD_GOS;
        }
        private void AddRemove_CD_CCP()
        {
            if_add_CD_CCP = !if_add_CD_CCP;
        }

        private (object, string) Hoe = (null, "");
        private (object, string) HoeCD = (null, "");
        private (object, string) Gos = (null, "");
        private (object, string) GosCD = (null, "");
        private (object, string) Ccp = (null, "");
        private (object, string) CcpCD = (null, "");

        private void HandleFinaHOEChanged((object, string) finalFiles)
        {
            Hoe = finalFiles;
            SosCode.HOE = finalFiles.Item2;
            ApplyHOE();
            StateHasChanged();
        }

        private void HandleFinaGOSChanged((object, string) finalFiles)
        {
            Gos = finalFiles;
            SosCode.GOS = finalFiles.Item2;
            StateHasChanged();
        }
        private void HandleFinaCCPChanged((object, string) finalFiles)
        {
            Ccp = finalFiles;
            SosCode.CCP = finalFiles.Item2;
            StateHasChanged();
        }
        private void HandleFinaHOECDChanged((object, string) finalFiles)
        {
            HoeCD = finalFiles;
            SosCode.CommonDirectionHOE = finalFiles.Item2;
            StateHasChanged();
        }

        private void HandleFinaGOSCDChanged((object, string) finalFiles)
        {
            GosCD = finalFiles;
            SosCode.CommonDirectionGOS = finalFiles.Item2;
            StateHasChanged();
        }

        private void HandleFinaCCPCDChanged((object, string) finalFiles)
        {
            CcpCD = finalFiles;
            SosCode.CommonDirectionCCP= finalFiles.Item2;
            StateHasChanged();
        }
    }
}