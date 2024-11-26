using System.Runtime.InteropServices;

namespace Rdx.Primitives;

public struct RdxBuffer : IDisposable
{
    private readonly IntPtr buffer;

    public RdxSlice FreeSlice { get; }

    public IntPtr WrittenFrom => buffer;
    public IntPtr WrittenTo => FreeSlice.From;

    public int WrittenLength => (int)(WrittenTo - WrittenFrom);

    public RdxBuffer(int length)
    {
        buffer = Marshal.AllocHGlobal(length);
        FreeSlice = new RdxSlice(buffer, buffer + length);
    }

    public void Dispose()
    {
        Marshal.FreeHGlobal(buffer);
    }
}