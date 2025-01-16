using Domain.Entities;

namespace Domain.Services;

public interface IMessageService
{
    Guid SaveMessage(AbstractMessage abstractMessage, string path);

    List<AbstractMessage> GetChatMessages(Guid companionId);

    (Guid[], string[]) SynchronizeHistory(Guid requestSentToId, List<Guid> messageIds);

    void SaveCredentials(Guid userId, string userName);

    (Guid userId, string userName)? ReadCredentials();
}