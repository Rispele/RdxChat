using System.Globalization;
using Rdx.Serialization.Parser;

namespace Rdx.Serialization.RdxToObjectConverter.DefaultConverters.Values;

public class DoubleConverter : DefaultConverterBase
{
    public override Type TargetType { get; } = typeof(double);
    public override string Serialize(RdxSerializer serializer, object obj)
    {
        if (obj is not double doubleObj)
        {
            throw new InvalidCastException();
        }
        
        return doubleObj.ToString(CultureInfo.InvariantCulture);
    }

    public override object Deserialize(ConverterArguments arguments)
    {
        if (arguments.Value is not ParserRdxValue parserRdxValue)
        {
            throw new NotImplementedException("Object is not a ParserRdxValue");
        }
        
        return double.Parse(parserRdxValue.Value, CultureInfo.InvariantCulture);
    }
}