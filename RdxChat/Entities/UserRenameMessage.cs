namespace RdxChat.Entities;

public class UserRenameMessage : AbstractMessage
{
    public string MessageType { get; } = MessageTypeMap.UserRename;

    public Guid UserId { get; set; }

    public DateTime RenameDateTime { get; set; }

    public string NewName { get; set; }
}