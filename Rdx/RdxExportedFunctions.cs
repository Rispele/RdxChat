using System.Runtime.InteropServices;

namespace Rdx;

public static class RdxExportedFunctions
{
    private const string libName = "librdx.so";

    [DllImport(libName)]
    public static extern ulong FromJDR(IntPtr[] tlv, IntPtr[] rdxj);

    [DllImport(libName)]
    public static extern ulong ToJDR(IntPtr[] jrdx, IntPtr[] tlv);

    [DllImport(libName)]
    public static extern ulong MergeRDX(IntPtr[] into, IntPtr[] inputs);
}