namespace SupervisorMobility.Client.Services.NotificationService
{
    public interface INotificationService
    {
        Task<List<Notification>> GetAllNotifications();
        Task<List<Notification>> GetAllNotificationsFromUser(int userid);
        Task<Notification> ReadNotification(int notifyId, Notification notify);
        Task<bool> DeleteNotification(int notifyId);
    }
}
