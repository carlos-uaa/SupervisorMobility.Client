using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;
using System.Text.RegularExpressions;
using ClosedXML.Excel;
using System.Net.Http.Headers;
using Blazorise.Extensions;
using DocumentFormat.OpenXml.Drawing;
using SupervisorMobility.Client.Data.Entities;

namespace SupervisorMobility.Client.Pages.Configuration.AssyChartPage
{
    public partial class UploadAssyCharts
    {
        // Breadcrumb links
        private List<BreadcrumbItem> _links;

        //Objects to Interacting with the archive upload
        private string FileName;
        private string ErrorMessageToDisplay;
        private IBrowserFile? FileSource;

        //List Plants to do bulk
        private List<Plant> _plants = new List<Plant>();
        private Dictionary<int, List<Area>> _areas = new Dictionary<int, List<Area>>();
        private Dictionary<int, Dictionary<int, List<Distribution>>> _distributions = new Dictionary<int, Dictionary<int, List<Distribution>>>();
        private List<Product> _products = new List<Product>();

        private List<EventMensaje> List_Events = new();
        private IList<string> _sourceMsgLoading = new List<string>();
        private IList<Color> _Colors = new List<Color>() { Color.Default, Color.Primary, Color.Secondary, Color.Success, Color.Info, Color.Default, Color.Primary, Color.Secondary, Color.Success, Color.Info };

        //Table to display elements in file, verificate data to upload
       
        private bool displayLoading = true;
        private bool displayResume = false;
        private bool activeUpload = false;
        private bool showTable = false;

        private List<string[]> BulkedData = new List<string[]>();

        private List<AssyChart> _assycharts = new();

        private FileUpload uploadResult = new();
        private UploadAssyChartResult retornedResult_Uploaded = new();

        public class EventMensaje
        {
            public string Mensaje { get; set; }
            public int Tipo { get; set; }
        }

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
                    new BreadcrumbItem(text: Localizer["home"], href: "#"),
                new BreadcrumbItem(text: Localizer["configuration"], href: "/configuration"),
                new BreadcrumbItem(text: Localizer["assychart"], href: "/assychart"),
                new BreadcrumbItem(text: Localizer["ACUploadAC"], href: "/UploadAssyCharts", disabled: true),
            };
            try
            {
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

                _products = await ProductsServices.GetProducts();
            }
            catch(Exception ex) { }
            finally
            {
                displayLoading = false;
            }



        }//end OnInitialized


        async Task OnChange(InputFileChangeEventArgs e)
        {
            //Clear Data
            FileName = string.Empty;
            FileSource = null;

            ErrorMessageToDisplay = string.Empty;
            List_Events.Clear();
            BulkedData.Clear();
            _assycharts.Clear();
            showTable = false;

            //Assign new data
            FileName = e.File.Name;
            FileSource = e.File;

            //Ragex to get extenxion of file, only CSV or Excel allowed files
            Regex regexcsv = new Regex(".+\\.csv", RegexOptions.Compiled);
            Regex regexlsx = new Regex(".+\\.xlsx", RegexOptions.Compiled);


            using var content = new MultipartFormDataContent();


            if (!regexcsv.IsMatch(e.File.Name) && !regexlsx.IsMatch(e.File.Name))
            {
                ErrorMessageToDisplay = Localizer["ACmsgOnlyCSV"];
            }
            else
            {
                var fileContent = new StreamContent(e.File.OpenReadStream(509600000));
                fileContent.Headers.ContentType = new MediaTypeHeaderValue(e.File.ContentType);

                MemoryStream ms = new MemoryStream();
                await fileContent.CopyToAsync(ms);
                var outputFileString = System.Text.Encoding.UTF8.GetString(ms.ToArray());
                bool isOkFile = false;

                if (regexcsv.IsMatch(e.File.Name))
                {
                    foreach (var item in outputFileString.Split(Environment.NewLine))
                    {
                        var addtolist = SplitCSV(item.ToString());

                        if (!addtolist.All(string.IsNullOrWhiteSpace))
                        {
                            BulkedData.Add(addtolist);
                        }

                    }


                }

                if (regexlsx.IsMatch(e.File.Name))
                {
                    try
                    {
                        BulkedData = await GetDataTableFromExcel(e.File);
                        isOkFile = true;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.ToString());
                    }
                }

                bool isFirstRow = true;
                bool isSecondRow = true;

                foreach (string[] row in BulkedData)
                {
                    Console.WriteLine($"Start Index: {BulkedData.Index(i => i == row)}");
                    foreach (var item in row)
                    {
                        Console.Write($"({row.Index(e => e == item)}) [{item}], ");
                    }
                    Console.WriteLine($"End: {BulkedData.Index(i => i == row)}");
                    Console.WriteLine($"");

                }

                int uploadtype = 0;
                int auxplant = 0;
                int auxarea = 0;
                int auxdistribution = 0;


                try
                {

                    int numero = int.Parse(BulkedData[0][1]);
                    // caso 1 mediante Ids
                    uploadtype = 1;
                    Console.WriteLine("El elemento [0][1] contiene numeros CASO 3.");
                    Console.WriteLine("Número: " + numero);
                }
                catch (FormatException ex)
                {
                    uploadtype = 2;

                    Console.WriteLine("El elemento [0][1] contiene texto. CASO 1 o 2");

                    //caso 2, creacion nueva o existe mediante Codigo
                }
                //1  - Creacion Masiva
                //2  - Mediante Codigo
                //3  - Mediante Ids

                //0 Create, 1 Read, 2 Update, 3 Adding, 4 error 
                switch (uploadtype)
                {
                    case 1:
                        //Con ids
                        if (_plants.Any(P => P.Code == BulkedData[0][1]))
                        {
                            auxplant = _plants.Index(P => P.Code == BulkedData[0][1]);
                            if (_plants[auxplant].Description == BulkedData[0][3] && BulkedData[0][3] != "§")
                            {

                                Console.WriteLine("La Planta Se actualiza");
                                var msgPlant = new EventMensaje();
                                msgPlant.Mensaje = $"Se Actualizara la descripcion de la planta: `{BulkedData[0][1]}` con descripcion `{BulkedData[0][3]}`";
                                msgPlant.Tipo = 2;
                                List_Events.Add(msgPlant);
                            }
                            else
                            {
                                Console.WriteLine("La Planta Existe");
                                var msgPlant = new EventMensaje();
                                msgPlant.Mensaje = $"La planta: `{BulkedData[0][1]}` con descripcion `{BulkedData[0][3]} existe`";
                                msgPlant.Tipo = 1;
                                List_Events.Add(msgPlant);
                            }
                            //Existe la planta
                        }
                        else
                        {
                            auxplant = -2;
                            Console.WriteLine("La Planta Se creara");
                            var msgPlant = new EventMensaje();
                            msgPlant.Mensaje = $"Se creara la planta: `{BulkedData[0][1]}` con descripcion `{BulkedData[0][3]}`";
                            msgPlant.Tipo = 0;
                            List_Events.Add(msgPlant);
                        }


                        foreach (string[] row in BulkedData)
                        {
                            if (isFirstRow)
                            {
                                isFirstRow = false;
                            }
                            else if (isSecondRow)
                            {
                                isSecondRow = false;
                            }
                            else
                            {
                                try
                                {
                                    if (auxplant == -2)
                                    {
                                        //Creacion total



                                        if (!List_Events.Any(e => e.Mensaje.Contains(row[1])))
                                        {
                                            var msgArea = new EventMensaje();
                                            msgArea.Mensaje = $"Se creara el area {row[1]} en la planta: `{BulkedData[0][1]}`";
                                            msgArea.Tipo = 0;
                                            List_Events.Add(msgArea);
                                        }
                                    }
                                    else
                                    {
                                        //verificar si existe

                                    }
                                }
                                catch
                                (Exception ex)
                                {
                                    Console.WriteLine("");

                                }

                            }

                        }



                        showTable = true;
                        break;
                    case 2:
                        //Con codigos
                        Plant Assy_Plant = new();
                        if (BulkedData[0][1] != "§")
                        {
                           
                            if (_plants.Any(P => P.Code == BulkedData[0][1]))
                            {
                                //Existe mediante su codigo

                                auxplant = _plants.Index(P => P.Code == BulkedData[0][1]);
                                if (_plants[auxplant].Description != BulkedData[0][3] && BulkedData[0][3] != "§")
                                {
                                    Assy_Plant = _plants.ElementAt(auxplant);
                                    Console.WriteLine("La Planta Se actualiza");
                                    var msgPlant = new EventMensaje();
                                    msgPlant.Mensaje = $"Se Actualizara la descripcion de la planta: `{BulkedData[0][1]}`. De `{Assy_Plant.Description}` a `{BulkedData[0][3]}`";
                                    Assy_Plant.Description = BulkedData[0][3];
                                    msgPlant.Tipo = 2;
                                    List_Events.Add(msgPlant);
                                }
                                else
                                {
                                    Assy_Plant = _plants.ElementAt(auxplant);
                                }
                            }
                            else
                            {
                                // La planta no existe pero se puede crear
                                 Assy_Plant.Code = BulkedData[0][1];
                                    
                                auxplant = -2;
                                Console.WriteLine("La Planta Se creara");

                                if (BulkedData[0][3] == "§")
                                {
                                    Assy_Plant.PlantId = -1;

                                    var msgPlant = new EventMensaje();
                                    msgPlant.Mensaje = $"Error en `D1`: Falta Descripcion para la Creacion de planta `{BulkedData[0][1]}`.";
                                    msgPlant.Tipo = 4;
                                    List_Events.Add(msgPlant);
                                }
                                else
                                {
                                    var msgPlant = new EventMensaje();
                                    msgPlant.Mensaje = $"Se creara la planta: `{BulkedData[0][1]}` con descripcion `{BulkedData[0][3]}`";
                                     Assy_Plant.Description = BulkedData[0][3];
                                    Assy_Plant.PlantId = -2;

                                    msgPlant.Tipo = 0;
                                    List_Events.Add(msgPlant);
                                }

                            }
                        }
                        else
                        {
                            auxplant = -3;
                            //error 
                            var msgPlant = new EventMensaje();
                            msgPlant.Mensaje = $"Error en `B1`: Falta PlantaId/Codigo de la planta.";
                            msgPlant.Tipo = 4;
                            List_Events.Add(msgPlant);
                        }

                        //recorremos el resto de los datos
                        foreach (string[] row in BulkedData)
                        {

                            if (isFirstRow)
                            {
                                isFirstRow = false;
                            }
                            else if (isSecondRow)
                            {
                                isSecondRow = false;
                            }
                            else
                            {
                                AssyChart Assychart_ToAdd = new();

                                if (auxplant != -3)
                                {
                                    Assychart_ToAdd.Plant = Assy_Plant;
                                }

                                //area
                                try
                                {

                                    if (auxplant < 0)
                                    {
                                        //si la planta no existe
                                        //Compruebo documento
                                        if (row[1] == "§")
                                        {
                                            //Mensaje de area no existe, mostrar 
                                            auxarea = -3;
                                            //0 Create, 1 Read, 2 Update, 3 Adding, 4 error, 5 advertencia
                                            var msgArea = new EventMensaje();
                                            msgArea.Mensaje = $"Error en `B{BulkedData.Index(r => r == row) + 1}`: Falta AreaId/Code del area .";
                                            msgArea.Tipo = 4;
                                            List_Events.Add(msgArea);
                                        }
                                        else
                                        {
                                            // code existe
                                            if (row[2] == "§")
                                            {
                                                auxarea = -1;
                                                //Descripcion no existe, lanzar error
                                                //0 Create, 1 Read, 2 Update, 3 Adding, 4 error 
                                                var msgArea = new EventMensaje();
                                                msgArea.Mensaje = $"Error en `C{BulkedData.Index(r => r == row) + 1}`: Falta Descripcion para la Creacion del area `{row[1]}` .";
                                                msgArea.Tipo = 4;
                                                List_Events.Add(msgArea);
                                            }
                                            else
                                            {
                                                //compruebo si mensaje de creacion ya existe
                                                if (!List_Events.Any(e => (e.Mensaje.Contains("creara") || e.Mensaje.Contains("created")) && e.Mensaje.Contains(row[1])))
                                                {
                                                    var msgArea = new EventMensaje();
                                                    msgArea.Mensaje = $"Se creara el area `{row[1]}` en la planta: `{BulkedData[0][1]}`";
                                                    msgArea.Tipo = 0;
                                                    List_Events.Add(msgArea);
                                                }
                                                auxarea = -2;
                                                Area area = new Area();
                                                area.Code = row[1];
                                                area.Description = row[2];
                                                Assychart_ToAdd.Area = area;
                                            }
                                        }

                                    }
                                    else
                                    {
                                        //cuando la planta existe

                                        if (row[1] == "§")
                                        {
                                            //Codigo vacio se muestra error
                                            auxarea = -3;
                                            //0 Create, 1 Read, 2 Update, 3 Adding, 4 error 
                                            var msgArea = new EventMensaje();
                                            msgArea.Mensaje = $"Error en `B{BulkedData.Index(r => r == row) + 1}`: Falta AreaId/Code del area .";
                                            msgArea.Tipo = 4;
                                            List_Events.Add(msgArea);
                                        }
                                        else
                                        {

                                            if (_areas.ElementAt(auxplant).Value.Any(a => a.Code == row[1]))
                                            {
                                                //area si existe
                                                auxarea = _areas.ElementAt(auxplant).Value.Index(a => a.Code == row[1]);

                                                Assychart_ToAdd.Area = _areas.ElementAt(auxplant).Value.ElementAt(auxarea);

                                                if (_areas.ElementAt(auxplant).Value.ElementAt(auxarea).Description != row[2] && row[2] != "§")
                                                {
                                                    //0 Create, 1 Read, 2 Update, 3 Adding, 4 error 
                                                    Console.WriteLine("La Area Se actualiza");
                                                    var msgArea = new EventMensaje();
                                                    msgArea.Mensaje = $"Se Actualizara la descripcion del Area: `{row[1]}`. De `{Assychart_ToAdd.Area.Description}` a `{row[2]}`";
                                                    Assychart_ToAdd.Area.Description = row[2];
                                                    msgArea.Tipo = 2;
                                                    List_Events.Add(msgArea);
                                                }
                                            }
                                            else
                                            {
                                                //area no existe
                                                if (row[2] == "§")
                                                {
                                                    //Mensaje de error descripcion no existe, no se puede crear
                                                    auxarea = -1;

                                                    //0 Create, 1 Read, 2 Update, 3 Adding, 4 error 
                                                    var msgArea = new EventMensaje();
                                                    msgArea.Mensaje = $"Error en `C{BulkedData.Index(r => r == row) + 1}`: Falta Descripcion para la Creacion del area `{row[1]}` .";
                                                    msgArea.Tipo = 4;
                                                    List_Events.Add(msgArea);
                                                }
                                                else
                                                {

                                                    //se comprueba si ya existe mensaje para no duplicar
                                                    if (!List_Events.Any(e => (e.Mensaje.Contains("creara") || e.Mensaje.Contains("created")) && e.Mensaje.Contains(row[1])))
                                                    {
                                                        var msgArea = new EventMensaje();
                                                        msgArea.Mensaje = $"Se creara el area `{row[1]}`n en la planta: `{BulkedData[0][1]}`";
                                                        msgArea.Tipo = 0;
                                                        List_Events.Add(msgArea);
                                                    }

                                                    Area area = new Area();
                                                    area.Code = row[1];
                                                    area.Description = row[2];
                                                    auxarea = -2;
                                                    Assychart_ToAdd.Area = area;
                                                }

                                            }
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine("");
                                    var msgArea = new EventMensaje();
                                    msgArea.Mensaje = $"Error en `B{BulkedData.Index(r => r == row) + 1}` o `C{BulkedData.Index(r => r == row) + 1}`: Area.";
                                    msgArea.Tipo = 4;
                                    List_Events.Add(msgArea);

                                }

                                //Distribucion
                                try
                                {
                                    //si la area no existe
                                    if (auxarea < 0)
                                    {
                                        //Compruebo documento
                                        if (row[3] == "§")
                                        {
                                            //Mensaje de distribucion vaciao no existe, mostrar error

                                            auxdistribution = -3;
                                            //0 Create, 1 Read, 2 Update, 3 Adding, 4 error 
                                            var msgDistribution = new EventMensaje();
                                            msgDistribution.Mensaje = $"Error en `D{BulkedData.Index(r => r == row) + 1}`: Falta DistributionId/Code de Distribucion .";
                                            msgDistribution.Tipo = 4;
                                            List_Events.Add(msgDistribution);
                                        }
                                        else
                                        {
                                            // code existe
                                            if (row[4] == "§")
                                            {
                                                auxdistribution = -1;
                                                //Descripcion no existe, lanzar error
                                                //0 Create, 1 Read, 2 Update, 3 Adding, 4 error 
                                                var msgDistribution = new EventMensaje();
                                                msgDistribution.Mensaje = $"Error en `E{BulkedData.Index(r => r == row) + 1}`: Falta Descripcion para la Creacion de la distribucion `{row[3]}` .";
                                                msgDistribution.Tipo = 4;
                                                List_Events.Add(msgDistribution);
                                            }
                                            else
                                            {
                                                //compruebo si mensaje de creacion ya existe
                                                if (!List_Events.Any(e => (e.Mensaje.Contains("creara") || e.Mensaje.Contains("created")) && e.Mensaje.Contains(row[3])))
                                                {
                                                    var msgDistribution = new EventMensaje();
                                                    if (auxarea != -3)
                                                        msgDistribution.Mensaje = $"Se creara la distribucion `{row[3]}` en la area: `{Assychart_ToAdd.Area?.Code}`";
                                                    else
                                                        msgDistribution.Mensaje = $"Se creara la distribucion `{row[3]}` ";
                                                    msgDistribution.Tipo = 0;
                                                    List_Events.Add(msgDistribution);
                                                }

                                                Distribution distri = new Distribution();
                                                distri.Code = row[3];
                                                distri.Description = row[4];
                                                Assychart_ToAdd.Distribution = distri;
                                                auxdistribution = -2;
                                            }
                                        }

                                    }
                                    else
                                    {
                                        //cuando la area existe

                                        if (row[3] == "§")
                                        {
                                            //Codigo vacio se muestra error
                                            auxdistribution = -3;

                                            var msgDistribution = new EventMensaje();
                                            msgDistribution.Mensaje = $"Error en `D{BulkedData.Index(r => r == row) + 1}`: Falta DistributionId/Code de Distribucion .";
                                            msgDistribution.Tipo = 4;
                                            List_Events.Add(msgDistribution);
                                        }
                                        else
                                        {

                                            if (_distributions.ElementAt(auxplant).Value.ElementAt(auxarea).Value.Any(a => a.Code == row[3]))
                                            {
                                                //distirbucion si existe
                                                auxdistribution = _distributions.ElementAt(auxplant ).Value.ElementAt(auxarea).Value.Index(a => a.Code == row[3]);

                                                Assychart_ToAdd.Distribution = _distributions.ElementAt(auxplant).Value.ElementAt(auxarea).Value.ElementAt(auxdistribution);

                                                if (_distributions.ElementAt(auxplant).Value.ElementAt(auxarea).Value.ElementAt(auxdistribution).Description != row[4] && row[4] != "§")
                                                {
                                                    //0 Create, 1 Read, 2 Update, 3 Adding, 4 error 
                                                    Console.WriteLine("La distribucion Se actualiza");

                                                    var msgDistribution = new EventMensaje();
                                                    msgDistribution.Mensaje = $"Se Actualizara la descripcion de la distribucion: `{row[3]}`. De `{Assychart_ToAdd.Distribution?.Description}` a `{row[4]}`";
                                                    Assychart_ToAdd.Distribution.Description = row[4];
                                                    msgDistribution.Tipo = 2;
                                                    List_Events.Add(msgDistribution);

                                                }
                                            }
                                            else
                                            {
                                                //distirbucion no existe
                                                if (row[4] == "§")
                                                {

                                                    //Mensaje de error descripcion no existe, no se puede crear
                                                    auxdistribution = -1;

                                                    //0 Create, 1 Read, 2 Update, 3 Adding, 4 error 
                                                    var msgDistribution = new EventMensaje();
                                                    msgDistribution.Mensaje = $"Error en `E{BulkedData.Index(r => r == row) + 1}`: Falta Descripcion para la Creacion de la distribucion `{row[3]}` .";
                                                    msgDistribution.Tipo = 4;
                                                    List_Events.Add(msgDistribution);
                                                }
                                                else
                                                {

                                                    //se comprueba si ya existe mensaje para no duplicar
                                                    if (!List_Events.Any(e => (e.Mensaje.Contains("creara") || e.Mensaje.Contains("created")) && e.Mensaje.Contains(row[3])))
                                                    {
                                                        var msgDistribution = new EventMensaje();
                                                        msgDistribution.Mensaje = $"Se creara la distribucion `{row[3]}`n en la planta: `{BulkedData[0][1]}`";
                                                        msgDistribution.Tipo = 0;
                                                        List_Events.Add(msgDistribution);
                                                    }

                                                    Distribution distri = new Distribution();
                                                    distri.Code = row[3];
                                                    distri.Description = row[4];
                                                    auxdistribution = -2;
                                                    Assychart_ToAdd.Distribution = distri;
                                                }

                                            }
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine("");
                                    var msgDistribucion = new EventMensaje();
                                    msgDistribucion.Mensaje = $"Error en D{BulkedData.Index(r => r == row) + 1}` o `E{BulkedData.Index(r => r == row) + 1}`: Distribucion.";
                                    msgDistribucion.Tipo = 4;
                                    List_Events.Add(msgDistribucion);

                                }


                                //creacion de rutas

                                RouteProductAssyChart GOSRute = new();

                                try
                                {
                                    if (row[5] == "§")
                                    {
                                        // error no product code or id
                                        //0 Create, 1 Read, 2 Update, 3 Adding, 4 error 5 Advertencia
                                        var msgProduct1 = new EventMensaje();
                                        msgProduct1.Mensaje = $"Error en F{BulkedData.Index(r => r == row) + 1}` falta Product1Id/Code";
                                        msgProduct1.Tipo = 4;
                                        List_Events.Add(msgProduct1);
                                    }
                                    else
                                    {
                                        if (_products.Any(p => p.Code == row[5]))
                                        {
                                            var indexGos = _products.Index(p => p.Code == row[5]);
                                            GOSRute.ProductId = _products.ElementAt(indexGos).ProductId;
                                            GOSRute.Product = _products.ElementAt(indexGos);

                                        }
                                        else
                                        {
                                            if (row[5] == "§")
                                            {
                                                // error no product descripcion missing
                                                //0 Create, 1 Read, 2 Update, 3 Adding, 4 error 5 Advertencia
                                                var msgProduct1 = new EventMensaje();
                                                msgProduct1.Mensaje = $"Error en G{BulkedData.Index(r => r == row) + 1}` falta descripcion para la creacion del producto";
                                                msgProduct1.Tipo = 4;
                                                List_Events.Add(msgProduct1);
                                            }
                                            else
                                            {
                                                //0 Create, 1 Read, 2 Update, 3 Adding, 4 error 5 Advertencia
                                                Product NewGosProduct = new();
                                                NewGosProduct.Code = row[5];
                                                NewGosProduct.Description = row[6];
                                                GOSRute.Product = NewGosProduct;

                                                // varificar que las rutas sean validas o aproximar lo maejor posible
                                                // pendiente
                                                //Mensaje de producto no existe y se creara
                                                
                                            }
                                        }
                                    }
                                }catch(Exception ex)
                                {
                                    Console.WriteLine("GoS Rute");
                                }



                                if (auxplant >= 0 && auxplant >= 0 && auxdistribution >= 0)
                                {
                                    var anyAssyChart = await AssyChartServices.GetAssyChartJobObservation(auxplant, auxarea, auxdistribution);

                                    if (anyAssyChart != null)
                                    {
                                        //0 Create, 1 Read, 2 Update, 3 Adding, 4 error 5 Advertencia
                                        var msgAssyChart = new EventMensaje();
                                        msgAssyChart.Mensaje = $"El assychart del renglon {BulkedData.Index(r => r == row) + 1}` ya existe en la base de datos";
                                        msgAssyChart.Tipo = 5;
                                        List_Events.Add(msgAssyChart);
                                    }
                                    else
                                    {
                                        if (!List_Events.Any(e => (e.Mensaje.Contains("creara") || e.Mensaje.Contains("created")) && e.Mensaje.Contains("assychart") && (e.Mensaje.Contains(Assychart_ToAdd.Plant.Code) && e.Mensaje.Contains(Assychart_ToAdd.Area.Code) && e.Mensaje.Contains(Assychart_ToAdd.Distribution.Code))))
                                        {
                                            //0 Create, 1 Read, 2 Update, 3 Adding, 4 error 5 Advertencia
                                            var msgAssyChart = new EventMensaje();
                                            msgAssyChart.Mensaje = $"Se creara el assychart de la planta: '{Assychart_ToAdd.Plant?.Code}', Area: '{Assychart_ToAdd.Area?.Code}' y Distribucion: '{Assychart_ToAdd.Distribution?.Code}'";
                                            msgAssyChart.Tipo = 0;
                                            List_Events.Add(msgAssyChart);

                                        }

                                    }
                                }
                                else
                                {
                                    //0 Create, 1 Read, 2 Update, 3 Adding, 4 error 5 Advertencia
                                    var msgAssyChart = new EventMensaje();
                                    if (auxplant != -3 && auxarea != -3 && auxdistribution != -3)
                                        msgAssyChart.Mensaje = $"Se creara el assychart de la planta: '{Assychart_ToAdd.Plant?.Code}', Area: '{Assychart_ToAdd.Area?.Code}' y Distribucion: '{Assychart_ToAdd.Distribution?.Code}'";
                                    else if (auxplant == -3 && auxarea != -3 && auxdistribution != -3)
                                        msgAssyChart.Mensaje = $"Se creara el assychart del Area: '{Assychart_ToAdd.Area?.Code}' y Distribucion: '{Assychart_ToAdd.Distribution?.Code}' ";
                                    else if (auxplant == -3 && auxarea == -3 && auxdistribution != -3)
                                        msgAssyChart.Mensaje = $"Se creara el assychart de la Distribucion: '{Assychart_ToAdd.Distribution?.Code}'";


                                    msgAssyChart.Tipo = 0;
                                    List_Events.Add(msgAssyChart);
                                }

                                _assycharts.Add(Assychart_ToAdd);
                            }


                        }//End Foreach


                        if (List_Events.Any(e => e.Tipo == 4))
                        {
                            showTable = false;
                        }
                        else
                        {
                            showTable = true;
                        }

                        break;

                }



                //foreach (string[] row in csv)
                //{
                //    try
                //    {
                //        if (isFirstRow)
                //        {
                //            plantToUse.PlantId = int.Parse(row[1]);
                //            plantToUse.Code = row[4];
                //            plantToUse.Description = row[7];
                //            isFirstRow = false;

                //        }
                //        else
                //        {
                //            var ToInsertIntoList = new BulkAndUpload();
                //            ToInsertIntoList.AssyChardId = row[0] != "" ? int.Parse(row[0]) : -1;
                //            ToInsertIntoList.IsActive = row[1] != "" ? bool.Parse(row[1]) : true;
                //            ToInsertIntoList.GOS = row[2] != "" ? row[2] : "";
                //            ToInsertIntoList.CCP = row[3] != "" ? row[3] : "";
                //            ToInsertIntoList.HOE = row[4] != "" ? row[4] : "";

                //            ToInsertIntoList.CreationDate = row[5] != "" ? DateTime.Parse(row[5]) : DateTime.Now;
                //            ToInsertIntoList.ModificationDate = row[6] != "" ? DateTime.Parse(row[6]) : DateTime.Now;

                //            ToInsertIntoList.Plant = plantToUse;
                //            ToInsertIntoList.PlantId = plantToUse.PlantId;

                //            ToInsertIntoList.ProductId = row[7] != "" ? int.Parse(row[7]) : -1;

                //            ToInsertIntoList.Product = new Product
                //            {
                //                ProductId = row[7] != "" ? int.Parse(row[7]) : -1,
                //                Code = row[8] != "" ? row[8] : "",
                //                Description = row[9] != "" ? row[9] : "",
                //                IsActive = row[10] != "" ? bool.Parse(row[10]) : true,
                //            };

                //            ToInsertIntoList.AreaId = int.Parse(row[11]);
                //            ToInsertIntoList.Area = new Area
                //            {
                //                AreaId = row[11] != "" ? int.Parse(row[11]) : -1,
                //                Code = row[12] != "" ? row[12] : "",
                //                Description = row[13] != "" ? row[13] : "",
                //                IsActive = row[14] != " " ? bool.Parse(row[14]) : true,
                //            };
                //            ToInsertIntoList.OperationId = row[15] != "" ? int.Parse(row[15]) : -1;
                //            ToInsertIntoList.Operation = new Operation
                //            {
                //                OperationId = row[15] != "" ? int.Parse(row[15]) : -1,
                //                Code = row[16] != "" ? row[16] : "",
                //                Description = row[17] != "" ? row[17] : "",
                //                IsActive = row[18] != "" ? bool.Parse(row[18]) : true,
                //            };
                //            ToInsertIntoList.DistributionId = row[19] != "" ? int.Parse(row[19]) : -1;
                //            ToInsertIntoList.Distribution = new Distribution
                //            {
                //                DistributionId = row[19] != "" ? int.Parse(row[19]) : -1,
                //                Code = row[20] != "" ? row[20] : "",
                //                Description = row[21] != "" ? row[21] : "",
                //                IsActive = row[22] != "" ? bool.Parse(row[22]) : true,
                //            };

                //            dataTableToShow.Add(ToInsertIntoList);

                //        }
                //    }
                //    catch (Exception ex)
                //    {

                //        ErrorMessageToDisplay = Localizer["ACmsgError"];
                //        Console.WriteLine($"{ex.Message}");
                //        showTableToShow = false;
                //    }//endtrycatch

                //}//end foreach 
                //active display table

                StateHasChanged();
            }//end else



        }//end function on change

        private async Task DownloadAssyChartFormat()
        {
            await AssyChartServices.DownloadAssyChartFormat();
        }
        public static async Task<List<string[]>> GetDataTableFromExcel(IBrowserFile file)
        {
            List<string[]> dtTable = new List<string[]>();
            List<string> Columns = new List<string>();


            using (MemoryStream memStream = new MemoryStream())
            {
                await file.OpenReadStream(file.Size).CopyToAsync(memStream);
                using (XLWorkbook workBook = new XLWorkbook(memStream, XLEventTracking.Disabled))
                {
                    //Read the first Sheet from Excel file.
                    IXLWorksheet workSheet = workBook.Worksheet(1);

                    foreach (IXLRow row in workSheet.Rows())
                    {
                        if (!row.IsEmpty())
                        {
                            Columns.Clear();

                            foreach (IXLCell cell in row.Cells(1, 20))
                            {
                                string toinsert = "§";

                                // Verificar si la celda no está vacía antes de obtener su valor
                                if (!cell.IsEmpty())
                                {
                                    toinsert = cell.Value.ToString();
                                }

                                Columns.Add(toinsert);
                            }
                            dtTable.Add(Columns.ToArray());
                        }


                    }


                }
            }

            return dtTable;
        }


        private string[] SplitCSV(string input)
        {
            //Excludes commas within quotes  
            Regex csvSplit = new Regex("(?:^|,)(\"(?:[^\"]+|\"\")*\"|[^,]*)", RegexOptions.Compiled);
            List<string> list = new List<string>();
            string curr = string.Empty;
            foreach (Match match in csvSplit.Matches(input))
            {
                curr = match.Value;
                if (0 == curr.Length) list.Add("§");

                list.Add(curr.TrimStart(','));
            }

            return list.ToArray();
        }


        void CancelFunction()
        {
            FileName = string.Empty;
            ErrorMessageToDisplay = string.Empty;
            BulkedData.Clear();
            List_Events.Clear();
            _assycharts.Clear();
            showTable = false;
            FileSource = null;
        }


        async void UploadFunction()
        {
            activeUpload = true;
            using var content = new MultipartFormDataContent();

            if (FileSource is not null)
            {

                var fileContent = new StreamContent(FileSource.OpenReadStream(509600000));
                fileContent.Headers.ContentType = new MediaTypeHeaderValue(FileSource.ContentType);

                content.Add(
                content: fileContent,
                name: "\"file\"",
                fileName: FileSource.Name);

                var newUploadResults = await FileUpDoServices.UploadFile(content);

                if (newUploadResults is not null)
                {
                    uploadResult = newUploadResults;

                    UploadAssyChartResult newDataResults = await FileUpDoServices.ProccedToUpdateData(uploadResult);

                    if (newDataResults is not null)
                    {
                        //tiene resultados
                        ErrorMessageToDisplay = "Upload Data Succesfull";
                        BulkedData.Clear();
                        _assycharts.Clear();
                        showTable = false;
                        List_Events.Clear();
                        displayResume = true;
                        activeUpload = false;
                        FileSource = null;
                        retornedResult_Uploaded = newDataResults;

                        base.StateHasChanged();

                    }
                    else
                    {
                        ErrorMessageToDisplay = "Fail, Upload Data, pls Call for admin";
                    }

                }
                else
                {
                    //error al subir el archivo
                }

            }
            else
            {
                ErrorMessageToDisplay = "Fail, Upload Data, pls Call for admin";
            }



        }


    }//end UploadAndBulk


}