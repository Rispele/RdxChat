using Domain.Dtos;

namespace RdxChat.Entities;

public class SynchronizationMessageDto : AbstractMessageDto
{
    public string MessageType { get; } = MessageTypeForDtosMap.Synchronization;

    public Guid RequestSentToId { get; set; }
    
    public Guid RequestSentFromId { get; set; }

    public List<Guid> MessageHistory { get; set; }
}