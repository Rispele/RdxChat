using Rdx.Serialization.Parser;

namespace Rdx.Serialization.RdxToObjectConverter.ValueParsers;

public class BoolValueParser : ValueParserBase
{
    public override Type TargetType { get; } = typeof(bool);
    public override object Parse(ParserRdxValue value, ConverterArguments arguments)
    {
        return ParsePrimitiveValue(bool.Parse, value, arguments);
    }
}