using System.Reflection;
using Rdx.Extensions;
using Rdx.Objects;
using Rdx.Objects.ValueObjects;
using Rdx.Serialization.Parser;

namespace Rdx.Serialization.RdxToObjectConverter;

public class SimpleConverter(IReplicaIdProvider replicaIdProvider)
{
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
                return ConvertToRdxValue(type, obj);
            }
        }

        if (type == typeof(string)
            || type == typeof(long) 
            || type == typeof(double)
            || type == typeof(bool)
            || type == typeof(int))
        {
            return ConvertToRdxValue(type, obj);
        }

        return ConvertToCustomObject(type, obj);
    }

    private object ConvertToCustomObject(Type type, object obj)
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
            var (replicaId, version) = ParseTimestamp(parserRdxPlex.Timestamp);
            dict["ReplicaId"] = replicaId;
            dict["Version"] = version;
        }

        var instance = type.GetConstructor(
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance,
                [])?
            .Invoke([]) ?? throw new MissingMethodException($"Could not find constructor for type {type}");

        if (type.GetParentTypes().Contains(typeof(RdxObject)))
        {
            dict["currentReplicaId"] = replicaIdProvider.GetReplicaId();
        }

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

    public object ConvertToRdxValue(Type type, object obj)
    {
        if (obj is not ParserRdxValue value)
        {
            throw new NotImplementedException("Object is not a ParserRdxValue");
        }

        var genericType = type.GetGenericArguments().SingleOrDefault() ?? type;
        if (genericType == typeof(string))
        {
            return ParseString(value);
        }

        if (genericType == typeof(double))
        {
            return ParseDouble(value);
        }

        if (genericType == typeof(long))
        {
            return ParseLong(value);
        }
        
        if (genericType == typeof(int))
        {
            return ParseInt(value);
        }

        if (genericType == typeof(bool))
        {
            return ParseBool(value);
        }

        throw new NotImplementedException();
    }

    private object ParseInt(ParserRdxValue parserRdxValue)
    {
        var value = int.Parse(parserRdxValue.Value);
        if (parserRdxValue.Timestamp is null)
        {
            return value;
        }

        var (replicaId, version) = ParseTimestamp(parserRdxValue.Timestamp);
        return new RdxValue<int>(value, replicaId, version, replicaIdProvider.GetReplicaId());
    }

    public object ConvertToList(Type type, object obj)
    {
        if (obj is not ParserRdxPlex plex)
        {
            throw new NotImplementedException("Object is not a ParserRdxPlex");
        }

        var genericType = type.GetGenericArguments().Single();
        return plex.Value.Select(value => ConvertToType(genericType, value)).ToList();
    }

    private object ParseBool(ParserRdxValue parserRdxValue)
    {
        var value = bool.Parse(parserRdxValue.Value);
        if (parserRdxValue.Timestamp is null)
        {
            return value;
        }

        var (replicaId, version) = ParseTimestamp(parserRdxValue.Timestamp);
        return new RdxValue<bool>(value, replicaId, version, replicaIdProvider.GetReplicaId());
    }

    private object ParseLong(ParserRdxValue parserRdxValue)
    {
        var value = long.Parse(parserRdxValue.Value);
        if (parserRdxValue.Timestamp is null)
        {
            return value;
        }

        var (replicaId, version) = ParseTimestamp(parserRdxValue.Timestamp);
        return new RdxValue<long>(value, replicaId, version, replicaIdProvider.GetReplicaId());
    }

    private object ParseDouble(ParserRdxValue parserRdxValue)
    {
        var value = double.Parse(parserRdxValue.Value);
        if (parserRdxValue.Timestamp is null)
        {
            return value;
        }

        var (replicaId, version) = ParseTimestamp(parserRdxValue.Timestamp);
        return new RdxValue<double>(value, replicaId, version, replicaIdProvider.GetReplicaId());
    }

    private object ParseString(ParserRdxValue parserRdxValue)
    {
        if (!parserRdxValue.Value.EndsWith('\"'))
        {
            throw new FormatException("Invalid RDX value");
        }

        var stringValue = parserRdxValue.Value[1..^1];
        if (parserRdxValue.Timestamp is null)
        {
            return stringValue;
        }

        var (replicaId, version) = ParseTimestamp(parserRdxValue.Timestamp);
        return new RdxValue<string>(stringValue, replicaId, version, replicaIdProvider.GetReplicaId());
    }

    private (long replicaId, long version) ParseTimestamp(string timestamp)
    {
        var chunks = timestamp.Split('-');
        return (int.Parse(chunks[0]), int.Parse(chunks[1]));
    }
}