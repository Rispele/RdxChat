namespace RdxChat.Entities;

public class SynchronizationMessage : AbstractMessage
{
    public string MessageType { get; } = MessageTypeMap.Synchronization;

    public Guid ReceiverId { get; set; }

    public Guid SenderId { get; set; }

    public List<Guid> MessageHistory { get; set; }
}