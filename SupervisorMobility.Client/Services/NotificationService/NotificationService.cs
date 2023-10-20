using Microsoft.JSInterop;
using System.Net.Http.Json;

namespace SupervisorMobility.Client.Services.NotificationService
{
    public class NotificationService: INotificationService
    {
        private readonly HttpClient _http;
        private readonly JsonSerializerOptions _options;
        private readonly IJSRuntime _js;

        // Constructor
        public NotificationService(HttpClient customHttpClientService, IJSRuntime jSRuntime)
        {
            _http = customHttpClientService;
            _js = jSRuntime;
            _options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        }

        public async Task<List<Notification>> GetAllNotifications()
        {
            
            var response = await _http.GetAsync($"notifications");
                Console.WriteLine($"on content");
                var content = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"under content");


                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"throw content bad code");

                    throw new ApplicationException(content);
                }

                var notifications = JsonSerializer.Deserialize<List<Notification>>(content, _options);
                return notifications;

          
           

        } 
        public async Task<List<Notification>> GetAllNotificationsFromUser(int userid)
        {
            
              var response = await _http.GetAsync($"notifications/{userid}");
                var content = await response.Content.ReadAsStringAsync();
                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"GetAllNotificationsFromUser - throw content bad code");

                    throw new ApplicationException(content);
                }

                var notifications = JsonSerializer.Deserialize<List<Notification>>(content, _options);
                return notifications;

        }

        public async Task<bool> DeleteNotification(int notifyId)
        {
            var response = await _http.DeleteAsync($"notifications/delete/{notifyId}");
            return response.IsSuccessStatusCode;
        }


        public async Task<Notification> ReadNotification(int notifyId, Notification notify)
        {

            var response = await _http.PutAsJsonAsync($"notifications/read/{notifyId}", notify);

            var notification = await response.Content.ReadFromJsonAsync<Notification>();
            return notification;
        }

    }
}
