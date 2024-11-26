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

        var id = Random.Shared.NextInt64();
        
        var t1 = factory.CreateTuple(
            factory.CreateValue(1, id, 1),
            factory.CreateValue(2, id, 1),
            Random.Shared.NextInt64(),
            version: 1);
        
        var t2 = factory.CreateTriple(
            factory.CreateValue(1, id, 1),
            factory.CreateValue(1, id, 1),
            factory.CreateValue(3, id, 1),
            Random.Shared.NextInt64(),
            version: 2);
        
        var t3 = factory.CreateTriple(
            factory.CreateValue(1, id, 1),
            factory.CreateValue(4, id, 1),
            factory.CreateValue(3, id, 1),
            Random.Shared.NextInt64(),
            version: 3);
        
        var slices = buffer.AppendObjects([t1, t2, t3]);
        var mergedSlice = buffer.Merge(slices);
        
        Console.WriteLine(buffer.ExtractObject(slices[0]));
        Console.WriteLine(buffer.ExtractObject(slices[1]));
        Console.WriteLine(buffer.ExtractObject(slices[2]));
        Console.WriteLine(buffer.ExtractObject(mergedSlice));

        // var (s1, s2, s3) = ("<@1-1 1:2>", "<@2-2 1:1:3>", "<@3-3 1:4:3>");
        // // var (s1, s2) = ("[1, 2]", "[1, 1, 3]");
        // // var (sb1, sb2, sb3) = (Marshal.StringToHGlobalAnsi(s1), Marshal.StringToHGlobalAnsi(s2), Marshal.StringToHGlobalAnsi(s3));
        // // var (sl1, sl2, sl3) = (sb1 + s1.Length, sb2 + s2.Length, sb3 + s3.Length);
        //
        // var buffer = new RdxBuffer(1024);
        //
        // var sliceOfSlices = RdxSlice.OfSlices([buffer.AppendObject(s1), buffer.AppendObject(s2), buffer.AppendObject(s3)]);
        // // RdxExportedFunctions.RDXJdrainExport(buffer.FreeSlice.Borders, [sb1, sl1]);
        // // var eos1 = buffer.WrittenTo;
        // // RdxExportedFunctions.RDXJdrainExport(buffer.FreeSlice.Borders, [sb2, sl2]);
        // // var eos2 = buffer.WrittenTo;
        // // RdxExportedFunctions.RDXJdrainExport(buffer.FreeSlice.Borders, [sb3, sl3]);
        // // var eos3 = buffer.WrittenTo;
        //
        // // var sliceOfSlices = new RdxSlice(new RdxSlice(buffer.WrittenFrom, eos1), new RdxSlice(eos1, eos2), new RdxSlice(eos2, eos3));
        // // var slices = new[] { buffer.WrittenFrom, eos1, eos1, eos2 };
        // // var slicesPtr = GCHandle.Alloc(slices, GCHandleType.Pinned).AddrOfPinnedObject();
        // // var sliceOfSlices = new[] { slicesPtr, slicesPtr + 32 };
        // RdxExportedFunctions.RDXYExport(buffer.FreeSlice.Borders, sliceOfSlices.slice.Borders);
        // RdxExportedFunctions.RDXJfeedExport(buffer.FreeSlice.Borders, [buffer.buffer, buffer.buffer + buffer.WrittenLength]);
        // Console.WriteLine(Marshal.PtrToStringAnsi(buffer.buffer, buffer.WrittenLength));
    }

    public static unsafe void Main()
    {
        Test1();
    }
}