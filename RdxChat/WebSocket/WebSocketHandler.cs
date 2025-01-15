using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using Domain.Dtos;
using Domain.Services;
using Fleck;
using Microsoft.AspNetCore.SignalR;
using Rdx.Serialization;
using RdxChat.Converters;
using RdxChat.Entities;
using RdxChat.Hubs;

namespace RdxChat.WebSocket;

public class WebSocketHandler : IWebSocketHandler
{
    public WebSocketServer Server { get; private set; }
    public ClientWebSocket Client { get; } = new();
    private readonly IHubContext<ChatHub> _hubContext;
    private readonly IMessageService _messageService;
    private readonly RdxSerializer rdxSerializer;

    public WebSocketHandler(IHubContext<ChatHub> hubContext, IMessageService messageService, RdxSerializer rdxSerializer)
    {
        _hubContext = hubContext;
        _messageService = messageService;
        this.rdxSerializer = rdxSerializer;
    }

    public async Task ConnectAsync()
    {
        var uri = "ws://127.0.0.1:20002";
        
        Server = new WebSocketServer(uri);
        var clients = new List<IWebSocketConnection>();
        Server.Start(socket =>
        {
            socket.OnOpen = () =>
            {
                Console.WriteLine("open");
                clients.Add(socket);
            };
            socket.OnClose = () =>
            {
                Console.WriteLine("close");
                clients.Remove(socket);
            };
            socket.OnMessage = SocketOnMessage;
        });
        await Client.ConnectAsync(new Uri("ws://127.0.0.1:20002"), new CancellationToken());
    }

    private async void SocketOnMessage(string message)
    {
        Console.WriteLine("message: " + message);

        var deserializedMessage = TryDeserializeMessage(message);
        switch (deserializedMessage)
        {
            case (SynchronizationMessageDto synchronizationMessage):
                var (messageToSaveIds, messagesToSend) = 
                    await _messageService.SynchronizeHistory(
                        synchronizationMessage.RequestSentToId,
                        synchronizationMessage.MessageHistory);
                if (messageToSaveIds.Length != 0 || messagesToSend.Length != 0)
                {
                    await SendMessage(rdxSerializer.Serialize(new HistoryUpdateMessageDto
                    {
                        MessagesToSave = messagesToSend.Select(x => rdxSerializer.Serialize(x)).ToList(),
                        MessageToSendIds = messageToSaveIds.ToList(),
                        RequestSentFromId = synchronizationMessage.RequestSentToId,
                        RequestSentToId = synchronizationMessage.RequestSentFromId
                    }));
                    
                    await _hubContext.Clients.All.SendAsync("NotifyPageReload");
                }
                
                break;
            case (ChatMessageDto chatMessage):
                await _hubContext.Clients.All.SendAsync("NotifyMessage", message);
                break;
            case (UserRenameMessageDto userRenameMessage):
                break;
            case (HistoryUpdateMessageDto historyUpdateMessage):
                foreach (var toSave in historyUpdateMessage.MessagesToSave)
                {
                    await _messageService.SaveMessageAsync(TryDeserializeMessage(toSave), historyUpdateMessage.RequestSentToId.ToString());
                }

                var messages = await _messageService.GetChatMessages(historyUpdateMessage.RequestSentToId);
                var messagesToSendLocal = messages
                    .Where(x => historyUpdateMessage.MessageToSendIds.Contains(x.MessageId))
                    .ToList();
                if (messagesToSendLocal.Count == 0)
                {
                    break;
                }
                await SendMessage(rdxSerializer.Serialize(new HistoryUpdateMessageDto
                {
                    MessagesToSave = messagesToSendLocal.Select(x => rdxSerializer.Serialize(x)).ToList(),
                    MessageToSendIds = new List<Guid>(),
                    RequestSentFromId = historyUpdateMessage.RequestSentToId,
                    RequestSentToId = historyUpdateMessage.RequestSentFromId
                }));
                await _hubContext.Clients.All.SendAsync("NotifyPageReload");
                
                break;
        }
    }

    public async Task SendMessage(string message)
    {
        if (Client.State != WebSocketState.Open)
        {
            await ConnectAsync();
        }
        await Client.SendAsync(EncodeMessage(message),
            WebSocketMessageType.Text, true, new CancellationToken());
    }

    private ArraySegment<byte> EncodeMessage(string message)
    {
        return new ArraySegment<byte>(Encoding.UTF8.GetBytes(message));
    }

    private AbstractMessageDto TryDeserializeMessage(string message)
    {
        if (message.StartsWith("{\"MessageType\":\"HistoryUpdate\""))
            return rdxSerializer.Deserialize<HistoryUpdateMessageDto>(message)!;
        if (message.StartsWith("{\"MessageType\":\"Synchronization\""))
            return rdxSerializer.Deserialize<SynchronizationMessageDto>(message)!;
        if (message.StartsWith("{\"MessageType\":\"ChatMessage\""))
            return rdxSerializer.Deserialize<ChatMessageDto>(message)!;
        if (message.StartsWith("{\"MessageType\":\"UserRename\""))
            return rdxSerializer.Deserialize<UserRenameMessageDto>(message)!;

        throw new InvalidOperationException();
    }
}