using Domain.Dtos;

namespace Domain.Services;

public interface IMessageService
{
    Task<Guid> SaveMessageAsync(AbstractMessageDto abstractMessageDto, string path);

    Task<List<AbstractMessageDto>> GetChatMessages(Guid requestSentToId);

    Task<(Guid[], object[])> SynchronizeHistory(Guid requestSentToId, List<Guid> messageIds);
}