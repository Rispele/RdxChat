using System.Text;
using Rdx.Extensions;
using Rdx.Objects;
using Rdx.Objects.PlexValues;

namespace Rdx.Serialization.Attributes.XPleSerializers;

public class RdxXPleSerializerAttribute : RdxSerializerAttribute
{
    public override string Serialize(RdxObject obj)
    {
        if (obj is not RdxPLEX xPle)
        {
            throw new ArgumentException($"Value must be a type of RdxXPle");
        }

        var sb = new StringBuilder();

        sb.Append('<').Append(RdxSerializationHelper.SerializeStamp(obj)).Append(' ');
        foreach (var value in xPle)
        {
            sb.Append(value.GetRdxSerializerAttribute(KnownSerializers.Values).Serialize(value)).Append(':');
        }

        if (xPle.Count != 0)
        {
            sb.Remove(sb.Length - 1, 1);
        }

        sb.Append('>');

        return sb.ToString();
    }
}