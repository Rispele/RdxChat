using Domain.Dtos;
using JetBrains.Annotations;
using Rdx.Serialization.Attributes.Markup;

namespace RdxChat.Entities;

public class SynchronizationMessageDto : AbstractMessageDto
{
    [RdxProperty]
    public string MessageType { get; [UsedImplicitly] private set; } = MessageTypeForDtosMap.Synchronization;

    [RdxProperty]
    public Guid RequestSentToId { get; [UsedImplicitly] set; }
    
    [RdxProperty]
    public Guid RequestSentFromId { get; [UsedImplicitly] set; }

    [RdxProperty]
    public List<Guid> MessageHistory { get; [UsedImplicitly] set; }
}