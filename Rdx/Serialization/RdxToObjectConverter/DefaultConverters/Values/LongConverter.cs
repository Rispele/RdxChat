using System.Globalization;
using Rdx.Serialization.Parser;

namespace Rdx.Serialization.RdxToObjectConverter.DefaultConverters.Values;

public class LongConverter : DefaultConverterBase
{
    public override Type TargetType { get; } = typeof(long);
    public override string Serialize(RdxSerializer serializer, object obj)
    {
        if (obj is not long longObj)
        {
            throw new InvalidCastException();
        }
        
        return longObj.ToString(CultureInfo.InvariantCulture);
    }

    public override object Deserialize(ConverterArguments arguments)
    {
        if (arguments.Value is not ParserRdxValue parserRdxValue)
        {
            throw new NotImplementedException("Object is not a ParserRdxValue");
        }
        
        return long.Parse(parserRdxValue.Value, CultureInfo.InvariantCulture);
    }
}