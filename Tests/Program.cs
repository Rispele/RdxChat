using Rdx.Objects;
using Rdx.Primitives;

namespace Tools;

static class Program
{
    public class ConstIdProvider : IReplicaIdProvider
    {
        private readonly long id = Random.Shared.NextInt64();

        public long GetReplicaId()
        {
            return id;
        }
    }

    public static unsafe void Test1()
    {
        var buffer = new RdxBuffer(1024);
        var factory = new RdxObjectFactory(new ConstIdProvider());


        var t1 = factory.NewTuple(factory.NewValue(1), factory.NewValue(2));

        var t2 = factory.NewTriple(
            factory.NewValue(1),
            factory.NewValue(1),
            factory.NewValue(3));
        
        var id = Random.Shared.NextInt64();
        var t3 = factory.Triple(
            factory.Value(1, id, 1),
            factory.Value(4, id, 1),
            factory.Value(3, id, 1),
            id,
            version: 3);
        
        var slices = buffer.AppendObjects([t1, t2, t3]);
        var mergedSlice = buffer.Merge(slices);
        
        Console.WriteLine(buffer.ExtractObject(slices[0]));
        Console.WriteLine(buffer.ExtractObject(slices[1]));
        Console.WriteLine(buffer.ExtractObject(slices[2]));
        Console.WriteLine(buffer.ExtractObject(mergedSlice));
    }

    public static unsafe void Main()
    {
        Test1();
    }
}