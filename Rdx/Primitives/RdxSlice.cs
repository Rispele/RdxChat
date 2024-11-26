using System.Runtime.InteropServices;

namespace Rdx.Primitives;

internal class RdxSlice : IDisposable
{
    private GCHandle gcHandle;

    public IntPtr[] Borders { get; }

    private RdxSlice(IntPtr from, IntPtr to, GCHandle gcHandle)
    {
        this.gcHandle = gcHandle;
        Borders = [from, to];
    }

    /// <summary>
    /// Generate slice of slices
    /// </summary>
    /// <returns>IntPtr to be freed after use</returns>
    public static RdxSlice Create(RdxBufferSlice[] slices)
    {
        if (slices.Length < 1)
        {
            throw new ArgumentException(
                "Could not append objects array of size less or equal 1. Only arrays of size more than 1 is allowed");
        }

        var slicesBorders = slices.SelectMany(s => s.Borders).ToArray();
        var sliceOfSlicesHandle = GCHandle.Alloc(slicesBorders, GCHandleType.Pinned);
        var sliceOfSlices = sliceOfSlicesHandle.AddrOfPinnedObject();
        
        //Default sizeof(long) is 8, so slices.Length * sizeof(long) * 2 should be 16 (two IntPtr, because they lay sequentially)
        return new RdxSlice(sliceOfSlices, sliceOfSlices + slices.Length * sizeof(long) * 2, sliceOfSlicesHandle);
    }

    public void Dispose()
    {
        gcHandle.Free();
    }
}