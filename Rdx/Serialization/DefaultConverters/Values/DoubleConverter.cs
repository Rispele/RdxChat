﻿using System.Globalization;
using Rdx.Serialization.Parser;

namespace Rdx.Serialization.DefaultConverters.Values;

public class DoubleConverter : IDefaultConverter
{
    public Type TargetType { get; } = typeof(double);

    public string Serialize(RdxSerializer serializer, object obj)
    {
        if (obj is not double doubleObj) throw new InvalidCastException();

        return doubleObj.ToString(CultureInfo.InvariantCulture);
    }

    public object Deserialize(SerializationArguments arguments)
    {
        if (arguments.Value is not ParserRdxValue parserRdxValue)
            throw new NotImplementedException("Object is not a ParserRdxValue");

        return double.Parse(parserRdxValue.Value, CultureInfo.InvariantCulture);
    }
}