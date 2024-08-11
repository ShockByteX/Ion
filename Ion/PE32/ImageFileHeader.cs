using System.Runtime.InteropServices;

namespace Ion.PE32;

[StructLayout(LayoutKind.Sequential)]
public struct ImageFileHeader
{
    public ushort Machine;
    public ushort NumberOfSections;
    public uint TimeDateStamp;
    public uint PointerToSymbolTable;
    public uint NumberOfSymbols;
    public ushort SizeOfOptionalHeader;
    public ushort Characteristics;

    public static int StructSize = Marshal.SizeOf<ImageFileHeader>();
}