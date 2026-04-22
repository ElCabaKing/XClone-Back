
namespace Application.Interfaces.Websockets;

public interface IUserWebsocket
{
    Task SendNotification(string userId, string message);
    Task SendNotificationToAll(string message);
}