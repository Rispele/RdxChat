namespace Rdx.Serialization;

public record SerializationArguments(RdxSerializer Serializer, Type Type, object Value);