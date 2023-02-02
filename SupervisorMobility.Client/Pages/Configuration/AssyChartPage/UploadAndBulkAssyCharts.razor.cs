using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;
using System.Text.RegularExpressions;
using SpreadsheetLight;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;


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


        private string FileSource;
        private string ErrorMessage;
        private string plant;
        private string model;
        List<string[]> csv = new List<string[]>();
        List<DtosBulkAndUpload> data = new();

        public bool Label_Switch1 { get; set; } = false;
        public bool Label_Switch2 { get; set; } = true;
        public bool Label_Switch3 { get; set; } = true;

        private void Submit()
        {
            
        }
        async Task OnChange(InputFileChangeEventArgs e)
        {
            FileSource = string.Empty;
            ErrorMessage = string.Empty;
            csv.Clear();

            var singleFile = e.File;

            Regex regexcsv = new Regex(".+\\.csv", RegexOptions.Compiled);
            Regex regexlsx = new Regex(".+\\.xlsx", RegexOptions.Compiled);
            if (!regexcsv.IsMatch(singleFile.Name) && !regexlsx.IsMatch(singleFile.Name))
            {
                ErrorMessage = $"Only CSV / XLSX files can be uploaded";
            }
            else
            {
                var stream = singleFile.OpenReadStream();

                MemoryStream ms = new MemoryStream();
                await stream.CopyToAsync(ms);
                FileSource = ($"data:{singleFile.ContentType};base64,{Convert.ToBase64String(ms.ToArray())}");
                stream.Close();
                var outputFileString = System.Text.Encoding.UTF8.GetString(ms.ToArray());

       

                if (regexcsv.IsMatch(singleFile.Name))
                    foreach (var item in outputFileString.Split(Environment.NewLine))
                    {
                        csv.Add(SplitCSV(item.ToString()));
                    }

                if (regexlsx.IsMatch(singleFile.Name))
                {
                    SLDocument sl = new SLDocument(stream);
                    int iRow = 2;
                    while (!string.IsNullOrEmpty(sl.GetCellValueAsString(iRow, 1)))
                    {
                        string clave = sl.GetCellValueAsString(iRow, 1);
                        string descripcion = sl.GetCellValueAsString(iRow, 2);
                        double costo = sl.GetCellValueAsDouble(iRow, 3);
                        double precio1 = sl.GetCellValueAsDouble(iRow, 4);
                        double precio2 = sl.GetCellValueAsDouble(iRow, 5);
                        double precio3 = sl.GetCellValueAsDouble(iRow, 6);
                        List<string> list = new List<string>();
                        list.Add(clave);
                        list.Add(descripcion);  

                        csv.Add(list.ToArray());
                        iRow++;
                    }
                }
                    


            }


        }//end function on change
       

       
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
        public int AssyChartId { get; set; }
        public string AssyChartIsActive { get; set; }
        public string AssyChartGos { get; set; }
        public string AssyChartCcp { get; set; }
        public string AssyChartHoe { get; set; }
        public DateTime AssyChartCreationDate { get; set; }
        public DateTime AssyChartModificationDate { get; set; }

        //Product
        public int ProductId { get; set; }
        public string ProductDescription { get; set; }
        public string ProductCode { get; set; }
        //Plant
        public int PlantId { get; set; }
        public string PlantDescription{ get; set;}
        public string PlantCode { get; set; } 
        //Area
        public int AreaId { get; set; }
        public string AreaDescription { get; set; }
        public string AreaCode { get; set; }
        //Distribution
        public int DistributionId { get; set; }
        public string DistributionDescription { get; set; }
        public string DistributionCode { get; set; }
        //Operation
        public int OperationId { get; set; }
        public string OperationDescription { get; set; }
        public string OperationCode { get; set; }



    }
}