using Rdx.Objects;

namespace Rdx.Serialization.Attributes;

public abstract class RdxSerializerAttribute : Attribute 
{
    public abstract string Serialize(RdxObject obj);
}