﻿using System.Collections;
using Rdx.Serialization.Parser;

namespace Rdx.Serialization.DefaultConverters.Collections;

public class ListConverter : IDefaultConverter
{
    public Type TargetType { get; } = typeof(List<>);

    public string Serialize(RdxSerializer serializer, object obj)
    {
        if (obj is not IEnumerable enumerable) throw new ArgumentException("Target type is not IEnumerable");

        return $"[{string.Join(", ", enumerable.Cast<object>().Select(serializer.Serialize))}]";
    }

    public object Deserialize(SerializationArguments arguments)
    {
        if (arguments.Value is not ParserRdxPlex plex) throw new NotImplementedException("Object is not a plex");

        var list = (IList)Activator.CreateInstance(arguments.Type)!;
        var genericType = arguments.Type.GetGenericArguments().Single();
        foreach (var item in plex.Value.Select(t => arguments.Serializer.ConvertToType(genericType, t))) list.Add(item);
        return list;
    }
}