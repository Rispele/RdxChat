namespace RdxChat.Entities;

public class ChatModel
{
    public Guid UserId { get; set; }

    public Guid CompanionId { get; set; }

    public List<ChatMessage> Messages { get; set; } = [];

    public string UserName { get; set; }
    
    public string CompanionName { get; set; }
}