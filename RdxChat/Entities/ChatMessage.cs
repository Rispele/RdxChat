namespace RdxChat.Entities;

public class ChatMessage : AbstractMessage
{
    public string MessageType { get; } = MessageTypeMap.ChatMessage;
    public string Message { get; set; } = null!;

    public Guid UserId { get; set; }

    public string UserName { get; set; }
}