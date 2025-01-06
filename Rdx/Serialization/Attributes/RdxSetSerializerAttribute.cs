﻿using System.Text;
using Rdx.Objects.PlexValues;
using Rdx.Serialization.Parser;
using Rdx.Serialization.RdxToObjectConverter;

namespace Rdx.Serialization.Attributes;

public class RdxSetSerializerAttribute : RdxSerializerAttribute
{
    public override string Serialize(RdxSerializer serializer, object obj)
    {
        if (obj is not RdxPLEX xPle)
        {
            throw new ArgumentException("Value must be a type of RdxPLEX");
        }

        var sb = new StringBuilder();

        sb.Append('{').Append(RdxSerializationHelper.SerializeStamp(xPle)).Append(' ');
        foreach (var value in xPle)
        {
            sb.Append(serializer.Serialize(value)).Append(", ");
        }

        if (xPle.Count != 0)
        {
            sb.Remove(sb.Length - 2, 2);
        }

        sb.Append('}');

        return sb.ToString();
    }

    public override object Deserialize(ConverterArguments converterArguments)
    {
        if (converterArguments.Value is not ParserRdxPlex plex)
        {
            throw new NotImplementedException("Object is not a plex");
        }
        
        if (plex.PlexType is not PlexType.Eulerian)
        {
            throw new NotImplementedException("Object is not an Eulerian Plex");
        }

        var genericType = converterArguments.Type.GetGenericArguments().Single();
        var (replicaId, version) = ParsingHelper.ParseTimestamp(plex.Timestamp ?? throw new InvalidOperationException());
        var values = plex.Value.Select(value => converterArguments.Converter.ConvertToType(genericType, value)).ToList();
        var setType = typeof(HashSet<>).MakeGenericType(genericType);
        var set = Activator.CreateInstance(setType, values.Capacity);
        var addMethod = setType.GetMethod("Add")!;
        foreach (var value in values)
        {
            addMethod.Invoke(set, [value]);
        }
        return converterArguments.Type
            .GetConstructor([setType, typeof(long), typeof(long), typeof(long)])!
            .Invoke([set, replicaId, version, converterArguments.Converter.GetReplicaId()]);
    }
}