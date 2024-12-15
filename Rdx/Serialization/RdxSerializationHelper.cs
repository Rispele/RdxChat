using Rdx.Objects;

namespace Rdx.Serialization;

public static class RdxSerializationHelper
{
    public static string SerializeStamp(RdxObject rdxObject)
    {
        return $"@{rdxObject.ReplicaId:X}-{rdxObject.Version:X}";
    }
}