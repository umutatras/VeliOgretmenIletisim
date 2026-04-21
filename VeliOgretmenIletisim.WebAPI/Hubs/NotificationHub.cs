using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace VeliOgretmenIletisim.WebAPI.Hubs;

[Authorize]
public class NotificationHub : Hub
{
    public override async Task OnConnectedAsync()
    {
        var userId = Context.User?.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!string.IsNullOrEmpty(userId))
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, userId);
            
            var role = Context.User?.FindFirstValue(ClaimTypes.Role);
            if (!string.IsNullOrEmpty(role))
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, role);
            }
        }
        
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var userId = Context.User?.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!string.IsNullOrEmpty(userId))
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, userId);
            
            var role = Context.User?.FindFirstValue(ClaimTypes.Role);
            if (!string.IsNullOrEmpty(role))
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, role);
            }
        }
        
        await base.OnDisconnectedAsync(exception);
    }
}
