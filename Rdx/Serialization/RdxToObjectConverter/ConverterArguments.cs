namespace Rdx.Serialization.RdxToObjectConverter.ValueParsers;

public record ConverterArguments(
    SimpleConverter Converter,
    Type Type,
    long ReplicaId,
    bool IsRdxValue);