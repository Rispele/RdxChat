using System.Globalization;
using Rdx.Serialization.Parser;

namespace Rdx.Serialization.RdxToObjectConverter.DefaultConverters.Values;

public class DateTimeConverter : DefaultConverterBase
{
    public override Type TargetType { get; } = typeof(DateTime);
    public override string Serialize(RdxSerializer serializer, object obj)
    {
        if (obj is not DateTime dateTime)
        {
            throw new InvalidCastException();
        }
        
        return dateTime.ToString(CultureInfo.InvariantCulture);
    }

    public override object Deserialize(ConverterArguments arguments)
    {
        if (arguments.Value is not ParserRdxValue parserRdxValue)
        {
            throw new NotImplementedException("Object is not a ParserRdxValue");
        }
        
        return DateTime.Parse(parserRdxValue.Value, CultureInfo.InvariantCulture);
    }
}