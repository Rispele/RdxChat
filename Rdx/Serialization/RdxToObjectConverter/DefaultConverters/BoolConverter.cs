using Rdx.Serialization.Parser;

namespace Rdx.Serialization.RdxToObjectConverter.ValueParsers;

public class BoolConverter : DefaultConverterBase
{
    public override Type TargetType { get; } = typeof(bool);
    public override object Convert(object value, ConverterArguments arguments)
    {
        if (value is not ParserRdxValue parserRdxValue)
        {
            throw new NotImplementedException("Object is not a ParserRdxValue");
        }
        
        return bool.Parse(parserRdxValue.Value);
    }
}