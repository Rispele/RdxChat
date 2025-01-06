using System.Collections;
using System.Text;
using Rdx.Objects.PlexValues;
using Rdx.Serialization.Parser;

namespace Rdx.Serialization.Attributes;

public class RdxXPleSerializerAttribute : RdxSerializerAttribute
{
    public override string Serialize(RdxSerializer serializer, object obj)
    {
        if (obj is not RdxPLEX xPle) throw new ArgumentException("Value must be a type of RdxXPle");

        var sb = new StringBuilder();

        sb.Append('<').Append(RdxSerializationHelper.SerializeStamp(xPle)).Append(' ');
        foreach (var value in xPle) sb.Append(serializer.Serialize(value)).Append(':');

        if (xPle.Count != 0) sb.Remove(sb.Length - 1, 1);

        sb.Append('>');

        return sb.ToString();
    }

    public override object Deserialize(SerializationArguments serializationArguments)
    {
        if (serializationArguments.Value is not ParserRdxPlex plex)
            throw new NotImplementedException("Object is not a plex");

        if (plex.PlexType is not PlexType.XPles) throw new NotImplementedException("Object is not an XPles Plex");

        var timestamp = plex.Timestamp ?? throw new InvalidOperationException();
        var genericType = serializationArguments.Type.GetGenericArguments().Single();
        var (replicaId, version) = SerializationHelper.ParseTimestamp(timestamp);
        var values = plex.Value
            .Select(value => serializationArguments.Serializer.ConvertToType(genericType, value))
            .ToList();
        var listType = typeof(List<>).MakeGenericType(genericType);
        var list = (IList)Activator.CreateInstance(listType, values.Capacity)!;
        foreach (var value in values) list.Add(value);
        return serializationArguments.Type
            .GetConstructor([listType, typeof(long), typeof(long), typeof(long)])!
            .Invoke([list, replicaId, version, serializationArguments.Serializer.GetReplicaId()]);
    }
}