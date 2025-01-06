﻿using System.Text;
using Rdx.Objects.PlexValues;
using Rdx.Serialization.Parser;
using Rdx.Serialization.RdxToObjectConverter;

namespace Rdx.Serialization.Attributes;

public class RdxTupleSerializerAttribute : RdxSerializerAttribute
{
    public override string Serialize(RdxSerializer serializer, object obj)
    {
        if (obj is not RdxPLEX xPle)
        {
            throw new ArgumentException("Value must be a type of RdxXPle");
        }

        var sb = new StringBuilder();

        sb.Append('<').Append(RdxSerializationHelper.SerializeStamp(xPle)).Append(' ');
        foreach (var value in xPle)
        {
            sb.Append(serializer.Serialize(value)).Append(':');
        }

        if (xPle.Count != 0)
        {
            sb.Remove(sb.Length - 1, 1);
        }

        sb.Append('>');

        return sb.ToString();
    }

    public override object Deserialize(ConverterArguments converterArguments)
    {
        if (converterArguments.Value is not ParserRdxPlex plex)
        {
            throw new NotImplementedException("Object is not a plex");
        }
        
        if (plex.PlexType is not PlexType.XPles)
        {
            throw new NotImplementedException("Object is not an XPles Plex");
        }

        if (plex.Value.Count != 2)
        {
            throw new InvalidOperationException("Tuple must have 2 items");
        }

        var genericType = converterArguments.Type.GetGenericArguments();
        var (replicaId, version) = ParsingHelper.ParseTimestamp(plex.Timestamp ?? throw new InvalidOperationException());
        var value1 = converterArguments.Converter.ConvertToType(genericType[0], plex.Value[0]);
        var value2 = converterArguments.Converter.ConvertToType(genericType[1], plex.Value[1]);
        return converterArguments.Type
            .GetConstructor([genericType[0], genericType[1], typeof(long), typeof(long), typeof(long)])!
            .Invoke([value1, value2, replicaId, version, converterArguments.Converter.GetReplicaId()]);
    }
}