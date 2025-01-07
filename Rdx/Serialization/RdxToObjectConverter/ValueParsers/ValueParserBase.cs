using Rdx.Objects.ValueObjects;
using Rdx.Serialization.Parser;

namespace Rdx.Serialization.RdxToObjectConverter.ValueParsers;

public abstract class ValueParserBase
{
    public abstract Type TargetType { get; }
    
    public abstract object Parse(ParserRdxValue value, ConverterArguments arguments);
    
    protected object ParsePrimitiveValue<TType>(
        Func<string, TType> parser,
        ParserRdxValue parserRdxValue,
        ConverterArguments arguments)
        where TType : notnull
    {
        var value = parser(parserRdxValue.Value);
        if (parserRdxValue.Timestamp is null || !arguments.IsRdxValue)
        {
            return value;
        }

        if (arguments.IsRdxValue && parserRdxValue.Timestamp is null)
        {
            throw new InvalidOperationException("Timestamp must be specified for rdx value");
        }

        var (replicaId, version) = ParsingHelper.ParseTimestamp(parserRdxValue.Timestamp);
        return new RdxValue<TType>(value, replicaId, version, arguments.ReplicaId);
    }
}