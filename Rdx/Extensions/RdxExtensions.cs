using System.Collections.Concurrent;
using System.Reflection;
using Rdx.Objects;
using Rdx.Serialization.Attributes;

namespace Rdx.Extensions;

public static class RdxExtensions
{
    public static RdxSerializerAttribute GetRdxSerializerAttribute(
        this RdxObject obj,
        ConcurrentDictionary<Type, RdxSerializerAttribute>? knownSerializers = null)
    {
        var type = obj.GetType();
        if (knownSerializers?.TryGetValue(type, out var result) ?? false)
        {
            return result;
        }

        var attribute = type.GetCustomAttribute(typeof(RdxSerializerAttribute), inherit: true);
        if (attribute is not RdxSerializerAttribute serializerAttribute)
        {
            throw new InvalidOperationException($"The type {type.FullName} is not marked as RdxSerializerAttribute");
        }

        knownSerializers?.AddOrUpdate(type, serializerAttribute, (_, _) => serializerAttribute);
        return serializerAttribute;

    }
}