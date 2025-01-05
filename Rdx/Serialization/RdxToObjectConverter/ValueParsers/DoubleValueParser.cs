using System.Globalization;
using Rdx.Serialization.Parser;

namespace Rdx.Serialization.RdxToObjectConverter.ValueParsers;

public class DoubleValueParser : ValueParserBase
{
    public override Type TargetType { get; } = typeof(double);
    public override object Parse(ParserRdxValue value, ConverterArguments arguments)
    {
        return ParsePrimitiveValue(
            str => double.Parse(str, CultureInfo.InvariantCulture),
            value,
            arguments);
    }
}