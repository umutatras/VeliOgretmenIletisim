namespace VeliOgretmenIletisim.Application.Interfaces.Notifications;

public interface INotificationService
{
    Task SendToUserAsync(Guid userId, string message, string type = "Information");
    Task SendToAllAsync(string message, string type = "Information");
    Task SendToRoleAsync(string role, string message, string type = "Information");
}
