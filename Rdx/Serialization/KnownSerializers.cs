using System.Collections.Concurrent;
using Rdx.Serialization.Attributes;

namespace Rdx.Serialization;

public static class KnownSerializers
{
    public static readonly ConcurrentDictionary<Type, RdxSerializerAttribute> Values = new();
}