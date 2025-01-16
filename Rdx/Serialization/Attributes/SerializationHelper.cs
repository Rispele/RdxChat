using System.Globalization;

namespace Rdx.Serialization.Attributes;

public static class SerializationHelper
{
    public static (long replicaId, long version) ParseTimestamp(string timestamp)
    {
        var chunks = timestamp.Split('-');
        return (long.Parse(chunks[0], NumberStyles.HexNumber), long.Parse(chunks[1], NumberStyles.HexNumber));
    }
}