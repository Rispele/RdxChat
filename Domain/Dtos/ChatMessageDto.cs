using JetBrains.Annotations;
using Rdx.Serialization.Attributes.Markup;
using RdxChat.Entities;

namespace Domain.Dtos;

public class ChatMessageDto : AbstractMessageDto
{
    [RdxProperty]
    public string MessageType { get; [UsedImplicitly] set; } = MessageTypeForDtosMap.ChatMessage;
    [RdxProperty]
    public string Message { get; [UsedImplicitly] set; }
    [RdxProperty]
    public Guid SenderId { get; [UsedImplicitly] set; }
    [RdxProperty]
    public string SenderName { get; [UsedImplicitly] set; }
    [RdxProperty]
    public Guid ReceiverId { get; [UsedImplicitly] set; }
}