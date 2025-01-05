using Rdx.Serialization.Parser;

namespace Rdx.Serialization.RdxToObjectConverter.ValueParsers;

public class IntValueParser : ValueParserBase
{
    public override Type TargetType { get; } = typeof(int);
    public override object Parse(ParserRdxValue value, ConverterArguments arguments)
    {
        return ParsePrimitiveValue(int.Parse, value, arguments);
    }
}