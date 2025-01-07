using Rdx.Serialization.Parser;

namespace Rdx.Serialization.RdxToObjectConverter.ValueParsers;

public class StringValueParser : ValueParserBase
{
    public override Type TargetType { get; } = typeof(string);

    public override object Parse(ParserRdxValue value, ConverterArguments arguments)
    {
        return ParsePrimitiveValue(
            str =>
            {
                if (!str.StartsWith('\"') || !str.EndsWith('\"'))
                {
                    throw new FormatException("Invalid RDX value");
                }

                return str[1..^1];
            },
            value,
            arguments);
    }
}