using Rdx.Extensions;
using Rdx.Objects;

namespace Rdx.Serialization;

public static class RdxSerializer
{
    public static string Serialize(RdxObject obj)
    {
        return obj.GetRdxSerializerAttribute(KnownSerializers.Values).Serialize(obj);
    }
}