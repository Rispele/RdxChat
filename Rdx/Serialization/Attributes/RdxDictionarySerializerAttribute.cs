using System.Text;
using Rdx.Objects.PlexValues;

namespace Rdx.Serialization.Attributes;

public class RdxDictionarySerializerAttribute : RdxSerializerAttribute
{
    public override string Serialize(RdxSerializer serializer, object obj)
    {
        if (obj is not RdxPLEX xPle)
        {
            throw new ArgumentException("Value must be a type of RdxPLEX");
        }

        var sb = new StringBuilder();

        sb.Append('{').Append(RdxSerializationHelper.SerializeStamp(xPle)).Append(' ');
        foreach (var value in xPle)
        {
            var (a, b) = ((object, object))value;
            var (serializedA, serializedB) = (serializer.Serialize(a), serializer.Serialize(b));
            sb.Append($"<{serializedA}:{serializedB}>").Append(", ");
        }

        if (xPle.Count != 0)
        {
            sb.Remove(sb.Length - 2, 2);
        }

        sb.Append('}');

        return sb.ToString();
    }
}