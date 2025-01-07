using Domain.Dtos;

namespace Domain.Services;

public interface IMessageService
{
    Task<Guid> SaveMessageAsync(ChatMessageDto chatMessageDto, string path);

    Task<List<ChatMessageDto>> GetChatMessages(ChatCredentialsDto chatCredentialsDto);
}