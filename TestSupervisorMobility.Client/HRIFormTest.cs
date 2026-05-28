using System.Reflection;
using System.Text.Json;
using Bunit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Components;
using MudBlazor.Services;
using MudBlazor;
using SupervisorMobility.Client.Data;
using SupervisorMobility.Client.Data.Entities;
using SupervisorMobility.Client.Data.Entites.Dtos.HRIDtos;
using SupervisorMobility.Client.Data.Entites.Dtos.HRICyclesDtos;
using SupervisorMobility.Client.Pages.Inicio.HRIMenu.HRIPage;
using SupervisorMobility.Client.Services.AreaService;
using SupervisorMobility.Client.Services.DepartmentService;
using SupervisorMobility.Client.Services.HRIServices;
using SupervisorMobility.Client.Services.PlantService;
using SupervisorMobility.Client.Services.UserService;
using SupervisorMobility.Client.Data.Entites.Dtos.HRIRevisionItemsDtos;

//// Services
using TestSupervisorMobility.Client.Services;

namespace TestSupervisorMobility.Client
{
    public class HRIFormTest
    {
        [Fact]
        public async Task HRIForm_Renders_And_AllowsFillingFields()
        {
            await using var ctx = new BunitContext();
            var fakeHRIService = new FakeHRIService();

            ctx.JSInterop.Mode = JSRuntimeMode.Loose;
            ctx.JSInterop.Setup<string>("localStorage.getItem", "user")
                .SetResult(JsonSerializer.Serialize(new User
                {
                    UserId = 99,
                    ObjectId = "test-user",
                    Name = "Test User",
                    UserType = 3,
                    IsActive = true
                }));
            ctx.JSInterop.SetupVoid("history.back");

            ctx.Services.AddMudServices();
            ctx.Services.AddSingleton<IDialogService>(new DialogService());
            ctx.Services.AddSingleton<IHRIService>(fakeHRIService);
            ctx.Services.AddSingleton<IHRIItemsService>(new FakeHRIItemsService());
            ctx.Services.AddSingleton<IHRILinesService>(new FakeHRILinesService());
            ctx.Services.AddSingleton<IHRIDocksService>(new FakeHRIDocksService());
            ctx.Services.AddSingleton<IDepartmentService>(new FakeDepartmentService());
            ctx.Services.AddSingleton<IPlantService>(new FakePlantService());
            ctx.Services.AddSingleton<IAreaService>(new FakeAreaService());
            ctx.Services.AddSingleton<IHRIRevisionItemsService>(new FakeHRIRevisionItemsService());
            ctx.Services.AddSingleton<IUserService>(new FakeUserService());

            var cut = ctx.Render(builder =>
            {
                builder.OpenComponent<MudDialogProvider>(0);
                builder.CloseComponent();

                builder.OpenComponent<HRIForm>(1);
                builder.CloseComponent();
            });

            var form = cut.FindComponent<HRIForm>();

            cut.WaitForAssertion(() =>
            {
                Assert.Contains("Creación de Hoja de Revisión Inicial", cut.Markup);
                Assert.Contains("Nombre de Item", cut.Markup);
                Assert.Contains("Número de Control", cut.Markup);
                Assert.Contains("Turnos", cut.Markup);
            });

            var instance = form.Instance;

            // Nombre del Item
            SetPrivateValue<int>(instance, "SelectedHRIItemId", 1);

            // Nombre de Línea
            SetPrivateValue<int>(instance, "SelectedHRILineId", 1);

            // Control Number
            var controlNumberInput = cut.FindAll("input").First(input => string.Equals(input.GetAttribute("id"), "controlNumber", StringComparison.OrdinalIgnoreCase));
            controlNumberInput.Change("HRI-001");

            // Dock Asignado
            SetPrivateValue<int>(instance, "SelectedHRIDockId", 1);

            // Planta
            SetPrivateValue<int>(instance, "SelectedPlantId", 1);

            // Área
            SetPrivateValue<int>(instance, "SelectedAreaId", 1);

            // Departamento
            SetPrivateValue<int>(instance, "SelectedDepartmentId", 1);

            // Supervisor
            SetPrivateValue<int>(instance, "SelectedSupervisorId", 1);

            // Senior Supervisor
            SetPrivateValue<int>(instance, "SelectedSeniorSupervisorId", 1);

            // Turnos
            SetPrivateValue<int>(instance, "NumeroCiclos", 5);
            InvokePrivateVoid(instance, "BuildCycles");

            var revisionItems = GetPrivateValue<List<CreateHRIRevisionItemDto>>(instance, "hriRevisionItems");
            revisionItems[0].RevisionPoint = "Punto de revisión 1";
            revisionItems[0].RevisionMethodId = 1;
            revisionItems[0].VeredictId = 1;
            revisionItems[0].FrequencyId = 1;

            SetPrivateValue(instance, "UploadedImages", new Dictionary<string, (string ContentType, string Uri)>
            {
                ["evidence.png"] = ("image/png", "data:image/png;base64,AAA=")
            });

              cut.FindAll("button").First(button => button.TextContent.Contains("Crear Documento", StringComparison.OrdinalIgnoreCase)).Click();

              //cut.WaitForAssertion(() => Assert.Contains("¿Estás seguro de que deseas crear el documento?", cut.Markup));

              cut.FindAll("button").First(button => button.TextContent.Contains("Guardar", StringComparison.OrdinalIgnoreCase)).Click();

              cut.WaitForAssertion(() => Assert.True(fakeHRIService.CreateHriCalled));

            Assert.NotNull(fakeHRIService.LastCreateDto);
            Assert.Equal(1, fakeHRIService.LastCreateDto!.HRIItemId);
            Assert.Equal(1, fakeHRIService.LastCreateDto.HRILinesId);
            Assert.Equal("HRI-001", fakeHRIService.LastCreateDto.ControlNumber);
            Assert.Equal(1, fakeHRIService.LastCreateDto.HRIDockId);
            Assert.Equal(1, fakeHRIService.LastCreateDto.PlantId);
            Assert.Equal(1, fakeHRIService.LastCreateDto.AreaId);
            Assert.Equal("Departamento de prueba", fakeHRIService.LastCreateDto.Department);
            Assert.Equal(1, fakeHRIService.LastCreateDto.SupervisorUserId);
            Assert.Equal(5, fakeHRIService.LastCreateDto.HriCycles.Count);

            Assert.Equal(1, GetPrivateValue<int>(instance, "SelectedHRIItemId"));
            Assert.Equal("HRI-001", GetPrivateValue<string>(instance, "controlNumber"));
            Assert.Equal(5, GetPrivateValue<int>(instance, "NumeroCiclos"));
            Assert.Equal(5, GetPrivateValue<List<CreateHRICyclesDto>>(instance, "hriCycles").Count);

            var renderedTextInputs = cut.FindAll("input")
                .Count(input => string.Equals(input.GetAttribute("type"), "text", StringComparison.OrdinalIgnoreCase));
            Assert.True(renderedTextInputs >= 2);
        }

        private static T GetPrivateValue<T>(object instance, string fieldName)
        {
            var type = instance.GetType();
            var property = type.GetProperty(fieldName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            if (property is not null)
            {
                return (T)property.GetValue(instance)!;
            }

            var field = type.GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);
            Assert.NotNull(field);
            return (T)field!.GetValue(instance)!;
        }

        private static void SetPrivateValue<T>(object instance, string propertyName, T value)
        {
            var property = instance.GetType().GetProperty(propertyName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            Assert.NotNull(property);
            property!.SetValue(instance, value);
        }

        private static void InvokePrivateVoid(object instance, string methodName)
        {
            var method = instance.GetType().GetMethod(methodName, BindingFlags.Instance | BindingFlags.NonPublic);
            Assert.NotNull(method);
            method!.Invoke(instance, Array.Empty<object>());
        }

    }
}