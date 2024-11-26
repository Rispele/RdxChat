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

    public RdxSlice(params RdxSlice[] slices)
    {
        var arr = slices.SelectMany(s => s.Borders).ToArray();
        var b = GCHandle.Alloc(arr, GCHandleType.Pinned).AddrOfPinnedObject();
        Borders = [b, b + slices.Length * 16];
    }
}