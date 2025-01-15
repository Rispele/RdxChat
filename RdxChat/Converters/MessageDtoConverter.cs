using Domain.Dtos;
using RdxChat.Entities;

namespace RdxChat.Converters;

public static class MessageDtoConverter
{
    public static HistoryUpdateMessage Convert(HistoryUpdateMessageDto dto)
    {
        return new HistoryUpdateMessage
        {
            MessagesToSave = dto.MessagesToSave,
            MessageToSendIds = dto.MessageToSendIds,
            RequestSentToId = dto.RequestSentToId,
            RequestSentFromId = dto.RequestSentFromId
        };
    }

    public static SynchronizationMessage Convert(SynchronizationMessageDto dto)
    {
        return new SynchronizationMessage
        {
            MessageId = dto.MessageId,
            RequestSentFromId = dto.RequestSentFromId,
            RequestSentToId = dto.RequestSentToId,
            MessageHistory = dto.MessageHistory
        };
    }

    public static UserRenameMessage Convert(UserRenameMessageDto dto)
    {
        return new UserRenameMessage
        {
            MessageId = dto.MessageId,
            UserId = dto.UserId,
            NewName = dto.NewName,
            RenameDateTime = dto.RenameDateTime
        };
    }

    public static ChatMessage Convert(ChatMessageDto dto)
    {
        return new ChatMessage
        {
            MessageId = dto.MessageId,
            Message = dto.Message,
            SendingTime = dto.SendingTime,
            UserId = dto.SenderId,
            UserName = dto.UserName
        };
    }

    public static AbstractMessage Convert(AbstractMessageDto dto)
    {
        AbstractMessage result = dto switch
        {
            HistoryUpdateMessageDto historyUpdateMessageDto => Convert(historyUpdateMessageDto),
            SynchronizationMessageDto synchronizationMessageDto => Convert(synchronizationMessageDto),
            UserRenameMessageDto userRenameMessageDto => Convert(userRenameMessageDto),
            ChatMessageDto chatMessageDto => Convert(chatMessageDto),
        };
        return result;
    }

    public static AbstractMessageDto Convert(object dto)
    {
        AbstractMessageDto result = dto switch
        {
            HistoryUpdateMessageDto historyUpdateMessageDto => historyUpdateMessageDto,
            SynchronizationMessageDto synchronizationMessageDto => synchronizationMessageDto,
            UserRenameMessageDto userRenameMessageDto => userRenameMessageDto,
            ChatMessageDto chatMessageDto => chatMessageDto,
        };
        return result;
    }
}