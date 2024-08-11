
using System.Runtime.InteropServices;
using System.Text;

namespace Ion.Native;

internal static class Psapi
{
    private const string LibraryName = "psapi";

    [DllImport(LibraryName)]
    public static extern bool EnumDeviceDrivers([In][Out] nint[]? ddAddresses, uint arraySizeBytes, out uint bytesNeeded);

    [DllImport(LibraryName)]
    public static extern int GetDeviceDriverBaseName(nint ddAddress, StringBuilder ddBaseName, int baseNameStringSizeChars);
}