using JetBrains.Annotations;
using Rdx.Serialization.Attributes.Markup;

namespace Domain.Entities;

public class ChatMessage : AbstractMessage
{
    [RdxProperty]
    public string MessageType { get; [UsedImplicitly] private set; } = MessageTypeMap.ChatMessage;
    
    [RdxProperty]
    public required string Message { get; [UsedImplicitly] set; }
    
    [RdxProperty]
    public required Guid SenderId { get; [UsedImplicitly] set; }
    
    [RdxProperty]
    public required string SenderName { get; [UsedImplicitly] set; }
    
    [RdxProperty]
    public required Guid ReceiverId { get; [UsedImplicitly] set; }
}