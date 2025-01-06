using System.Globalization;
using Rdx.Objects.ValueObjects;
using Rdx.Serialization.Parser;
using Rdx.Serialization.RdxToObjectConverter;

namespace Rdx.Serialization.Attributes;

public class RdxValueSerializerAttribute : RdxSerializerAttribute
{
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

    public override object Deserialize(ConverterArguments converterArguments)
    {
        if (converterArguments.Value is not ParserRdxValue value)
        {
            throw new NotImplementedException("Object is not a ParserRdxValue");
        }

        if (converterArguments.Type == typeof(RdxValue<int>))
        {
            return ConvertValue(int.Parse, value, converterArguments.Converter.GetReplicaId());
        }

        if (converterArguments.Type == typeof(RdxValue<double>))
        {
            return ConvertValue(
                parser => double.Parse(parser, CultureInfo.InvariantCulture),
                value,
                converterArguments.Converter.GetReplicaId());
        }

        if (converterArguments.Type == typeof(RdxValue<long>))
        {
            return ConvertValue(long.Parse, value, converterArguments.Converter.GetReplicaId());
        }

        if (converterArguments.Type == typeof(RdxValue<bool>))
        {
            return ConvertValue(bool.Parse, value, converterArguments.Converter.GetReplicaId());
        }

        if (converterArguments.Type == typeof(RdxValue<string>))
        {
            return ConvertString(converterArguments.Converter, value);
        }

        if (converterArguments.Type == typeof(RdxValue<DateTime>))
        {
            return ConvertValue(t => DateTime.Parse(t, CultureInfo.InvariantCulture), value, converterArguments.Converter.GetReplicaId());
        }

        throw new ArgumentException($"Type: {converterArguments.Value.GetType()} is not allowed");
    }

    private object ConvertString(SimpleConverter converter, ParserRdxValue value)
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