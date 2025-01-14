using Domain.Dtos;

namespace RdxChat.Entities;

public class UserRenameMessageDto : AbstractMessageDto
{
    public string MessageType { get; } = MessageTypeForDtosMap.UserRename;

    public Guid MessageId { get; set; }

    public Guid UserId { get; set; }

    public DateTime RenameDateTime { get; set; }

    public string NewName { get; set; }
}