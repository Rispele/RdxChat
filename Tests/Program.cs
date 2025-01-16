using System.Text.Json;
using NUnit.Framework;
using Rdx.Objects;
using Rdx.Objects.PlexValues;
using Rdx.Primitives;
using Rdx.Serialization;
using Tests.Serializer;

namespace Tests;

internal static class Program
{
    [Test]
    public static void m()
    {
        Console.WriteLine(JsonSerializer.Serialize("\"abc\""));
        Console.WriteLine(JsonSerializer.Deserialize<string>(JsonSerializer.Serialize("\"abc\"")));
        Console.WriteLine(JsonSerializer.Deserialize<string>("\u0022abc\u0022"));
        Console.WriteLine("\u0022abc\u0022");
        File.WriteAllText("123.txt", JsonSerializer.Serialize("\"abc\""));
    }
    
    [Test]
    public static void Test1()
    {
        var provider = new ConstIdProvider();
        var serializer = new RdxSerializer(provider);
        var buffer = new RdxBuffer(1024, serializer);
        var factory = new RdxObjectFactory(provider);

        var t1 = factory
            .NewTuple(1, factory.NewValue(2));

        Console.WriteLine(serializer.Serialize(t1));

        var t2 = factory
            .NewTuple(factory.NewValue(1), factory.NewValue(1));

        var id = Random.Shared.NextInt64();
        var t3 = factory.Tuple(
            factory.Value(1, id, 1),
            factory.Value(4, id, 1),
            id,
            version: 3);

        var slices = buffer.AppendObjects([t1, t2, t3]);
        // var slices = buffer.AppendObjects([t1, t2]);
        var mergedSlice = buffer.Merge(slices);

        var deserializedMerged = serializer.Deserialize<RdxTuple<int, int>>(buffer.ExtractObject(mergedSlice));
        
        Console.WriteLine(buffer.ExtractObject(slices[0]));
        Console.WriteLine(buffer.ExtractObject(slices[1]));
        Console.WriteLine(buffer.ExtractObject(slices[2]));
        Console.WriteLine(buffer.ExtractObject(mergedSlice));
    }
}