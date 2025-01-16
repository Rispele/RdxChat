using System.Text.Json;
using Domain.Entities;
using Domain.Services;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Mvc;
using Rdx.Serialization;
using RdxChat.WebSocket;

namespace RdxChat.Controllers;

public class ChatController : Controller
{
    private readonly IMessageService _messageService;
    private readonly IWebSocketHandler _webSocketHandler;
    private readonly RdxSerializer _rdxSerializer;

    public ChatController(
        IMessageService messageService, 
        IWebSocketHandler webSocketHandler, 
        RdxSerializer rdxSerializer)
    {
        _messageService = messageService;
        _webSocketHandler = webSocketHandler;
        _rdxSerializer = rdxSerializer;
    }

    [HttpGet("chat")]
    public IActionResult Chat([FromQuery] Guid receiverId, [FromQuery] Guid senderId)
    {
        var chatMessages = _messageService
            .GetChatMessages(receiverId)
            .OfType<ChatMessage>()
            .ToList();
        var lastCompanionMessage = chatMessages
            .Where(x => x.SenderId != senderId)
            .MaxBy(x => x.SendingTime);
        var companionName = "Unknown";
        if (lastCompanionMessage != null)
        {
            companionName = lastCompanionMessage.SenderName;
        }
        
        return View(new ChatModel
        {
            CompanionId = receiverId,
            UserId = senderId,
            Messages = chatMessages,
            UserName = RequestContextFactory.Build(Request).GetUserName(),
            CompanionName = companionName
        });
    }

    [HttpPost("save-message")]
    public async Task SaveMessage([FromBody] SendMessageModel sendMessageModel)
    {
        var chatMessage = new ChatMessage
        {
            MessageId = sendMessageModel.MessageId,
            SendingTime = sendMessageModel.SendingTime ?? DateTime.Now,
            Message = sendMessageModel.Message,
            SenderId = sendMessageModel.SenderId,
            SenderName = sendMessageModel.SenderName,
            ReceiverId = sendMessageModel.ReceiverId
        };

        var currentUserId = RequestContextFactory.Build(Request).GetUserId();
        if (currentUserId != chatMessage.SenderId && currentUserId != chatMessage.ReceiverId)
        {
            throw new InvalidOperationException();
        }
        
        var messageId = _messageService.SaveMessage(chatMessage,
            currentUserId == chatMessage.SenderId 
                ? chatMessage.SenderId.ToString() 
                : chatMessage.ReceiverId.ToString());
        chatMessage.MessageId = messageId;

        if (sendMessageModel.ShouldSend)
        {
            await SendMessage(chatMessage);
        }
    }
    
    [HttpGet("sync-history")]
    public async Task SyncHistory([FromQuery] Guid companionId)
    {
        var currentUserId = RequestContextFactory.Build(Request).GetUserId();

        var serialized = _rdxSerializer.Serialize(new SynchronizationMessage
        {
            MessageHistory = _messageService.GetChatMessages(currentUserId)
                .Select(x => x.MessageId).ToList(),
            RequestSentFromId = currentUserId,
            RequestSentToId = companionId
        });
        
        await _webSocketHandler.SendMessage(serialized);
    }

    public class ChatModel
    {
        public required Guid UserId { get; set; }
        public required Guid CompanionId { get; set; }
        public required List<ChatMessage> Messages { get; set; } = [];
        public required string UserName { get; set; }
        public required string CompanionName { get; set; }
    }
    
    public class SendMessageModel
    {
        public Guid MessageId { get; set; }
        public DateTime? SendingTime { get; set; }
        public string Message { get; set; }
        public Guid SenderId { get; set; }
        public string SenderName { get; set; }
        public Guid ReceiverId { get; set; }
        public bool ShouldSend { get; set; }
    }
    
    private async Task SendMessage(ChatMessage chatMessage)
    {
        var serialized = _rdxSerializer.Serialize(chatMessage);
        await _webSocketHandler.SendMessage(serialized);
    }
}