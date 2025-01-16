using Microsoft.AspNetCore.SignalR;

namespace RdxChat.Hubs;

public class ChatHub : Hub
{
    public override async Task OnConnectedAsync()
    {
        Console.WriteLine("Connected " + Context.ConnectionId);
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        Console.WriteLine("Disconnected " + Context.ConnectionId);
        Console.WriteLine(exception?.Message);
        await base.OnDisconnectedAsync(exception);
    }
}