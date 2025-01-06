using System.Runtime.InteropServices;

namespace Rdx;

public static class RdxExportedFunctions
{
    private const string libName = "liblibrdx.so";

    [DllImport(libName)]
    public static extern ulong RDXJdrainExport(IntPtr[] tlv, IntPtr[] rdxj);

    [DllImport(libName)]
    public static extern ulong RDXJfeedExport(IntPtr[] jrdx, IntPtr[] tlv);

    [DllImport(libName)]
    public static extern ulong RDXYExport(IntPtr[] into, IntPtr[] inputs);
}