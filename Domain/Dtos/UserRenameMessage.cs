using Domain.Dtos;
using JetBrains.Annotations;
using Rdx.Serialization.Attributes.Markup;

namespace RdxChat.Entities;

public class UserRenameMessageDto : AbstractMessageDto
{
    [RdxProperty]
    public string MessageType { get; [UsedImplicitly] private set; } = MessageTypeForDtosMap.UserRename;

    [RdxProperty]
    public Guid MessageId { get; [UsedImplicitly] set; }

    [RdxProperty]
    public Guid UserId { get; [UsedImplicitly] set; }

    [RdxProperty]
    public DateTime RenameDateTime { get; [UsedImplicitly] set; }

    [RdxProperty]
    public string NewName { get; [UsedImplicitly] set; }
}