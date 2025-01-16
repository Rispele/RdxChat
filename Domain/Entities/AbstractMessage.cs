using JetBrains.Annotations;
using Rdx.Serialization.Attributes.Markup;

namespace Domain.Entities;

public abstract class AbstractMessage
{
    [RdxProperty]
    public Guid MessageId { get; [UsedImplicitly] set; }
    
    [RdxProperty]
    public DateTime SendingTime { get; [UsedImplicitly] set; }
}