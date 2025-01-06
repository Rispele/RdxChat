using System.Globalization;
using Rdx.Serialization.Parser;

namespace Rdx.Serialization.RdxToObjectConverter.ValueParsers;

public class DoubleConverter : DefaultConverterBase
{
    public override Type TargetType { get; } = typeof(double);
    public override object Convert(object value, ConverterArguments arguments)
    {
        if (value is not ParserRdxValue parserRdxValue)
        {
            throw new NotImplementedException("Object is not a ParserRdxValue");
        }
        
        return double.Parse(parserRdxValue.Value, CultureInfo.InvariantCulture);
    }
}