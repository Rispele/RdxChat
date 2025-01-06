using System.Globalization;
using Rdx.Serialization.Parser;

namespace Rdx.Serialization.DefaultConverters.Values;

public class DateTimeConverter : IDefaultConverter
{
    public Type TargetType { get; } = typeof(DateTime);

    public string Serialize(RdxSerializer serializer, object obj)
    {
        if (obj is not DateTime dateTime) throw new InvalidCastException();

        return dateTime.ToString(CultureInfo.InvariantCulture);
    }

    public object Deserialize(SerializationArguments arguments)
    {
        if (arguments.Value is not ParserRdxValue parserRdxValue)
            throw new NotImplementedException("Object is not a ParserRdxValue");

        return DateTime.Parse(parserRdxValue.Value, CultureInfo.InvariantCulture);
    }
}