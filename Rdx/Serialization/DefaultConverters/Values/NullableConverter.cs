using System.Reflection;
using Rdx.Serialization.Parser;

namespace Rdx.Serialization.DefaultConverters.Values;

public class NullableConverter : IDefaultConverter
{
    public Type TargetType { get; } = typeof(Nullable<>);

    private static readonly MethodInfo HasValueFunc = typeof(Nullable<>).GetMethod("HasValue")!;

    public string Serialize(RdxSerializer serializer, object obj)
    {
        if (!obj.GetType().IsGenericType
         || obj.GetType().GetGenericTypeDefinition() != TargetType) throw new InvalidCastException();

        return (bool)HasValueFunc.Invoke(obj, [])!
            ? serializer.Serialize(obj)
            : "null";
    }

    public object Deserialize(SerializationArguments arguments)
    {
        if (arguments.Value is not ParserRdxValue parserRdxValue)
            throw new NotImplementedException("Object is not a ParserRdxValue");

        if (parserRdxValue.Value == "null")
        {
            return Activator.CreateInstance(arguments.Type)!;
        }

        var deserializedValue = arguments.Serializer.Deserialize(
            arguments.Type.GetGenericArguments().Single(),
            parserRdxValue.Value);
        return Activator.CreateInstance(arguments.Type, deserializedValue)!;
    }
}