using Rdx.Serialization.Parser;

namespace Rdx.Serialization.RdxToObjectConverter.ValueParsers;

public class LongValueParser : ValueParserBase
{
    public override Type TargetType { get; } = typeof(long);
    public override object Parse(ParserRdxValue value, ConverterArguments arguments)
    {
        return ParsePrimitiveValue(long.Parse, value, arguments);
    }
}