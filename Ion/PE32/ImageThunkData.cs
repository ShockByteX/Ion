using System.Runtime.InteropServices;

namespace Ion.PE32;

[StructLayout(LayoutKind.Explicit)]
public struct ImageThunkData
{
    [FieldOffset(0)] public uint ForwarderString; // PBYTE 

    [FieldOffset(0)] public uint Function; // PDWORD

    [FieldOffset(0)] public uint Ordinal;

    [FieldOffset(0)] public uint AddressOfData; // PIMAGE_IMPORT_BY_NAME

    public static readonly int StructSize = Marshal.SizeOf<ImageThunkData>(); 
}