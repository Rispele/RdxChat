using Rdx.Objects.ValueObjects;
using Rdx.Serialization.Parser;

namespace Rdx.Serialization.RdxToObjectConverter.ValueParsers;

public abstract class DefaultConverterBase
{
    public abstract Type TargetType { get; }

    public abstract object Convert(object value, ConverterArguments arguments);
}