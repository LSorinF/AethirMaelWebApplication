using Microsoft.AspNetCore.SignalR;

namespace AethirMaelWebApplication.Server.Hubs
{
    // Hub-ul este punctul de conexiune pentru WebSockets
    // Putem defini metode specifice aici, dar pentru push notifications server->client
    // este suficient sa mostenim clasa Hub.
    public class NotificationHub : Hub
    {
    }
}