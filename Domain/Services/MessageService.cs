using Domain.Dtos;
using Rdx.Serialization;

namespace Domain.Services;

public class MessageService : IMessageService
{
    private readonly RdxSerializer rdxSerializer;

    public MessageService(RdxSerializer rdxSerializer)
    {
        this.rdxSerializer = rdxSerializer;
    }

    public async Task<Guid> SaveMessageAsync(AbstractMessageDto abstractMessageDto, string path)
    {
        abstractMessageDto.MessageId = abstractMessageDto.MessageId == new Guid() ? Guid.NewGuid() : abstractMessageDto.MessageId;
        using (var w = File.AppendText(path))
        {
            await w.WriteLineAsync(rdxSerializer.Serialize(abstractMessageDto));
        }

        return abstractMessageDto.MessageId;
    }

    public async Task<List<AbstractMessageDto>> GetChatMessages(Guid requestSentToId)
    {
        var result = new List<AbstractMessageDto>();
        var lines = File.ReadLines(requestSentToId.ToString());
        foreach (var line in lines)
        {
            var chatMessageDto = rdxSerializer.Deserialize<ChatMessageDto>(line);
            result.Add(chatMessageDto);
        }

        return result.OrderBy(x => x.SendingTime).ToList();
    }

    public async Task<(Guid[], object[])> SynchronizeHistory(Guid requestSentToId, List<Guid> messageIds)
    {
        var myHistoryMessages = await GetChatMessages(requestSentToId);
        var messageToSaveIds = messageIds
            .Where(x => myHistoryMessages.All(m => m.MessageId != x))
            .ToArray();
        var messagesToSend = myHistoryMessages
            .Where(x => x.MessageId != new Guid() && !messageIds.Contains(x.MessageId))
            .Select(x => (object)x)
            .ToArray();
        return (messageToSaveIds, messagesToSend);
    }
}