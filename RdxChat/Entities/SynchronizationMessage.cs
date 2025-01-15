namespace RdxChat.Entities;

public class SynchronizationMessage : AbstractMessage
{
    public string MessageType { get; } = MessageTypeMap.Synchronization;

    public Guid RequestSentFromId { get; set; }
    
    public Guid RequestSentToId { get; set; }

    public List<Guid> MessageHistory { get; set; }
}