using Rdx.Objects;
using Rdx.Serialization.RdxToObjectConverter;

namespace Rdx.Serialization.Attributes;

public abstract class RdxSerializerAttribute : Attribute 
{
    public abstract string Serialize(RdxSerializer serializer, object obj);
    
    public abstract object Deserialize(SimpleConverter converter, Type type, object obj);
}