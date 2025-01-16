using JetBrains.Annotations;
using Rdx.Serialization.Attributes.Markup;

namespace Domain.Entities;

public class HistoryUpdateMessage : AbstractMessage
{
    [RdxProperty]
    public string MessageType { get; [UsedImplicitly] private set; } = MessageTypeMap.HistoryUpdate;

    [RdxProperty]
    public required Guid RequestSentToId { get; [UsedImplicitly] set; }
    
    [RdxProperty]
    public required Guid RequestSentFromId { get; [UsedImplicitly] set; }

    [RdxProperty]
    public required List<string> MessagesToSave { get; [UsedImplicitly] set; }
    
    [RdxProperty]
    public required List<Guid> MessageToSendIds { get; [UsedImplicitly] set; }
}