using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;
using System.Text.RegularExpressions;
using ClosedXML.Excel;
using System.Net.Http.Headers;
using Microsoft.JSInterop;
using DocumentFormat.OpenXml.Office.CustomUI;
using DocumentFormat.OpenXml.Spreadsheet;
using SupervisorMobility.Client.Services.UserService;

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
        private bool displayResume = false;
        private bool activeUpload = false;
        private bool showTableToShow = false;
        private List<string[]> csv = new List<string[]>();
        private List<BulkAndUpload> dataTableToShow = new();

        private FileUpload uploadResult = new();
        private UploadAssyChartResult retornedResult = new();


        protected async override Task OnInitializedAsync()
        {
            _plants = await PlantServices.GetPlants();
        }


        async Task OnChange(InputFileChangeEventArgs e)
        {
            //Clear Data
            FileName = string.Empty;
            FileSource = null;

            ErrorMessageToDisplay = string.Empty;
            csv.Clear();
            dataTableToShow.Clear();
            showTableToShow = false;


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
                        var addtolist = SplitCSV(item.ToString());


                        if (!addtolist.All(string.IsNullOrWhiteSpace))
                        {
                            csv.Add(addtolist);
                        }

                    }

                    csv.RemoveAt(1);
                }

                if (regexlsx.IsMatch(e.File.Name))
                {
                    try
                    {
                        csv = await GetDataTableFromExcel(e.File);
                        isOkFile = true;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.ToString());
                    }
                }

                bool isFirstRow = true;


                showTableToShow = true;


                foreach (string[] row in csv)
                {
                    try
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
                            ToInsertIntoList.AssyChardId = row[0] != "" ? int.Parse(row[0]) : -1;
                            ToInsertIntoList.IsActive = row[1] != "" ? bool.Parse(row[1]) : true;
                            ToInsertIntoList.GOS = row[2] != "" ? row[2] : "";
                            ToInsertIntoList.CCP = row[3] != "" ? row[3] : "";
                            ToInsertIntoList.HOE = row[4] != "" ? row[4] : "";

                            ToInsertIntoList.CreationDate = row[5] != "" ? DateTime.Parse(row[5]) : DateTime.Now;
                            ToInsertIntoList.ModificationDate = row[6] != "" ? DateTime.Parse(row[6]) : DateTime.Now;

                            ToInsertIntoList.Plant = plantToUse;
                            ToInsertIntoList.PlantId = plantToUse.PlantId;

                            ToInsertIntoList.ProductId = row[7] != "" ? int.Parse(row[7]) : -1;

                            ToInsertIntoList.Product = new Product
                            {
                                ProductId = row[7] != "" ? int.Parse(row[7]) : -1,
                                Code = row[8] != "" ? row[8] : "",
                                Description = row[9] != "" ? row[9] : "",
                                IsActive = row[10] != "" ? bool.Parse(row[10]) : true,
                            };

                            ToInsertIntoList.AreaId = int.Parse(row[11]);
                            ToInsertIntoList.Area = new Area
                            {
                                AreaId = row[11] != "" ? int.Parse(row[11]) : -1,
                                Code = row[12] != "" ? row[12] : "",
                                Description = row[13] != "" ? row[13] : "",
                                IsActive = row[14] != " " ? bool.Parse(row[14]) : true,
                            };
                            ToInsertIntoList.OperationId = row[15] != "" ? int.Parse(row[15]) : -1;
                            ToInsertIntoList.Operation = new Operation
                            {
                                OperationId = row[15] != "" ? int.Parse(row[15]) : -1,
                                Code = row[16] != "" ? row[16] : "",
                                Description = row[17] != "" ? row[17] : "",
                                IsActive = row[18] != "" ? bool.Parse(row[18]) : true,
                            };
                            ToInsertIntoList.DistributionId = row[19] != "" ? int.Parse(row[19]) : -1;
                            ToInsertIntoList.Distribution = new Distribution
                            {
                                DistributionId = row[19] != "" ? int.Parse(row[19]) : -1,
                                Code = row[20] != "" ? row[20] : "",
                                Description = row[21] != "" ? row[21] : "",
                                IsActive = row[22] != "" ? bool.Parse(row[22]) : true,
                            };

                            dataTableToShow.Add(ToInsertIntoList);

                        }
                    }
                    catch (Exception ex)
                    {

                        ErrorMessageToDisplay = "File Corrupted/ File Content Error / Error of procesing. ";
                        Console.WriteLine($"{ex.Message}");
                        showTableToShow = false;
                    }//endtrycatch

                }//end foreach 
                 //active display table



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
                        csv.Clear();
                        dataTableToShow.Clear();

                        displayResume = true;
                        activeUpload = false;
                        showTableToShow = false;
                        FileSource = null;
                        retornedResult = newDataResults;

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