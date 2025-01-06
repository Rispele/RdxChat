using System.Collections.Concurrent;
using System.Reflection;
using Rdx.Extensions;
using Rdx.Objects;
using Rdx.Serialization.Attributes;
using Rdx.Serialization.Parser;
using Rdx.Serialization.RdxToObjectConverter.ValueParsers;

namespace Rdx.Serialization.RdxToObjectConverter;

public class SimpleConverter
{
    private readonly DefaultConverterBase[] defaultConverters;
    private readonly IReplicaIdProvider replicaIdProvider;
    private readonly ConcurrentDictionary<Type, RdxSerializerAttribute> knownSerializers;
    private readonly ConcurrentDictionary<Type, (string name, PropertyInfo propertyInfo)[]> knownTypes;

    public SimpleConverter(
        IReplicaIdProvider replicaIdProvider,
        DefaultConverterBase[] defaultConverters,
        ConcurrentDictionary<Type, RdxSerializerAttribute> knownSerializers,
        ConcurrentDictionary<Type, (string name, PropertyInfo propertyInfo)[]> knownTypes)
    {
        this.replicaIdProvider = replicaIdProvider;
        this.knownSerializers = knownSerializers;
        this.knownTypes = knownTypes;
        this.defaultConverters = defaultConverters;
    }

    public long GetReplicaId() => replicaIdProvider.GetReplicaId();

    public object ConvertToType(Type type, object obj)
    {
        var serializer = type.FindRdxSerializerAttribute(knownSerializers);
        if (serializer is not null)
        {
            return serializer.Deserialize(this, type, obj);
        }

        if (TryConvertWithDefaultConverters(type, obj, out var converted))
        {
            return converted!;
        }

        if (type.IsGenericType && TryConvertWithDefaultConverters(type.GetGenericTypeDefinition(), obj, out converted))
        {
            return converted!;
        }

        return ConvertToCustomObject(type, obj);
    }

    private bool TryConvertWithDefaultConverters(Type type, object obj, out object? converted)
    {
        var converter = defaultConverters.FirstOrDefault(t => t.TargetType == type);

        if (converter is null)
        {
            converted = null;
            return false;
        }

        converted = converter.Convert(obj, new ConverterArguments(this, type, GetReplicaId(), true));
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
            if (!dict.TryGetValue(name, out var value))
            {
                continue;
            }

            var parsed = ConvertToType(propertyInfo.PropertyType, value);
            propertyInfo.SetValue(instance, parsed);
        }

        return instance;
    }

    private Dictionary<string, object> ExtractParameterValues(Type type, object obj)
    {
        if (obj is not ParserRdxPlex parserRdxPlex)
        {
            throw new InvalidOperationException($"ParserRdxPlex expected, but was {obj.GetType()}");
        }

        if (parserRdxPlex.Value.Any(t => t is not ParserRdxPlex plex || plex.Value.Count != 2))
        {
            throw new InvalidOperationException("Invalid structure");
        }

        var dict = new Dictionary<string, object>();
        foreach (var plex in parserRdxPlex.Value.Cast<ParserRdxPlex>())
        {
            if (plex.Value[0] is not ParserRdxValue keyValue)
            {
                throw new InvalidOperationException("Invalid structure: key value must be of type ParserRdxValue");
            }

            var key = keyValue.Value[1..^1];
            dict[key] = plex.Value[1];
        }

        if (parserRdxPlex.Timestamp is not null)
        {
            var (replicaId, version) = ParsingHelper.ParseTimestamp(parserRdxPlex.Timestamp);
            dict["ReplicaId"] = replicaId;
            dict["Version"] = version;
        }

        if (type.GetParentTypes().Contains(typeof(RdxObject)))
        {
            dict["currentReplicaId"] = replicaIdProvider.GetReplicaId();
        }

        return dict;
    }
}