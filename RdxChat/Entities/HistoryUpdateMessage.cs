namespace RdxChat.Entities;

public class HistoryUpdateMessage : AbstractMessage
{
    public string MessageType { get; } = MessageTypeForDtosMap.HistoryUpdate;

    public Guid ReceiverId { get; set; }

    public Guid SenderId { get; set; }

    public Guid RequestSentToId { get; set; }

    public List<string> MessagesToSave { get; set; }
    
    public List<Guid> MessageToSendIds { get; set; }
}