using Domain.Dtos;
using Domain.Services;
using Microsoft.AspNetCore.Mvc;
using RdxChat.Entities;

namespace RdxChat.Controllers;

public class ChatController : Controller
{
    private readonly IMessageService _messageService;

    public ChatController(IMessageService messageService)
    {
        _messageService = messageService;
    }

    [HttpGet("chat")]
    public async Task<IActionResult> Chat([FromQuery] Guid receiverId, [FromQuery] Guid senderId)
    {
        var chatMessageDtos = await _messageService.GetChatMessages(new ChatCredentialsDto
        {
            ReceiverId = receiverId,
            SenderId = senderId
        });
        var chatMessages = chatMessageDtos.Select(x => new ChatMessage
        {
            Message = x.Message,
            SendingTime = x.SendingTime,
            UserId = x.SenderId,
            UserName = "UserName"
        }).ToList();

        return View(new ChatModel
        {
            CompanionId = receiverId,
            UserId = senderId,
            Messages = chatMessages,
            UserName = "UserName",
            CompanionName = "CompanionName"
        });
    }

    [HttpPost("save-message")]
    public async Task<Guid> SaveMessage([FromBody] SendMessageModel sendMessageModel)
    {
        var messageId = await _messageService.SaveMessageAsync(new ChatMessageDto
        {
            Message = sendMessageModel.Message,
            ReceiverId = sendMessageModel.ReceiverId,
            SenderId = sendMessageModel.SenderId,
            SendingTime = DateTime.Now
        }, sendMessageModel.SavePath);
        return messageId;
    }

    public class SendMessageModel
    {
        public string Message { get; set; }
        public Guid SenderId { get; set; }
        public Guid ReceiverId { get; set; }
        public string SavePath { get; set; }
    }
}