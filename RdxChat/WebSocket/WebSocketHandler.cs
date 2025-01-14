using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using Domain.Dtos;
using Domain.Services;
using Fleck;
using Microsoft.AspNetCore.SignalR;
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

    public WebSocketHandler(IHubContext<ChatHub> hubContext, IMessageService messageService)
    {
        _hubContext = hubContext;
        _messageService = messageService;
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
                var shouldReload = false;
                var (messageToSaveIds, messagesToSend) = 
                    await _messageService.SynchronizeHistory(new ChatCredentialsDto
                    {
                        ReceiverId = synchronizationMessage.ReceiverId,
                        SenderId = synchronizationMessage.SenderId,
                        RequestSentToId = synchronizationMessage.RequestSentToId
                    }, synchronizationMessage.MessageHistory);
                if (messageToSaveIds.Length != 0 || messagesToSend.Length != 0)
                {
                    await SendMessage(JsonSerializer.Serialize(new HistoryUpdateMessageDto
                    {
                        ReceiverId = synchronizationMessage.ReceiverId,
                        SenderId = synchronizationMessage.SenderId,
                        MessagesToSave = messagesToSend.Select(x => JsonSerializer.Serialize(x)).ToList(),
                        MessageToSendIds = messageToSaveIds.ToList(),
                        RequestSentToId = synchronizationMessage.SenderId
                    }));
                    
                    // await SendMessage(JsonSerializer.Serialize(new SynchronizationMessageDto
                    // {
                    //     ReceiverId = synchronizationMessage.ReceiverId,
                    //     SenderId = synchronizationMessage.SenderId,
                    //     RequestSentToId = synchronizationMessage.RequestSentToId == synchronizationMessage.ReceiverId 
                    //         ? synchronizationMessage.SenderId
                    //         : synchronizationMessage.ReceiverId,
                    //     MessageHistory = messageToSaveIds.ToList()
                    // }));
                    
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

                var messages = await _messageService.GetChatMessages(new ChatCredentialsDto
                {
                    ReceiverId = historyUpdateMessage.ReceiverId,
                    SenderId = historyUpdateMessage.SenderId,
                    RequestSentToId = historyUpdateMessage.RequestSentToId,
                });
                var messagesToSendLocal = messages
                    .Where(x => historyUpdateMessage.MessageToSendIds.Contains(x.MessageId))
                    .ToList();
                if (messagesToSendLocal.Count == 0)
                {
                    break;
                }
                await SendMessage(JsonSerializer.Serialize(new HistoryUpdateMessageDto
                {
                    MessagesToSave = messagesToSendLocal.Select(x => JsonSerializer.Serialize(x)).ToList(),
                    MessageToSendIds = new List<Guid>(),
                    ReceiverId = historyUpdateMessage.ReceiverId,
                    SenderId = historyUpdateMessage.SenderId,
                    RequestSentToId = historyUpdateMessage.RequestSentToId == historyUpdateMessage.ReceiverId 
                        ? historyUpdateMessage.SenderId 
                        : historyUpdateMessage.ReceiverId
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
            return JsonSerializer.Deserialize<HistoryUpdateMessageDto>(message)!;
        if (message.StartsWith("{\"MessageType\":\"Synchronization\""))
            return JsonSerializer.Deserialize<SynchronizationMessageDto>(message)!;
        if (message.StartsWith("{\"MessageType\":\"ChatMessage\""))
            return JsonSerializer.Deserialize<ChatMessageDto>(message)!;
        if (message.StartsWith("{\"MessageType\":\"UserRename\""))
            return JsonSerializer.Deserialize<UserRenameMessageDto>(message)!;

        throw new InvalidOperationException();
    }
}