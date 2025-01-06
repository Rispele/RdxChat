using System.Globalization;
using Rdx.Objects.ValueObjects;
using Rdx.Serialization.Parser;
using Rdx.Serialization.RdxToObjectConverter;
using Rdx.Serialization.RdxToObjectConverter.ValueParsers;

namespace Rdx.Serialization.Attributes;

public class RdxValueSerializerAttribute : RdxSerializerAttribute
{
    private readonly DefaultConverterBase[] parsers =
    [
        new BoolConverter(),
        new DateTimeConverter(),
        new DoubleConverter(),
        new IntConverter(),
        new LongConverter(),
        new StringConverter(),
    ];
    
    public override string Serialize(RdxSerializer _, object obj)
    {
        return obj switch
        {
            RdxValue<int> intValue => $"{intValue.Value}{RdxSerializationHelper.SerializeStamp(intValue)}",
            RdxValue<double> doubleValue =>
                $"{doubleValue.Value.ToString(CultureInfo.InvariantCulture)}{RdxSerializationHelper.SerializeStamp(doubleValue)}",
            RdxValue<long> longValue => $"{longValue.Value}{RdxSerializationHelper.SerializeStamp(longValue)}",
            RdxValue<bool> boolValue =>
                $"{boolValue.Value.ToString(CultureInfo.InvariantCulture)}{RdxSerializationHelper.SerializeStamp(boolValue)}",
            RdxValue<string> stringValue =>
                $"\"{stringValue.Value}\"{RdxSerializationHelper.SerializeStamp(stringValue)}",
            RdxValue<DateTime> dateTimeValue =>
                $"\"{dateTimeValue.Value.ToString(CultureInfo.InvariantCulture)}\"{RdxSerializationHelper.SerializeStamp(dateTimeValue)}",
            _ => throw new ArgumentException($"Type: {obj.GetType()} is not allowed")
        };
    }

    public override object Deserialize(SimpleConverter converter, Type type, object obj)
    {
        if (obj is not ParserRdxValue value)
        {
            throw new NotImplementedException("Object is not a ParserRdxValue");
        }

        if (type == typeof(RdxValue<int>))
        {
            return ConvertValue(int.Parse, value, converter.GetReplicaId());
        }
        if (type == typeof(RdxValue<double>))
        {
            return ConvertValue(
                t => double.Parse(t, CultureInfo.InvariantCulture),
                value,
                converter.GetReplicaId());
        }
        if (type == typeof(RdxValue<long>))
        {
            return ConvertValue(long.Parse, value, converter.GetReplicaId());
        }
        if (type == typeof(RdxValue<bool>))
        {
            return ConvertValue(bool.Parse, value, converter.GetReplicaId());
        }
        if (type == typeof(RdxValue<string>))
        {
            return ConvertValue(
                str =>
                {
                    if (!str.StartsWith('\"') || !str.EndsWith('\"'))
                    {
                        throw new FormatException("Invalid RDX value");
                    }

                    return str[1..^1];
                },
                value,
                converter.GetReplicaId());
        }
        if (type == typeof(RdxValue<DateTime>))
        {
            return ConvertValue(t => DateTime.Parse(t, CultureInfo.InvariantCulture), value, converter.GetReplicaId());
        }

        throw new ArgumentException($"Type: {obj.GetType()} is not allowed");
    }
    
    private object ConvertValue<TType>(
        Func<string, TType> parser,
        object parserRdxValueObj,
        long currentReplicaId)
        where TType : notnull
    {
        if (parserRdxValueObj is not ParserRdxValue parserRdxValue)
        {
            throw new NotImplementedException("Object is not a ParserRdxValue");
        }

        var value = parser(parserRdxValue.Value);
        if (parserRdxValue.Timestamp is null)
        {
            throw new InvalidOperationException("Timestamp must be specified for rdx value");
        }

        var (replicaId, version) = ParsingHelper.ParseTimestamp(parserRdxValue.Timestamp);
        return new RdxValue<TType>(value, replicaId, version, currentReplicaId);
    }
}