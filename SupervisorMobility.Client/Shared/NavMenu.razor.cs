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

namespace SupervisorMobility.Client.Shared
{
    public partial class NavMenu
    {

        DateTime today = DateTime.Today;
        DateTime yesterday = DateTime.Today.AddDays(-1);
        DateTime thisWeek = DateTime.Today.AddDays(7);

        //Past job observation
        public List<User> users;
        public User user;
        public List<JobObservation> jobObservations = new();
        public List<JobObservation> lateObservations = new();
        public List<JobObservation> todayObservations = new();
        public List<JobObservation> thisWeekObservations = new();

        protected async override Task OnInitializedAsync()
        {

            await LateDates();

            //job obseravtion
            users = await UsersService.GetUsers();
            user = users.FirstOrDefault()!;

            if(user != null)
            {
                jobObservations = await JobObservationService.GetAllJobObservations();

                foreach(var jobobs in jobObservations)
                {
                    if(jobobs.Observer == user.Name)
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

        void JobObservationUpdate(int jobObservationId)
        {
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

        void NavigateTo(int id)
        {
            
            NavigationManager.NavigateTo($"/");
            NavigationManager.NavigateTo($"jobobservation/updatejobobservation/{id}");
        }
    }
}
