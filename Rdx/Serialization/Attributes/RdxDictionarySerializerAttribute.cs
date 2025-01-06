using System.Collections;
using System.Text;
using Rdx.Objects.PlexValues;
using Rdx.Serialization.Parser;

namespace Rdx.Serialization.Attributes;

public class RdxDictionarySerializerAttribute : RdxSerializerAttribute
{
    public override string Serialize(RdxSerializer serializer, object obj)
    {
        if (obj is not RdxPLEX xPle) throw new ArgumentException("Value must be a type of RdxPLEX");

        var sb = new StringBuilder();

        sb.Append('{').Append(RdxSerializationHelper.SerializeStamp(xPle)).Append(' ');
        foreach (var value in xPle)
        {
            var (a, b) = ((object, object))value;
            var (serializedA, serializedB) = (serializer.Serialize(a), serializer.Serialize(b));
            sb.Append($"<{serializedA}:{serializedB}>").Append(", ");
        }

        if (xPle.Count != 0) sb.Remove(sb.Length - 2, 2);

        sb.Append('}');

        return sb.ToString();
    }

    public override object Deserialize(SerializationArguments serializationArguments)
    {
        if (serializationArguments.Value is not ParserRdxPlex plex)
            throw new NotImplementedException("Object is not a plex");

        if (plex.PlexType is not PlexType.Eulerian) throw new NotImplementedException("Object is not an Eulerian Plex");

        var timestamp = plex.Timestamp ?? throw new InvalidOperationException();
        var genericTypes = serializationArguments.Type.GetGenericArguments();
        var (replicaId, version) = SerializationHelper.ParseTimestamp(timestamp);
        var values = plex.Value
            .Select(value => RdxSerializationHelper.ConvertToTuple(
                serializationArguments.Serializer,
                genericTypes[0],
                genericTypes[1],
                value));

        var dictionaryType = typeof(Dictionary<,>).MakeGenericType(genericTypes);
        var dictionary = (IDictionary)Activator.CreateInstance(dictionaryType)!;
        foreach (var (key, value) in values) dictionary.Add(key, value);
        return serializationArguments.Type
            .GetConstructor([
                typeof(IDictionary<,>).MakeGenericType(genericTypes), typeof(long), typeof(long), typeof(long)
            ])!
            .Invoke([dictionary, replicaId, version, serializationArguments.Serializer.GetReplicaId()]);
    }
}