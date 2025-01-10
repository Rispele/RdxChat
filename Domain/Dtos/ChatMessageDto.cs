using JetBrains.Annotations;
using Rdx.Serialization.Attributes.Markup;

namespace Domain.Dtos;

public class ChatMessageDto
{
    [RdxProperty]
    public Guid MessageId { get; [UsedImplicitly] set; }
    [RdxProperty]
    public string Message { get; [UsedImplicitly] set; }
    [RdxProperty]
    public DateTime SendingTime { get; [UsedImplicitly] set; }
    [RdxProperty]
    public Guid SenderId { get; [UsedImplicitly] set; }
    [RdxProperty]
    public string SenderName { get; [UsedImplicitly] set; }
    [RdxProperty]
    public Guid ReceiverId { get; [UsedImplicitly] set; }
}