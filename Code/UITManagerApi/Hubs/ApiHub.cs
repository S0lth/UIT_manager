using Microsoft.AspNetCore.SignalR;

namespace UITManagerApi.Hubs ;


public class ApiHub : Hub {
    /// <summary>
    /// Sends a message to all connected SignalR clients.
    /// </summary>
    public async Task Send(int message) {
        await Clients.All.SendAsync("ReceiveMessage", message);
    }
}