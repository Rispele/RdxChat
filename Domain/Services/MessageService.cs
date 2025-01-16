using System.Text;
using Domain.Entities;
using Rdx.Serialization;

namespace Domain.Services;

public class MessageService : IMessageService
{
    private readonly RdxSerializer _rdxSerializer;

    public MessageService(RdxSerializer rdxSerializer)
    {
        _rdxSerializer = rdxSerializer;
    }

    public Guid SaveMessage(AbstractMessage abstractMessage, string path)
    {
        abstractMessage.MessageId = abstractMessage.MessageId == new Guid() ? Guid.NewGuid() : abstractMessage.MessageId;
        while (!IsFileReady(path)) { }
        using var w = File.AppendText(path);
        w.WriteLine(_rdxSerializer.Serialize(abstractMessage));
        return abstractMessage.MessageId;
    }

    public List<AbstractMessage> GetChatMessages(Guid companionId)
    {
        var path = companionId.ToString();
        
        if (!File.Exists(path))
        {
            return [];
        }

        while (!IsFileReady(path)) { }
        var result = new List<AbstractMessage>();
        const Int32 bufferSize = 128;
        using var fileStream = File.OpenRead(path);
        using var streamReader = new StreamReader(fileStream, Encoding.UTF8, true, bufferSize);
        String line;
        while ((line = streamReader.ReadLine()) != null)
        {
            if (string.IsNullOrEmpty(line))
            {
                continue;
            }

            var msg = _rdxSerializer.Deserialize<ChatMessage>(line);
            result.Add(msg);
        }
        return result.OrderBy(x => x.SendingTime).ToList();
    }

    public (Guid[], string[]) SynchronizeHistory(Guid requestSentToId, List<Guid> messageIds)
    {
        var myHistoryMessages = GetChatMessages(requestSentToId);
        var messageToSaveIds = messageIds
            .Where(x => myHistoryMessages.All(m => m.MessageId != x))
            .ToArray();
        var messagesToSend = myHistoryMessages
            .Where(x => x.MessageId != new Guid() && !messageIds.Contains(x.MessageId))
            .Select(x => _rdxSerializer.Serialize(x))
            .ToArray();
        return (messageToSaveIds, messagesToSend);
    }

    public void SaveCredentials(Guid userId, string userName)
    { 
        File.WriteAllLines("credentials", [userId.ToString(), userName]);
    }

    public (Guid userId, string userName)? ReadCredentials()
    {
        if (!File.Exists("credentials")) return null;
        var lines = File.ReadLines("credentials").ToArray();
        return (Guid.Parse(lines[0]), lines[1]);
    }
    
    private static bool IsFileReady(string path)
    {
        if (!File.Exists(path))
        {
            return true;
        }
        try
        {
            using var inputStream = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.None);
            return inputStream.Length > 0;
        }
        catch (Exception)
        {
            return false;
        }
    }
}