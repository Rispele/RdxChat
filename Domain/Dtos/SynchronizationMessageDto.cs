using Domain.Dtos;

namespace RdxChat.Entities;

public class SynchronizationMessageDto : AbstractMessageDto
{
    public string MessageType { get; } = MessageTypeForDtosMap.Synchronization;

    public Guid ReceiverId { get; set; }

    public Guid SenderId { get; set; }

    public Guid RequestSentToId { get; set; }

    public List<Guid> MessageHistory { get; set; }
}