using Microsoft.JSInterop;

namespace SupervisorMobility.Client.Services.NotificationService
{
    public class NotificationService: INotificationService
    {
        private readonly HttpClient _http;
        private readonly HttpClient _httpBridge;
        private readonly JsonSerializerOptions _options;
        private readonly IJSRuntime _js;

        // Constructor
        public NotificationService(CustomHttpClientService customHttpClientService, IJSRuntime jSRuntime)
        {
            _http = customHttpClientService.GetApiHttpClient();
            _httpBridge = customHttpClientService.GetBridgeHttpClient();
            _js = jSRuntime;
            _options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        }

        public async Task<List<Notification>> GetAllNotifications()
        {
            var response = await _http.GetAsync($"notifications");
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new ApplicationException(content);
            }

            var notifications = JsonSerializer.Deserialize<List<Notification>>(content, _options);

            return notifications;
        }

    }
}
