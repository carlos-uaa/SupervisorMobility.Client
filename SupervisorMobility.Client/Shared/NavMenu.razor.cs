using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using System.Net.Http;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.Web.Virtualization;
using Microsoft.AspNetCore.Components.WebAssembly.Http;
using Microsoft.JSInterop;
using MudBlazor;
using SupervisorMobility.Client;
using SupervisorMobility.Client.Shared;
using SupervisorMobility.Client.Services;
using SupervisorMobility.Client.Data.Entities;
using SupervisorMobility.Client.Pages.Configuration.PlantPage;
using DocumentFormat.OpenXml.EMMA;

namespace SupervisorMobility.Client.Shared
{
    public partial class NavMenu
    {

        DateTime today = DateTime.Today;
        DateTime yesterday = DateTime.Today.AddDays(-1);
        DateTime thisWeek = DateTime.Today.AddDays(7);

        //User
        private string json = string.Empty;
        public User user = new();

        //Past job observation
        public List<JobObservation> jobObservations = new();
        public List<JobObservation> lateObservations = new();
        public List<JobObservation> todayObservations = new();
        public List<JobObservation> thisWeekObservations = new();



        protected async override Task OnInitializedAsync()
        {

            await LateDates();

            //user
            await GetUserAsync();

            if (user != null)
            {
                jobObservations = await JobObservationService.GetAllJobObservations();

                foreach(var jobobs in jobObservations)
                {
                    if(jobobs.Supervisor.Name == user.Name)
                    {
                        //yesterday
                        if (Convert.ToDateTime(yesterday.ToShortDateString()).Date >= Convert.ToDateTime(jobobs.DateStart?.ToShortDateString()).Date && Convert.ToDateTime(yesterday.ToShortDateString()).Date >= Convert.ToDateTime(jobobs.DateEnd?.ToShortDateString()).Date)
                        {
                            lateObservations.Add(jobobs);

                            lateObservations = lateObservations.OrderBy(x => x.DateStart).ToList();

                        }

                        if (Convert.ToDateTime(jobobs.DateStart?.ToShortDateString()).Date >= Convert.ToDateTime(today.ToShortDateString()).Date && Convert.ToDateTime(today.ToShortDateString()).Date <= Convert.ToDateTime(jobobs.DateEnd?.ToShortDateString()).Date 
                            && Convert.ToDateTime(jobobs.DateStart?.ToShortDateString()).Date <= Convert.ToDateTime(thisWeek.ToShortDateString()).Date)
                        {

                            //today
                            if (Convert.ToDateTime(today.ToShortDateString()).Date == Convert.ToDateTime(jobobs.DateStart?.ToShortDateString()).Date && Convert.ToDateTime(today.ToShortDateString()).Date == Convert.ToDateTime(jobobs.DateEnd?.ToShortDateString()).Date)
                            {
                                todayObservations.Add(jobobs);
                            }
                            //this week
                            else
                            {
                                thisWeekObservations.Add(jobobs);
                                thisWeekObservations = thisWeekObservations.OrderBy(x => x.DateStart).ToList();

                            }



                        }
                    }
                }

            }

        }

        public async Task Refresh()
        {
            await GetUserAsync();
            lateObservations.Clear();
            todayObservations.Clear();
            thisWeekObservations.Clear();

            if (user != null)
            {
                jobObservations = await JobObservationService.GetAllJobObservations();

                foreach (var jobobs in jobObservations)
                {
                    if (jobobs.Supervisor.Name == user.Name)
                    {
                        //yesterday
                        if (Convert.ToDateTime(yesterday.ToShortDateString()).Date >= Convert.ToDateTime(jobobs.DateStart?.ToShortDateString()).Date && Convert.ToDateTime(yesterday.ToShortDateString()).Date >= Convert.ToDateTime(jobobs.DateEnd?.ToShortDateString()).Date)
                        {
                            lateObservations.Add(jobobs);

                            lateObservations = lateObservations.OrderBy(x => x.DateStart).ToList();

                        }

                        if (Convert.ToDateTime(jobobs.DateStart?.ToShortDateString()).Date >= Convert.ToDateTime(today.ToShortDateString()).Date && Convert.ToDateTime(today.ToShortDateString()).Date <= Convert.ToDateTime(jobobs.DateEnd?.ToShortDateString()).Date
                            && Convert.ToDateTime(jobobs.DateStart?.ToShortDateString()).Date <= Convert.ToDateTime(thisWeek.ToShortDateString()).Date)
                        {

                            //today
                            if (Convert.ToDateTime(today.ToShortDateString()).Date == Convert.ToDateTime(jobobs.DateStart?.ToShortDateString()).Date && Convert.ToDateTime(today.ToShortDateString()).Date == Convert.ToDateTime(jobobs.DateEnd?.ToShortDateString()).Date)
                            {
                                todayObservations.Add(jobobs);
                            }
                            //this week
                            else
                            {
                                thisWeekObservations.Add(jobobs);
                                thisWeekObservations = thisWeekObservations.OrderBy(x => x.DateStart).ToList();

                            }



                        }
                    }
                }

            }
        }
        public async Task LateDates()
        {
            jobObservations = await JobObservationService.GetAllJobObservations();
            foreach (var jobobs in jobObservations)
            {
                if (Convert.ToDateTime(jobobs.DateEnd?.ToShortDateString()).Date < DateTime.Today && jobobs.Status != 4)
                {
                    jobobs.Status = 3;
                    await JobObservationService.UpdateJobObservation(jobobs);
                }
            }
        }

        //Local storage user
        private async Task GetUserAsync()
        {
            if (!await TryGetAsync())
                user = new();
        }

        private async Task<bool> TryGetAsync()
        {
            bool hasProperty = await HasPropertyAsync();
            if (hasProperty)
            {
                json = await js.InvokeAsync<string>("localStorage.getItem", "user");
                user = JsonSerializer.Deserialize<User>(json) ?? new();
            }
            return hasProperty;
        }

        private async Task<bool> HasPropertyAsync()
            => await js.InvokeAsync<bool>("localStorage.hasOwnProperty", "user");


        void JobObservationUpdate(int jobObservationId)
        {
            NavigationManager.NavigateTo($"/");
            NavigationManager.NavigateTo($"jobobservation/updatejobobservation/{jobObservationId}");
        }

        private bool visible = false;
        private int jobId;
        private void OpenDialog2(int id)
        {
            jobId = id;
            visible = true;
        }
        void Close() => visible = false;

        private DialogOptions dialogOptions = new() { CloseOnEscapeKey = true, MaxWidth = MaxWidth.Large, FullWidth = true };

    }
}
