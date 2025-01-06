using Rdx.Serialization.Parser;

namespace Rdx.Serialization.RdxToObjectConverter.ValueParsers;

public class ListConverter : DefaultConverterBase
{
    public override Type TargetType { get; } = typeof(List<>);

    public override object Convert(object value, ConverterArguments arguments)
    {
        if (value is not ParserRdxPlex plex)
        {
            throw new NotImplementedException("Object is not a plex");
        }

        var genericType = arguments.Type.GetGenericArguments().Single();
        return plex.Value.Select(t => arguments.Converter.ConvertToType(genericType, t)).ToList();
    }
}