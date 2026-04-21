namespace VeliOgretmenIletisim.Application.Interfaces.Notifications;

public interface INotificationDispatcher
{
    void ScheduleEmail(string to, string subject, string body);
    void ScheduleSms(string phoneNumber, string message);
}
