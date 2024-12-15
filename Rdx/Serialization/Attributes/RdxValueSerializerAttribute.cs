using System.Globalization;
using Rdx.Objects.ValueObjects;

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
            _ => throw new ArgumentException($"Type: {obj.GetType()} is not allowed")
        };
    }
}