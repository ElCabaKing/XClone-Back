using Application.Interfaces.Websockets;
using Microsoft.AspNetCore.SignalR;

namespace Infrastructure.Websockets;

public class UserWebsocket(
    IHubContext<NotificationHub> hub
    ) : IUserWebsocket
{
    private readonly IHubContext<NotificationHub> _hub = hub;

    public async Task SendNotification(
        string userId, 
        string message)
    {
        await _hub.Clients
            .User(userId)
            .SendAsync("ReceiveNotification", message);
    }

    public async Task SendNotificationToAll(
        string message)
    {
        await _hub.Clients
            .All
            .SendAsync("ReceiveNotification", message);
    }
}