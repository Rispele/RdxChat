namespace RdxChat.Entities;

public abstract class AbstractMessage
{
    public string MessageType { get; set; }

    public Guid MessageId { get; set; }
    
    public DateTime SendingTime { get; set; }
}