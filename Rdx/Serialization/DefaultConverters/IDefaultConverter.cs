namespace Rdx.Serialization.DefaultConverters;

public interface IDefaultConverter
{
    public Type TargetType { get; }

    public string Serialize(RdxSerializer serializer, object obj);

    public object Deserialize(SerializationArguments arguments);
}