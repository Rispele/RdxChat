using System.Reflection;
using Rdx.Extensions;
using Rdx.Objects;
using Rdx.Objects.PlexValues;
using Rdx.Objects.ValueObjects;
using Rdx.Serialization.Parser;
using Rdx.Serialization.RdxToObjectConverter.ValueParsers;

namespace Rdx.Serialization.RdxToObjectConverter;

public class SimpleConverter
{
    private readonly Dictionary<Type, ValueParserBase> valueParsers;
    private readonly IReplicaIdProvider replicaIdProvider;

    public SimpleConverter(IReplicaIdProvider replicaIdProvider, ValueParserBase[] valueParsers)
    {
        this.replicaIdProvider = replicaIdProvider;
        this.valueParsers = valueParsers.ToDictionary(t => t.TargetType);
    }

    public object ConvertToType(Type type, object obj)
    {
        if (type.IsGenericType)
        {
            if (type.GetGenericTypeDefinition() == typeof(List<>))
            {
                return ConvertToList(type, obj);
            }

            if (type.GetGenericTypeDefinition() == typeof(RdxValue<>))
            {
                return ConvertToRdxValue(type.GetGenericArguments().Single(), obj, true);
            }

            if (type.GetGenericTypeDefinition() == typeof(RdxXPle<>))
            {
                return ConvertToRdxXPle(type, obj);
            }

            if (type.GetGenericTypeDefinition() == typeof(RdxTuple<,>))
            {
                return ConvertToRdxTuple(type, obj);
            }
        }

        if (valueParsers.ContainsKey(type))
        {
            return ConvertToRdxValue(type, obj, false);
        }

        return ConvertToCustomObject(type, obj);
    }
    
    private object ConvertToCustomObject(Type type, object obj)
    {
        var dict = ExtractParameterValues(type, obj);
        return FillObjectWithParameterValues(type, dict);
    }

    private object FillObjectWithParameterValues(Type type, Dictionary<string, object> dict)
    {
        var instance = type.GetConstructor(
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance,
                [])?
            .Invoke([]) ?? throw new MissingMethodException($"Could not find constructor for type {type}");

        var properties = type.GetObjectProperties();

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

    private object ConvertToRdxValue(Type type, object obj, bool isRdxValue)
    {
        if (obj is not ParserRdxValue value)
        {
            throw new NotImplementedException("Object is not a ParserRdxValue");
        }

        if (!valueParsers.TryGetValue(type, out var parser))
        {
            throw new NotImplementedException($"Could not find parser for the given type {type}");
        }

        return parser.Parse(value, new ConverterArguments(replicaIdProvider.GetReplicaId(), isRdxValue));
    }

    private object ConvertToRdxTuple(Type type, object obj)
    {
        if (obj is not ParserRdxPlex plex)
        {
            throw new NotImplementedException("Object is not a plex");
        }

        if (plex.Value.Count != 2)
        {
            throw new InvalidOperationException("Tuple must have 2 items");
        }

        var genericType = type.GetGenericArguments();
        var (replicaId, version) = ParsingHelper.ParseTimestamp(plex.Timestamp ?? throw new InvalidOperationException());
        var value1 = ConvertToType(genericType[0], plex.Value[0]);
        var value2 = ConvertToType(genericType[1], plex.Value[1]);
        return type
            .GetConstructor([genericType[0], genericType[1], typeof(long), typeof(long), typeof(long)])!
            .Invoke([value1, value2, replicaId, version, replicaIdProvider.GetReplicaId()]);
    }
    
    private object ConvertToRdxXPle(Type type, object obj)
    {
        if (obj is not ParserRdxPlex plex)
        {
            throw new NotImplementedException("Object is not a plex");
        }

        var genericType = type.GetGenericArguments().Single();
        var (replicaId, version) = ParsingHelper.ParseTimestamp(plex.Timestamp ?? throw new InvalidOperationException());
        var values = plex.Value.Select(value => ConvertToType(genericType, value)).ToList();
        return type
            .GetConstructor([typeof(List<object>), typeof(long), typeof(long), typeof(long)])!
            .Invoke([values, replicaId, version, replicaIdProvider.GetReplicaId()]);
    }
    
    private object ConvertToList(Type type, object obj)
    {
        if (obj is not ParserRdxPlex plex)
        {
            throw new NotImplementedException("Object is not a plex");
        }

        var genericType = type.GetGenericArguments().Single();
        return plex.Value.Select(value => ConvertToType(genericType, value)).ToList();
    }
}