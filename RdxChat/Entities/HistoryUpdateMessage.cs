namespace RdxChat.Entities;

public class HistoryUpdateMessage : AbstractMessage
{
    public string MessageType { get; } = MessageTypeForDtosMap.HistoryUpdate;

    public Guid RequestSentToId { get; set; }
    
    public Guid RequestSentFromId { get; set; }

    public List<string> MessagesToSave { get; set; }
    
    public List<Guid> MessageToSendIds { get; set; }
}