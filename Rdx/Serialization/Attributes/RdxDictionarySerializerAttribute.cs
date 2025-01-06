using System.Text;
using Rdx.Objects.PlexValues;
using Rdx.Serialization.Parser;
using Rdx.Serialization.RdxToObjectConverter;

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

    public override object Deserialize(ConverterArguments converterArguments)
    {
        if (converterArguments.Value is not ParserRdxPlex plex)
        {
            throw new NotImplementedException("Object is not a plex");
        }

        if (plex.PlexType is not PlexType.Eulerian)
        {
            throw new NotImplementedException("Object is not an Eulerian Plex");
        }
        
        var genericTypes = converterArguments.Type.GetGenericArguments();
        var (replicaId, version) = ParsingHelper.ParseTimestamp(plex.Timestamp ?? throw new InvalidOperationException());
        var values = plex.Value
            .Select(value => RdxSerializationHelper.ConvertToTuple(converterArguments.Converter, typeof(ValueTuple<,>).MakeGenericType(genericTypes), value))
            .ToArray();

        var dictionaryType = typeof(Dictionary<,>).MakeGenericType(genericTypes);
        var dictionary = Activator.CreateInstance(dictionaryType);
        var addMethod = dictionaryType.GetMethod("Add")!;
        foreach (var (key, value) in values)
        {
            addMethod.Invoke(dictionary, [key, value]);
        }
        return converterArguments.Type
            .GetConstructor([typeof(IDictionary<,>).MakeGenericType(genericTypes), typeof(long), typeof(long), typeof(long)])!
            .Invoke([dictionary, replicaId, version, converterArguments.Converter.GetReplicaId()]);
    }
}