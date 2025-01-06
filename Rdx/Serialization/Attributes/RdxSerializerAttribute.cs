namespace Rdx.Serialization.Attributes;

public abstract class RdxSerializerAttribute : Attribute
{
    public abstract string Serialize(RdxSerializer serializer, object obj);

    public abstract object Deserialize(SerializationArguments serializationArguments);
}