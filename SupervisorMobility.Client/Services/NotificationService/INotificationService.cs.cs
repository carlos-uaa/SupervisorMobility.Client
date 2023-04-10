namespace SupervisorMobility.Client.Services.NotificationService
{
    public interface INotificationService
    {
        Task<List<Notification>> GetAllNotifications();
        Task<List<Notification>> GetAllNotificationsFromUser(int userid);

    }
}
