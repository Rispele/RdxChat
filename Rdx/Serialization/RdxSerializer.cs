using System.Collections.Concurrent;
using System.Globalization;
using System.Reflection;
using System.Text.RegularExpressions;
using Rdx.Extensions;
using Rdx.Objects;
using Rdx.Serialization.Attributes;
using Rdx.Serialization.Parser;
using Rdx.Serialization.RdxToObjectConverter;
using Rdx.Serialization.RdxToObjectConverter.ValueParsers;
using Rdx.Serialization.Tokenizer;

namespace Rdx.Serialization;

public partial class RdxSerializer
{
    private readonly IReplicaIdProvider replicaIdProvider;
    private readonly SimpleConverter simpleConverter;

    [GeneratedRegex("\\s{2,}")]
    private static partial Regex ClearJRdxRegex();

    private readonly ConcurrentDictionary<Type, RdxSerializerAttribute> knownSerializers = new();
    private readonly ConcurrentDictionary<Type, (string name, PropertyInfo propertyInfo)[]> knownTypes = new();

    public RdxSerializer(IReplicaIdProvider replicaIdProvider, params ValueParserBase[] customParsers)
    {
        this.replicaIdProvider = replicaIdProvider;

        ValueParserBase[] parsers =
        [
            new BoolValueParser(),
            new DateTimeValueParser(),
            new DoubleValueParser(),
            new IntValueParser(),
            new LongValueParser(),
            new StringValueParser(),
        ];

        simpleConverter = new SimpleConverter(replicaIdProvider, parsers.Concat(customParsers).ToArray());
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

        if (TrySerializeValueObject(obj, out var result))
        {
            return result!;
        }

        if (obj is string objString)
        {
            return $"\"{objString}\"";
        }

        return SerializeCustomObject(obj);
    }

    private bool TrySerializeValueObject(object obj, out string? result)
    {
        switch (obj)
        {
            case long longObj:
                result = longObj.ToString();
                return true;
            case double doubleObj:
                result = doubleObj.ToString(CultureInfo.InvariantCulture);
                return true;
            case int intObj:
                result = intObj.ToString();
                return true;
            case bool boolObj:
                result = boolObj.ToString(CultureInfo.InvariantCulture);
                return true;
        }

        result = null;
        return false;
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