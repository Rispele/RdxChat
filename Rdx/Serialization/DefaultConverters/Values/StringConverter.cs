﻿using Rdx.Serialization.Parser;

namespace Rdx.Serialization.DefaultConverters.Values;

public class StringConverter : IDefaultConverter
{
    public Type TargetType { get; } = typeof(string);

    public string Serialize(RdxSerializer serializer, object obj)
    {
        if (obj is not string strObj) throw new InvalidCastException();

        return $"\"{strObj.Replace("\"", "\\\"")}\"";
    }

    public object Deserialize(SerializationArguments arguments)
    {
        if (arguments.Value is not ParserRdxValue parserRdxValue)
            throw new NotImplementedException("Object is not a ParserRdxValue");

        var str = parserRdxValue.Value;
        if (!str.StartsWith('\"') || !str.EndsWith('\"')) throw new FormatException("Invalid RDX value");

        return str[1..^1].Replace("\\\"", "\"");
    }
}