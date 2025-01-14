using Domain.Dtos;

namespace Domain.Services;

public interface IMessageService
{
    Task<Guid> SaveMessageAsync(AbstractMessageDto abstractMessageDto, string path);

    Task<List<AbstractMessageDto>> GetChatMessages(ChatCredentialsDto chatCredentialsDto);

    Task<(Guid[], object[])> SynchronizeHistory(ChatCredentialsDto chatCredentialsDto, List<Guid> messageIds);
}