using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using Domain.Entities;
using Domain.Services;
using Fleck;
using Microsoft.AspNetCore.SignalR;
using Rdx.Serialization;
using RdxChat.Hubs;
using Websocket.Client;

namespace RdxChat.WebSocket;

public class WebSocketHandler : IWebSocketHandler
{
    private WebSocketServer Server { get; set; } = null!;
    private ClientWebSocket Client { get; } = new();
    private readonly IHubContext<ChatHub> _hubContext;
    private readonly IMessageService _messageService;
    private readonly RdxSerializer _rdxSerializer;

    public WebSocketHandler(IHubContext<ChatHub> hubContext, IMessageService messageService, RdxSerializer rdxSerializer)
    {
        _hubContext = hubContext;
        _messageService = messageService;
        _rdxSerializer = rdxSerializer;
    }

    public async Task ConnectAsync()
    {
        var url = "ws://127.0.0.1:20002";
        
        // TODO: убрать, когда будет выделенный ws сервер

        #region server

        Server = new WebSocketServer(url);
        Server.Start(socket =>
        {
            socket.OnMessage = SocketOnMessage;
        });
        await Client.ConnectAsync(new Uri("ws://127.0.0.1:20002"), new CancellationToken());

        #endregion

        // TODO: раскомментить это после нахождения выделенного сервера
        
        #region clientConnection

        // using (var client = new WebsocketClient(new Uri(url)))
        // {
        //     client.MessageReceived.Subscribe(msg => SocketOnMessage(msg.Text!));
        //     await client.Start();
        // }

        #endregion
    }

    private async void SocketOnMessage(string message)
    {
        var deserializedMessage = TryDeserializeMessage(message);
        switch (deserializedMessage)
        {
            case (SynchronizationMessage synchronizationMessage):
                var credentials = _messageService.ReadCredentials();
                if (credentials is null)
                {
                    throw new InvalidOperationException();
                }

                // TODO: раскомментить, когда запуск будет происходить с разных машин.
                // Хранение в памяти данных клиентов невозможно, поскольку есть завязка на хранение в куках.
                
                // if (credentials.Value.userId != synchronizationMessage.RequestSentToId)
                // {
                //     return;
                // }

                var (messageToSaveIds, messagesToSend) = 
                     _messageService.SynchronizeHistory(
                        synchronizationMessage.RequestSentToId,
                        synchronizationMessage.MessageHistory);
                
                if (messageToSaveIds.Length != 0 || messagesToSend.Length != 0)
                {
                    await SendMessage(_rdxSerializer.Serialize(new HistoryUpdateMessage
                    {
                        MessagesToSave = messagesToSend.ToList(),
                        MessageToSendIds = messageToSaveIds.ToList(),
                        RequestSentFromId = synchronizationMessage.RequestSentToId,
                        RequestSentToId = synchronizationMessage.RequestSentFromId
                    }));
                    
                    await _hubContext.Clients.All.SendAsync("NotifyPageReload");
                }
                
                break;
            case (ChatMessage chatMessage):
                var clientSerializedMessage = JsonSerializer.Serialize(chatMessage);
                await _hubContext.Clients.All.SendAsync("NotifyMessage", clientSerializedMessage);
                break;
            case (HistoryUpdateMessage historyUpdateMessage):
                foreach (var toSave in historyUpdateMessage.MessagesToSave)
                {
                    _messageService.SaveMessage(TryDeserializeMessage(toSave), historyUpdateMessage.RequestSentToId.ToString());
                }

                var messages = _messageService.GetChatMessages(historyUpdateMessage.RequestSentToId);
                var messagesToSendLocal = messages
                    .Where(x => historyUpdateMessage.MessageToSendIds.Contains(x.MessageId))
                    .ToList();
                if (messagesToSendLocal.Count == 0)
                {
                    break;
                }
                await SendMessage(_rdxSerializer.Serialize(new HistoryUpdateMessage
                {
                    MessagesToSave = messagesToSendLocal.Select(x => _rdxSerializer.Serialize(x)).ToList(),
                    MessageToSendIds = [],
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

    private AbstractMessage TryDeserializeMessage(string message)
    {
        if (message.StartsWith($"{{\"MessageType\":\"{MessageTypeMap.HistoryUpdate}\""))
            return _rdxSerializer.Deserialize<HistoryUpdateMessage>(message);
        if (message.StartsWith($"{{\"MessageType\":\"{MessageTypeMap.Synchronization}\""))
            return _rdxSerializer.Deserialize<SynchronizationMessage>(message);
        if (message.StartsWith($"{{\"MessageType\":\"{MessageTypeMap.ChatMessage}\""))
            return _rdxSerializer.Deserialize<ChatMessage>(message);

        throw new InvalidOperationException();
    }
}