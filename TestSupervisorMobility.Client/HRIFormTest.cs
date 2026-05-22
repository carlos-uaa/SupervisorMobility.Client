using System.Reflection;
using System.Text.Json;
using Bunit;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor.Services;
using SupervisorMobility.Client.Data.Entities;
using SupervisorMobility.Client.Data.Entites.Dtos.HRICyclesDtos;
using SupervisorMobility.Client.Pages.Inicio.HRIMenu.HRIPage;
using SupervisorMobility.Client.Services.AreaService;
using SupervisorMobility.Client.Services.DepartmentService;
using SupervisorMobility.Client.Services.HRIServices;
using SupervisorMobility.Client.Services.PlantService;
using SupervisorMobility.Client.Services.UserService;

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
            ctx.Services.AddSingleton<IHRIService>(new FakeHRIService());
            ctx.Services.AddSingleton<IHRIItemsService>(new FakeHRIItemsService());
            ctx.Services.AddSingleton<IHRILinesService>(new FakeHRILinesService());
            ctx.Services.AddSingleton<IHRIDocksService>(new FakeHRIDocksService());
            ctx.Services.AddSingleton<IDepartmentService>(new FakeDepartmentService());
            ctx.Services.AddSingleton<IPlantService>(new FakePlantService());
            ctx.Services.AddSingleton<IAreaService>(new FakeAreaService());
            ctx.Services.AddSingleton<IHRIRevisionItemsService>(new FakeHRIRevisionItemsService());
            ctx.Services.AddSingleton<IUserService>(new FakeUserService());

            var cut = ctx.Render<HRIForm>();

            cut.WaitForAssertion(() =>
            {
                Assert.Contains("Creación de Hoja de Revisión Inicial", cut.Markup);
                Assert.Contains("Nombre de Item", cut.Markup);
                Assert.Contains("Número de Control", cut.Markup);
                Assert.Contains("Turnos", cut.Markup);
            });

            var controlNumberInput = cut.FindAll("input").First(input => string.Equals(input.GetAttribute("type"), "text", StringComparison.OrdinalIgnoreCase));
            controlNumberInput.Change("HRI-001");

            SetPrivateValue(cut.Instance, "NumeroCiclos", 5);
            InvokePrivateVoid(cut.Instance, "BuildCycles");

            var instance = cut.Instance;
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