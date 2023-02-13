using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;
using System.Text.RegularExpressions;
using ClosedXML.Excel;
using System.Net.Http.Headers;

namespace SupervisorMobility.Client.Pages.Configuration.AssyChartPage
{
    public partial class UploadAssyCharts
    {
                // Breadcrumb links
        private List<BreadcrumbItem> _links = new List<BreadcrumbItem>
        {
            new BreadcrumbItem("Home", href: "#"),
            new BreadcrumbItem("Configuration", href: "/configuration"),
            new BreadcrumbItem("Assy Chart", href: "/assychart"),
            new BreadcrumbItem("Upload Assy Chart", href: "/UploadAssyCharts", disabled: true),
        };
        
        //Objects to Interacting with the archive upload
        private string FileName;
        private string ErrorMessageToDisplay;
        private IBrowserFile? FileSource;

        //List Plants to do bulk
        private List<Plant> _plants = new List<Plant>();
        private int plantId = 0;
        private bool AllPlantsSwitch = true;

        //Table to display elements in file, verificate data to upload
        private bool showTableToShow = false;
        private List<string[]> csv = new List<string[]>();
        private List<BulkAndUpload> dataTableToShow = new();

        private UploadResult uploadResult = new();

        protected async override Task OnInitializedAsync()
        {
            _plants = await PlantServices.GetPlants();
        }

      
        async Task OnChange(InputFileChangeEventArgs e)
        {
            //Clear Data
            FileName = string.Empty;
            ErrorMessageToDisplay = string.Empty;
            csv.Clear();
            dataTableToShow.Clear();
            showTableToShow = false;
            FileSource = null;


            //Assign new data
            FileName = e.File.Name;
            FileSource = e.File;

            //Ragex to get extenxion of file, only CSV or Excel allowed files
            Regex regexcsv = new Regex(".+\\.csv", RegexOptions.Compiled);
            Regex regexlsx = new Regex(".+\\.xlsx", RegexOptions.Compiled);


            using var content = new MultipartFormDataContent();


            if (!regexcsv.IsMatch(e.File.Name) && !regexlsx.IsMatch(e.File.Name))
            {
                ErrorMessageToDisplay = $"Only CSV / XLSX files can be uploaded";
            }
            else
            {
                var fileContent = new StreamContent(e.File.OpenReadStream(509600000));
                fileContent.Headers.ContentType = new MediaTypeHeaderValue(e.File.ContentType);

                MemoryStream ms = new MemoryStream();
                await fileContent.CopyToAsync(ms);
                var outputFileString = System.Text.Encoding.UTF8.GetString(ms.ToArray());
                bool isOkFile = false;

                Plant plantToUse = new Plant();

                if (regexcsv.IsMatch(e.File.Name))
                {
                    foreach (var item in outputFileString.Split(Environment.NewLine))
                    {
                        csv.Add(SplitCSV(item.ToString()));
                    }
                    csv.RemoveAt(1);
                    csv.RemoveAt(csv.Count - 1);
                }

                if (regexlsx.IsMatch(e.File.Name))
                {
                    try
                    {
                        csv = await GetDataTableFromExcel(e.File);
                        isOkFile = true;
                    }catch (Exception ex)
                    {
                        Console.WriteLine(ex.ToString());
                    }
                }

                bool isFirstRow = true;


                try
                {
                    foreach (string[] row in csv)
                    {
                        if (isFirstRow)
                        {
                            plantToUse.PlantId = int.Parse(row[1]);
                            plantToUse.Code = row[4];
                            plantToUse.Description = row[7];
                            isFirstRow = false;
                        }
                        else
                        {
                            var ToInsertIntoList = new BulkAndUpload();
                            ToInsertIntoList.AssyChardId = int.Parse(row[0]);
                            ToInsertIntoList.IsActive = bool.Parse(row[1]);
                            ToInsertIntoList.GOS = row[2];
                            ToInsertIntoList.CCP = row[3];
                            ToInsertIntoList.HOE = row[4];
                            ToInsertIntoList.CreationDate = DateTime.Parse(row[5]);
                            ToInsertIntoList.ModificationDate = DateTime.Parse(row[6]);
                            ToInsertIntoList.Plant = plantToUse;
                            ToInsertIntoList.PlantId = plantToUse.PlantId;
                            ToInsertIntoList.ProductId = int.Parse(row[7]);
                            ToInsertIntoList.Product = new Product
                            {
                                ProductId = int.Parse(row[7]),
                                Code = row[8],
                                Description = row[9],
                                IsActive = bool.Parse(row[10])
                            };
                            ToInsertIntoList.AreaId = int.Parse(row[11]);
                            ToInsertIntoList.Area = new Area
                            {
                                Code = row[12],
                                Description = row[13],
                                IsActive = bool.Parse(row[14])
                            };
                            ToInsertIntoList.OperationId = int.Parse(row[15]);
                            ToInsertIntoList.Operation = new Operation
                            {
                                AreaId = int.Parse(row[11]),
                                Code = row[16],
                                Description = row[17],
                                IsActive = bool.Parse(row[18])
                            };
                            ToInsertIntoList.DistributionId = int.Parse(row[19]);
                            ToInsertIntoList.Distribution = new Distribution
                            {
                                Code = row[20],
                                Description = row[21],
                                IsActive = bool.Parse(row[22])
                            };

                            dataTableToShow.Add(ToInsertIntoList);

                        }

                    }
                    //active display table
                    showTableToShow = true;
                }
                catch (Exception ex)
                {
                    ErrorMessageToDisplay = "Archivo Incorrecto/ Contenido Incorrecto";
                    Console.WriteLine(ex.ToString());

                }


            }//end else


        }//end function on change


        public static async Task<List<string[]>> GetDataTableFromExcel(IBrowserFile file)
        {
            List<string[]> dtTable = new List<string[]>();
            List<BulkAndUpload> dtTableToReturn = new List<BulkAndUpload>();
            List<string> Columns = new List<string>();
            BulkAndUpload ToInsertInDtoTable = new();


            using (MemoryStream memStream = new MemoryStream())
            {
                await file.OpenReadStream(file.Size).CopyToAsync(memStream);
                using (XLWorkbook workBook = new XLWorkbook(memStream, XLEventTracking.Disabled))
                {
                    //Read the first Sheet from Excel file.
                    IXLWorksheet workSheet = workBook.Worksheet(1);

                    //Loop through the Worksheet rows.
                    bool firstRow = true;
                    bool SecondRow = true;
                    foreach (IXLRow row in workSheet.Rows())
                    {
                        //Use the first row to add columns to DataTable.
                        if (firstRow && SecondRow)
                        {
                            Columns.Clear();
                            foreach (IXLCell cell in row.Cells())
                            {
                                Columns.Add(cell.Value.ToString());
                            }
                            dtTable.Add(Columns.ToArray());
                            firstRow = false;
                        }
                        else if (SecondRow && !firstRow)
                        {
                            SecondRow = false;
                        }
                        else
                        {
                            //Verificamos que no es columna vacia
                            if (!row.IsEmpty())
                            {
                                Columns.Clear();
                                foreach (IXLCell cell in row.Cells())
                                {
                                    Columns.Add(cell.Value.ToString());
                                }
                                dtTable.Add(Columns.ToArray());
                            }
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
                if (0 == curr.Length) list.Add("");

                list.Add(curr.TrimStart(','));
            }

            return list.ToArray();
        }


        void CancelFunction()
        {

            FileName = string.Empty;
            ErrorMessageToDisplay = string.Empty;
            csv.Clear();
            dataTableToShow.Clear();
            showTableToShow = false;
            FileSource = null;


        }

        async void UploadFunction()
        {
            Console.WriteLine("Into Upload");
            using var content = new MultipartFormDataContent();
            Console.WriteLine($"Filesource is {FileSource?.Name}");

            if (FileSource is not null)
            {
                Console.WriteLine("Into Not NUll");

                var fileContent = new StreamContent(FileSource.OpenReadStream(509600000));
                fileContent.Headers.ContentType = new MediaTypeHeaderValue(FileSource.ContentType);

                content.Add(
                content: fileContent,
                name: "\"file\"",
                fileName: FileSource.Name);

                var newUploadResults = await FileUploadService.UploadFile(content);
                Console.WriteLine("New Results");

                if (newUploadResults is not null)
                {
                    uploadResult = newUploadResults;
                    Console.WriteLine($"Into not null result {uploadResult.FileName} {uploadResult.StorageFileName}");
                    var newDataResults = await FileUploadService.SetNewData(uploadResult);  
                }

                Console.WriteLine($"Out not null result {uploadResult.FileName} {uploadResult.StorageFileName}");

            }
            else
            {
                Console.WriteLine(" Archivo vacio");
            }



        }

    }//end UploadAndBulk


}