using System.Runtime.InteropServices;

namespace Ion.PE32;

[StructLayout(LayoutKind.Explicit)]
public struct ImageImportDescriptor
{
    [FieldOffset(0)] public int Characteristics;

    [FieldOffset(0)] public int OriginalFirstThunk;

    [FieldOffset(4)] public int TimeDateStamp;

    [FieldOffset(8)] public int ForwarderChain;

    [FieldOffset(12)] public int Name;

    [FieldOffset(16)] public int FirstThunk;

    public static readonly int StructSize = Marshal.SizeOf<ImageImportDescriptor>();
}