using System.Collections;
using Rdx.Serialization.Parser;

namespace Rdx.Serialization.DefaultConverters.Collections;

public class DictionaryConverter : IDefaultConverter
{
    public Type TargetType { get; } = typeof(Dictionary<,>);

    public string Serialize(RdxSerializer serializer, object obj)
    {
        if (obj is not IDictionary dictionary) throw new ArgumentException("Target type is not IDictionary");

        var keyValues = dictionary.Keys
            .Cast<object>()
            .Select(k => $"<{serializer.Serialize(k)}:{serializer.Serialize(dictionary[k]!)}>");
        return $"{{{string.Join(", ", keyValues)}}}";
    }

    public object Deserialize(SerializationArguments arguments)
    {
        if (arguments.Value is not ParserRdxPlex plex) throw new NotImplementedException("Object is not a plex");

        var dictionary = (IDictionary)Activator.CreateInstance(arguments.Type)!;
        var genericTypes = arguments.Type.GetGenericArguments();
        var keyValues =
            plex.Value.Select(t => RdxSerializationHelper.ConvertToTuple(
                arguments.Serializer,
                genericTypes[0],
                genericTypes[1],
                t));
        foreach (var (key, value) in keyValues) dictionary[key] = value;
        return dictionary;
    }
}