using System.Text.Json;
using Domain.Dtos;

namespace Domain.Services;

public class MessageService : IMessageService
{
    public async Task<Guid> SaveMessageAsync(ChatMessageDto chatMessageDto, string path)
    {
        chatMessageDto.MessageId = Guid.NewGuid();
        using (var w = File.AppendText(path))
        {
            await w.WriteLineAsync(JsonSerializer.Serialize(chatMessageDto));
        }

        return chatMessageDto.MessageId;
    }

    public async Task<List<ChatMessageDto>> GetChatMessages(ChatCredentialsDto chatCredentialsDto)
    {
        var result = new List<ChatMessageDto>();
        try
        {
            var lines = File.ReadLines(chatCredentialsDto.SenderId.ToString());
            foreach (var line in lines)
            {
                try
                {
                    var chatMessageDto = (ChatMessageDto)JsonSerializer.Deserialize(line, typeof(ChatMessageDto))!;
                    result.Add(chatMessageDto);
                }
                catch { }
            }

            return result;
        }
        catch (Exception e)
        {
            return result;
        }
    }
}