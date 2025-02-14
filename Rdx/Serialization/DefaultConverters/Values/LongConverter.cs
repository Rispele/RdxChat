﻿using System.Globalization;
using Rdx.Serialization.Parser;

namespace Rdx.Serialization.DefaultConverters.Values;

public class LongConverter : IDefaultConverter
{
    public Type TargetType { get; } = typeof(long);

    public string Serialize(RdxSerializer serializer, object obj)
    {
        if (obj is not long longObj) throw new InvalidCastException();

        return longObj.ToString(CultureInfo.InvariantCulture);
    }

    public object Deserialize(SerializationArguments arguments)
    {
        if (arguments.Value is not ParserRdxValue parserRdxValue)
            throw new NotImplementedException("Object is not a ParserRdxValue");

        return long.Parse(parserRdxValue.Value, CultureInfo.InvariantCulture);
    }
}