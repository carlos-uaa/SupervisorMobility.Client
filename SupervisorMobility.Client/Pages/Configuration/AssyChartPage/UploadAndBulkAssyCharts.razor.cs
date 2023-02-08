using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;
using System.Text.RegularExpressions;
using SpreadsheetLight;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Excel.Helper;
using DocumentFormat.OpenXml.Bibliography;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.Drawing.Charts;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace SupervisorMobility.Client.Pages.Configuration.AssyChartPage
{
    public partial class UploadAndBulkAssyCharts
    {

        // Breadcrumb links
        private List<BreadcrumbItem> _links = new List<BreadcrumbItem>
        {
            new BreadcrumbItem("Home", href: "#"),
            new BreadcrumbItem("Configuration", href: "/configuration"),
            new BreadcrumbItem("Assy Chart", href: "/assychart"),
            new BreadcrumbItem("Upload Assy Chart", href: "/UploadAssyCharts", disabled: true),
        };


      

        private string FileName;
        private string ErrorMessage;
        private string plant;
        private string model;

        private bool showTableToShow = false;
        private List<string[]> csv = new List<string[]>();
        private List<DtosBulkAndUpload> dataTableToShow = new();

        private UploadResult uploadResult = new();

        public bool Label_Switch1 { get; set; } = false;
        public bool Label_Switch2 { get; set; } = true;
        public bool Label_Switch3 { get; set; } = true;

        private void Submit()
        {

        }
        async Task OnChange(InputFileChangeEventArgs e)
        {
            FileName = string.Empty;
            ErrorMessage = string.Empty;
            csv.Clear();

            FileName = e.File.Name;

            Regex regexcsv = new Regex(".+\\.csv", RegexOptions.Compiled);
            Regex regexlsx = new Regex(".+\\.xlsx", RegexOptions.Compiled);

            using var content = new MultipartFormDataContent();



            if (!regexcsv.IsMatch(e.File.Name) && !regexlsx.IsMatch(e.File.Name))
            {
                ErrorMessage = $"Only CSV / XLSX files can be uploaded";
            }
            else
            {
                var fileContent = new StreamContent(e.File.OpenReadStream(509600000));
                fileContent.Headers.ContentType = new MediaTypeHeaderValue(e.File.ContentType);


                content.Add(
                    content: fileContent,
                    name : "\"file\"",
                    fileName: e.File.Name);



                var newUploadResults = await FileUploadService.UploadFile(content);
                Console.WriteLine("start");

                if (newUploadResults is not null) {
                    Console.WriteLine("Entro");

                    uploadResult = newUploadResults;
                        //get data to display on table
                }

                Console.WriteLine("End");


                //function read file and show in view, no optim
                //var stream = FileSource.OpenReadStream();

                //MemoryStream ms = new MemoryStream();
                //await stream.CopyToAsync(ms);
                //stream.Close();

                //var outputFileString = System.Text.Encoding.UTF8.GetString(ms.ToArray());

                //if (regexcsv.IsMatch(FileSource.Name))
                //    foreach (var item in outputFileString.Split(Environment.NewLine))
                //    {
                //        csv.Add(SplitCSV(item.ToString()));
                //    }

                //if (regexlsx.IsMatch(FileSource.Name))
                //{
                //    csv = await GetDataTableFromExcel(e.File);
                //    DtosBulkAndUpload ToInsertInDtoTable = new();
                //    foreach (string[] row in csv)
                //    {
                //        foreach(string value in row)
                //        {
                //            ToInsertInDtoTable.AssyChartId = value;
                //         }
                //    }
                //    dataTableToShow.Add(ToInsertInDtoTable);
                //}
                //showTableToShow = true;
            }//end else


        }//end function on change

        private string? GetStoredFileName(string fileName)
        {
            var ResultUpload = uploadResult;
            if (ResultUpload is not null)
                  return ResultUpload.StorageFileName;


            return "File not found.";                
            
        }

        public static async Task<List<string[]>> GetDataTableFromExcel(IBrowserFile file)
        {
            List<string[]> dtTable = new List<string[]>();
            List<DtosBulkAndUpload> dtTableToReturn = new List<DtosBulkAndUpload>();
            List<string> Columns = new List<string>();
            DtosBulkAndUpload ToInsertInDtoTable = new();


            using (MemoryStream memStream = new MemoryStream())
            {
                await file.OpenReadStream(file.Size).CopyToAsync(memStream);
                using (XLWorkbook workBook = new XLWorkbook(memStream, XLEventTracking.Disabled))
                {
                    //Read the first Sheet from Excel file.
                    IXLWorksheet workSheet = workBook.Worksheet(1);

                    //Loop through the Worksheet rows.
                    bool firstRow = true;
                    foreach (IXLRow row in workSheet.Rows())
                    {
                        //Use the first row to add columns to DataTable.
                        if (firstRow)
                        {
                            firstRow = false;

                        }
                        else
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

        }

        void UploadFunction()
        {

        }

    }//end UploadAndBulk

    public class DtosBulkAndUpload
    {
        public string AssyChartId { get; set; }
        public string AssyChartIsActive { get; set; }
        public string AssyChartGos { get; set; }
        public string AssyChartCcp { get; set; }
        public string AssyChartHoe { get; set; }
        public string AssyChartCreationDate { get; set; }
        public string AssyChartModificationDate { get; set; }

        //Product
        public string ProductId { get; set; }
        public string ProductDescription { get; set; }
        public string ProductCode { get; set; }
        //Plant
        public string PlantId { get; set; }
        public string PlantDescription { get; set; }
        public string PlantCode { get; set; }
        //Area
        public string AreaId { get; set; }
        public string AreaDescription { get; set; }
        public string AreaCode { get; set; }
        //Distribution
        public string DistributionId { get; set; }
        public string DistributionDescription { get; set; }
        public string DistributionCode { get; set; }
        //Operation
        public string OperationId { get; set; }
        public string OperationDescription { get; set; }
        public string OperationCode { get; set; }



    }
}