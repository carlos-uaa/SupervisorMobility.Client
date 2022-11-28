global using Microsoft.AspNetCore.Components;
global using SupervisorMobility.Client.Data.Entities;
global using SupervisorMobility.Client.Services.AreaService;
global using SupervisorMobility.Client.Services.ChecklistService;
global using SupervisorMobility.Client.Services.DistributionService;
global using SupervisorMobility.Client.Services.GroupService;
global using SupervisorMobility.Client.Services.OperationService;
global using SupervisorMobility.Client.Services.PlantService;
global using SupervisorMobility.Client.Services.ProductsService;
global using SupervisorMobility.Client.Services.QuestionTypeService;
global using SupervisorMobility.Client.Services.SupportDocumentTypeService;
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
builder.Services.AddScoped<IQuestionTypeService, QuestionTypeService>();
builder.Services.AddScoped<IOperationService, OperationService>();
builder.Services.AddScoped<ISupportDocumentTypeService, SupportDocumentTypeService>();
builder.Services.AddScoped<IDistributionService, DistributionService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IGroupService, GroupService>();

await builder.Build().RunAsync();
