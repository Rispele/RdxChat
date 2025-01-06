namespace Rdx.Serialization.RdxToObjectConverter.DefaultConverters;

public abstract class DefaultConverterBase
{
    public abstract Type TargetType { get; }
    
    public abstract string Serialize(RdxSerializer serializer, object obj);

    public abstract object Deserialize(ConverterArguments arguments);
}