using System.Collections.Concurrent;
using System.Globalization;
using System.Reflection;
using System.Text.RegularExpressions;
using Rdx.Extensions;
using Rdx.Objects;
using Rdx.Serialization.Attributes;
using Rdx.Serialization.Parser;
using Rdx.Serialization.RdxToObjectConverter;
using Rdx.Serialization.RdxToObjectConverter.DefaultConverters;
using Rdx.Serialization.RdxToObjectConverter.DefaultConverters.Collections;
using Rdx.Serialization.RdxToObjectConverter.DefaultConverters.Values;
using Rdx.Serialization.Tokenizer;

namespace Rdx.Serialization;

public partial class RdxSerializer
{
    private readonly IReplicaIdProvider replicaIdProvider;
    private readonly SimpleConverter simpleConverter;
    private readonly DefaultConverterBase[] defaultConverters =
    [
        new BoolConverter(),
        new DateTimeConverter(),
        new DoubleConverter(),
        new IntConverter(),
        new LongConverter(),
        new StringConverter(),
        new ListConverter()
    ];
    
    [GeneratedRegex("\\s{2,}")]
    private static partial Regex ClearJRdxRegex();

    private readonly ConcurrentDictionary<Type, RdxSerializerAttribute> knownSerializers = new();
    private readonly ConcurrentDictionary<Type, (string name, PropertyInfo propertyInfo)[]> knownTypes = new();

    public RdxSerializer(IReplicaIdProvider replicaIdProvider, params DefaultConverterBase[] customParsers)
    {
        this.replicaIdProvider = replicaIdProvider;
        
        simpleConverter = new SimpleConverter(
            replicaIdProvider,
            defaultConverters.Concat(customParsers).ToArray(),
            knownSerializers,
            knownTypes);
    }

    #region serialization

    public string Serialize(object obj)
    {
        var serializer = obj.GetType().FindRdxSerializerAttribute(knownSerializers);
        if (serializer is not null)
        {
            return serializer.Serialize(this, obj);
        }

        if (obj is RdxObject rdxObject)
        {
            return SerializeCustomObject(rdxObject);
        }

        var type = obj.GetType();
        if (TrySerializeWithDefaultConverters(type, obj, out var serialized))
        {
            return serialized!;
        }

        if (type.IsGenericType && TrySerializeWithDefaultConverters(type.GetGenericTypeDefinition(), obj, out serialized))
        {
            return serialized!;
        }

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

            if (value is null)
            {
                continue;
            }

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

        var converted = simpleConverter.ConvertToType(type, parser.Parse());
        return converted;
    }

    #endregion
}