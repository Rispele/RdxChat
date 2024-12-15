using Rdx.Objects;
using Rdx.Objects.ValueObjects;

namespace Rdx.Serialization.Attributes;

public class RdxValueSerializerAttribute : RdxSerializerAttribute
{
    public override string Serialize(RdxObject obj)
    {
        return obj switch
        {
            RdxValue<int> intValue => $"{intValue.Value}{RdxSerializationHelper.SerializeStamp(obj)}",
            RdxValue<double> doubleValue => $"{doubleValue.Value}{RdxSerializationHelper.SerializeStamp(obj)}",
            RdxValue<long> longValue => $"{longValue.Value}{RdxSerializationHelper.SerializeStamp(obj)}",
            RdxValue<bool> boolValue => $"{boolValue.Value}{RdxSerializationHelper.SerializeStamp(obj)}",
            RdxValue<string> rdxValue => $"\"{rdxValue.Value}\"{RdxSerializationHelper.SerializeStamp(obj)}",
            _ => throw new ArgumentException($"Type: {obj.GetType()} is not allowed")
        };
    }
}