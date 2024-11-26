using System.Runtime.InteropServices;

namespace Rdx.Primitives;

[StructLayout(LayoutKind.Sequential)]
public struct RdxSlice
{
    public IntPtr[] Borders { get; }

    public IntPtr From => Borders[0];

    public IntPtr To => Borders[1];

    public RdxSlice(IntPtr from, IntPtr to)
    {
        Borders = [from, to];
    }
    
    /// <summary>
    /// Generate slice of slices
    /// </summary>
    /// <returns>IntPtr to be freed after use</returns>
    public static (GCHandle toFree, RdxSlice slice) OfSlices(RdxSlice[] slices)
    {
        if (slices.Length < 1)
        {
            throw new ArgumentException(
                "Could not append objects array of size less or equal 1. Only arrays of size more than 1 is allowed");
        }
        var slicesBorders = slices.SelectMany(s => s.Borders).ToArray();
        var sliceOfSlicesHandle = GCHandle.Alloc(slicesBorders, GCHandleType.Pinned);
        var sliceOfSlices = sliceOfSlicesHandle.AddrOfPinnedObject();
        return (sliceOfSlicesHandle, new RdxSlice(sliceOfSlices, sliceOfSlices + slices.Length * sizeof(long) * 2)); //sizeof(int) * 2 should be 16
    }
}