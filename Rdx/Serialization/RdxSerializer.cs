using System.Collections.Concurrent;
using System.Globalization;
using System.Reflection;
using Rdx.Extensions;
using Rdx.Objects;
using Rdx.Serialization.Attributes;
using Rdx.Serialization.Parser;
using Rdx.Serialization.RdxToObjectConverter;
using Rdx.Serialization.Tokenizer;

namespace Rdx.Serialization;

public class RdxSerializer
{
    private readonly IReplicaIdProvider replicaIdProvider;
    private readonly SimpleConverter simpleConverter;
    
    private readonly ConcurrentDictionary<Type, RdxSerializerAttribute> knownSerializers = new();
    private readonly ConcurrentDictionary<Type, (string name, PropertyInfo propertyInfo)[]> knownTypes = new();

    public RdxSerializer(IReplicaIdProvider replicaIdProvider)
    {
        this.replicaIdProvider = replicaIdProvider;
        simpleConverter = new SimpleConverter(replicaIdProvider);
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
        return (TType) Deserialize(typeof(TType), jRdx);
    }
    
    public object Deserialize(Type type, string jRdx)
    {
        var tokenSource = new RdxTokenizer(jRdx).Tokenize();
        var parser = new RdxParser(new TokensReader(tokenSource, jRdx));
        var parsed = parser.Parse();
        
        var converted = simpleConverter.ConvertToType(type, parsed);
        return converted;
    }

    #endregion
}