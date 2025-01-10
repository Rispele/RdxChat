using System.Globalization;
using Rdx.Serialization.Parser;

namespace Rdx.Serialization.DefaultConverters.Values;

public class GuidConverter : IDefaultConverter
{
    public Type TargetType { get; } = typeof(Guid);
    public string Serialize(RdxSerializer serializer, object obj)
    {
        if (obj is not Guid guid)
            throw new InvalidCastException();
        
        return $"\"{guid}\"";
    }

    public object Deserialize(SerializationArguments arguments)
    {
        if (arguments.Value is not ParserRdxValue parserRdxValue)
            throw new NotImplementedException("Object is not a ParserRdxValue");

        return Guid.Parse(parserRdxValue.Value.Trim('\"'), CultureInfo.InvariantCulture);
    }
}