using System.Runtime.InteropServices;

namespace Rdx;

public static class RdxExportedFunctions
{
    private const string libName = "liblibrdx.so";
    
    [DllImport(libName)]
    public static extern unsafe ulong RDXJdrainExport(IntPtr[] tlv, IntPtr[] rdxj);

    [DllImport(libName)]
    public static extern unsafe ulong RDXJfeedExport(IntPtr[] jrdx, IntPtr[] tlv);

    [DllImport(libName)]
    public static extern unsafe ulong RDXYExport(IntPtr[] into, IntPtr[] inputs);
}