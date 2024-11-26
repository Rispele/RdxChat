using System.Runtime.InteropServices;
using Rdx;
using Rdx.Primitives;

namespace Tools;

static class Program
{
    public static unsafe void Test1()
    {
        var (s1, s2, s3) = ("<@1-1 1:2>", "<@1-2 1:1:3>", "<@1-3 1:4:3>");
        // var (s1, s2) = ("[1, 2]", "[1, 1, 3]");
        var (sb1, sb2, sb3) = (Marshal.StringToHGlobalAnsi(s1), Marshal.StringToHGlobalAnsi(s2), Marshal.StringToHGlobalAnsi(s3));
        var (sl1, sl2, sl3) = (sb1 + s1.Length, sb2 + s2.Length, sb3 + s3.Length);

        var buffer = new RdxBuffer(1024);
        
        RdxExportedFunctions.RDXJdrainExport(buffer.FreeSlice.Borders, [sb1, sl1]);
        var eos1 = buffer.WrittenTo;
        RdxExportedFunctions.RDXJdrainExport(buffer.FreeSlice.Borders, [sb2, sl2]);
        var eos2 = buffer.WrittenTo;
        RdxExportedFunctions.RDXJdrainExport(buffer.FreeSlice.Borders, [sb3, sl3]);
        var eos3 = buffer.WrittenTo;
        
        var sliceOfSlices = new RdxSlice(new RdxSlice(buffer.WrittenFrom, eos1), new RdxSlice(eos1, eos2), new RdxSlice(eos2, eos3));
        // var slices = new[] { buffer.WrittenFrom, eos1, eos1, eos2 };
        // var slicesPtr = GCHandle.Alloc(slices, GCHandleType.Pinned).AddrOfPinnedObject();
        // var sliceOfSlices = new[] { slicesPtr, slicesPtr + 32 };
        RdxExportedFunctions.RDXYExport(buffer.FreeSlice.Borders, sliceOfSlices.Borders);
        var eom = buffer.WrittenTo;
        RdxExportedFunctions.RDXJfeedExport(buffer.FreeSlice.Borders, [buffer.WrittenFrom, eom]);
        Console.WriteLine(Marshal.PtrToStringAnsi(eom, (int)(buffer.WrittenTo - eom)));
    }
    
    public static unsafe void Main()
    {
        Test1();
    }
}