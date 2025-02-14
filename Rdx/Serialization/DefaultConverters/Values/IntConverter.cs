﻿using System.Globalization;
using Rdx.Serialization.Parser;

namespace Rdx.Serialization.DefaultConverters.Values;

public class IntConverter : IDefaultConverter
{
    public Type TargetType { get; } = typeof(int);

    public string Serialize(RdxSerializer serializer, object obj)
    {
        if (obj is not int intObj) throw new InvalidCastException();

        return intObj.ToString(CultureInfo.InvariantCulture);
    }

    public object Deserialize(SerializationArguments arguments)
    {
        if (arguments.Value is not ParserRdxValue parserRdxValue)
            throw new NotImplementedException("Object is not a ParserRdxValue");

        return int.Parse(parserRdxValue.Value, CultureInfo.InvariantCulture);
    }
}