namespace RdxChat.Entities;

public class ChatMessage
{
    public string Message { get; set; } = null!;

    public DateTime SendingTime { get; set; }

    public Guid UserId { get; set; }

    public string UserName { get; set; }
}