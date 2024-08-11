using System.Runtime.InteropServices;

namespace Ion.PE32;

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
public struct ImageSectionHeader
{
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 8)]
    public string Name;
    public int VirtualSize;
    public int VirtualAddress;
    public int SizeOfRawData;
    public int PointerToRawData;
    public int PointerToRelocations;
    public int PointerToLineNumbers;
    public ushort NumberOfRelocations;
    public ushort NumberOfLineNumbers;
    public uint Characteristics;

    public static int StructSize = Marshal.SizeOf<ImageSectionHeader>();

    public override string ToString() => $"{Name} (VirtualAddress: 0x{VirtualAddress:x}, VirtualSize: 0x{VirtualSize:x})";
}