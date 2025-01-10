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

    public async Task<Guid> SaveMessageAsync(ChatMessageDto chatMessageDto, string path)
    {
        chatMessageDto.MessageId = Guid.NewGuid();
        using (var w = File.AppendText(path))
        {
            await w.WriteLineAsync(rdxSerializer.Serialize(chatMessageDto));
        }

        return chatMessageDto.MessageId;
    }

    public async Task<List<ChatMessageDto>> GetChatMessages(ChatCredentialsDto chatCredentialsDto)
    {
        var result = new List<ChatMessageDto>();
        var lines = File.ReadLines(chatCredentialsDto.SenderId.ToString());
        foreach (var line in lines)
        {
            var chatMessageDto = rdxSerializer.Deserialize<ChatMessageDto>(line);
            result.Add(chatMessageDto);
        }

        return result;
    }
}