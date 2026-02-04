using Ion.Interop;
using Ion.Interop.Handles;
using Ion.Memory;
using Ion.Properties;
using Ion.Validation;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Ion.Extensions;

internal static class SafeProcessHandleExtension
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void ReadMemory(this SafeProcessHandle handle, nint address, nint buffer, int size)
    {
        Ensure.IsValid(handle);
        Ensure.IsValid(address);

        Ensure.Win32(Kernel32.ReadProcessMemory(handle, address, buffer, (nuint)size, out var read) && size == (int)read,
            () => string.Format(Resources.ErrorFailedToReadFrom, address.ToHexadecimalString()));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int WriteMemory(this SafeProcessHandle handle, nint address, nint buffer, int size)
    {
        return handle.WriteMemoryProtected(address, size, () =>
        {
            Ensure.Win32(Kernel32.WriteProcessMemory(handle, address, buffer, (nuint)size, out var written));
            Ensure.Win32((int)written == size);
        });
    }

    public static int WriteMemoryProtected(this SafeProcessHandle handle, nint address, int length, Action write)
    {
        Ensure.IsValid(handle);
        Ensure.IsValid(address);

        IMemoryProtection? protection = null;

        try
        {
            protection = MemoryProtection.Change(handle, address, length);

            write();

            return length;
        }
        catch (Exception e)
        {
            throw new Win32Exception(string.Format(Resources.ErrorFailedToWriteTo, address.ToHexadecimalString()), e);
        }
        finally
        {
            protection?.Dispose();
        }
    }

    public static PageProtectionFlags GetMemoryProtection(this SafeProcessHandle handle, nint address)
    {
        Ensure.Win32(Query(handle, address, out var memoryInfo) > 0);

        return memoryInfo.MemoryProtection;
    }

    public static int Query(this SafeProcessHandle handle, nint address, out MemoryBasicInformation memoryInfo)
    {
        Ensure.IsValid(handle);

        var result = Kernel32.VirtualQueryEx(handle, address, out memoryInfo);

        return result;
    }

    public static void ReleaseMemoryPage(this SafeProcessHandle handle, nint address)
    {
        Ensure.IsValid(handle);
        Ensure.IsValid(address);
        Ensure.Win32(Kernel32.VirtualFreeEx(handle, address, 0, MemoryReleaseFlags.Release), () => string.Format(Resources.ErrorFailedToReleaseMemoryPage, address.ToHexadecimalString()));
    }

    public static void ReleaseMemoryPage(this SafeProcessHandle handle, nint address, int size)
    {
        Ensure.IsValid(handle);
        Ensure.IsValid(address);
        Ensure.Win32(Kernel32.VirtualFreeEx(handle, address, size, MemoryReleaseFlags.Decommit), () => string.Format(Resources.ErrorFailedToReleaseMemoryPage, address.ToHexadecimalString()));
    }
}