global using Microsoft.AspNetCore.Components;
global using SupervisorMobility.Client.Data.Entities;
global using SupervisorMobility.Client.Services.AreaService;
global using SupervisorMobility.Client.Services.ChecklistService;
global using SupervisorMobility.Client.Services.PlantService;
global using System.Text.Json;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;
using SupervisorMobility.Client;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Connection to API
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("http://localhost:8080/api/") });

// Services
builder.Services.AddMudServices();
builder.Services.AddScoped<IPlantService, PlantService>();
builder.Services.AddScoped<IAreaService, AreaService>();
builder.Services.AddScoped<IChecklistService, ChecklistService>();

await builder.Build().RunAsync();
