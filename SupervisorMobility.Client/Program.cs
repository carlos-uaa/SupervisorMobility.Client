global using Microsoft.AspNetCore.Components;
global using SupervisorMobility.Client.Data.Entities;
global using SupervisorMobility.Client.Data.Entities.CDMS;
global using SupervisorMobility.Client.Data.Entities.CDMS.Documents;
global using SupervisorMobility.Client.Data.Entities.CDMS.Folders;
global using SupervisorMobility.Client.Services.AreaService;
global using SupervisorMobility.Client.Services.ChecklistService;
global using SupervisorMobility.Client.Services.DistributionService;
global using SupervisorMobility.Client.Services.GroupService;
global using SupervisorMobility.Client.Services.GlobalDataService;
global using SupervisorMobility.Client.Services.GlosaryService;
global using SupervisorMobility.Client.Services.JobObservationTypeService;
global using SupervisorMobility.Client.Services.JobObservationService;
global using SupervisorMobility.Client.Services.LoginService;
global using SupervisorMobility.Client.Services.LupService;
global using SupervisorMobility.Client.Services.OperationService;
global using SupervisorMobility.Client.Services.PlantService;
global using SupervisorMobility.Client.Services.GuideService;
global using SupervisorMobility.Client.Services.ProductsService;
global using SupervisorMobility.Client.Services.QuestionTypeService;
global using SupervisorMobility.Client.Services.SupportDocumentTypeService;
global using SupervisorMobility.Client.Services.AssyChartService;
global using SupervisorMobility.Client.Services.FileUploadAndDownloadService;
global using SupervisorMobility.Client.Services.UserService;
global using SupervisorMobility.Client.Services.BridgeCDMSService;
global using SupervisorMobility.Client.Services.NotificationService;
global using SupervisorMobility.Client.Services.AttendanceService;
global using System.Text.Json;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;
using SupervisorMobility.Client;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using SupervisorMobility.Client.Services.GlobalDataService;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Services
builder.Services.AddMudServices();
builder.Services.AddSingleton<GlobalDataService>();
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
builder.Services.AddScoped<ILupService, LupService>();
builder.Services.AddScoped<ILoginService, LoginService>();
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<IAttendanceService, AttendanceService>();
builder.Services.AddScoped<IBridgeCDMSService, BridgeCDMSService>();
builder.Services.AddScoped<IFileUploadAndDownloadService, FileUploadAndDownloadService>();


// Connection to API
builder.Services.AddScoped<CustomHttpClientService>();

//builder.Services.AddCors(policy => {
//    policy.AddPolicy("Cors", builder =>
//        builder
//            .WithOrigins("https://localhost:44398")
//            .SetIsOriginAllowedToAllowWildcardSubdomains()
//            .AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
//});



//Active Directory
builder.Services.AddMsalAuthentication(options =>
{
    builder.Configuration.Bind("AzureAd", options.ProviderOptions.Authentication);

    options.ProviderOptions.LoginMode = "login";
    options.ProviderOptions.Cache.CacheLocation = "sessionStorage";
});



await builder.Build().RunAsync();

public class CustomHttpClientService
{
    private readonly HttpClient _apiHttpClient;
    private readonly HttpClient _bridgeHttpClient;
    private readonly HttpClient _ADHttpClient;

    public CustomHttpClientService()
    {
        _apiHttpClient = new HttpClient { BaseAddress = new Uri("http://localhost:10201/api/") };
        //_bridgeHttpClient = new HttpClient { BaseAddress = new Uri("http://10.91.49.2:3000/") };
        //_ADHttpClient = new HttpClient { BaseAddress = new Uri("http://10.91.49.9:4251/") };
        //_apiHttpClient = new HttpClient { BaseAddress = new Uri("http://10.91.117.12:10201/api/") };
        _bridgeHttpClient = new HttpClient { BaseAddress = new Uri("http://10.91.117.5:3000/") };
        _ADHttpClient = new HttpClient { BaseAddress = new Uri("http://10.91.116.212:4251/") };
    }

    public HttpClient GetADHttpClient()
    {
        return _ADHttpClient;
    }
        public HttpClient GetApiHttpClient()
    {
        return _apiHttpClient;
    }

    public HttpClient GetBridgeHttpClient()
    {
        return _bridgeHttpClient;
    }
}
