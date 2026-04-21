using Microsoft.AspNetCore.SignalR;
using VeliOgretmenIletisim.Application.Interfaces.Notifications;
using VeliOgretmenIletisim.Infrastructure.Hubs;

namespace VeliOgretmenIletisim.Infrastructure.Services;

public class SignalRNotificationService : INotificationService
{
    private readonly IHubContext<NotificationHub> _hubContext;

    public SignalRNotificationService(IHubContext<NotificationHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public async Task SendToUserAsync(Guid userId, string message, string type = "Information")
    {
        await _hubContext.Clients.Group(userId.ToString()).SendAsync("ReceiveNotification", new { message, type });
    }

    public async Task SendToAllAsync(string message, string type = "Information")
    {
        await _hubContext.Clients.All.SendAsync("ReceiveNotification", new { message, type });
    }

    public async Task SendToRoleAsync(string role, string message, string type = "Information")
    {
        await _hubContext.Clients.Group(role).SendAsync("ReceiveNotification", new { message, type });
    }
}
