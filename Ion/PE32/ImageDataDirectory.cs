using System.Runtime.InteropServices;

namespace Ion.PE32;

[StructLayout(LayoutKind.Sequential)]
public struct ImageDataDirectory
{
    public int VirtualAddress;
    public int Size;
}