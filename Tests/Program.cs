using Rdx.Objects;
using Rdx.Primitives;
using Rdx.Serialization;
using Tests.Serializer;

namespace Tests;

static class Program
{
    public static unsafe void Test1()
    {
        // var buffer = new RdxBuffer(1024);
        var serializer = new RdxSerializer(new ConstIdProvider());
        var factory = new RdxObjectFactory(new ConstIdProvider());

        var t1 = factory.NewTuple(1, factory.NewValue(2));

        Console.WriteLine(serializer.Serialize(t1));
        
        // var t2 = factory.NewTriple(
            // factory.NewValue(1),
            // factory.NewValue(1),
            // factory.NewValue(3));
        
        // var id = Random.Shared.NextInt64();
        // var t3 = factory.Triple(
            // factory.Value(1, id, 1),
            // factory.Value(4, id, 1),
            // factory.Value(3, id, 1),
            // id,
            // version: 3);
        
        // var slices = buffer.AppendObjects([t1, t2, t3]);
        // var slices = buffer.AppendObjects([t1, t2]);
        // var mergedSlice = buffer.Merge(slices);

        // Console.WriteLine(buffer.ExtractObject(slices[0]));
        // Console.WriteLine(buffer.ExtractObject(slices[1]));
        // Console.WriteLine(buffer.ExtractObject(slices[2]));
        // Console.WriteLine(buffer.ExtractObject(mergedSlice));
    }

    // public static unsafe void Main()
    // {
    //     Test1();
    // }
}