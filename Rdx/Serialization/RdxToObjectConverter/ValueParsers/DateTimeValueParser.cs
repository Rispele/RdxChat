using System.Globalization;
using Rdx.Serialization.Parser;

namespace Rdx.Serialization.RdxToObjectConverter.ValueParsers;

public class DateTimeValueParser : ValueParserBase
{
    public override Type TargetType { get; } = typeof(DateTime);
    public override object Parse(ParserRdxValue value, ConverterArguments arguments)
    {
        return ParsePrimitiveValue(
            str => DateTime.Parse(str, CultureInfo.InvariantCulture),
            value,
            arguments);
    }
}