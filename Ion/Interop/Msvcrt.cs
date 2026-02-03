using System.Runtime.InteropServices;

namespace Ion.Interop;

internal static unsafe class Msvcrt
{
    public const string LibraryName = "msvcrt.dll";

    [DllImport(LibraryName, EntryPoint = "memcpy", CallingConvention = CallingConvention.Cdecl, SetLastError = false)]
    public static extern nint memcpy(nint dst, nint src, int count);

    [DllImport(LibraryName, EntryPoint = "memcpy", CallingConvention = CallingConvention.Cdecl, SetLastError = false)]
    public static extern nint memcpy(void* dst, void* src, int count);

    [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
    static extern int memcmp(byte[] b1, byte[] b2, int count);
}