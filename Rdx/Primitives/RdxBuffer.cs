using System.Runtime.InteropServices;
using Rdx.Objects;

namespace Rdx.Primitives;

public class RdxBuffer : IDisposable
{
    private readonly int length;
    public readonly IntPtr buffer;

    public RdxSlice FreeSlice { get; private set; }

    public int WrittenLength => (int)(FreeSlice.From - buffer);

    public RdxBuffer(int length)
    {
        this.length = length;
        buffer = Marshal.AllocHGlobal(length);
        FreeSlice = new RdxSlice(buffer, buffer + length);
    }
    
    public RdxSlice AppendObject(string rdxObject)
    {
        var serializedBegin = Marshal.StringToHGlobalAnsi(rdxObject);
        var serializedEnd = serializedBegin + rdxObject.Length;

        var tlvBegin = FreeSlice.From;
        RdxExportedFunctions.RDXJdrainExport(FreeSlice.Borders, [serializedBegin, serializedEnd]);
        
        Marshal.FreeHGlobal(serializedBegin);
        
        return new RdxSlice(tlvBegin, FreeSlice.From);
    }

    public RdxSlice AppendObject(RdxObject rdxObject)
    {
        var serialized = rdxObject.Serialize();
        var serializedBegin = Marshal.StringToHGlobalAnsi(serialized);
        var serializedEnd = serializedBegin + serialized.Length;

        var tlvBegin = FreeSlice.From;
        RdxExportedFunctions.RDXJdrainExport(FreeSlice.Borders, [serializedBegin, serializedEnd]);
        
        Marshal.FreeHGlobal(serializedBegin);
        
        return new RdxSlice(tlvBegin, FreeSlice.From);
    }
    
    public RdxSlice[] AppendObjects(RdxObject[] objects)
    {
        return objects.Select(AppendObject).ToArray();
    }

    public RdxSlice Merge(RdxSlice[] slices)
    {
        var (toFree, sliceOfSlices) = RdxSlice.OfSlices(slices);

        var beginOfMerged = FreeSlice.From;
        RdxExportedFunctions.RDXYExport(FreeSlice.Borders, sliceOfSlices.Borders);
        var endOfMerged = FreeSlice.From;
        
        toFree.Free();
        return new RdxSlice(beginOfMerged, endOfMerged);
    }

    public string ExtractObject(RdxSlice slice)
    {
        var beginExtracted = FreeSlice.From;
        RdxExportedFunctions.RDXJfeedExport(FreeSlice.Borders, slice.Borders);
        var endOfExtracted = FreeSlice.From;

        return Marshal.PtrToStringAnsi(beginExtracted, (int)(endOfExtracted - beginExtracted));
    }
    
    public void Clear()
    {
        FreeSlice = new RdxSlice(buffer, buffer + length);
    }
    
    public void Dispose()
    {
        Marshal.FreeHGlobal(buffer);
    }
}