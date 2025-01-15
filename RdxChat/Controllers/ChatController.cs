using System.Text.Json;
using Domain.Dtos;
using Domain.Services;
using Microsoft.AspNetCore.Mvc;
using Rdx.Serialization;
using RdxChat.Converters;
using RdxChat.Entities;
using RdxChat.WebSocket;

namespace RdxChat.Controllers;

public class ChatController : Controller
{
    private readonly IMessageService _messageService;
    private readonly IWebSocketHandler _webSocketHandler;
    private readonly RdxSerializer rdxSerializer;

    public ChatController(
        IMessageService messageService, 
        IWebSocketHandler webSocketHandler, 
        RdxSerializer rdxSerializer)
    {
        _messageService = messageService;
        _webSocketHandler = webSocketHandler;
        this.rdxSerializer = rdxSerializer;
    }

    [HttpGet("chat")]
    public async Task<IActionResult> Chat([FromQuery] Guid receiverId, [FromQuery] Guid senderId)
    {
        var chatMessageDtos = await _messageService.GetChatMessages(senderId);
        var chatMessages = chatMessageDtos
            .Where(x => x is ChatMessageDto)
            .Select(x => MessageDtoConverter.Convert((ChatMessageDto)x))
            .ToList();
        var lastCompanionMessage = chatMessages
            .Where(x => x.UserId != senderId)
            .MaxBy(x => x.SendingTime);
        var companionName = "Unknown";
        if (lastCompanionMessage != null)
        {
            companionName = lastCompanionMessage.UserName;
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
    public async Task<string> SaveMessage([FromBody] SendMessageModel sendMessageModel)
    {
        var chatMessageDto = new ChatMessageDto
        {
            Message = sendMessageModel.Message,
            MessageId = sendMessageModel.MessageId,
            ReceiverId = sendMessageModel.ReceiverId,
            SenderId = sendMessageModel.SenderId,
            UserName = sendMessageModel.SenderName,
            SendingTime = DateTime.Now
        };
        var messageId = await _messageService.SaveMessageAsync(chatMessageDto, sendMessageModel.SavePath);
        chatMessageDto.MessageId = messageId;
        return rdxSerializer.Serialize(chatMessageDto);
    }

    [HttpPost("send-message")]
    public async Task SendMessage([FromBody] SendMessageModel sendMessageModel)
    {
        var chatMessageDto = new ChatMessageDto
        {
            Message = sendMessageModel.Message,
            MessageId = sendMessageModel.MessageId,
            ReceiverId = sendMessageModel.ReceiverId,
            SenderId = sendMessageModel.SenderId,
            UserName = sendMessageModel.SenderName,
            SendingTime = DateTime.Now
        };
        var serialized = rdxSerializer.Serialize(chatMessageDto);
        await _webSocketHandler.SendMessage(serialized);
    }
    
    [HttpGet("sync-history")]
    public async Task SyncHistory([FromQuery] Guid companionId)
    {
        var currentUserId = RequestContextFactory.Build(Request).GetUserId();

        var serialized = rdxSerializer.Serialize(new SynchronizationMessageDto
        {
            MessageHistory = (await _messageService.GetChatMessages(currentUserId))
                .Select(x => x.MessageId).ToList(),
            RequestSentFromId = currentUserId,
            RequestSentToId = companionId
        });
        
        await _webSocketHandler.SendMessage(serialized);
    }

    public class SendMessageModel
    {
        public string MessageType { get; set; }
        public string Message { get; set; }
        public Guid MessageId { get; set; }
        public Guid SenderId { get; set; }
        public string SenderName { get; set; }
        public Guid ReceiverId { get; set; }
        public string SavePath { get; set; }
    }
}