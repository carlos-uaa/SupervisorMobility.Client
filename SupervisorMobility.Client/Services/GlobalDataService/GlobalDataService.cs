using Microsoft.JSInterop;
using System.Net.Http.Json;

namespace SupervisorMobility.Client.Services.GlobalDataService
{
    public class GlobalDataService
    {
        public string LoggedUser = "Log in";
        
        public event Action? OnNotificationsChanged;
        public void NotifyNotificationsChanged() => OnNotificationsChanged?.Invoke();
    }
}
