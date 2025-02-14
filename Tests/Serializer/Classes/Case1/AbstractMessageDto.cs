﻿using JetBrains.Annotations;
using Rdx.Serialization.Attributes.Markup;

namespace Tests.Serializer.Classes.Case1;

public class AbstractMessageDto
{
    [RdxProperty]
    public string MessageType { get; [UsedImplicitly] set; }

    [RdxProperty]
    public Guid? MessageId { get; [UsedImplicitly] set; }
    
    [RdxProperty]
    public DateTime SendingTime { get; [UsedImplicitly] set; }
}