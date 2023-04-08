namespace SupervisorMobility.Client.Services.NotificationService
{
    public interface INotificationService
    {
        Task<List<Notification>> GetAllNotifications();

    }
}
