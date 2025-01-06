using Rdx.Serialization.Parser;

namespace Rdx.Serialization.RdxToObjectConverter.ValueParsers;

public class StringConverter : DefaultConverterBase
{
    public override Type TargetType { get; } = typeof(string);

    public override object Convert(object value, ConverterArguments arguments)
    {
        if (value is not ParserRdxValue parserRdxValue)
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