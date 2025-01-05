using System.Globalization;

namespace Rdx.Serialization.RdxToObjectConverter;

public class ParsingHelper
{
    public static (long replicaId, long version) ParseTimestamp(string timestamp)
    {
        var chunks = timestamp.Split('-');
        return (int.Parse(chunks[0], NumberStyles.HexNumber), int.Parse(chunks[1], NumberStyles.HexNumber));
    }
}