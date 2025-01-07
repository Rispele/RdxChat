namespace Domain.Dtos;

public class ChatMessageDto
{
    public Guid MessageId { get; set; }
    public string Message { get; set; }
    public DateTime SendingTime { get; set; }
    public Guid SenderId { get; set; }
    public string SenderName { get; set; }
    public Guid ReceiverId { get; set; }
}