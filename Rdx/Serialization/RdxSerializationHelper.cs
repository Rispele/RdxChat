using Rdx.Objects;
using Rdx.Serialization.Parser;

namespace Rdx.Serialization;

public static class RdxSerializationHelper
{
    public static string SerializeStamp(RdxObject rdxObject)
    {
        return $"@{rdxObject.ReplicaId:X}-{rdxObject.Version:X}";
    }
    
    public static (object, object) ConvertToTuple(RdxSerializer converter, Type type, object obj)
    {
        if (obj is not ParserRdxPlex plex)
        {
            throw new NotImplementedException("Object is not a plex");
        }

        if (plex.Value.Count != 2)
        {
            throw new InvalidOperationException("Tuple must have 2 items");
        }

        var genericType = type.GetGenericArguments();
        var value1 = converter.ConvertToType(genericType[0], plex.Value[0]);
        var value2 = converter.ConvertToType(genericType[1], plex.Value[1]);
        return (value1, value2);
    }
}