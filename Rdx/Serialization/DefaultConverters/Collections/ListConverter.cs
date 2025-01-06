using Rdx.Serialization.Parser;

namespace Rdx.Serialization.DefaultConverters.Collections;

public class ListConverter : DefaultConverterBase
{
    public override Type TargetType { get; } = typeof(List<>);

    public override string Serialize(RdxSerializer serializer, object obj)
    {
        throw new NotImplementedException();
    }

    public override object Deserialize(SerializationArguments arguments)
    {
        if (arguments.Value is not ParserRdxPlex plex)
        {
            throw new NotImplementedException("Object is not a plex");
        }

        var genericType = arguments.Type.GetGenericArguments().Single();
        return plex.Value.Select(t => arguments.Serializer.ConvertToType(genericType, t)).ToList();
    }
}