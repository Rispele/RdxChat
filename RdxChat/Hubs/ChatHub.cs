using System.Text.Json;
using Domain.Dtos;
using Microsoft.AspNetCore.SignalR;
using Rdx.Serialization;

namespace RdxChat.Hubs;

public class ChatHub : Hub
{
    private readonly RdxSerializer rdxSerializer;

    public ChatHub(RdxSerializer rdxSerializer)
    {
        this.rdxSerializer = rdxSerializer;
    }

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

    public async Task SendMessage(string message, Guid receiverId, Guid senderId)
    {
        var dto = new ChatMessageDto
        {
            Message = message,
            ReceiverId = receiverId,
            SenderId = senderId,
            SendingTime = DateTime.Now
        };
        await Clients.Others.SendAsync("NotifyMessage", rdxSerializer.Serialize(dto));
    }
}