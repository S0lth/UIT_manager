using Microsoft.AspNetCore.SignalR;

namespace UITManagerApi.Hubs ;


public class ApiHub : Hub {
    public async Task Send(int message) {
        await Clients.All.SendAsync("ReceiveMessage", message);
    }
}