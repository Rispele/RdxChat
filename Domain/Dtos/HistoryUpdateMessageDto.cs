using Domain.Dtos;

namespace RdxChat.Entities;

public class HistoryUpdateMessageDto : AbstractMessageDto
{
    public string MessageType { get; } = MessageTypeForDtosMap.HistoryUpdate;

    public Guid RequestSentToId { get; set; }
    
    public Guid RequestSentFromId { get; set; }

    public List<string> MessagesToSave { get; set; }
    
    public List<Guid> MessageToSendIds { get; set; }
}