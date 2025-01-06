using System.Collections.Concurrent;
using System.Reflection;
using System.Text.RegularExpressions;
using Rdx.Extensions;
using Rdx.Objects;
using Rdx.Serialization.Attributes;
using Rdx.Serialization.DefaultConverters;
using Rdx.Serialization.DefaultConverters.Collections;
using Rdx.Serialization.DefaultConverters.Values;
using Rdx.Serialization.Parser;
using Rdx.Serialization.Tokenizer;

namespace Rdx.Serialization;

public partial class RdxSerializer(IReplicaIdProvider replicaIdProvider)
{
    private readonly IDefaultConverter[] defaultConverters =
    [
        new BoolConverter(),
        new DateTimeConverter(),
        new DoubleConverter(),
        new IntConverter(),
        new LongConverter(),
        new StringConverter(),
        new ListConverter(),
        new HashSetConverter(),
        new DictionaryConverter()
    ];

    private readonly ConcurrentDictionary<Type, RdxSerializerAttribute> knownSerializers = new();
    private readonly ConcurrentDictionary<Type, (string name, PropertyInfo propertyInfo)[]> knownTypes = new();

    [GeneratedRegex("\\s{2,}")]
    private static partial Regex ClearJRdxRegex();

    public long GetReplicaId()
    {
        return replicaIdProvider.GetReplicaId();
    }

    #region serialization

    public string Serialize(object obj)
    {
        var serializer = obj.GetType().FindRdxSerializerAttribute(knownSerializers);
        if (serializer is not null) return serializer.Serialize(this, obj);

        if (obj is RdxObject rdxObject) return SerializeCustomObject(rdxObject);

        var type = obj.GetType();
        if (TrySerializeWithDefaultConverters(type, obj, out var serialized)) return serialized!;

        if (type.IsGenericType &&
            TrySerializeWithDefaultConverters(type.GetGenericTypeDefinition(), obj, out serialized))
            return serialized!;

        return SerializeCustomObject(obj);
    }

    private bool TrySerializeWithDefaultConverters(Type type, object obj, out string? serialized)
    {
        var converter = defaultConverters.FirstOrDefault(t => t.TargetType == type);

        if (converter is null)
        {
            serialized = null;
            return false;
        }

        serialized = converter.Serialize(this, obj);
        return true;
    }


    private string SerializeCustomObject(RdxObject obj)
    {
        return $"{{{RdxSerializationHelper.SerializeStamp(obj)} {string.Join(", ", SerializeCustomObjectInner(obj))}}}";
    }

    private string SerializeCustomObject(object obj)
    {
        return $"{{{string.Join(", ", SerializeCustomObjectInner(obj))}}}";
    }

    private IEnumerable<string> SerializeCustomObjectInner(object obj)
    {
        var properties = obj.GetType().GetObjectProperties(knownTypes);
        foreach (var (name, property) in properties)
        {
            var value = property.GetValue(obj);

            if (value is null) continue;

            yield return $"<\"{name}\":{Serialize(value)}>";
        }
    }

    #endregion

    #region deserialization

    public TType Deserialize<TType>(string jRdx)
    {
        return (TType)Deserialize(typeof(TType), jRdx);
    }

    public object Deserialize(Type type, string jRdx)
    {
        var cleared = ClearJRdxRegex().Replace(jRdx.Trim(), " ");
        var tokenSource = new RdxTokenizer(cleared).Tokenize();
        var parser = new RdxParser(new TokensReader(tokenSource, cleared));

        var converted = ConvertToType(type, parser.Parse());
        return converted;
    }

    internal object ConvertToType(Type type, object obj)
    {
        var serializer = type.FindRdxSerializerAttribute(knownSerializers);
        if (serializer is not null) return serializer.Deserialize(new SerializationArguments(this, type, obj));

        if (TryDeserializeWithDefaultConverters(type, obj, out var converted, type)) return converted!;

        if (type.IsGenericType &&
            TryDeserializeWithDefaultConverters(type, obj, out converted, type.GetGenericTypeDefinition()))
            return converted!;

        return ConvertToCustomObject(type, obj);
    }

    private bool TryDeserializeWithDefaultConverters(Type type, object obj, out object? converted, Type typeToCheck)
    {
        var converter = defaultConverters.FirstOrDefault(t => t.TargetType == typeToCheck);

        if (converter is null)
        {
            converted = null;
            return false;
        }

        converted = converter.Deserialize(new SerializationArguments(this, type, obj));
        return true;
    }

    private object ConvertToCustomObject(Type type, object obj)
    {
        return FillObjectWithParameterValues(type, ExtractParameterValues(type, obj));
    }

    private object FillObjectWithParameterValues(
        Type type,
        Dictionary<string, object> dict)
    {
        var instance = type.GetConstructor(
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance,
                [])?
            .Invoke([]) ?? throw new MissingMethodException($"Could not find constructor for type {type}");

        var properties = type.GetObjectProperties(knownTypes);

        foreach (var (name, propertyInfo) in properties)
        {
            if (!dict.TryGetValue(name, out var value)) continue;

            var parsed = ConvertToType(propertyInfo.PropertyType, value);
            propertyInfo.SetValue(instance, parsed);
        }

        return instance;
    }

    private Dictionary<string, object> ExtractParameterValues(Type type, object obj)
    {
        if (obj is not ParserRdxPlex parserRdxPlex)
            throw new InvalidOperationException($"ParserRdxPlex expected, but was {obj.GetType()}");

        if (parserRdxPlex.Value.Any(t => t is not ParserRdxPlex plex || plex.Value.Count != 2))
            throw new InvalidOperationException("Invalid structure");

        var dict = new Dictionary<string, object>();
        foreach (var plex in parserRdxPlex.Value.Cast<ParserRdxPlex>())
        {
            if (plex.Value[0] is not ParserRdxValue keyValue)
                throw new InvalidOperationException("Invalid structure: key value must be of type ParserRdxValue");

            var key = keyValue.Value[1..^1];
            dict[key] = plex.Value[1];
        }

        if (parserRdxPlex.Timestamp is not null)
        {
            var (replicaId, version) = SerializationHelper.ParseTimestamp(parserRdxPlex.Timestamp);
            dict["ReplicaId"] = replicaId;
            dict["Version"] = version;
        }

        if (type.GetParentTypes().Contains(typeof(RdxObject)))
            dict["currentReplicaId"] = replicaIdProvider.GetReplicaId();

        return dict;
    }

    #endregion
}