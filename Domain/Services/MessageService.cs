using Domain.Dtos;
using Rdx.Serialization;

namespace Domain.Services;

public class MessageService : IMessageService
{
    private readonly RdxSerializer rdxSerializer;
    private readonly IUserService _userService;

    public MessageService(RdxSerializer rdxSerializer, IUserService userService)
    {
        this.rdxSerializer = rdxSerializer;
        _userService = userService;
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

    public async Task<List<AbstractMessageDto>> GetChatMessages(ChatCredentialsDto chatCredentialsDto)
    {
        var result = new List<AbstractMessageDto>();
        var lines = File.ReadLines(chatCredentialsDto.RequestSentToId.ToString());
        foreach (var line in lines)
        {
            var chatMessageDto = rdxSerializer.Deserialize<ChatMessageDto>(line);
            result.Add(chatMessageDto);
        }

        return result.OrderBy(x => x.SendingTime).ToList();
    }

    public async Task<(Guid[], object[])> SynchronizeHistory(ChatCredentialsDto chatCredentialsDto, List<Guid> messageIds)
    {
        var myHistoryMessages = await GetChatMessages(chatCredentialsDto);
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