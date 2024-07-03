using global::System;
using global::System.Collections.Generic;
using global::System.Linq;
using global::System.Threading.Tasks;
using global::Microsoft.AspNetCore.Components;
using System.Net.Http;
using System.Net.Http.Json;
using Microsoft.JSInterop;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.Web.Virtualization;
using Microsoft.AspNetCore.Components.WebAssembly.Http;
using SupervisorMobility.Client;
using SupervisorMobility.Client.Shared;
using SupervisorMobility.Client.Services;
using SupervisorMobility.Client.Data.Resources;
using Microsoft.Extensions.Localization;
using BlazorCameraStreamer;
using Blazored.SessionStorage;
using MudBlazor;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace SupervisorMobility.Client.Pages
{
    public partial class TestMarco
    {
        [Inject]
        private IDialogService DialogService { get; set; }
        public List<string> RawAnalisis { get; set; } = new List<string>();
        public List<string> RawAnalisisBk { get; set; } = new List<string>();

        private IEnumerable<string> _selectedValues = new List<string>();
        public List<Section> Sections { get; set; } = new List<Section>();

        public string stepName { get; set; } = "";
        bool showAddStepDialog = false;
        private DialogOptions dialogResourcesOptions = new()
        {
            CloseOnEscapeKey = true,
            MaxWidth = MaxWidth.Small,
            FullWidth = true,
            CloseButton = true
        };
        public class Section
        {
            public List<Analisis> Analisis { get; set; } = new List<Analisis>();
            public string Paso { get; set; } = "";
        }

        public class Analisis
        {
            public string Text { get; set; }
            public string PuntoCritico { get; set; } = string.Empty;
            public string Razon { get; set; }
        }

        protected async override Task OnInitializedAsync()
        {
            AddRawItem();
            StateHasChanged();
        }

        private void AddSection()
        {
            Sections.Add(new Section());
        }

        private void AddAnalisis(int sectionIndex)
        {
            Sections[sectionIndex].Analisis.Add(new Analisis());
        }

        private void AddPaso(int sectionIndex)
        {
            //Sections[sectionIndex].Pasos.Add("");
        }

        private void ApplyHighlights(int sectionIndex, int analisisIndex)
        {
            var analisis = Sections[sectionIndex].Analisis[analisisIndex];
            var text = analisis.Text;
            var term = analisis.PuntoCritico;
            if (string.IsNullOrEmpty(term))
            {
                return;
            }

            var highlightedText = ReplaceInsensitive(text, term);
            analisis.Text = highlightedText;
        }

        private static string Normalize(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return string.Empty;
            }

            return input.Normalize(NormalizationForm.FormD).Where(c => CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark).Aggregate(new StringBuilder(), (sb, c) => sb.Append(c)).ToString().ToLowerInvariant();
        }

        private static string ReplaceInsensitive(string text, string search)
        {
            if (string.IsNullOrEmpty(search))
            {
                return text;
            }

            string normalizedText = Normalize(text);
            string normalizedSearch = Normalize(search);
            var result = Regex.Replace(normalizedText, Regex.Escape(normalizedSearch), m =>
            {
                int startIndex = normalizedText.IndexOf(m.Value, m.Index, StringComparison.OrdinalIgnoreCase);
                string originalMatch = text.Substring(startIndex, search.Length);
                return $"<mark>{originalMatch}</mark>";
            }, RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
            return result;
        }

        private string GetAnalisisText(int sectionIndex, int analisisIndex)
        {
            return Sections[sectionIndex].Analisis[analisisIndex].Text;
        }

        private string GetHighlightedText(int sectionIndex, int analisisIndex)
        {
            return Sections[sectionIndex].Analisis[analisisIndex].PuntoCritico;
        }

        void CreateBakup()
        {
            RawAnalisisBk = ObjectCloner.ObjectCloner.DeepClone(RawAnalisis);
        }
        void RestoreBakup()
        {
            RawAnalisis = ObjectCloner.ObjectCloner.DeepClone(RawAnalisisBk);
            Sections.Clear();
        }
        void AddRawItem()
        {
            RawAnalisis.Add("");
        }

        void RemoveRawItem(string item)
        {
            if (RawAnalisis.Count > 1)
            {
                RawAnalisis.Remove(item);
            }
        }
        void RemoveSectionItem(Section item)
        {

            if (Sections.Count > 0)
            {
                // Obtener los textos de los análisis de la sección que se va a eliminar
                var textsToReinsert = item.Analisis.Select(analisis => analisis.Text).ToList();

                // Para cada texto, encontrar su posición correcta en RawAnalisis según RawAnalisisBk
                foreach (var text in textsToReinsert)
                {
                    int indexinsert = RawAnalisisBk.IndexOf(text);

                    // Encontrar el índice donde insertar en RawAnalisis
                    int insertPosition = RawAnalisis
                        .Select((value, index) => new { Value = value, Index = index })
                        .Where(x => RawAnalisisBk.IndexOf(x.Value) >= indexinsert)
                        .Select(x => x.Index)
                        .DefaultIfEmpty(RawAnalisis.Count)
                        .First();

                    // Insertar el texto en la posición correcta
                    RawAnalisis.Insert(insertPosition, text);
                }

                // Eliminar la sección de Sections
                Sections.Remove(item);
            }
        }

        public void AddStep()
        {
            showAddStepDialog = true;
        }

        private void CloseResourcesDialog()
        {
            showAddStepDialog = false;
        }
        public async void confirmStep()
        {

            if (!string.IsNullOrEmpty(stepName))
            {

                Section SectiontoAdd = new Section();
                foreach (string item in _selectedValues)
                {
                    Analisis ToAdd = new Analisis();
                    ToAdd.Text = item;
                    SectiontoAdd.Analisis.Add(ToAdd);
                    RawAnalisis.Remove(item);
                }

                SectiontoAdd.Paso = stepName;

                Sections.Add(SectiontoAdd);

                stepName = string.Empty;
                _selectedValues = new List<string>();
                CloseResourcesDialog();

            }
            else
            {
                bool? result = await DialogService.ShowMessageBox(
                   "Warning",
                    "Es necesario el texto!",
                   yesText: "Ok!");
                var state = result == null ? "Canceled" : "Deleted!";
                StateHasChanged();
            }

        }
    }
}