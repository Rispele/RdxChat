using JetBrains.Annotations;
using Rdx.Serialization.Attributes.Markup;

namespace Domain.Entities;

public class SynchronizationMessage : AbstractMessage
{
    [RdxProperty]
    public string MessageType { get; [UsedImplicitly] private set; } = MessageTypeMap.Synchronization;

    [RdxProperty]
    public required Guid RequestSentToId { get; [UsedImplicitly] set; }
    
    [RdxProperty]
    public required Guid RequestSentFromId { get; [UsedImplicitly] set; }

    [RdxProperty]
    public required List<Guid> MessageHistory { get; [UsedImplicitly] set; }
}