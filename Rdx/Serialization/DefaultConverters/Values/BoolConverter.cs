using System.Globalization;
using Rdx.Serialization.Parser;

namespace Rdx.Serialization.DefaultConverters.Values;

public class BoolConverter : DefaultConverterBase
{
    public override Type TargetType { get; } = typeof(bool);
    public override string Serialize(RdxSerializer serializer, object obj)
    {
        if (obj is not bool boolObj)
        {
            throw new InvalidCastException();
        }
        
        return boolObj.ToString(CultureInfo.InvariantCulture);
    }

    public override object Deserialize(SerializationArguments arguments)
    {
        if (arguments.Value is not ParserRdxValue parserRdxValue)
        {
            throw new NotImplementedException("Object is not a ParserRdxValue");
        }
        
        return bool.Parse(parserRdxValue.Value);
    }
}