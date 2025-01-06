using System.Collections.Concurrent;
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
    private readonly ConcurrentDictionary<Type, (string name, PropertyInfo propertyInfo)[]> knownTypes;

    public SimpleConverter(
        IReplicaIdProvider replicaIdProvider,
        ValueParserBase[] valueParsers,
        ConcurrentDictionary<Type, (string name, PropertyInfo propertyInfo)[]> knownTypes)
    {
        this.replicaIdProvider = replicaIdProvider;
        this.knownTypes = knownTypes;
        this.valueParsers = valueParsers.ToDictionary(t => t.TargetType);
    }

    public object ConvertToType(Type type, object obj)
    {
        if (TryConvertGenericType(type, obj, out var convertToRdxValue))
        {
            return convertToRdxValue!;
        }

        if (valueParsers.ContainsKey(type))
        {
            return ConvertToRdxValue(type, obj, false);
        }

        return ConvertToCustomObject(type, obj);
    }

    private bool TryConvertGenericType(Type type, object obj, out object? converted)
    {
        converted = null;
        if (!type.IsGenericType)
        {
            return false;
        }
        
        var genericDefinition = type.GetGenericTypeDefinition();
        if (genericDefinition == typeof(List<>))
        {
            converted = ConvertToList(type, obj);
        }
        else if (genericDefinition == typeof(RdxValue<>))
        {
            converted = ConvertToRdxValue(type.GetGenericArguments().Single(), obj, true);
        }
        else if (genericDefinition == typeof(RdxXPle<>))
        {
            converted = ConvertToRdxXPle(type, obj);
        }
        else if (genericDefinition == typeof(RdxTuple<,>))
        {
            converted = ConvertToRdxTuple(type, obj);
        }
        else if (genericDefinition == typeof(RdxDictionary<,>))
        {
            converted = ConvertToRdxDictionary(type, obj);
        }
        else if (genericDefinition == typeof(ValueTuple<,>))
        {
            converted = ConvertToTuple(type, obj);
        }
        else if (genericDefinition == typeof(RdxSet<>))
        {
            converted = ConvertToRdxSet(type, obj);
        }
        else
        {
            return false;
        }

        return true;
    }

    private object ConvertToRdxDictionary(Type type, object obj)
    {
        if (obj is not ParserRdxPlex plex)
        {
            throw new NotImplementedException("Object is not a plex");
        }
        
        var genericTypes = type.GetGenericArguments();
        var (replicaId, version) = ParsingHelper.ParseTimestamp(plex.Timestamp ?? throw new InvalidOperationException());
        var values = plex.Value
            .Select(value => ConvertToTuple(typeof(ValueTuple<,>).MakeGenericType(genericTypes), value))
            .ToArray();

        var dictionaryType = typeof(Dictionary<,>).MakeGenericType(genericTypes);
        var dictionary = Activator.CreateInstance(dictionaryType);
        var addMethod = dictionaryType.GetMethod("Add")!;
        foreach (var (key, value) in values)
        {
            addMethod.Invoke(dictionary, [key, value]);
        }
        return type
            .GetConstructor([typeof(IDictionary<,>).MakeGenericType(genericTypes), typeof(long), typeof(long), typeof(long)])!
            .Invoke([dictionary, replicaId, version, replicaIdProvider.GetReplicaId()]);
    }

    private (object, object) ConvertToTuple(Type type, object obj)
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
        var value1 = ConvertToType(genericType[0], plex.Value[0]);
        var value2 = ConvertToType(genericType[1], plex.Value[1]);
        return (value1, value2);
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
        var listType = typeof(List<>).MakeGenericType(genericType);
        var list = Activator.CreateInstance(listType, values.Capacity);
        var addMethod = listType.GetMethod("Add")!;
        foreach (var value in values)
        {
            addMethod.Invoke(list, [value]);
        }
        return type
            .GetConstructor([listType, typeof(long), typeof(long), typeof(long)])!
            .Invoke([list, replicaId, version, replicaIdProvider.GetReplicaId()]);
    }
    
    private object ConvertToRdxSet(Type type, object obj)
    {
        if (obj is not ParserRdxPlex plex)
        {
            throw new NotImplementedException("Object is not a plex");
        }

        var genericType = type.GetGenericArguments().Single();
        var (replicaId, version) = ParsingHelper.ParseTimestamp(plex.Timestamp ?? throw new InvalidOperationException());
        var values = plex.Value.Select(value => ConvertToType(genericType, value)).ToList();
        var setType = typeof(HashSet<>).MakeGenericType(genericType);
        var set = Activator.CreateInstance(setType, values.Capacity);
        var addMethod = setType.GetMethod("Add")!;
        foreach (var value in values)
        {
            addMethod.Invoke(set, [value]);
        }
        return type
            .GetConstructor([setType, typeof(long), typeof(long), typeof(long)])!
            .Invoke([set, replicaId, version, replicaIdProvider.GetReplicaId()]);
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