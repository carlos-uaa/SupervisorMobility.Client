global using Microsoft.AspNetCore.Components;
global using SupervisorMobility.Client.Data.Entities;
global using SupervisorMobility.Client.Services.AreaService;
global using SupervisorMobility.Client.Services.ChecklistService;
global using SupervisorMobility.Client.Services.DistributionService;
global using SupervisorMobility.Client.Services.GroupService;
global using SupervisorMobility.Client.Services.GlosaryService;
global using SupervisorMobility.Client.Services.JobObservationTypeService;
global using SupervisorMobility.Client.Services.JobObservationService;
global using SupervisorMobility.Client.Services.OperationService;
global using SupervisorMobility.Client.Services.PlantService;
global using SupervisorMobility.Client.Services.GuideService;
global using SupervisorMobility.Client.Services.ProductsService;
global using SupervisorMobility.Client.Services.QuestionTypeService;
global using SupervisorMobility.Client.Services.SupportDocumentTypeService;
global using SupervisorMobility.Client.Services.AssyChartService;
global using SupervisorMobility.Client.Services.FileUploadAndDownloadService;
global using SupervisorMobility.Client.Services.UserService;
global using System.Text.Json;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;
using SupervisorMobility.Client;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Connection to API
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("https://localhost:7017/api/") });

// Services
builder.Services.AddMudServices();
builder.Services.AddScoped<IPlantService, PlantService>();
builder.Services.AddScoped<IGuideService, GuideService>();
builder.Services.AddScoped<IAreaService, AreaService>();
builder.Services.AddScoped<IAssyChartService, AssyChartService>();
builder.Services.AddScoped<IChecklistService, ChecklistService>();
builder.Services.AddScoped<IQuestionTypeService, QuestionTypeService>();
builder.Services.AddScoped<IOperationService, OperationService>();
builder.Services.AddScoped<ISupportDocumentTypeService, SupportDocumentTypeService>();
builder.Services.AddScoped<IDistributionService, DistributionService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IGroupService, GroupService>();
builder.Services.AddScoped<IGlosaryService, GlosaryService>();
builder.Services.AddScoped<IJobObservationTypeService, JobObservationTypeService>();
builder.Services.AddScoped<IJobObservationService, JobObservationService>();
builder.Services.AddScoped<IFileUploadAndDownloadService, FileUploadAndDownloadService>();
//cors


await builder.Build().RunAsync();
