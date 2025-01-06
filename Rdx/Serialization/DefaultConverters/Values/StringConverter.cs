using Rdx.Serialization.Parser;

namespace Rdx.Serialization.DefaultConverters.Values;

public class StringConverter : DefaultConverterBase
{
    public override Type TargetType { get; } = typeof(string);

    public override string Serialize(RdxSerializer serializer, object obj)
    {
        if (obj is not string strObj)
        {
            throw new InvalidCastException();
        }
        
        return $"\"{strObj}\"";
    }

    public override object Deserialize(SerializationArguments arguments)
    {
        if (arguments.Value is not ParserRdxValue parserRdxValue)
        {
            throw new NotImplementedException("Object is not a ParserRdxValue");
        }
        
        var str = parserRdxValue.Value;
        if (!str.StartsWith('\"') || !str.EndsWith('\"'))
        {
            throw new FormatException("Invalid RDX value");
        }

        return str[1..^1];
    }
}