global using Microsoft.AspNetCore.Components;
global using SupervisorMobility.Client.Data.Entities;
global using SupervisorMobility.Client.Data.Entities.IS;
global using SupervisorMobility.Client.Data.Entities.CDMS;
global using SupervisorMobility.Client.Data.Entities.CDMS.Documents;
global using SupervisorMobility.Client.Data.Entities.CDMS.Folders;
global using SupervisorMobility.Client.Data.Entities.IS;
global using SupervisorMobility.Client.Services.AreaService;
global using SupervisorMobility.Client.Services.JobStructureService;
global using SupervisorMobility.Client.Services.DistributionService;
global using SupervisorMobility.Client.Services.HeadCountService;
global using SupervisorMobility.Client.Services.GroupService;
global using SupervisorMobility.Client.Services.PillarService;
global using SupervisorMobility.Client.Services.GlobalDataService;
global using SupervisorMobility.Client.Services.SignatureImageService;
global using SupervisorMobility.Client.Services.GlosaryService;
global using SupervisorMobility.Client.Services.JobObservationTypeService;
global using SupervisorMobility.Client.Services.JobObservationService;
global using SupervisorMobility.Client.Services.LoginService;
global using SupervisorMobility.Client.Services.LupService;
global using SupervisorMobility.Client.Services.KaizenService;
global using SupervisorMobility.Client.Services.ChecklistAnswerService;
global using SupervisorMobility.Client.Services.TreeServices;
global using SupervisorMobility.Client.Services.OperationService;
global using SupervisorMobility.Client.Services.PlantService;
global using SupervisorMobility.Client.Services.GuideService;
global using SupervisorMobility.Client.Services.ProductsService;
global using SupervisorMobility.Client.Services.QuestionTypeService;
global using SupervisorMobility.Client.Services.SupportDocumentTypeService;
global using SupervisorMobility.Client.Services.AssyChartService;
global using SupervisorMobility.Client.Services.FileUploadAndDownloadService;
global using SupervisorMobility.Client.Services.UserService;
global using SupervisorMobility.Client.Services.PATService;
global using SupervisorMobility.Client.Services.BridgeCDMSService;
global using SupervisorMobility.Client.Services.NotificationService;
global using SupervisorMobility.Client.Services.AttendanceService;
global using SupervisorMobility.Client.Services.ILUService;
global using SupervisorMobility.Client.Services.SOSReviewService;
global using SupervisorMobility.Client.Services.DepartmentService;
global using SupervisorMobility.Client.Services.BreadcrumsService;
global using SupervisorMobility.Client.Services.HCIService;
global using SupervisorMobility.Client.Services.SOS_Data_Service;
global using SupervisorMobility.Client.Services.IS_Services.AppearanceService;
global using SupervisorMobility.Client.Services.IS_Services.LogbookAppearanceService;
global using System.Text.Json;
global using SupervisorMobility.Client.Services.IS_Services.DataPanelService;
global using SupervisorMobility.Client.Services.IS_Services.PartService;
global using SupervisorMobility.Client.Services.IS_Services.ProblemDefectService;
global using SupervisorMobility.Client.Services.IS_Services.CheckpointService;

global using SupervisorMobility.Client.Services.TestServices;

global using SupervisorMobility.Client.Services.SOS_Services.SOSHubService;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;
using SupervisorMobility.Client;
using DocumentFormat.OpenXml.Spreadsheet;
using AutoMapper;
using Blazored.LocalStorage;
using Blazored.SessionStorage;
using System.ComponentModel;
using System.Net.Http;
using SupervisorMobility.Client.Services.TestServices;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Services
builder.Services.AddLocalization();
builder.Services.AddAutoMapper(typeof(AutoMapperProfiles));
builder.Services.AddBlazoredLocalStorage();
builder.Services.AddBlazoredSessionStorage();
builder.Services.AddMudServices();
builder.Services.AddSingleton<GlobalDataService>();
builder.Services.AddSingleton<SignatureImageService>();
builder.Services.AddScoped<IBreadcrumbService, BreadcrumbService>();
builder.Services.AddScoped<ITreeService, TreeService>();
builder.Services.AddScoped<IPlantService, PlantService>();
builder.Services.AddScoped<IHeadCountService, HeadCountService>();
builder.Services.AddScoped<IPATService, PATService>();
builder.Services.AddScoped<IILUService, ILUService>();
builder.Services.AddScoped<IGuideService, GuideService>();
builder.Services.AddScoped<IAreaService, AreaService>();
builder.Services.AddScoped<IAssyChartService, AssyChartService>();
builder.Services.AddScoped<IJobStructureService, JobStructureService>();
builder.Services.AddScoped<IQuestionTypeService, QuestionTypeService>();
builder.Services.AddScoped<IOperationService, OperationService>();
builder.Services.AddScoped<ISupportDocumentTypeService, SupportDocumentTypeService>();
builder.Services.AddScoped<IDistributionService, DistributionService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IGroupService, GroupService>();
builder.Services.AddScoped<IPillarService, PillarService>();
builder.Services.AddScoped<IDepartmentService, DepartmentService>();
builder.Services.AddScoped<IGlosaryService, GlosaryService>();
builder.Services.AddScoped<IJobObservationTypeService, JobObservationTypeService>();
builder.Services.AddScoped<IJobObservationService, JobObservationService>();
builder.Services.AddScoped<ILupService, LupService>();
builder.Services.AddScoped<IChecklistAnswerService, ChecklistAnswerService>();
builder.Services.AddScoped<ILoginService, LoginService>();
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<IAttendanceService, AttendanceService>();
builder.Services.AddScoped<IBridgeCDMSService, BridgeCDMSService>();
builder.Services.AddScoped<ISOSReviewService, SOSReviewService>();
builder.Services.AddScoped<ISOSDataService, SOSDataService>();
builder.Services.AddScoped<IFileUploadAndDownloadService, FileUploadAndDownloadService>();
builder.Services.AddScoped<IKaizenService, KaizenService>();
builder.Services.AddScoped<IHCIService, HCIService>();
builder.Services.AddScoped<IAppearanceService, AppearanceService>();
builder.Services.AddScoped<ILogbookAppearanceService, LogbookAppearanceService>();
builder.Services.AddScoped<IPartServices, PartServices>();

//For testing video uploads
builder.Services.AddScoped<ITestService, TestService>();

//IS Apariencia Plantilla
builder.Services.AddScoped<IDataPanelService, DataPanelService>();
builder.Services.AddScoped<IPartServices, PartServices>();
builder.Services.AddScoped<IProblemDefectServices, ProblemDefectServices>();
builder.Services.AddScoped<ICheckPointService, CheckpointService>();

//Sos services
builder.Services.AddScoped<ISOSHubService, SOSHubService>();


// Connection to API
var env = builder.HostEnvironment;
if (env.IsDevelopment())
{
    builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("https://localhost:10201/api/"), Timeout = TimeSpan.FromMinutes(15) }); ;
}
else
{
    builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("https://10.91.117.12:10203/api/"), Timeout = TimeSpan.FromMinutes(10) });
}



//Active Directory
builder.Services.AddMsalAuthentication(options =>
{
    builder.Configuration.Bind("AzureAd", options.ProviderOptions.Authentication);

    options.ProviderOptions.LoginMode = "login";
    options.ProviderOptions.Cache.CacheLocation = "sessionStorage";
});




await builder.Build().RunAsync();

public class AutoMapperProfiles : Profile
{
    public AutoMapperProfiles()
    {
        CreateMap<ChecklistAnswer, ChecklistAnswerDto>().ReverseMap();
        CreateMap<ChecklistAnswerDto, ChecklistAnswer>().ReverseMap();

        CreateMap<int?, int>().ConvertUsing<IntTypeConverter>();
        CreateMap<DateTime?, DateTime>().ConvertUsing<DateTimeTypeConverter>();


        CreateMap<JobObservation, JobObservationNulls>()
            .ForMember(dest => dest.PlantId, opt => opt.MapFrom(src => src.PlantId != 0 ? src.PlantId : (int?)null))
            .ForMember(dest => dest.Plant, opt => opt.MapFrom(src => src.Plant != null ? src.Plant : null))
            .ForMember(dest => dest.AreaId, opt => opt.MapFrom(src => src.AreaId != 0 ? (int?)src.AreaId : null))
            .ForMember(dest => dest.Area, opt => opt.MapFrom(src => src.Area != null ? src.Area : null))
            .ForMember(dest => dest.DistributionId, opt => opt.MapFrom(src => src.DistributionId != 0 ? (int?)src.DistributionId : null))
            .ForMember(dest => dest.Distribution, opt => opt.MapFrom(src => src.Distribution != null ? src.Distribution : null))
            .ForMember(dest => dest.OperationId, opt => opt.MapFrom(src => src.OperationId != 0 ? (int?)src.OperationId : null))
            .ForMember(dest => dest.Operation, opt => opt.MapFrom(src => src.Operation != null ? src.Operation : null))
            .ForMember(dest => dest.SupervisorId, opt => opt.MapFrom(src => src.SupervisorId != 0 ? (int?)src.SupervisorId : null))
            .ForMember(dest => dest.Supervisor, opt => opt.MapFrom(src => src.Supervisor != null ? src.Supervisor : null))
            .ForMember(dest => dest.OperatorId, opt => opt.MapFrom(src => src.OperatorId != 0 ? (int?)src.OperatorId : null))
            .ForMember(dest => dest.Operator, opt => opt.MapFrom(src => src.Operator != null ? src.Operator : null));


        CreateMap<JobObservationNulls, JobObservation>()
            .ForMember(dest => dest.PlantId, opt => opt.MapFrom(src => src.PlantId != null ? src.PlantId : 0))
            .ForMember(dest => dest.Plant, opt => opt.MapFrom(src => src.Plant != null ? src.Plant : new Plant()))
            .ForMember(dest => dest.AreaId, opt => opt.MapFrom(src => src.AreaId != null ? (int?)src.AreaId : 0))
            .ForMember(dest => dest.Area, opt => opt.MapFrom(src => src.Area != null ? src.Area : new Area()))
            .ForMember(dest => dest.DistributionId, opt => opt.MapFrom(src => src.DistributionId != null ? (int?)src.DistributionId : 0))
            .ForMember(dest => dest.Distribution, opt => opt.MapFrom(src => src.Distribution != null ? src.Distribution : new Distribution()))
            .ForMember(dest => dest.OperationId, opt => opt.MapFrom(src => src.OperationId != null ? (int?)src.OperationId : 0))
            .ForMember(dest => dest.Operation, opt => opt.MapFrom(src => src.Operation != null ? src.Operation : new Operation()))
            .ForMember(dest => dest.SupervisorId, opt => opt.MapFrom(src => src.SupervisorId != null ? (int?)src.SupervisorId : 0))
            .ForMember(dest => dest.Supervisor, opt => opt.MapFrom(src => src.Supervisor != null ? src.Supervisor : new User()))
            .ForMember(dest => dest.OperatorId, opt => opt.MapFrom(src => src.OperatorId != null ? (int?)src.OperatorId : 0))
            .ForMember(dest => dest.Operator, opt => opt.MapFrom(src => src.Operator != null ? src.Operator : new User()));


    }
}


public class IntTypeConverter : ITypeConverter<int?, int>
{
    public int Convert(int? source, int destination, ResolutionContext context)
    {
        return source.HasValue ? source.Value : destination;
    }
}
public class DateTimeTypeConverter : ITypeConverter<DateTime?, DateTime>
{
    public DateTime Convert(DateTime? source, DateTime destination, ResolutionContext context)
    {
        return source.HasValue ? source.Value : destination;
    }
}

