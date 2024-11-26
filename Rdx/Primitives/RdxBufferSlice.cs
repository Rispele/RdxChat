using System.Runtime.InteropServices;

namespace Rdx.Primitives;

[StructLayout(LayoutKind.Sequential)]
public struct RdxBufferSlice
{
    public IntPtr[] Borders { get; }

    public IntPtr From => Borders[0];

    public IntPtr To => Borders[1];

    public RdxBufferSlice(IntPtr from, IntPtr to)
    {
        Borders = [from, to];
    }
}