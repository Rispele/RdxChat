using System.Runtime.InteropServices;
using Rdx.Objects;
using Rdx.Serialization;

namespace Rdx.Primitives;

public class RdxBuffer : IDisposable
{
    public readonly IntPtr buffer;
    private readonly int length;
    private readonly RdxSerializer serializer;

    public RdxBuffer(int length, RdxSerializer serializer)
    {
        this.length = length;
        this.serializer = serializer;
        buffer = Marshal.AllocHGlobal(length);
        FreeBufferSlice = new RdxBufferSlice(buffer, buffer + length);
    }

    public RdxBufferSlice FreeBufferSlice { get; private set; }

    public int WrittenLength => (int)(FreeBufferSlice.From - buffer);

    public void Dispose()
    {
        Marshal.FreeHGlobal(buffer);
    }

    public RdxBufferSlice AppendObject(string rdxObject)
    {
        var serializedBegin = Marshal.StringToHGlobalAnsi(rdxObject);
        var serializedEnd = serializedBegin + rdxObject.Length;

        var tlvBegin = FreeBufferSlice.From;
        RdxExportedFunctions.FromJDR(FreeBufferSlice.Borders, [serializedBegin, serializedEnd]);

        Marshal.FreeHGlobal(serializedBegin);

        return new RdxBufferSlice(tlvBegin, FreeBufferSlice.From);
    }

    public RdxBufferSlice AppendObject(RdxObject rdxObject)
    {
        var serialized = serializer.Serialize(rdxObject);
        var serializedBegin = Marshal.StringToHGlobalAnsi(serialized);
        var serializedEnd = serializedBegin + serialized.Length;

        var tlvBegin = FreeBufferSlice.From;
        RdxExportedFunctions.FromJDR(FreeBufferSlice.Borders, [serializedBegin, serializedEnd]);

        Marshal.FreeHGlobal(serializedBegin);

        return new RdxBufferSlice(tlvBegin, FreeBufferSlice.From);
    }

    public RdxBufferSlice[] AppendObjects(RdxObject[] objects)
    {
        return objects.Select(AppendObject).ToArray();
    }

    public RdxBufferSlice Merge(RdxBufferSlice[] slices)
    {
        using var sliceOfSlices = RdxSlice.Create(slices);

        var beginOfMerged = FreeBufferSlice.From;
        RdxExportedFunctions.MergeRDX(FreeBufferSlice.Borders, sliceOfSlices.Borders);
        var endOfMerged = FreeBufferSlice.From;

        return new RdxBufferSlice(beginOfMerged, endOfMerged);
    }

    public string ExtractObject(RdxBufferSlice bufferSlice)
    {
        var beginExtracted = FreeBufferSlice.From;
        RdxExportedFunctions.ToJDR(FreeBufferSlice.Borders, bufferSlice.Borders);
        var endOfExtracted = FreeBufferSlice.From;

        return Marshal.PtrToStringAnsi(beginExtracted, (int)(endOfExtracted - beginExtracted));
    }

    public void Clear()
    {
        FreeBufferSlice = new RdxBufferSlice(buffer, buffer + length);
    }
}