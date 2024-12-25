using Microsoft.AspNetCore.SignalR;

namespace UITManagerWebServer.Hubs ;


public class WebAppHub : Hub {
    /// <summary>
    /// Sends a message to all connected SignalR clients.
    /// </summary>
    public async Task Send(int message) {
        await Clients.All.SendAsync("ReceiveMessage", message);
    }
}