using Domain.Dtos;
using JetBrains.Annotations;
using Rdx.Serialization.Attributes.Markup;

namespace RdxChat.Entities;

public class HistoryUpdateMessageDto : AbstractMessageDto
{
    [RdxProperty]
    public string MessageType { get; [UsedImplicitly] private set; } = MessageTypeForDtosMap.HistoryUpdate;

    [RdxProperty]
    public Guid RequestSentToId { get; [UsedImplicitly] set; }
    
    [RdxProperty]
    public Guid RequestSentFromId { get; [UsedImplicitly] set; }

    [RdxProperty]
    public List<string> MessagesToSave { get; [UsedImplicitly] set; }
    
    [RdxProperty]
    public List<Guid> MessageToSendIds { get; [UsedImplicitly] set; }
}